using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using static Audiophile.Application.DTOs.OrderDTO;
using Microsoft.Extensions.Logging;
using System.Data;


namespace Audiophile.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;


        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<OrderReadDTO> ProcessOrder(OrderCreateDTO dto, int userId)
        {
            if (dto.OrderItems == null || !dto.OrderItems.Any())
            {
                throw new ArgumentException("L'ordine deve contenere almeno un prodotto");
            }

            try
            {
                await _orderRepository.BeginTransactionAsync();

                var customer = new CustomerInfo
                {
                    UserId = userId,
                    FullName = dto.CustomerFullName,
                    Email = dto.CustomerEmail,
                    Phone = dto.CustomerPhone,
                    Address = dto.CustomerAddress,
                    City = dto.CustomerCity,
                    Country = dto.CustomerCountry,
                    ZipCode = dto.ZIPCode
                };

                List<OrderItem> orderItems = new List<OrderItem>();
                decimal subtotal = 0;

                foreach (var itemDto in dto.OrderItems)
                {
                    var product = await _productRepository.GetProductById(itemDto.ProductId);

                    if(product == null)
                    {
                        throw new ArgumentException($"Prodotto con ID {itemDto.ProductId} non trovato");
                    }

                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        throw new ArgumentException(
                            $"Stock insufficiente per {product.Name}. " +
                            $"Disponibili: {product.StockQuantity}, Richiesti: {itemDto.Quantity}");
                    }


                    OrderItem orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                    };

                    orderItems.Add(orderItem);
                    subtotal += product.Price * itemDto.Quantity;

                    product.StockQuantity -= itemDto.Quantity;
                    await _productRepository.UpdateProduct(product);
                }

                decimal shipping = 50m;
                decimal vat = subtotal * 0.20m;
                decimal total = subtotal + vat + shipping;

                // ===== CREA ORDER =====
                var order = new Order
                {
                    CustomerInfo = customer,
                    Items = orderItems,
                    Subtotal = subtotal,
                    Shipping = shipping,
                    VAT = vat,
                    Total = total,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                var createdOrder = await _orderRepository.CreateOrder(order);

                await _orderRepository.CommitTransactionAsync();

                _logger.LogInformation(
                    "Ordine {OrderId} creato con successo per utente {UserId}",
                    createdOrder.Id, userId);

                return MapToReadDTO(createdOrder);

            }
            catch (Exception ex)
            {
                // ===== ROLLBACK IN CASO DI ERRORE =====
                await _orderRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Errore durante la creazione dell'ordine per utente {UserId}", userId);
                throw;
            }
        
        }

        public async Task<OrderReadDTO?> GetOrderById(int orderId, int userId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order == null)
                return null;

            // Verifica che l'ordine appartenga all'utente
            if (order.CustomerInfo.UserId != userId)
            {
                _logger.LogWarning(
                    "Utente {UserId} ha tentato di accedere all'ordine {OrderId} di un altro utente",
                    userId, orderId);
                return null;
            }

            return MapToReadDTO(order);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return orders.Select(MapToReadDTO).ToList();
        }


        public async Task<bool> UpdateOrder(UpdateOrderDTO dto, int userId)
        {
            if (string.IsNullOrEmpty(dto.CustomerEmail))
            {
                return false;
            }

            var customer = await _orderRepository.GetCustomerByEmailAsync(dto.CustomerEmail);
            if (customer == null || customer.UserId != userId)
            {
                _logger.LogWarning(
                    "Utente {UserId} ha tentato di aggiornare un ordine non suo",
                    userId);
                return false;
            }

            var existingOrder = await _orderRepository.GetOrderById(dto.OrderId);
            if (existingOrder == null || existingOrder.CustomerInfoId != customer.Id)
            {
                return false;
            }

            // Non permettere modifiche agli ordini già consegnati o cancellati
            if (existingOrder.Status == OrderStatus.Delivered ||
                existingOrder.Status == OrderStatus.Cancelled)
            {
                _logger.LogWarning(
                    "Tentativo di modificare ordine {OrderId} con status {Status}",
                    dto.OrderId, existingOrder.Status);
                return false;
            }

            try
            {
                await _orderRepository.BeginTransactionAsync();

                // Aggiorna status se fornito
                if (!string.IsNullOrEmpty(dto.OrderStatus) &&
                    Enum.TryParse<OrderStatus>(dto.OrderStatus, out var newStatus))
                {
                    existingOrder.Status = newStatus;
                }

                // Aggiorna dati cliente
                if (!string.IsNullOrEmpty(dto.CustomerFullName))
                    customer.FullName = dto.CustomerFullName;
                if (!string.IsNullOrEmpty(dto.CustomerPhone))
                    customer.Phone = dto.CustomerPhone;
                if (!string.IsNullOrEmpty(dto.CustomerAddress))
                    customer.Address = dto.CustomerAddress;
                if (!string.IsNullOrEmpty(dto.CustomerCity))
                    customer.City = dto.CustomerCity;
                if (!string.IsNullOrEmpty(dto.CustomerCountry))
                    customer.Country = dto.CustomerCountry;
                if (!string.IsNullOrEmpty(dto.ZIPCode))
                    customer.ZipCode = dto.ZIPCode;

                // Aggiorna items se forniti
                if (dto.OrderItems != null && dto.OrderItems.Any())
                {
                    // Ripristina stock dei vecchi items
                    foreach (var oldItem in existingOrder.Items)
                    {
                        var product = await _productRepository.GetProductById(oldItem.ProductId);
                        if (product != null)
                        {
                            product.StockQuantity += oldItem.Quantity;
                            await _productRepository.UpdateProduct(product);
                        }
                    }

                    // Rimuovi vecchi items
                    await _orderRepository.RemoveOrderItemsAsync(existingOrder.Items.ToList());

                    // Crea nuovi items
                    decimal newSubtotal = 0;
                    var newOrderItems = new List<OrderItem>();

                    foreach (var itemDto in dto.OrderItems)
                    {
                        var product = await _productRepository.GetProductById(itemDto.ProductId);
                        if (product == null || product.StockQuantity < itemDto.Quantity)
                        {
                            throw new ArgumentException(
                                $"Stock insufficiente per prodotto ID {itemDto.ProductId}");
                        }

                        newOrderItems.Add(new OrderItem
                        {
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            UnitPrice = product.Price
                        });

                        newSubtotal += product.Price * itemDto.Quantity;
                        product.StockQuantity -= itemDto.Quantity;
                        await _productRepository.UpdateProduct(product);
                    }

                    existingOrder.Items = newOrderItems;
                    existingOrder.Subtotal = newSubtotal;
                    existingOrder.VAT = newSubtotal * 0.20m;
                    existingOrder.Shipping = 50m;
                    existingOrder.Total = existingOrder.Subtotal + existingOrder.VAT + existingOrder.Shipping;
                }

                existingOrder.UpdatedAt = DateTime.UtcNow;

                var result = await _orderRepository.UpdateOrder(existingOrder, customer);
                await _orderRepository.CommitTransactionAsync();

                _logger.LogInformation("Ordine {OrderId} aggiornato con successo", dto.OrderId);
                return result;
            }
            catch (Exception ex)
            {
                await _orderRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Errore durante l'aggiornamento dell'ordine {OrderId}", dto.OrderId);
                throw;
            }
        }

        public async Task<bool> DeleteOrder(int orderId, int userId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order == null || order.CustomerInfo.UserId != userId)
            {
                return false;
            }

            // Non permettere cancellazione di ordini già spediti o consegnati
            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            {
                _logger.LogWarning(
                    "Tentativo di eliminare ordine {OrderId} con status {Status}",
                    orderId, order.Status);
                return false;
            }

            try
            {
                await _orderRepository.BeginTransactionAsync();

                // Ripristina lo stock
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetProductById(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        await _productRepository.UpdateProduct(product);
                    }
                }

                var result = await _orderRepository.DeleteOrder(orderId);
                await _orderRepository.CommitTransactionAsync();

                _logger.LogInformation("Ordine {OrderId} eliminato con successo", orderId);
                return result;
            }
            catch (Exception ex)
            {
                await _orderRepository.RollbackTransactionAsync();
                _logger.LogError(ex, "Errore durante l'eliminazione dell'ordine {OrderId}", orderId);
                throw;
            }
        }

        private OrderReadDTO MapToReadDTO(Order order)
        {
            return new OrderReadDTO
            {
                Id = order.Id,
                CustomerFullName = order.CustomerInfo.FullName,
                CustomerEmail = order.CustomerInfo.Email,
                CustomerPhone = order.CustomerInfo.Phone,
                CustomerAddress = order.CustomerInfo.Address,
                CustomerCity = order.CustomerInfo.City,
                CustomerCountry = order.CustomerInfo.Country,
                ZipCode = order.CustomerInfo.ZipCode,
                Subtotal = order.Subtotal,
                Shipping = order.Shipping,
                VAT = order.VAT,
                Total = order.Total,
                Status = order.Status.ToString(),
                //CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.Items.Select(item => new ReadOrderItemDTO
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.UnitPrice * item.Quantity
                }).ToList()
            };
        }


    }
}
