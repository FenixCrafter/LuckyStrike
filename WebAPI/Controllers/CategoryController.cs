using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<Category[]>> GetAll()
        {
            return Ok(await _context.Categories.ToArrayAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Add(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return BadRequest("Vul een naam in");

            _context.Categories.Add(new()
            {
                Name = name
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch]
        public async Task<ActionResult> Update(int id, string name)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(category => category.Id == id);
            if (category == null)
                return BadRequest();

            if (String.IsNullOrWhiteSpace(name))
                return BadRequest("Vul een naam in");

            category.Name = name;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Categorie niet gevonden");

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}