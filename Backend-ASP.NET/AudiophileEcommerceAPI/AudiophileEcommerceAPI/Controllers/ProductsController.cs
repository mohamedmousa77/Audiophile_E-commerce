using Audiophile.Application.Services;
using Audiophile.Application.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace AudiophileEcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        private readonly ProductService _productService;
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetAll()
            => Ok(await _productService.GetAllProducts());

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDTO?>> GetById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetByCategory(string category)
        {
            var products = await _productService.GetByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductReadDTO>> Create([FromBody] ProductCreateDTO product)
        {
            if (product == null) return BadRequest("Product cannot be null");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var readDto = await _productService.CreateProduct(product);
                return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);

            } catch (ArgumentException ex)
              {
                return BadRequest(new { error = ex.Message });
              }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDTO product)
        {
            if (product == null || product.Id != id) return BadRequest("Product ID mismatch");
            var updated = await _productService.UpdateProduct(product);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductReadDTO>>> GetFilteredProducts (bool? isPromotion = null, bool? isNew = null)
        {
            var products = await _productService.GetFilteredProducts(isPromotion, isNew);

            return Ok(products);
        }

    }
}
