using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _net_integrador.Models;
using _net_integrador.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace _net_integrador.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IRepositorioUsuario _usuarioRepo;

    public UsuarioController(ILogger<UsuarioController> logger, IRepositorioUsuario usuarioRepo)
    {
        _logger = logger;
        _usuarioRepo = usuarioRepo;
    }

    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        var listaUsuarios = _usuarioRepo.ObtenerUsuarios();
        return View(listaUsuarios);
    }

    [Authorize(Policy = "Administrador")]
    [HttpGet]
    public IActionResult Agregar()
    {
        return View();
    }

    [Authorize(Policy = "Administrador")]
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuarioSeleccionado = _usuarioRepo.ObtenerUsuarioId(id);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (usuarioSeleccionado == null) return NotFound();
        if (usuarioSeleccionado.rol == "Admin" && 
            (userIdClaim == null || usuarioSeleccionado.id != int.Parse(userIdClaim.Value)))
        {
            TempData["Error"] = "No puedes editar a otro administrador.";
            return RedirectToAction("Index");
        }

        ViewBag.Accion = "Editar";
        return View(usuarioSeleccionado);
    }

    [Authorize(Policy = "Administrador")]
    public IActionResult Eliminar(int id)
    {
        var usuario = _usuarioRepo.ObtenerUsuarioId(id);
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (usuario == null) return NotFound();

        if (userIdClaim != null && usuario.id == int.Parse(userIdClaim.Value))
        {
            TempData["Error"] = "No puedes desactivarte a ti mismo.";
            return RedirectToAction("Index");
        }

        if (usuario.rol == "Admin")
        {
            TempData["Error"] = "No puedes desactivar a otro administrador.";
            return RedirectToAction("Index");
        }

        _usuarioRepo.EliminarUsuario(id);
        TempData["Exito"] = "Usuario desactivado con éxito";
        return RedirectToAction("Index");
    }


    [Authorize(Policy = "Administrador")]
    public IActionResult Activar(int id)
    {
        _usuarioRepo.ActivarUsuario(id);
        TempData["Exito"] = "Usuario activado con éxito";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Editar(Usuario usuarioEditado)
    {
        if (ModelState.IsValid)
        {
            var usuarioOriginal = _usuarioRepo.ObtenerUsuarioId(usuarioEditado.id);
            if (usuarioOriginal == null) return NotFound();

            if (usuarioOriginal.rol == "Admin")
            {
                usuarioEditado.rol = "Admin";
            }

            if (string.IsNullOrEmpty(usuarioEditado.avatar))
            {
                string nombreCompleto = $"{usuarioEditado.nombre} {usuarioEditado.apellido}";
                usuarioEditado.avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nombreCompleto)}&background=343a40&color=fff&rounded=true&size=128";
            }

            _usuarioRepo.ActualizarUsuario(usuarioEditado);
            TempData["Exito"] = "Datos guardados con éxito";
            return RedirectToAction("Index");
        }

        return View("Editar", usuarioEditado);
    }

    [HttpPost]
    [Authorize(Policy = "Administrador")]
    public IActionResult Agregar(Usuario usuarioNuevo)
    {
        if (ModelState.IsValid)
        {
            string nombreCompleto = $"{usuarioNuevo.nombre} {usuarioNuevo.apellido}";
            usuarioNuevo.avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nombreCompleto)}&background=343a40&color=fff&rounded=true&size=128";
            _usuarioRepo.AgregarUsuario(usuarioNuevo);
            TempData["Exito"] = "Usuario agregado con éxito";
            return RedirectToAction("Index");
        }
        return View("Agregar", usuarioNuevo);
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index");
        }
        return View();
    }

[HttpPost]
public async Task<IActionResult> Login(string email, string password)
{
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        ViewBag.Error = "Debe ingresar el email y la contraseña.";
        return View();
    }

    var usuario = _usuarioRepo.ObtenerUsuarioEmail(email);

    if (usuario == null || usuario.password != password)
    {
        ViewBag.Error = "El email o la contraseña son incorrectos.";
        return View();
    }

    if (usuario.estado == 0)
    {
        ViewBag.Error = "El usuario se encuentra desactivado. Contacte al administrador.";
        return View();
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.email),
        new Claim(ClaimTypes.Role, usuario.rol),
        new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString())
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var authProperties = new AuthenticationProperties
    {
        IsPersistent = false
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties
    );

    TempData["Exito"] = "Inicio de sesión exitoso.";
    return RedirectToAction("Index", "Home");
}


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpGet]
    public IActionResult EditarPerfil()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToAction("Login");
        }

        int usuarioId = Convert.ToInt32(userIdClaim.Value);
        var usuario = _usuarioRepo.ObtenerUsuarioId(usuarioId);

        if (usuario == null)
        {
            return RedirectToAction("Login");
        }
        ViewBag.Accion = "EditarPerfil";
        return View("Editar", usuario);
    }

    [Authorize]
    [HttpPost]
    public IActionResult EditarPerfil(Usuario usuarioEditado)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToAction("Login");
        }

        int usuarioId = Convert.ToInt32(userIdClaim.Value);

        if (usuarioId != usuarioEditado.id)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            var usuarioOriginal = _usuarioRepo.ObtenerUsuarioId(usuarioId);
            if (usuarioOriginal != null)
            {
                usuarioEditado.rol = usuarioOriginal.rol;
                usuarioEditado.estado = usuarioOriginal.estado;
            }

            if (string.IsNullOrEmpty(usuarioEditado.avatar))
            {
                string nombreCompleto = $"{usuarioEditado.nombre} {usuarioEditado.apellido}";
                usuarioEditado.avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nombreCompleto)}&background=343a40&color=fff&rounded=true&size=128";
            }

            _usuarioRepo.ActualizarUsuario(usuarioEditado);
            TempData["Exito"] = "Tu perfil se ha actualizado correctamente.";
            return RedirectToAction("Index", "Home");
        }

        return View("Editar", usuarioEditado);
    }

}