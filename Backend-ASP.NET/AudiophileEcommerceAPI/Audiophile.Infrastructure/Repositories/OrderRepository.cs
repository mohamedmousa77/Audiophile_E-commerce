using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudiophileEcommerceAPI.Services
{
    public class OrderRepository : IOrderRepository
    {
        public readonly AppDbContext _appDbContext;
        public OrderRepository(AppDbContext appDbContext) { 
            _appDbContext = appDbContext; 
        }
        public async Task<Order> CreateOrder(OrderDTO orderDTO)
        {

            // Create and save the cx info
            var customer = new CustomerInfo
            {
                FullName = orderDTO.FullName,
                Email = orderDTO.Email,
                Phone = orderDTO.Phone,
                Address = orderDTO.Address,
                City = orderDTO.City,
                Country = orderDTO.Country,
                ZipCode = orderDTO.ZipCode
            };
            _appDbContext.Customers.Add(customer);
            await _appDbContext.SaveChangesAsync();

            // Prepare the order
            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var item in orderDTO.Items)
            {
                var product = await _appDbContext.Products.FindAsync(item.ProductId);
                if (product == null) throw new Exception($"Product with ID {item.ProductId} not found.");


                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
                subtotal += product.Price * item.Quantity;
            }

            // Caclulate total price 
            decimal shipping = 50m;
            decimal vat = subtotal * 0.20m;
            decimal total = subtotal + vat + shipping;

            // 4. Create and save order
            var order = new Order
            {
                CustomerInfoId = customer.Id,
                Subtotal = subtotal,
                Shipping = shipping,
                VAT = vat,
                Total = total,
                Items = orderItems
            };

            _appDbContext.Orders.Add(order);
            await _appDbContext.SaveChangesAsync();


            return order;

        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            _appDbContext.OrderItems.RemoveRange(order.Items);
            _appDbContext.Orders.Remove(order);

            var customer = await _appDbContext.Customers.FindAsync(order.CustomerInfoId);
            if (customer != null)
            {
                _appDbContext.Customers.Remove(customer);
            }

            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrders()
        {
            return await _appDbContext.Orders
                .Include(o => o.CustomerInfoId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Select(o => new OrderDTO
                {
                    FullName = o.CustomerInfo.FullName,
                    Email = o.CustomerInfo.Email,
                    Phone = o.CustomerInfo.Phone,
                    Address = o.CustomerInfo.Address,
                    City = o.CustomerInfo.City,
                    Country = o.CustomerInfo.Country,
                    ZipCode = o.CustomerInfo.ZipCode,
                    Items = o.Items.Select(i => new OrderItemDTO
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<OrderDTO?> GetOrderById(int id)
        {
            var order = await _appDbContext.Orders
                .Include(o => o.CustomerInfo)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDTO
            {
                FullName = order.CustomerInfo.FullName,
                Email = order.CustomerInfo.Email,
                Phone = order.CustomerInfo.Phone,
                Address = order.CustomerInfo.Address,
                City = order.CustomerInfo.City,
                Country = order.CustomerInfo.Country,
                ZipCode = order.CustomerInfo.ZipCode,
                Items = order.Items.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<bool> UpdateOrder(OrderDTO orderDTO)
        {
            var customer = await _appDbContext.Customers
                .FirstOrDefaultAsync( c=> c.Email == orderDTO.Email);

            if (customer == null) return false;

            customer.FullName = orderDTO.FullName;
            customer.Phone = orderDTO.Phone;
            customer.Address = orderDTO.Address;
            customer.City = orderDTO.City;
            customer.Country = orderDTO.Country;
            customer.ZipCode = orderDTO.ZipCode;

            var order = await _appDbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.CustomerInfoId == customer.Id);

            if (order == null) return false;

            _appDbContext.OrderItems.RemoveRange(order.Items);

            decimal subtotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in orderDTO.Items)
            {
                var product = await _appDbContext.Products.FindAsync(item.ProductId);
                
                if (product == null) continue;

                subtotal += product.Price * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            order.Items = orderItems;
            order.Subtotal = subtotal;
            order.VAT = subtotal * 0.20m;
            order.Shipping = 50m;
            order.Total = order.Subtotal + order.VAT + order.Shipping;

            await _appDbContext.SaveChangesAsync();
            return true;
        }


    }
}
