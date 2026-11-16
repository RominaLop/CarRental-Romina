
using CarRental.Data;
using CarRental.Dtos;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenService _jwtService;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AuthController(
            AppDbContext context,
            JwtTokenService jwtService,
            IPasswordHasher<AppUser> passwordHasher)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("El nombre de usuario ya existe.");
            }

            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return Created("", new
            {
                message = "Usuario registrado correctamente.",
                user.Username,
                user.Email,
                user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Inicio de sesión exitoso.",
                token,
                username = user.Username,
                role = user.Role
            });
        }
    }
}
