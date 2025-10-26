using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers;

[Authorize]
public class TipoInmuebleController : Controller
{
    private readonly ILogger<TipoInmuebleController> _logger;
    private readonly IRepositorioTipoInmueble _tipoInmuebleRepo;

    public TipoInmuebleController(ILogger<TipoInmuebleController> logger, IRepositorioTipoInmueble tipoInmuebleRepo)
    {
        _logger = logger;
        _tipoInmuebleRepo = tipoInmuebleRepo;
    }

    public IActionResult Index()
    {
        var listaTiposInmueble = _tipoInmuebleRepo.ObtenerTiposInmueble();
        return View(listaTiposInmueble);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var tipoSeleccionado = _tipoInmuebleRepo.ObtenerTipoInmuebleId(id);
        return View(tipoSeleccionado);
    }

    [Authorize(Policy = "Administrador")]
    public IActionResult Desactivar(int id)
    {
        bool estaEnUso = _tipoInmuebleRepo.EstaEnUso(id);

        if (estaEnUso)
        {
            TempData["Error"] = "No puedes desactivar este tipo de inmueble porque está en uso en un inmueble.";
            return RedirectToAction("Index");
        }

        _tipoInmuebleRepo.DesactivarTipoInmueble(id);
        TempData["Exito"] = "Tipo de inmueble desactivado con éxito.";
        return RedirectToAction("Index");
    }

    [Authorize(Policy = "Administrador")]
    public IActionResult Activar(int id)
    {
        _tipoInmuebleRepo.ActivarTipoInmueble(id);
        TempData["Exito"] = "Tipo de inmueble activado con éxito.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Editar(TipoInmueble tipoEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        _tipoInmuebleRepo.ActualizarTipoInmueble(tipoEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(TipoInmueble tipoNuevo)
    {
        if (ModelState.IsValid)
        {
            _tipoInmuebleRepo.AgregarTipoInmueble(tipoNuevo);
            TempData["Exito"] = "Tipo de inmueble agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", tipoNuevo);
    }
}
