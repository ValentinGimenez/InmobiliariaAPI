using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers;

[Authorize]
public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IRepositorioInmueble _repositorioInmueble;
    private readonly IRepositorioPropietario _repositorioPropietario;
    private readonly IRepositorioTipoInmueble _repositorioTipoInmueble;

    public InmuebleController(
        ILogger<InmuebleController> logger,
        IRepositorioInmueble repositorioInmueble,
        IRepositorioPropietario repositorioPropietario,
        IRepositorioTipoInmueble repositorioTipoInmueble
    )
    {
        _logger = logger;
        _repositorioInmueble = repositorioInmueble;
        _repositorioPropietario = repositorioPropietario;
        _repositorioTipoInmueble = repositorioTipoInmueble;
    }

    public IActionResult Index()
    {
        var listaInmuebles = _repositorioInmueble.ObtenerInmuebles();
        var propietarios = _repositorioPropietario.ObtenerPropietarios()
            .Where(p => p.estado == 1)
            .Select(p => new {
                Id = p.id,
                Nombre = $"{p.nombre} {p.apellido}"
            }).ToList();
    
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "Nombre");
        return View(listaInmuebles);
    }

    public IActionResult Activar(int id)
    {
        _repositorioInmueble.ActivarOferta(id);
        return RedirectToAction("Index");
    }
  
    public IActionResult Detalles(int id)
    {
        var inmueble = _repositorioInmueble.ObtenerInmuebleId(id);
        return View(inmueble);
    }

    public IActionResult PorPropietario(int propietarioId)
    {
        var listaInmuebles = _repositorioInmueble.ObtenerInmueblesPorPropietario(propietarioId);

        var propietarios = _repositorioPropietario.ObtenerPropietarios()
            .Select(p => new {
                Id = p.id,
                Nombre = $"{p.nombre} {p.apellido}"
            }).ToList();

        ViewBag.Propietarios = new SelectList(propietarios, "Id", "Nombre", propietarioId);
        ViewBag.PropietarioId = propietarioId;

        return View("Index", listaInmuebles);
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        var propietarios = _repositorioPropietario.ObtenerPropietariosActivos();
        var tipos = _repositorioTipoInmueble.ObtenerTiposInmueble()
            .Where(t => t.estado == 1)
            .ToList();

        ViewBag.Propietarios = new SelectList(propietarios, "id", "NombreCompleto");
        ViewBag.TiposInmueble = new SelectList(tipos, "id", "tipo");
        return View();
    }

    [HttpGet]
    public IActionResult Editar(int id)
    {
        var inmuebleSeleccionado = _repositorioInmueble.ObtenerInmuebleId(id);
        return View(inmuebleSeleccionado);
    }

    public IActionResult Suspender(int id)
    {
        _repositorioInmueble.SuspenderOferta(id);
        return RedirectToAction("Index");
    }
   
    [HttpPost]
    public IActionResult Editar(Inmueble inmuebleEditado)
    {
        TempData["Exito"] = "Datos guardados con éxito";
        _repositorioInmueble.ActualizarInmueble(inmuebleEditado);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Agregar(Inmueble inmuebleNuevo)
    {
        if (ModelState.IsValid)
        {
            _repositorioInmueble.AgregarInmueble(inmuebleNuevo);
            TempData["Exito"] = "Inmueble agregado con éxito";
            return RedirectToAction("Index");
        }

        if (inmuebleNuevo.id_propietario <= 0)
        {
            ModelState.AddModelError("id_propietario", "Debe seleccionar un propietario");
        }
        if (inmuebleNuevo.id_tipo <= 0)
        {
            ModelState.AddModelError("id_tipo", "Debe seleccionar un tipo de inmueble");
        }

        var tipos = _repositorioTipoInmueble.ObtenerTiposInmueble()
            .Where(t => t.estado == 1)
            .ToList();

        var propietarios = _repositorioPropietario.ObtenerPropietarios()
            .Where(p => p.estado == 1)
            .ToList();

        ViewBag.TiposInmueble = new SelectList(tipos, "id", "tipo", inmuebleNuevo.id_tipo);
        ViewBag.Propietarios = new SelectList(propietarios, "id", "NombreCompleto");

        return View("Agregar", inmuebleNuevo);
    }
}
