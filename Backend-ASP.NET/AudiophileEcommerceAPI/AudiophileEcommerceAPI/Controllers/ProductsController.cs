using Asp.Versioning;
using Audiophile.Application.DTOs;
using Audiophile.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AudiophileEcommerceAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [EnableRateLimiting("api")]
    public class ProductsController: ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductReadDTO>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc")
        {
            // Validazione
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;

            _logger.LogInformation(
                "Fetching products - Page: {Page}, Size: {Size}, Category: {Category}",
                pageNumber, pageSize, category ?? "all");

            var result = await _productService.GetAllProductsAsync(
                pageNumber, pageSize, category, minPrice, maxPrice, searchTerm, sortBy, sortOrder);

            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Total-Pages", result.TotalPages.ToString());

            return Ok(result);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "id" })]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", id);
                return NotFound(new { message = $"Prodotto con ID {id} non trovato" });
            }

            return Ok(product);
        }



        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetByCategory(string category)
        {
            var products = await _productService.GetByCategory(category);
            return Ok(products);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductCreateDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO dto)
        {
            _logger.LogInformation("Creating new product: {Name}", dto.Name);

            var product = await _productService.CreateProductAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductUpdateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDTO dto)
        {
            _logger.LogInformation("Updating product: {ProductId}", id);

            var product = await _productService.UpdateProductAsync(id, dto);

            if (product == null)
            {
                return NotFound(new { message = $"Prodotto con ID {id} non trovato" });
            }

            return Ok(product);
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product: {ProductId}", id);

            var result = await _productService.DeleteProductAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Prodotto con ID {id} non trovato" });
            }

            return NoContent();
        }

        //[HttpGet("categories")]
        //[ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        //[ResponseCache(Duration = 3600)] // Cache 1 ora
        //public async Task<IActionResult> GetCategories()
        //{
        //    var categories = await _productService.GetCategoriesAsync(); // TODO: Implement GetCategoriesAsunc
        //    return Ok(categories);
        //}

        [HttpGet("new-arrivals")]
        [ProducesResponseType(typeof(IEnumerable<ProductReadDTO>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> GetNewArrivals()
        {
            var products = await _productService.GetFilteredProductsAsync(false, true);
            return Ok(products);
        }

        [HttpGet("promotions")]
        [ProducesResponseType(typeof(IEnumerable<ProductReadDTO>), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> GetPromotions()
        {
            var products = await _productService.GetFilteredProductsAsync(true, false);
            return Ok(products);
        }
    }       
}
