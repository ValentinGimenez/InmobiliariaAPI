using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly ILogger<PropietarioController> _logger;
        private readonly IRepositorioPropietario _repositorio;

        public PropietarioController(ILogger<PropietarioController> logger, IRepositorioPropietario repositorio)
        {
            _logger = logger;
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var listaPropietarios = _repositorio.ObtenerPropietarios();
            return View(listaPropietarios);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var propietarioSeleccionado = _repositorio.ObtenerPropietarioId(id);
            if (propietarioSeleccionado == null)
            {
                return NotFound();
            }
            return View(propietarioSeleccionado);
        }

        public IActionResult Eliminar(int id)
        {
            bool eliminacionExitosa = _repositorio.EliminarPropietario(id);

            if (eliminacionExitosa)
            {
                TempData["Exito"] = "Inquilino eliminado correctamente";
            }
            else
            {
                TempData["Error"] = "No se puede eliminar el inquilino porque tiene un contrato activo";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Activar(int id)
        {
            _repositorio.ActivarPropietario(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Editar(Propietario propietario)
        {
            if (!ModelState.IsValid)
            {
                return View(propietario);
            }
            else
            {
                try
                {
                    bool error = false;
                    if (_repositorio.ExisteDni(propietario.dni, propietario.id))
                    {
                        ModelState.AddModelError("dni", "Este dni ya está registrado");
                        error = true;
                    }
                    if (_repositorio.ExisteEmail(propietario.email, propietario.id))
                    {
                        ModelState.AddModelError("email", "Este email ya está registrado");
                        error = true;
                    }
                    if (error)
                    {
                        return View(propietario);
                    }
                    propietario.nombre = propietario.nombre?.ToUpper() ?? "";
                    propietario.apellido = propietario.apellido?.ToUpper() ?? "";
                    propietario.email = propietario.email?.ToLower() ?? "";
                    _repositorio.ActualizarPropietario(propietario);
                    TempData["Exito"] = "Datos guardados con éxito";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost]
        public IActionResult Agregar(Propietario propietarioNuevo)
        {
            if (ModelState.IsValid)
            {
                bool error = false;
                if (_repositorio.ExisteDni(propietarioNuevo.dni))
                {
                    ModelState.AddModelError("dni", "Este DNI ya está registrado");
                    error = true;
                }

                if (_repositorio.ExisteEmail(propietarioNuevo.email))
                {
                    ModelState.AddModelError("email", "Este email ya está registrado");
                    error = true;
                }
                if (error)
                {
                    return View();
                }

                propietarioNuevo.estado = 1;
                propietarioNuevo.nombre = propietarioNuevo.nombre.ToUpper();
                propietarioNuevo.apellido = propietarioNuevo.apellido.ToUpper();
                propietarioNuevo.email = propietarioNuevo.email.ToLower();
                _repositorio.AgregarPropietario(propietarioNuevo);

                TempData["Exito"] = "Propietario agregado con éxito";
                return RedirectToAction("Index");
            }
            return View("Agregar", propietarioNuevo);
        }
    }
}