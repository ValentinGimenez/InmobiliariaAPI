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
    public class PagosController : ControllerBase
    {
        private readonly IRepositorioPago _pagoRepo;
        private readonly IRepositorioAuditoria _auditoriaRepo;

        public PagosController(IRepositorioPago pagoRepo, IRepositorioAuditoria auditoriaRepo)
        {
            _pagoRepo = pagoRepo;
            _auditoriaRepo = auditoriaRepo;
        }

        // GET api/Pagos/contrato/5
        [HttpGet("contrato/{contratoId:int}")]
        public ActionResult<List<Pago>> GetPorContrato(int contratoId)
        {
            var listaPagos = _pagoRepo.ObtenerPagosPorContrato(contratoId)
                .Where(p => p.fecha_pago != null) 
                .ToList();

            if (listaPagos == null || listaPagos.Count == 0)
                return NotFound(new { message = "No se encontraron pagos para este contrato." });

            return Ok(listaPagos);
        }


        // PUT api/Pagos/recibir/5
        [HttpPut("recibir/{id:int}")]
        public ActionResult Recibir(int id)
        {
            var pago = _pagoRepo.ObtenerPagoId(id);
            if (pago == null)
                return NotFound(new { message = "Pago no encontrado." });

            if (pago.estado == EstadoPago.recibido)
                return BadRequest(new { message = "El pago ya fue recibido." });

            pago.estado = EstadoPago.recibido;
            pago.fecha_pago = DateTime.Now;
            _pagoRepo.ActualizarPago(pago);

            return Ok(new { message = "Pago recibido con éxito.", pago });
        }

        // PUT api/Pagos/anular/5
        [HttpPut("anular/{id:int}")]
        public ActionResult Anular(int id)
        {
            var pago = _pagoRepo.ObtenerPagoId(id);
            if (pago == null)
                return NotFound(new { message = "Pago no encontrado." });

            if (pago.estado == EstadoPago.anulado)
                return BadRequest(new { message = "El pago ya fue anulado anteriormente." });

            pago.estado = EstadoPago.anulado;
            pago.fecha_pago = DateTime.Now;
            _pagoRepo.ActualizarPago(pago);

            _auditoriaRepo.InsertarRegistroAuditoria(
                TipoAuditoria.Pago,
                pago.id,
                AccionAuditoria.Anular,
                User.Identity?.Name ?? "Anónimo"
            );

            return Ok(new { message = "Pago anulado con éxito.", pago });
        }
    }
}
