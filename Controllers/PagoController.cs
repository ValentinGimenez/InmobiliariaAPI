using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers;
[Authorize]
public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly IRepositorioPago _pagoRepo;
    private readonly IRepositorioAuditoria _auditoriaRepo;

    public PagoController(ILogger<PagoController> logger, IRepositorioPago pagoRepo, IRepositorioAuditoria auditoriaRepo)
    {
        _logger = logger;
        _pagoRepo = pagoRepo;
        _auditoriaRepo = auditoriaRepo;
    }

    public IActionResult Index(int contratoId)
    {
        var listaPagos = _pagoRepo.ObtenerPagosPorContrato(contratoId);
        ViewBag.ContratoId = contratoId;
        return View(listaPagos);
    }
    
    [HttpPost]
    public IActionResult Recibir()
    {
        if (!int.TryParse(HttpContext.Request.Form["id"], out int id))
        {
            TempData["Error"] = "ID de pago inválido.";
            return RedirectToAction("Index", "Contrato");
        }

        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", "Contrato");
        }
        
        var estadoAnterior = pago.estado.ToString();
        pago.estado = EstadoPago.recibido;
        pago.fecha_pago = DateTime.Now; // Se establece la fecha de pago al momento actual
        _pagoRepo.ActualizarPago(pago);

        _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pago.id,
            AccionAuditoria.Recibir, 
            User.Identity.Name ?? "Anónimo"
        );
        
        TempData["Exito"] = "Pago recibido con éxito";
        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }

    [HttpPost]
    public IActionResult Anular()
    {
        if (!int.TryParse(HttpContext.Request.Form["id"], out int id))
        {
            TempData["Error"] = "ID de pago inválido.";
            return RedirectToAction("Index", "Contrato");
        }
        
        var pago = _pagoRepo.ObtenerPagoId(id);
        if (pago == null)
        {
            TempData["Error"] = "Pago no encontrado.";
            return RedirectToAction("Index", "Contrato");
        }

        var estadoAnterior = pago.estado.ToString();
        pago.estado = EstadoPago.anulado;
        pago.fecha_pago = DateTime.Now; // Se establece la fecha de anulación
        _pagoRepo.ActualizarPago(pago);

        _auditoriaRepo.InsertarRegistroAuditoria(
            TipoAuditoria.Pago,
            pago.id,
            AccionAuditoria.Anular, 
            User.Identity.Name ?? "Anónimo"
        );

        TempData["Exito"] = "Pago anulado con éxito";
        return RedirectToAction("Index", new { contratoId = pago.id_contrato });
    }
}