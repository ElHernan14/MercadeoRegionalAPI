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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MercadeoRegionalAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration configuration;

        public UsuariosController(DataContext context, IConfiguration Configuration)
        {
            _context = context;
            configuration = Configuration;
        }

        // GET: api/Usuarios
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuarioActual = User.Identity.Name;
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(x => x.email == usuarioActual);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario([FromBody] Usuario usuario)
        {
            var usuarioActual = User.Identity.Name;
            var id = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.email == usuarioActual);
            if (id.id != usuario.id)
            {
                return BadRequest();
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuario.password,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
            usuario.password = hashed;

            var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.email),
                        new Claim(ClaimTypes.Role, Usuario.GetValuesEnum(usuario.rol)),
                    };

            _context.Usuarios.Update(usuario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(usuario);
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
