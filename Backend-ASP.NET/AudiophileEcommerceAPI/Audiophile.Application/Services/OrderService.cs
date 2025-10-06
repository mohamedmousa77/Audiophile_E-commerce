using Audiophile.Application.DTOs;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using static Audiophile.Application.DTOs.OrderDTO;

namespace Audiophile.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;


        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderReadDTO> ProcessOrder(OrderCreateDTO dto)
        {

            // Create and save the cx info Entity
            var customer = new CustomerInfo
            {
                FullName = dto.customerFullName,
                Email = dto.customerEmail,
                Phone = dto.customerPhone,
                Address = dto.customerAddress,
                City = dto.customerCity,
                Country = dto.customerCountry,
                ZipCode = dto.ZIPCode
            };


            List<OrderItem> orderItems = new List<OrderItem>();
            decimal subtotal = 0;
            var productsToUpdate = new List<Product>();

            foreach (var itemDto in dto.orderItems)
            {
                var product = await _productRepository.GetProductById(itemDto.productId);
                if (product == null || product.StockQuantity < itemDto.itemQuantity)
                {
                    throw new ArgumentException($"Il prodotto ID {itemDto.productId} non è disponibile o lo stock è insufficiente.");
                }

                OrderItem orderItem = new OrderItem
                {
                    ProductId= itemDto.productId,
                    Quantity= itemDto.itemQuantity,
                    UnitPrice = product.Price,
                };

                orderItems.Add(orderItem);
                subtotal += product.Price * itemDto.itemQuantity;

                product.StockQuantity -= itemDto.itemQuantity;
                productsToUpdate.Add(orderItem);
            }

            // Caclulate total price 
            decimal shipping = 50m;
            decimal vat = subtotal * 0.20m;
            decimal total = subtotal + vat + shipping;

            // 4. Create and save order
            Order order = new Order
            {
                CustomerInfoId = customer.Id,
                CustomerInfo = customer,
                Subtotal = subtotal,
                Shipping = shipping,
                VAT = vat,
                Total = total,
                Items = orderItems,
                Status = "Pending"
            };

            var CreatedOrder = await _orderRepository.CreateOrder(order);

            foreach (var product in productsToUpdate)
            {
                await _productRepository.UpdateProduct(product);
            }


            var readDto = new OrderReadDTO
            {

                Id = CreatedOrder.Id,
                CustomerAddress = CreatedOrder.CustomerInfo.Address,
                CustomerCity = CreatedOrder.CustomerInfo.City,
                CustomerCountry = CreatedOrder.CustomerInfo.Country,
                CustomerEmail = CreatedOrder.CustomerInfo.Email,
                CustomerFullName = CreatedOrder.CustomerInfo.FullName,
                CustomerPhone = CreatedOrder.CustomerInfo.Phone,
                ZIPCode = CreatedOrder.CustomerInfo.ZipCode,
                totalOrderAmount = total,
                OrderItems = CreatedOrder.Items.Select(itemEntity => new ReadOrderItemDTO
                {
                    id = itemEntity.Id,
                    itemQuantity = itemEntity.Quantity,
                    productId = itemEntity.ProductId,
                    UnitPrice = itemEntity.UnitPrice,
                }).ToList()
            };

            return readDto;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            return await _orderRepository.DeleteOrder(id);
        }

        public async Task<IEnumerable<OrderReadDTO>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();
            return orders.Select(p => new OrderReadDTO
            {
                Id = p.Id,
                totalOrderAmount = p.Total,
                ZIPCode = p.CustomerInfo.ZipCode,
                CustomerAddress = p.CustomerInfo.Address,
                CustomerCity = p.CustomerInfo.City,
                CustomerCountry = p.CustomerInfo.Country,
                CustomerEmail = p.CustomerInfo.Email,
                CustomerFullName = p.CustomerInfo.FullName,
                CustomerPhone = p.CustomerInfo.Phone,
                OrderItems = p.Items.Select(p => new ReadOrderItemDTO
                {
                    id = p.Id,
                    itemQuantity = p.Quantity,
                    productId = p.ProductId,
                    UnitPrice = p.UnitPrice,
                }).ToList()
            }).ToList();
        }
        public async Task<OrderReadDTO?> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            
            if (order == null) return null;

            return new OrderReadDTO
            {
                CustomerAddress = order.CustomerInfo.Address,
                CustomerCity = order.CustomerInfo.City,
                CustomerCountry = order.CustomerInfo.Country,
                CustomerEmail = order.CustomerInfo.Email,
                CustomerFullName = order.CustomerInfo.FullName,
                CustomerPhone = order.CustomerInfo.Phone,
                ZIPCode = order.CustomerInfo.ZipCode,

                Id = order.Id,
                totalOrderAmount = order.Subtotal,

                OrderItems = order.Items.Select(order => new ReadOrderItemDTO
                {
                    id = order.Id,
                    itemQuantity = order.Quantity,
                    productId = order.ProductId,
                    UnitPrice = order.UnitPrice,
                }).ToList(),

            };
        }

        public async Task<bool> UpdateOrder(UpdateOrderDTO dto)
        {
            if (dto.CustomerEmail == null) return false;

            var customer = await _orderRepository.GetCustomerByEmailAsync(dto.CustomerEmail);
            if (customer == null) return false;

            var existingOrder = await _orderRepository.GetOrderById(dto.OrderId);
            if (existingOrder == null || existingOrder.CustomerInfoId != customer.Id) return false;

            if (dto.OrderStatus != null)
            {
                // Esempio di Logica: Non puoi aggiornare l'ordine se è già stato consegnato
                if (existingOrder.Status == "Delivered") return false;
                existingOrder.Status = dto.OrderStatus;
            }

            if (dto.CustomerFullName != null) customer.FullName = dto.CustomerFullName;
            if (dto.CustomerCountry != null) customer.Country = dto.CustomerCountry;
            if (dto.CustomerAddress != null) customer.Address = dto.CustomerAddress;
            if (dto.CustomerCity != null) customer.City = dto.CustomerCity;
            if (dto.ZIPCode != null) customer.ZipCode = dto.ZIPCode;

            if (dto.OrderItems != null && dto.OrderItems.Any())
            {
                await _orderRepository.RemoveOrderItemsAsync(existingOrder.Items.ToList());

                decimal SubTotal = 0;
                var newOrderItems = new List<OrderItem>();

                foreach (var itemDto in dto.OrderItems)
                {
                    var product = await _productRepository.GetProductById(itemDto.productId);
                    if (product == null) continue;
                    newOrderItems.Add(new OrderItem
                    {
                        ProductId = itemDto.productId,
                        Quantity = itemDto.itemQuantity,
                        UnitPrice = product.Price
                    });
                    SubTotal += product.Price * itemDto.itemQuantity;
                }
                existingOrder.Items = newOrderItems;
                existingOrder.Subtotal = SubTotal;
                existingOrder.VAT = SubTotal * 0.20m;
                existingOrder.Shipping = 50m;
                existingOrder.Total = existingOrder.Subtotal + existingOrder.VAT + existingOrder.Shipping;

            }
            return await _orderRepository.UpdateOrder(existingOrder, customer); 
        }
    }
}
