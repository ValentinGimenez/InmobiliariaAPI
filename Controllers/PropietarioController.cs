using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using _net_integrador.Utils;

namespace _net_integrador.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropietariosController : ControllerBase
    {
        private readonly IRepositorioPropietario _repo;
        private readonly IConfiguration _cfg;

        public PropietariosController(IRepositorioPropietario repo, IConfiguration cfg)
        {
            _repo = repo;
            _cfg = cfg;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Login([FromForm] string Usuario, [FromForm] string Clave)
        {
            var p = _repo.ObtenerPorEmail(Usuario);
            if (p == null || string.IsNullOrEmpty(p.clave) || !BCrypt.Net.BCrypt.Verify(Clave, p.clave))
                return Unauthorized("Correo electrónico o contraseña incorrectos.");

            var token = GenerarToken(p);
            return Content(token, "text/plain", Encoding.UTF8);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<Propietario> Get()
        {
            var id = User.GetUserIdOrThrow();
            var p = _repo.ObtenerPropietarioId(id);
            if (p == null)
                return NotFound();

            p.clave = null;
            return Ok(p);
        }

        [HttpPut("actualizar")]
        [Authorize]
        public ActionResult<Propietario> Actualizar([FromBody] Propietario dto)
        {
            var id = User.GetUserIdOrThrow();
            var p = _repo.ObtenerPropietarioId(id);
            if (p == null)
                return NotFound();

            p.nombre = string.IsNullOrWhiteSpace(dto.nombre) ? p.nombre : dto.nombre;
            p.apellido = string.IsNullOrWhiteSpace(dto.apellido) ? p.apellido : dto.apellido;
            p.dni = string.IsNullOrWhiteSpace(dto.dni) ? p.dni : dto.dni;
            p.email = string.IsNullOrWhiteSpace(dto.email) ? p.email : dto.email;
            p.telefono = string.IsNullOrWhiteSpace(dto.telefono) ? p.telefono : dto.telefono;

            var actualizado = _repo.ActualizarPropietario(p);
            actualizado.clave = null;
            return Ok(actualizado);
        }

        [HttpPut("changePassword")]
        [Authorize]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return BadRequest("La nueva contraseña debe tener al menos 6 caracteres.");

            var id = User.GetUserIdOrThrow();
            var p = _repo.ObtenerPropietarioId(id);
            if (p == null)
                return NotFound();

            if (string.IsNullOrEmpty(p.clave) || !BCrypt.Net.BCrypt.Verify(currentPassword, p.clave))
                return Unauthorized("La contraseña actual es incorrecta.");

            var ok = _repo.CambiarPassword(id, newPassword);
            if (!ok)
                return StatusCode(500, "No se pudo actualizar la contraseña.");

            return NoContent();
        }

        private string GenerarToken(Propietario p)
        {
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, p.id.ToString()),
                new Claim(ClaimTypes.Name, p.email ?? string.Empty),
                new Claim(ClaimTypes.Role, "Propietario"),
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
