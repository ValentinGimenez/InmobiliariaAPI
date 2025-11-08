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

        [HttpGet]
        public ActionResult<List<Inquilino>> GetAll()
        {
            var lista = _repo.ObtenerInquilinos();
            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Inquilino> GetById(int id)
        {
            var i = _repo.ObtenerInquilinoId(id);
            if (i == null) return NotFound(new { message = "Inquilino no encontrado." });
            return Ok(i);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public ActionResult<Inquilino> Crear([FromForm] IFormFile? imagen, [FromForm] string inquilino)
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
                if (_repo.ExisteDni(nuevo.dni)) errores["dni"] = "Este DNI ya está registrado";
                if (_repo.ExisteEmail(nuevo.email)) errores["email"] = "Este email ya está registrado";
                if (errores.Count > 0) return Conflict(new { errors = errores });

                if (nuevo.estado == 0) nuevo.estado = 1;

                if (imagen != null && imagen.Length > 0)
                    nuevo.imagen = FileStorage.SaveImage(imagen, _env);

                _repo.AgregarInquilino(nuevo);
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

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(20_000_000)]
        public ActionResult<Inquilino> Editar(int id, [FromForm] IFormFile? nuevaImagen, [FromForm] string inquilino)
        {
            try
            {
                var actual = _repo.ObtenerInquilinoId(id);
                if (actual == null) return NotFound(new { message = "Inquilino no encontrado." });

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };
                var editado = JsonSerializer.Deserialize<Inquilino>(inquilino, opts);
                if (editado == null) return BadRequest(new { message = "Datos del inquilino inválidos." });

                editado.id = id;
                editado.nombre = editado.nombre?.ToUpper() ?? "";
                editado.apellido = editado.apellido?.ToUpper() ?? "";
                editado.email = editado.email?.ToLower() ?? "";

                var errores = new Dictionary<string, string>();
                if (_repo.ExisteDni(editado.dni, id)) errores["dni"] = "Este DNI ya está registrado";
                if (_repo.ExisteEmail(editado.email, id)) errores["email"] = "Este email ya está registrado";
                if (errores.Count > 0) return Conflict(new { errors = errores });

                if (nuevaImagen != null && nuevaImagen.Length > 0)
                {
                    var rutaNueva = FileStorage.SaveImage(nuevaImagen, _env);
                    FileStorage.DeleteFile(actual.imagen, _env);
                    editado.imagen = rutaNueva;
                }
                else
                {
                    editado.imagen = actual.imagen;
                }

                _repo.ActualizarInquilino(editado);
                return Ok(editado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar inquilino");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var actual = _repo.ObtenerInquilinoId(id);
                if (actual == null) return NotFound(new { message = "Inquilino no encontrado." });

                var ok = _repo.EliminarInquilino(id);
                if (!ok) return BadRequest(new { message = "No se puede eliminar el inquilino porque tiene un contrato activo." });

                return Ok(new { message = "Inquilino eliminado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar inquilino");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

    }
}
