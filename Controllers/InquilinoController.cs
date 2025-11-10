using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using _net_integrador.Utils;

namespace _net_integrador.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class InquilinosController : ControllerBase
    {
        private readonly ILogger<InquilinosController> _logger;
        private readonly IRepositorioInquilino _repo;
        private readonly IWebHostEnvironment _env;

        public InquilinosController(ILogger<InquilinosController> logger, IRepositorioInquilino repo, IWebHostEnvironment env)
        {
            _logger = logger;
            _repo = repo;
            _env = env;
        }

        // GET api/Inquilinos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Inquilino>> GetById(int id)
        {
            var i = await _repo.ObtenerInquilinoId(id);
            if (i == null) return NotFound(new { message = "Inquilino no encontrado." });
            return Ok(i);
        }

        // POST api/Inquilinos
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public async Task<ActionResult<Inquilino>> Crear([FromForm] IFormFile? imagen, [FromForm] string inquilino)
        {
            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };
                var nuevo = JsonSerializer.Deserialize<Inquilino>(inquilino, opts);
                if (nuevo == null) return BadRequest(new { message = "Datos del inquilino inválidos." });

                nuevo.nombre = nuevo.nombre?.ToUpper() ?? "";
                nuevo.apellido = nuevo.apellido?.ToUpper() ?? "";
                nuevo.email = nuevo.email?.ToLower() ?? "";

                var errores = new Dictionary<string, string>();

                if (await _repo.ExisteDni(nuevo.dni)) errores["dni"] = "Este DNI ya está registrado";
                if (await _repo.ExisteEmail(nuevo.email)) errores["email"] = "Este email ya está registrado"; 

                if (errores.Count > 0) return Conflict(new { errors = errores });

                if (nuevo.estado == 0) nuevo.estado = 1;

                if (imagen != null && imagen.Length > 0)
                    nuevo.imagen = FileStorage.SaveImage(imagen, _env);

                await _repo.AgregarInquilino(nuevo);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.id }, nuevo);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear inquilino");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }
    }
}
