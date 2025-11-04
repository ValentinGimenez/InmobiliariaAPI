using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // GET /api/test  → público
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get() => Ok(new { mensaje = "API OK ✅" });

        // GET /api/test/secure → requiere JWT (sirve para probar auth)
        [HttpGet("secure")]
        [Authorize]
        public IActionResult Secure() => Ok(new { mensaje = "Acceso con token válido 🔒" });
    }
}
