using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallEcommerece.Dtos;
using SmallEcommerece.Models;
using SmallEcommerece.Services;

namespace SmallEcommerece.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string filePath = null;

            if (productDto.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
                filePath = Path.Combine("images", fileName);

                using (var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create))
                {
                    await productDto.Image.CopyToAsync(stream);
                }
            }

            var product = new Product
            {
                ProductCode = productDto.ProductCode,
                Name = productDto.Name,
                Category = productDto.Category,
                Price = productDto.Price,
                MinimumQuantity = productDto.MinimumQuantity,
                DiscountRate = productDto.DiscountRate,
                ImagePath = filePath
            };

            var created = await _productService.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDto dto)
        {

            var existing = await _productService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            if (dto.Image != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existing.ImagePath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existing.ImagePath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existing.ImagePath = $"images/{fileName}";
            }

            existing.ProductCode = dto.ProductCode;
            existing.Name = dto.Name;
            existing.Category = dto.Category;
            existing.Price = dto.Price;
            existing.MinimumQuantity = dto.MinimumQuantity;
            existing.DiscountRate = dto.DiscountRate;

            await _productService.UpdateAsync(existing);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _productService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
