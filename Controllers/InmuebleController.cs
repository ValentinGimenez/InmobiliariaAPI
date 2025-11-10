using System.Text.Json;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using _net_integrador.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

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

        // GET api/Inmuebles
        [HttpGet]
        public async Task<ActionResult<List<Inmueble>>> Get()
        {
            var pid = User.GetUserIdOrThrow();
            var lista = await _repo.ObtenerPorPropietario(pid);
            return Ok(lista);
        }

        [HttpGet("alquilados")]
        public async Task<ActionResult<IEnumerable<Inmueble>>> GetInmueblesAlquilados()
        {
            var pid = User.GetUserIdOrThrow();
            var todos = await _repo.ObtenerPorPropietario(pid);
            var alquilados = todos.Where(x => x.estado == 3).ToList();
            return Ok(alquilados);
        }

        // POST api/Inmuebles/cargar
        [HttpPost("cargar")]
        [RequestSizeLimit(20_000_000)]
        public async Task<ActionResult<Inmueble>> Cargar([FromForm] IFormFile imagen, [FromForm] string inmueble)
        {
            var pid = User.GetUserIdOrThrow();

            var nuevo = JsonSerializer.Deserialize<Inmueble>(inmueble);
            if (nuevo == null) return BadRequest("Datos del inmueble inválidos.");

            nuevo.id_propietario = pid;
            if (nuevo.estado == 0) nuevo.estado = 1; 

            if (imagen != null && imagen.Length > 0)
                nuevo.imagen = FileStorage.SaveImage(imagen, _env);

            nuevo = await _repo.AgregarInmueble(nuevo);
            return Ok(nuevo);
        }

        // PUT api/Inmuebles/actualizar
        [HttpPut("actualizar")]
        public async Task<ActionResult<Inmueble>> Actualizar([FromBody] Inmueble body)
        {
            var pid = User.GetUserIdOrThrow();

            var actual = await _repo.ObtenerInmuebleId(body.id);
            if (actual.id == 0 || actual.id_propietario != pid)
                return NotFound("Inmueble no encontrado o no pertenece al propietario.");

            if (body.estado != 0)
            {
                actual.estado = body.estado;
            }

            var res = await _repo.ActualizarInmueble(actual);
            return Ok(res);
        }

        // GET api/Inmuebles/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Inmueble>> GetById(int id)
        {
            var pid = User.GetUserIdOrThrow();
            var inmueble = await _repo.ObtenerInmuebleId(id);
            if (inmueble == null)
                return NotFound(new { message = "Inmueble no encontrado." });
            if (inmueble.id_propietario != pid)
                return Unauthorized(new { message = "No tienes permiso para acceder a este inmueble." });
            return Ok(inmueble);
        }
    }
}
