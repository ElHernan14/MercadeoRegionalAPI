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
    public class DetalleCarritosController : ControllerBase
    {
        private readonly DataContext _context;

        public DetalleCarritosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/DetalleCarritos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleCarrito>>> GetCarrito()
        {
            return await _context.Carrito.ToListAsync();
        }

        // GET: api/DetalleCarritos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleCarrito>> GetDetalleCarrito(int id)
        {
            var detalleCarrito = await _context.Carrito.FindAsync(id);

            if (detalleCarrito == null)
            {
                return NotFound();
            }

            return detalleCarrito;
        }

        // PUT: api/DetalleCarritos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalleCarrito(int id, DetalleCarrito detalleCarrito)
        {
            if (id != detalleCarrito.Id)
            {
                return BadRequest();
            }

            _context.Entry(detalleCarrito).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetalleCarritoExists(id))
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

        // POST: api/DetalleCarritos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DetalleCarrito>> PostDetalleCarrito(DetalleCarrito detalleCarrito)
        {
            _context.Carrito.Add(detalleCarrito);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetalleCarrito", new { id = detalleCarrito.Id }, detalleCarrito);
        }

        // DELETE: api/DetalleCarritos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleCarrito(int id)
        {
            var detalleCarrito = await _context.Carrito.FindAsync(id);
            if (detalleCarrito == null)
            {
                return NotFound();
            }

            _context.Carrito.Remove(detalleCarrito);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DetalleCarritoExists(int id)
        {
            return _context.Carrito.Any(e => e.Id == id);
        }
    }
}
