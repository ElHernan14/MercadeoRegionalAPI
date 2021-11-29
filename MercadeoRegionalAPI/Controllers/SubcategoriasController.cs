using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MercadeoRegionalAPI.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MercadeoRegionalAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SubcategoriasController : ControllerBase
    {
        private readonly DataContext _context;

        public SubcategoriasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Subcategorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subcategoria>>> GetSubcategoria()
        {
            return await _context.Subcategorias.ToListAsync();
        }

        // GET: api/Subcategorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subcategoria>> GetSubcategoria(int id)
        {
            var subcategoria = await _context.Subcategorias.FindAsync(id);

            if (subcategoria == null)
            {
                return NotFound();
            }

            return subcategoria;
        }

        // PUT: api/Subcategorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubcategoria(int id, Subcategoria subcategoria)
        {
            if (id != subcategoria.id)
            {
                return BadRequest();
            }

            _context.Entry(subcategoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubcategoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Subcategorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subcategoria>> PostSubcategoria(Subcategoria subcategoria)
        {
            _context.Subcategorias.Add(subcategoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubcategoria", new { id = subcategoria.id }, subcategoria);
        }

        // DELETE: api/Subcategorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategoria(int id)
        {
            var subcategoria = await _context.Subcategorias.FindAsync(id);
            if (subcategoria == null)
            {
                return NotFound();
            }

            _context.Subcategorias.Remove(subcategoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubcategoriaExists(int id)
        {
            return _context.Subcategorias.Any(e => e.id == id);
        }
    }
}
