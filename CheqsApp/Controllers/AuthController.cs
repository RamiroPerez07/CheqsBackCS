using CheqsApp.Contexts;
using CheqsApp.DTO;
using CheqsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace CheqsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
        {
            // Validar los datos del usuario
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest("El nombre de usuario ya está en uso.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest("El correo electrónico ya está registrado.");
            }

            // Crear un nuevo usuario
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Role = UserRole.User, // Puedes ajustar esto según lo que necesites
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Generar el hash de la contraseña
            user.PasswordHash = HashPassword(model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generar el token JWT
            var token = GenerateJwtToken(user);

            // Responder con el token
            return Ok(new { Token = token });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
        {
            // Buscar al usuario en la base de datos
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
            {
                return Unauthorized("Nombre de usuario o contraseña incorrectos.");
            }

            // Verificar la contraseña
            if (!VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Nombre de usuario o contraseña incorrectos.");
            }

            // Generar el token JWT
            var token = GenerateJwtToken(user);

            // Devolver el token
            return Ok(new { 
                Token = token, 
                user.Username,  
                user.Email,
                Rol = user.Role.ToString(),
            });
        }


        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Authorize]  // Esto requiere que el usuario esté autenticado
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            // Obtener el nombre de usuario del usuario autenticado
            var username = User.Identity?.Name;

            // Buscar al usuario en la base de datos
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Verificar la contraseña actual
            if (!VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                return Unauthorized("La contraseña actual es incorrecta.");
            }

            // Validar que la nueva contraseña cumpla con los requisitos de seguridad, si es necesario

            // Actualizar la contraseña
            user.PasswordHash = HashPassword(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada exitosamente.");
        }

        // Método para verificar si la contraseña coincide con el hash
        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
                new System.Security.Claims.Claim("id", user.Id.ToString())
            };

            var secretKey = _configuration["Jwt:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    
}
