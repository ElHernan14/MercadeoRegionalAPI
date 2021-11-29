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
        public async Task<ActionResult<IEnumerable<DetalleCarrito>>> GetCarritos()
        {
            var usuarioActual = User.Identity.Name;
            var carritos = await _context.Carrito.Where(x => x.usuario.email == usuarioActual).ToListAsync();
            return Ok(carritos);
        }

        [HttpGet("{estado}")]
        public async Task<ActionResult<IEnumerable<DetalleCarrito>>> GetCarritosXEstado(int estado)
        {
            var usuarioActual = User.Identity.Name;
            var carritos = await _context.Carrito.Where(x => x.usuario.email == usuarioActual)
                .Where(x => x.estado == estado).ToListAsync();
            return Ok(carritos);
        }

        [HttpGet("{estado}")]
        public async Task<ActionResult<decimal>> GetTotalCarritoXEstado(int estado)
        {
            try
            {
                var usuarioActual = User.Identity.Name;
                decimal total = 0;
                var carritos = await _context.Carrito.Where(x => x.usuario.email == usuarioActual)
                    .Include(x => x.articulo).Where(x => x.estado == estado).ToListAsync();
                var facturas = await _context.Facturas.Where(x => x.usuario.email == usuarioActual).ToListAsync();

                if (estado != 1)
                {
                    if (carritos != null)
                    {
                        foreach (DetalleCarrito c in carritos)
                        {
                            decimal precio = c.articulo.precio + c.articulo.iva;
                            total += precio;
                        }
                    }
                }
                else
                {
                    if (carritos != null && facturas != null)
                    {
                        foreach (DetalleCarrito c in carritos)
                        {
                            foreach (Factura f in facturas)
                            {
                                decimal precio = f.montoTotal;
                                total += precio;
                            }
                        }
                    }
                }
                return Ok(total);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/DetalleCarritos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DetalleCarrito>> PostDetalleCarrito(DetalleCarrito detalleCarrito)
        {
            try{
                var username = User.Identity.Name;
                var usuarioActual = await _context.Usuarios.SingleOrDefaultAsync(x => x.email == username);
                var articulo = await _context.Articulos.SingleOrDefaultAsync(x => x.id == detalleCarrito.idArticulo);
                if((usuarioActual.id != detalleCarrito.idUsuario) || (articulo == null))
                {
                    return BadRequest();
                }
                _context.Carrito.Add(detalleCarrito);
                await _context.SaveChangesAsync();
                return Ok(detalleCarrito);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/DetalleCarritos/5
        [HttpDelete("{id}/{idArticulo}")]
        public async Task<IActionResult> DeleteDetalleCarrito(int id, int idArticulo)
        {
            try
            {
                var detalleCarrito = await _context.Carrito.Where(x => x.idArticulo == idArticulo)
                .Where(x => x.id == id).FirstAsync();
                if (detalleCarrito == null)
                {
                    return NotFound();
                }

                _context.Carrito.Remove(detalleCarrito);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex);
            }
        }

        private bool DetalleCarritoExists(int id)
        {
            return _context.Carrito.Any(e => e.id == id);
        }
    }
}
