using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepositorioContrato _contratoRepo;

        public PagosController(IRepositorioPago pagoRepo, IRepositorioContrato contratoRepo)
        {
            _pagoRepo = pagoRepo;
            _contratoRepo = contratoRepo;
        }

        // GET api/Pagos/contrato/5
        [HttpGet("contrato/{contratoId:int}")]
        public async Task<ActionResult<List<Pago>>> GetPorContrato(int contratoId)
        {
            var pid = User.GetUserIdOrThrow();

            var contrato = await _contratoRepo.ObtenerContratoConInmueble(contratoId);
            if (contrato == null)
                return NotFound(new { message = "Contrato no encontrado." });

            if (contrato.Inmueble?.id_propietario != pid)
                return Forbid("No tiene permiso para ver pagos de este contrato.");

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
            var pid = User.GetUserIdOrThrow();

            var pago = await _pagoRepo.ObtenerPagoIdConContrato(id);
            if (pago == null)
                return NotFound(new { message = "Pago no encontrado." });

            if (pago.Contrato?.Inmueble?.id_propietario != pid)
                return Forbid("No tiene permiso para modificar este pago.");

            if (pago.estado == 1)
                return BadRequest(new { message = "El pago ya fue recibido." });

            pago.estado = 1;
            pago.fecha_pago = DateTime.Now;
            await _pagoRepo.ActualizarPago(pago);

            return Ok(new { message = "Pago recibido con éxito.", pago });
        }
    }
}
