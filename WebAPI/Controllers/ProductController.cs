using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Handlers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FileHandler _fileHandler;
        
        public ProductController(ApplicationDbContext context, FileHandler fileHandler)
        {
            _context = context;
            _fileHandler = fileHandler;
        }
        
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductModel product, [FromForm] IFormFile[] files)
        {
            _context.Products.Add(new()
            {
                CategoryId = product.CategoryId,
                Name = product.Name,
                Price = product.Price,
            });
            await _context.SaveChangesAsync();

            if (files.Length > 0)
            {
                var file = files[0];

                var path = _fileHandler.GetProductImagePath(product.Name);

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Dispose();
                    fileStream.Close();
                }
            }

            return Ok(product);
        }
        
        [HttpGet("GetProducts")]
        public async Task<ActionResult<Dictionary<string, Product[]>>> GetProducts()
        {
            var dic = new Dictionary<string, Product[]>();

            var categories = await _context.Categories
                .Include(category => category.Products)
                .ToListAsync();

            categories.ForEach((category) =>
            {
                dic.Add(category.Name, category.Products.ToArray());
            });

            return Ok(dic);
        }
        
        [HttpGet("GetProduct/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound("Product niet gevonden");
            }
            
            return Ok(product);
        }
        
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromForm] int id, [FromForm] ProductModel product, [FromForm] IFormFile[] files)
        {
            var productToUpdate = await _context.Products.FindAsync(id);
            if (productToUpdate == null)
                return NotFound("Product niet gevonden");

            if (files.Length > 0)
            {
                var file = files[0];

                var path = _fileHandler.GetProductImagePath(product.Name);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Dispose();
                }
            }
            else if (productToUpdate.Name != product.Name)
            {
                System.IO.File.Move(_fileHandler.GetProductImagePath(productToUpdate.Name), _fileHandler.GetProductImagePath(product.Name));
            }

            productToUpdate.Name = product.Name;
            productToUpdate.Price = product.Price;
            productToUpdate.CategoryId = product.CategoryId;
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
                return NotFound("Product niet gevonden");
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var path = _fileHandler.GetProductImagePath(product.Name);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            return Ok();
        }
    }
}