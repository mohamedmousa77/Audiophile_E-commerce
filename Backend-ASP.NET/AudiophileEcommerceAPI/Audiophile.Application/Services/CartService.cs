using Audiophile.Domain.Interfaces;
using Audiophile.Domain.Models;
using static Audiophile.Application.DTOs.CartDTOs;

namespace Audiophile.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }
        public async Task<CartReadDTO?> GetCart(int customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerId(customerId);
            if (cart == null) return null;

            return MapToReadDTO(cart);
        }
        public async Task<CartReadDTO> AddOrUpdate (CartItemUpdateDTO dto)
        {
            if (dto.Quantity <= 0) 
            {
                return await RemoveItem(dto.CustomerId, dto.ProductID);
            }

            var product = await _productRepository.GetProductByIdAsync(dto.ProductID);
            if (product == null)
            {
                throw new ArgumentException($"Prodotto con ID {dto.ProductID} non trovato.");
            }
            if (product.StockQuantity < dto.Quantity)
            {
                throw new ArgumentException($"Stock insufficiente per il prodotto ID {dto.ProductID}. Disponibile: {product.StockQuantity}");
            }

            var cart = await _cartRepository.GetCartByCustomerId(dto.CustomerId);
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerInfoId = dto.CustomerId,
                    Items = new List<CartItem>()
                };
            }

            var existingItem = cart.Items?.FirstOrDefault(item => item.ProductId == dto.ProductID);
            if (existingItem != null)
            { 
                existingItem.Quantity = dto.Quantity;
            }else
            {
                cart.Items?.Add(new CartItem
                {
                    ProductId = dto.ProductID,
                    Quantity = dto.Quantity,
                    CartId =   cart.Id,
                });
            }
            var updatedCart = await _cartRepository.SaveCart(cart);

            return MapToReadDTO(updatedCart);
        }
        public async Task<CartReadDTO> RemoveItem(int customerId, int productId)
        {
            var cart = await _cartRepository.GetCartByCustomerId(customerId);
            if (cart == null) return new CartReadDTO(); 

            var itemToRemove = cart.Items?.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Items!.Remove(itemToRemove);

                var updatedCart = await _cartRepository.SaveCart(cart);
                return MapToReadDTO(updatedCart);
            }

            return MapToReadDTO(cart);
        }

        public async Task<bool> ClearCart(int customerId)
        {
            return await _cartRepository.ClearCart(customerId);
        }

        private CartReadDTO MapToReadDTO(Cart cart)
        {
            var itemDtos = cart.Items?.Select(item => new CartItemReadDTO
            {
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? "Nome Prodotto Sconosciuto", 
                Quantity = item.Quantity,
                UnitPrice = item.Product?.Price ?? 0,
            }).ToList() ?? new List<CartItemReadDTO>();
            return new CartReadDTO
            {
                CartId = cart.Id,
                Items = itemDtos,
                TotalAmount = itemDtos.Sum(i => i.ItemTotal)
            };
        }


    }
}
