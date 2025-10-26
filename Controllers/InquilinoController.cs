using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly ILogger<InquilinoController> _logger;
        private readonly IRepositorioInquilino _inquilinoRepo;

        public InquilinoController(ILogger<InquilinoController> logger, IRepositorioInquilino inquilinoRepo)
        {
            _logger = logger;
            _inquilinoRepo = inquilinoRepo;
        }

        public IActionResult Index()
        {
            var listaInquilinos = _inquilinoRepo.ObtenerInquilinos();
            return View(listaInquilinos);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(Inquilino inquilinoNuevo)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilinoNuevo);
            }
            else
            {
                try
                {
                    bool error = false;
                    if (_inquilinoRepo.ExisteDni(inquilinoNuevo.dni))
                    {
                        ModelState.AddModelError("dni", "Este DNI ya está registrado");
                        error = true;
                    }

                    if (_inquilinoRepo.ExisteEmail(inquilinoNuevo.email))
                    {
                        ModelState.AddModelError("email", "Este email ya está registrado");
                        error = true;
                    }
                    if (error)
                    {
                        return View();
                    }
                    inquilinoNuevo.nombre = inquilinoNuevo.nombre?.ToUpper() ?? "";
                    inquilinoNuevo.apellido = inquilinoNuevo.apellido?.ToUpper() ?? "";
                    inquilinoNuevo.email = inquilinoNuevo.email?.ToLower() ?? "";
                    inquilinoNuevo.estado = 1;
                    _inquilinoRepo.AgregarInquilino(inquilinoNuevo);
                    TempData["Exito"] = "Datos guardados con éxito";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var inquilinoSeleccionado = _inquilinoRepo.ObtenerInquilinoId(id);
            if (inquilinoSeleccionado == null) return NotFound();
            return View(inquilinoSeleccionado);
        }

        [HttpPost]
        public IActionResult Editar(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }
            else
            {
                try
                {


                    bool error = false;
                    if (_inquilinoRepo.ExisteDni(inquilino.dni, inquilino.id))
                    {
                        ModelState.AddModelError("dni", "Este DNI ya está registrado");
                        error = true;
                    }

                    if (_inquilinoRepo.ExisteEmail(inquilino.email, inquilino.id))
                    {
                        ModelState.AddModelError("email", "Este email ya está registrado");
                        error = true;
                    }
                    if (error)
                    {
                        return View();
                    }
                    inquilino.nombre = inquilino.nombre?.ToUpper() ?? "";
                    inquilino.apellido = inquilino.apellido?.ToUpper() ?? "";
                    inquilino.email = inquilino.email?.ToLower() ?? "";
                    _inquilinoRepo.ActualizarInquilino(inquilino);
                    TempData["Exito"] = "Datos guardados con éxito";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }

        }

        public IActionResult Eliminar(int id)
        {
            bool eliminacionExitosa = _inquilinoRepo.EliminarInquilino(id);

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
            _inquilinoRepo.ActivarInquilino(id);
            return RedirectToAction("Index");
        }
    }
}
