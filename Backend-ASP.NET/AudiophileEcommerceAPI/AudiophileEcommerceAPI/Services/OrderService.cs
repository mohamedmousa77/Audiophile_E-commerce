using AudiophileEcommerceAPI.Data;
using AudiophileEcommerceAPI.DTOs;
using AudiophileEcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudiophileEcommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        public readonly AppDbContext _appDbContext;
        public OrderService(AppDbContext appDbContext) { 
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
                    UniPrice = product.Price
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

        public Task<bool> DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDTO>> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDTO?> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrder(OrderDTO orderDTO)
        {
            throw new NotImplementedException();
        }

       
    }
}
