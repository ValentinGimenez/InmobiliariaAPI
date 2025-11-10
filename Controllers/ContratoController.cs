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
    public class ContratosController : ControllerBase
    {
        private readonly ILogger<ContratosController> _logger;
        private readonly IRepositorioContrato _contratoRepo;
        private readonly IRepositorioPago _pagoRepo;
        private readonly IRepositorioInquilino _inquilinoRepo;
        private readonly IRepositorioInmueble _inmuebleRepo;

        public ContratosController(
            ILogger<ContratosController> logger,
            IRepositorioContrato contratoRepo,
            IRepositorioPago pagoRepo,
            IRepositorioInquilino inquilinoRepo,
            IRepositorioInmueble inmuebleRepo
             )
        {
            _logger = logger;
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
        }

        // GET api/Contratos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Contrato>> GetById(int id)
        {
            var contrato = await _contratoRepo.ObtenerContratoId(id);
            if (contrato == null) return NotFound(new { message = "Contrato no encontrado." });
            return Ok(contrato);
        }

        // GET api/Contratos/vigentes/mios
        [HttpGet]
        public async Task<ActionResult<List<Contrato>>> GetVigentesDelPropietario()
        {
            var pid = User.GetUserIdOrThrow();
            var lista = await _contratoRepo.ObtenerVigentesPorPropietario(pid);
            return Ok(lista);
        }

        // GET api/Contratos/vigente-del-inmueble/123
        [HttpGet("vigente-del-inmueble/{idInmueble:int}")]
        public async Task<ActionResult<Contrato>> GetVigentePorInmueble(int idInmueble)
        {
            var pid = User.GetUserIdOrThrow();
            var c = await _contratoRepo.ObtenerVigentePorInmuebleYPropietario(idInmueble, pid);
            if (c == null) return NotFound(new { message = "No hay contrato vigente para ese inmueble o no te pertenece." });
            return Ok(c);
        }

        // POST api/Contratos
        [HttpPost]
        public async Task<ActionResult<Contrato>> Crear([FromBody] Contrato contrato)
        {
            if (contrato == null)
                return BadRequest(new { message = "Datos inválidos." });

            if (!contrato.fecha_inicio.HasValue)
                return BadRequest(new { message = "Fecha de inicio requerida." });

            if (contrato.DuracionEnMeses <= 0 && !contrato.fecha_fin.HasValue)
                return BadRequest(new { message = "Defina DuracionEnMeses o fecha_fin." });

            if (contrato.DuracionEnMeses > 0 && contrato.fecha_inicio.HasValue)
                contrato.fecha_fin = contrato.fecha_inicio.Value.AddMonths(contrato.DuracionEnMeses);

            if (contrato.id_inmueble == 0)
                return BadRequest(new { message = "Debe seleccionar un inmueble." });

            if (!contrato.monto_mensual.HasValue)
                return BadRequest(new { message = "Debe ingresar un monto mensual." });

            contrato.estado = 1;

            var id = await _contratoRepo.AgregarContrato(contrato);

            if (contrato.id_inmueble.HasValue && contrato.id_inmueble.Value > 0)
            {
                await _inmuebleRepo.MarcarComoAlquilado(contrato.id_inmueble.Value);
            }

            contrato.id = id;
            return CreatedAtAction(nameof(GetById), new { id }, contrato);
        }

        // PUT api/Contratos/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Contrato>> Editar(int id, [FromBody] Contrato contratoEditado)
        {
            if (contratoEditado == null || id != contratoEditado.id)
                return BadRequest(new { message = "Datos inválidos." });

            if (contratoEditado.DuracionEnMeses <= 0 && !contratoEditado.fecha_inicio.HasValue)
                return BadRequest(new { message = "Fecha de inicio requerida." });

            if (contratoEditado.DuracionEnMeses > 0 && contratoEditado.fecha_inicio.HasValue)
                contratoEditado.fecha_fin = contratoEditado.fecha_inicio.Value.AddMonths(contratoEditado.DuracionEnMeses);

            if (contratoEditado.id_inmueble.HasValue)
            {
                var existentes = await _contratoRepo.ObtenerContratoPorInmueble(contratoEditado.id_inmueble.Value, contratoEditado.id);  
                bool haySolape = existentes.Any(c =>
                    contratoEditado.fecha_inicio <= c.fecha_fin && contratoEditado.fecha_fin >= c.fecha_inicio);
                if (haySolape)
                    return Conflict(new { message = "Las fechas se solapan con otro contrato del mismo inmueble." });
            }

            await _contratoRepo.ActualizarContrato(contratoEditado);
            return Ok(contratoEditado);
        }

        // POST api/Contratos/5/cancelar
        [HttpPost("{id:int}/cancelar")]
        public async Task<ActionResult> Cancelar(int id, [FromBody] DateTime fechaTerminacion)
        {
            var contrato = await _contratoRepo.ObtenerContratoId(id);
            if (contrato == null) return NotFound(new { message = "Contrato no encontrado." });

            if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)
                return BadRequest(new { message = "El contrato no tiene fechas de inicio/fin válidas." });

            if (fechaTerminacion.Date >= contrato.fecha_fin.Value.Date)
                return BadRequest(new { message = "La terminación debe ser anterior a la fecha de finalización original." });

            if (fechaTerminacion.Date <= contrato.fecha_inicio.Value.Date)
                return BadRequest(new { message = "La terminación debe ser posterior a la fecha de inicio." });

            decimal multa = CalcularMulta(contrato, fechaTerminacion);

            contrato.multa = multa;
            contrato.fecha_terminacion_anticipada = fechaTerminacion;
            contrato.estado = 0;

            await _contratoRepo.ActualizarContrato(contrato);

            var nuevoPago = new Pago
            {
                id_contrato = contrato.id,
                nro_pago = 999,
                fecha_pago = null,
                estado = EstadoPago.pendiente,
                concepto = "Multa por rescisión anticipada"
            };
            await _pagoRepo.AgregarPago(nuevoPago);

            return Ok(new
            {
                success = true,
                multaValor = multa
            });
        }

        private decimal CalcularMulta(Contrato contrato, DateTime fechaTerminacion)
        {
            if (!contrato.fecha_fin.HasValue || !contrato.fecha_inicio.HasValue || !contrato.monto_mensual.HasValue)
                return 0m;

            var duracionTotalDias = (contrato.fecha_fin.Value - contrato.fecha_inicio.Value).TotalDays;
            var tiempoTranscurridoDias = (fechaTerminacion - contrato.fecha_inicio.Value).TotalDays;

            decimal duracionTotalMeses = (decimal)duracionTotalDias / 30.4375m;
            decimal tiempoTranscurridoMeses = (decimal)tiempoTranscurridoDias / 30.4375m;

            decimal mesesMulta = (tiempoTranscurridoMeses < (duracionTotalMeses / 2m)) ? 2m : 1m;
            return contrato.monto_mensual.Value * mesesMulta;
        }
    }
}
