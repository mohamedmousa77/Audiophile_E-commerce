using Audiophile.Application.Services;
using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static Audiophile.Application.DTOs.OrderDTO;

namespace Audiophile.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _productRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ProcessOrderAsync_WithValidData_CreatesOrder()
        {
            // Arrange
            var orderDto = new OrderCreateDTO
            {
                CustomerFullName = "Test Customer",
                CustomerEmail = "customer@test.com",
                CustomerPhone = "1234567890",
                CustomerAddress = "Test Address",
                CustomerCity = "Test City",
                CustomerCountry = "Test Country",
                ZIPCode = "12345",
                OrderItems = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 100m,
                StockQuantity = 10
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdWithLockAsync(1))
                .ReturnsAsync(product);

            _orderRepositoryMock
                .Setup(x => x.CreateOrder(It.IsAny<Order>()))
                .ReturnsAsync((Order o) =>
                {
                    o.Id = 1;
                    return o;
                });

            // Act
            var result = await _orderService.ProcessOrder(orderDto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(orderDto.CustomerEmail, result.CustomerEmail);

            // Verify stock was decremented
            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(It.Is<Product>(p => p.StockQuantity == 8)),
                Times.Once
            );
        }

        [Fact]
        public async Task ProcessOrderAsync_WithInsufficientStock_ThrowsException()
        {
            // Arrange
            var orderDto = new OrderCreateDTO
            {
                CustomerEmail = "test@test.com",
                OrderItems = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO { ProductId = 1, Quantity = 100 }
                }
            };

            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Price = 100m,
                StockQuantity = 5
            };

            _productRepositoryMock
                .Setup(x => x.GetProductByIdWithLockAsync(1))
                .ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _orderService.ProcessOrder(orderDto, 1)
            );

            // Verify rollback was called
            _orderRepositoryMock.Verify(
                x => x.RollbackTransactionAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task CancelOrderAsync_WithValidOrder_RestoresStock()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Status = OrderStatus.Pending,
                CustomerInfo = new CustomerInfo { UserId = 1 },
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            var product = new Product
            {
                Id = 1,
                StockQuantity = 5
            };

            _orderRepositoryMock
                .Setup(x => x.GetOrderById(1))
                .ReturnsAsync(order);

            _productRepositoryMock
                .Setup(x => x.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _orderService.CancelOrderAsync(1, 1, "Test reason");

            // Assert
            Assert.True(result);

            // Verify stock was restored
            _productRepositoryMock.Verify(
                x => x.UpdateProductAsync(It.Is<Product>(p => p.StockQuantity == 7)),
                Times.Once
            );
        }
    }
}