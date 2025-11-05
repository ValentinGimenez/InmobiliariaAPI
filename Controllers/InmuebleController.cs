using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _net_integrador.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InmueblesController : ControllerBase
    {
        private readonly IRepositorioInmueble _repo;
        private readonly IWebHostEnvironment _env;

        public InmueblesController(IRepositorioInmueble repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        private int GetPropietarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        [HttpGet]
        public ActionResult<List<Inmueble>> Get()
        {
            var pid = GetPropietarioId();
            if (pid == 0) return Unauthorized();

            var lista = _repo.ObtenerPorPropietario(pid);
            return Ok(lista);
        }

        [HttpPost("cargar")]
        [RequestSizeLimit(20_000_000)]
        public ActionResult<Inmueble> Cargar([FromForm] IFormFile imagen, [FromForm] string inmueble)
        {
            var pid = GetPropietarioId();
            if (pid == 0) return Unauthorized();

            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
            var nuevo = JsonSerializer.Deserialize<Inmueble>(inmueble, opts);
            if (nuevo == null) return BadRequest("Datos del inmueble inválidos.");

            nuevo.id_propietario = pid;
            if (nuevo.estado == 0) nuevo.estado = Estado.Disponible;

            nuevo = _repo.AgregarInmueble(nuevo);

            if (imagen != null && imagen.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "Uploads");
                Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
                var fullPath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(fullPath, FileMode.Create))
                    imagen.CopyTo(fs);

                var relativa = Path.Combine("Uploads", fileName).Replace("\\", "/");
                _repo.ActualizarImagen(nuevo.id, relativa);
                nuevo.imagen = relativa;
            }

            return Ok(nuevo);
        }
        [HttpPut("actualizar")]
        public ActionResult<Inmueble> Actualizar([FromBody] Inmueble body)
        {
            var pid = GetPropietarioId();
            if (pid == 0) return Unauthorized();

            var actual = _repo.ObtenerInmuebleId(body.id);
            if (actual.id == 0 || actual.id_propietario != pid)
                return NotFound("Inmueble no encontrado o no pertenece al propietario.");

            if (body.estado != 0) 
            {
                actual.estado = body.estado;
            }
            var res = _repo.ActualizarInmueble(actual);
            return Ok(res);
        }
    }
}
