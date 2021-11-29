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
    public class ArticulosController : ControllerBase
    {
        private readonly DataContext _context;

        public ArticulosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Articulos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulos()
        {
            var usuarioActual = User.Identity.Name;
            var articulos = await _context.Articulos.Where(x => x.usuario.email == usuarioActual).ToListAsync();
            return Ok(articulos);
        }

        // GET: api/Articulos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Articulo>> GetArticulo(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);

            if (articulo == null)
            {
                return NotFound();
            }

            return Ok(articulo);
        }

        // PUT: api/Articulos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutArticulo([FromBody] Articulo articulo)
        {
            _context.Articulos.Update(articulo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloExists(articulo.id))
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

        // POST: api/Articulos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Articulo>> PostArticulo([FromBody] Articulo articulo)
        {
            _context.Articulos.Add(articulo);
            await _context.SaveChangesAsync();

            return Ok(articulo);
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.id == id);
        }

        [HttpGet("{descripcion}/{localidad}")]
        public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulosLike(string descripcion, string localidad)
        {
            try
            {
                var usuarioActual = User.Identity.Name;
                var articulos = await _context.Articulos.Include(x => x.usuario).Include(x => x.subcategoria)
                                .Where(x => x.descripcion.Contains(descripcion)).Where
                                (x => x.subcategoria.descripcion.Contains(descripcion)).Where
                                (x => x.marca.Contains(descripcion)).Where
                                (x => x.usuario.localidad.nombre.Contains(localidad)).Where
                                (x => x.usuario.email == usuarioActual).Where
                                (x => x.estado == 1).ToListAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{estado}")]
        public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulosXEstado(int estado)
        {
            try
            {
                var usuarioActual = User.Identity.Name;
                var articulos = await _context.Articulos.Include(x => x.usuario).Include(x => x.subcategoria)
                                .Where(x => x.usuario.email == usuarioActual)
                                .Where(x => x.estado == estado).ToListAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{local}/{sub}")]
        public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulosXLocalidadSubcategoria(int local, int sub)
        {
            try
            {
                var usuarioActual = User.Identity.Name;
                var articulos = await _context.Articulos.Include(x => x.usuario).Include(x => x.subcategoria)
                                .Where(x => x.usuario.email == usuarioActual)
                                .Where(x => x.idSubcategoria == sub)
                                .Where(x => x.estado == 1)
                                .Where(x => x.usuario.idLocalidad == local).ToListAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
