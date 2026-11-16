
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        [HttpGet("publico")]
        public IActionResult Publico()
        {
            return Ok("Este endpoint es público.");
        }

        [Authorize]
        [HttpGet("privado")]
        public IActionResult Privado()
        {
            return Ok(new
            {
                message = "Acceso autorizado.",
                usuario = User.Identity?.Name,
                rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin")]
        public IActionResult SoloAdmin()
        {
            return Ok("Acceso permitido: eres administrador.");
        }
    }
}
