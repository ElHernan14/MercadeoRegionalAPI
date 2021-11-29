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
    public class FacturasController : ControllerBase
    {
        private readonly DataContext _context;

        public FacturasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            return await _context.Facturas.ToListAsync();
        }

        [HttpGet("{idArticulo}")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasXArticulo(int idArticulo)
        {
            try
            {
                List<Factura> facturasArticulo = null;
                var carritos = await _context.Carrito.AsNoTracking().Where(x => x.idArticulo == idArticulo).ToListAsync();
                var facturas = await _context.Facturas.ToListAsync();
                foreach(Factura f in facturas)
                {
                    foreach (DetalleCarrito c in carritos)
                    {
                        if(f.idCarrito == c.id)
                        {
                            facturasArticulo.Add(f);
                        }
                    }
                }
                return Ok(facturasArticulo);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idArticulo}")]
        public async Task<ActionResult<decimal>> GetTotalFacturasXArticulo(int idArticulo)
        {
            try
            {
                decimal montoTotal = 0;
                var carritos = await _context.Carrito.AsNoTracking().Where(x => x.idArticulo == idArticulo).ToListAsync();
                var facturas = await _context.Facturas.ToListAsync();
                foreach (Factura f in facturas)
                {
                    foreach (DetalleCarrito c in carritos)
                    {
                        if (f.idCarrito == c.id)
                        {
                            montoTotal += f.montoTotal;
                        }
                    }
                }
                return Ok(montoTotal);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{idArticulo}")]
        public async Task<ActionResult<Factura>> GetFacturaXArticuloUsuario(int idArticulo)
        {
            try
            {
                var username = User.Identity.Name;
                var carrito = await _context.Carrito.AsNoTracking()
                    .Where(x => x.idArticulo == idArticulo)
                    .Where(x => x.usuario.email == username).FirstAsync();
                var factura = await _context.Facturas.AsNoTracking()
                    .Where(x => x.idCarrito == carrito.id)
                    .Where(x => x.usuario.email == username).FirstAsync();
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            try
            {
                var username = User.Identity.Name;
                var usuarioActual = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.email == username);
                var articulo = await _context.Articulos.AsNoTracking().SingleOrDefaultAsync(x => x.id == factura.carrito.idArticulo);
                var vendedor = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.id == factura.carrito.articulo.idUsuario);
                var carrito = await _context.Carrito.AsNoTracking().SingleOrDefaultAsync(x => x.id == factura.idCarrito);

                if (usuarioActual.id != factura.idUsuario)
                {
                    return BadRequest();
                }

                //Calcular monto total
                var precioArticulo = articulo.precio + articulo.iva;
                var montoTotal = precioArticulo * factura.cantidad;
                factura.montoTotal = montoTotal;
                //Recalcular stock articulo
                articulo.cantidadStock -= factura.cantidad;
                _context.Articulos.Update(articulo);
                await _context.SaveChangesAsync();
                //Actualizar estado carrito
                carrito.estado = 1;
                _context.Carrito.Update(carrito);
                await _context.SaveChangesAsync();
                //Actualizar saldos vendedor/comprador
                vendedor.saldo += montoTotal;
                usuarioActual.saldo -= montoTotal;
                _context.Usuarios.Update(vendedor);
                _context.Usuarios.Update(usuarioActual);
                await _context.SaveChangesAsync();

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                return Ok(factura);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.id == id);
        }
    }
}
