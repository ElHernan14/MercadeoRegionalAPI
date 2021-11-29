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
using System.IdentityModel.Tokens.Jwt;

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
        [HttpPut]
        public async Task<IActionResult> PutUsuario([FromBody] Usuario usuario)
        {
            try
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
                await _context.SaveChangesAsync();

                return Ok(usuario);              
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuario)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: usuario.password,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
            usuario.password = hashed;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.id }, usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.id == id);

        }


        // POST api/<controller>/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] Login login)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.password,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await _context.Usuarios.FirstOrDefaultAsync(x => x.email == login.nick);
                if (p == null || p.password != hashed || !p.estado)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.email),
                        new Claim("FullName", p.nombre + " " + p.apellido),
                        new Claim(ClaimTypes.Role, p.rolNombre),
                    };

                    var token = new JwtSecurityToken(
                        issuer: configuration["TokenAuthentication:Issuer"],
                        audience: configuration["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> modificarEstado(int id)
        {
            var usuario = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.estado = !usuario.estado;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        [HttpPut("{saldo}")]
        public async Task<IActionResult> modificarSaldo(decimal saldo)
        {
            var usuarioActual = User.Identity.Name;
            var usuario = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(x => x.email == usuarioActual);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.saldo = saldo;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }
    }
}
