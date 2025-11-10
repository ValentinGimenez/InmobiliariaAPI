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

        public PagosController(IRepositorioPago pagoRepo)
        {
            _pagoRepo = pagoRepo;
        }

        // GET api/Pagos/contrato/5
        [HttpGet("contrato/{contratoId:int}")]
        public async Task<ActionResult<List<Pago>>> GetPorContrato(int contratoId)
        {
            var pid = User.GetUserIdOrThrow();
            var listaPagos = await _pagoRepo.ObtenerPagosPorContrato(contratoId);

            var pagosConFecha = listaPagos.Where(p => p.fecha_pago != null).ToList();

            if (pagosConFecha == null || pagosConFecha.Count == 0)
                return NotFound(new { message = "No se encontraron pagos para este contrato." });

            return Ok(pagosConFecha);
        }

        // PUT api/Pagos/recibir/5
        [HttpPut("recibir/{id:int}")]
        public async Task<ActionResult> Recibir(int id)
        {
            var pago = await _pagoRepo.ObtenerPagoId(id);
            if (pago == null)
                return NotFound(new { message = "Pago no encontrado." });

            if (pago.estado == EstadoPago.recibido)
                return BadRequest(new { message = "El pago ya fue recibido." });

            pago.estado = EstadoPago.recibido;
            pago.fecha_pago = DateTime.Now;
            await _pagoRepo.ActualizarPago(pago);

            return Ok(new { message = "Pago recibido con éxito.", pago });
        }
    }
}
