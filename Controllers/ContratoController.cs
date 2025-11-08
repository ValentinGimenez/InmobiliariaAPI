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
        private readonly IRepositorioAuditoria _auditoriaRepo;

        public ContratosController(
            ILogger<ContratosController> logger,
            IRepositorioContrato contratoRepo,
            IRepositorioPago pagoRepo,
            IRepositorioInquilino inquilinoRepo,
            IRepositorioInmueble inmuebleRepo,
            IRepositorioAuditoria auditoriaRepo)
        {
            _logger = logger;
            _contratoRepo = contratoRepo;
            _pagoRepo = pagoRepo;
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
            _auditoriaRepo = auditoriaRepo;
        }

        // GET api/Contratos?fechaInicio=2025-01-01&fechaFin=2025-12-31&diasVencimiento=30
        // [HttpGet]
        // public ActionResult<IEnumerable<Contrato>> Get([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] int? diasVencimiento)
        // {
        //     IEnumerable<Contrato> lista;

        //     if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio <= fechaFin)
        //     {
        //         lista = _contratoRepo.ObtenerContratosVigentesPorRango(fechaInicio.Value, fechaFin.Value);
        //         return Ok(lista);
        //     }
        //     if (diasVencimiento.HasValue && diasVencimiento.Value > 0)
        //     {
        //         lista = _contratoRepo.ObtenerContratosPorVencimiento(diasVencimiento.Value);
        //         return Ok(lista);
        //     }

        //     lista = _contratoRepo.ObtenerContratos();
        //     return Ok(lista);
        // }

        // GET api/Contratos/5
        [HttpGet("{id:int}")]
        public ActionResult<Contrato> GetById(int id)
        {
            var contrato = _contratoRepo.ObtenerContratoId(id);
            if (contrato == null) return NotFound(new { message = "Contrato no encontrado." });
            return Ok(contrato);
        }

        // GET api/Contratos/vigentes/mios
        // [HttpGet("vigentes/mios")]
        [HttpGet]
        public ActionResult<List<Contrato>> GetVigentesDelPropietario()
        {
            var pid = User.GetUserIdOrThrow();
            var lista = _contratoRepo.ObtenerVigentesPorPropietario(pid);
            return Ok(lista);
        }

        // GET api/Contratos/vigente-del-inmueble/123
        [HttpGet("vigente-del-inmueble/{idInmueble:int}")]
        public ActionResult<Contrato> GetVigentePorInmueble(int idInmueble)
        {
            var pid = User.GetUserIdOrThrow();
            var c = _contratoRepo.ObtenerVigentePorInmuebleYPropietario(idInmueble, pid);
            if (c == null) return NotFound(new { message = "No hay contrato vigente para ese inmueble o no te pertenece." });
            return Ok(c);
        }

        // POST api/Contratos
        [HttpPost]
        public ActionResult<Contrato> Crear([FromBody] Contrato contrato)
        {
            if (contrato == null) return BadRequest(new { message = "Datos inválidos." });

            // Validaciones mínimas
            if (!contrato.fecha_inicio.HasValue)
                return BadRequest(new { message = "Fecha de inicio requerida." });

            if (contrato.DuracionEnMeses <= 0 && !contrato.fecha_fin.HasValue)
                return BadRequest(new { message = "Defina DuracionEnMeses o fecha_fin." });

            // Autocalcular fecha_fin si viene duración
            if (contrato.DuracionEnMeses > 0 && contrato.fecha_inicio.HasValue)
                contrato.fecha_fin = contrato.fecha_inicio.Value.AddMonths(contrato.DuracionEnMeses);

            if (contrato.id_inmueble == 0)
                return BadRequest(new { message = "Debe seleccionar un inmueble." });

            if (!contrato.monto_mensual.HasValue)
                return BadRequest(new { message = "Debe ingresar un monto mensual." });

            contrato.estado = 1; // vigente
            var id = _contratoRepo.AgregarContrato(contrato);

            // marcar inmueble alquilado
            if (contrato.id_inmueble.HasValue && contrato.id_inmueble.Value > 0)
                _inmuebleRepo.MarcarComoAlquilado(contrato.id_inmueble.Value);

            // auditoría
            _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Contrato, id, AccionAuditoria.Crear, User.Identity?.Name ?? "Anónimo");

            contrato.id = id;
            return CreatedAtAction(nameof(GetById), new { id }, contrato);
        }

        // PUT api/Contratos/5
        [HttpPut("{id:int}")]
        public ActionResult<Contrato> Editar(int id, [FromBody] Contrato contratoEditado)
        {
            if (contratoEditado == null || id != contratoEditado.id)
                return BadRequest(new { message = "Datos inválidos." });

            if (contratoEditado.DuracionEnMeses <= 0 && !contratoEditado.fecha_inicio.HasValue)
                return BadRequest(new { message = "Fecha de inicio requerida." });

            if (contratoEditado.DuracionEnMeses > 0 && contratoEditado.fecha_inicio.HasValue)
                contratoEditado.fecha_fin = contratoEditado.fecha_inicio.Value.AddMonths(contratoEditado.DuracionEnMeses);

            if (contratoEditado.id_inmueble.HasValue)
            {
                var existentes = _contratoRepo.ObtenerContratoPorInmueble(contratoEditado.id_inmueble.Value, contratoEditado.id);
                bool haySolape = existentes.Any(c =>
                    contratoEditado.fecha_inicio <= c.fecha_fin && contratoEditado.fecha_fin >= c.fecha_inicio);
                if (haySolape)
                    return Conflict(new { message = "Las fechas se solapan con otro contrato del mismo inmueble." });
            }

            _contratoRepo.ActualizarContrato(contratoEditado);
            return Ok(contratoEditado);
        }

        // POST api/Contratos/5/cancelar
        [HttpPost("{id:int}/cancelar")]
        public ActionResult Cancelar(int id, [FromBody] DateTime fechaTerminacion)
        {
            var contrato = _contratoRepo.ObtenerContratoId(id);
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

            _contratoRepo.ActualizarContrato(contrato);

            _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Contrato, contrato.id, AccionAuditoria.Anular, User.Identity?.Name ?? "Anónimo");

            var nuevoPago = new Pago
            {
                id_contrato = contrato.id,
                nro_pago = 999,
                fecha_pago = null,
                estado = EstadoPago.pendiente,
                concepto = "Multa por rescisión anticipada"
            };
            _pagoRepo.AgregarPago(nuevoPago);

            return Ok(new
            {
                success = true,
                multaValor = multa
            });
        }

        // GET api/Contratos/5/calcular-multa?fechaTerminacion=2025-07-10
        [HttpGet("{id:int}/calcular-multa")]
        public ActionResult CalcularMultaAjax(int id, [FromQuery] string fechaTerminacion)
        {
            var contrato = _contratoRepo.ObtenerContratoId(id);
            if (contrato == null) return NotFound(new { message = "Contrato no encontrado" });

            if (!DateTime.TryParse(fechaTerminacion, out var fecha))
                return BadRequest(new { message = "Fecha inválida" });

            if (!contrato.fecha_inicio.HasValue || !contrato.fecha_fin.HasValue)
                return BadRequest(new { message = "El contrato no tiene fechas válidas." });

            if (fecha.Date >= contrato.fecha_fin.Value.Date)
                return BadRequest(new { message = "La fecha debe ser ANTERIOR al fin original." });

            if (fecha.Date <= contrato.fecha_inicio.Value.Date)
                return BadRequest(new { message = "La fecha debe ser POSTERIOR al inicio." });

            var ultPago = _pagoRepo.ObtenerFechaUltimoPagoRealizado(id);
            if (ultPago.HasValue && fecha.Date <= ultPago.Value.Date)
                return BadRequest(new { message = $"Debe ser POSTERIOR al último pago ({ultPago.Value:dd/MM/yyyy})." });

            if (!contrato.monto_mensual.HasValue)
                return BadRequest(new { message = "El contrato no tiene monto mensual definido." });

            var mesesTrans = ((fecha.Year - contrato.fecha_inicio.Value.Year) * 12) + fecha.Month - contrato.fecha_inicio.Value.Month;
            if (fecha.Day >= contrato.fecha_inicio.Value.Day) mesesTrans += 1;

            int pagosRealizados = _pagoRepo.ContarPagosRealizados(id);
            int mesesAdeudados = Math.Max(0, mesesTrans - pagosRealizados);

            decimal multa = CalcularMulta(contrato, fecha);
            decimal totalAdeudado = contrato.monto_mensual.Value * mesesAdeudados;

            return Ok(new
            {
                multaValor = multa,
                mesesAdeudados,
                totalAdeudadoValor = totalAdeudado
            });
        }

        // POST api/Contratos/renovar
        [HttpPost("renovar")]
        public ActionResult Renovar([FromBody] Contrato contrato)
        {
            if (contrato == null) return BadRequest(new { message = "Datos inválidos." });

            if (contrato.DuracionEnMeses <= 0 && !contrato.fecha_fin.HasValue)
                return BadRequest(new { message = "Defina duración o fecha_fin." });

            if (!contrato.fecha_inicio.HasValue)
                return BadRequest(new { message = "Fecha de inicio requerida." });

            if (contrato.DuracionEnMeses > 0)
                contrato.fecha_fin = contrato.fecha_inicio.Value.AddMonths(contrato.DuracionEnMeses);

            if (contrato.id_inmueble == 0 || !contrato.id_inmueble.HasValue)
                return BadRequest(new { message = "El inmueble no está definido." });

            if (!contrato.monto_mensual.HasValue)
                return BadRequest(new { message = "El monto mensual no está definido." });


            contrato.estado = 1;
            var idNuevo = _contratoRepo.AgregarContrato(contrato);

            _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Contrato, idNuevo, AccionAuditoria.Crear, User.Identity?.Name ?? "Anónimo");

            return Ok(new { id = idNuevo });
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
