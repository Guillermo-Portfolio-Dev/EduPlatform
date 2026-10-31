using EduPlatform.Gateway.Models;
using EduPlatform.Gateway.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduPlatform.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var dbUser = await _userRepository.GetByUsernameAsync(request.Username);

            if (dbUser == null || dbUser.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            // 1. Generar Access Token y Refresh Token
            var accessToken = GenerateJwtToken(dbUser);
            var refreshToken = GenerateRefreshToken();

            // 2. Guardar el Refresh Token en la base de datos (válido por 7 días)
            await _userRepository.SaveRefreshTokenAsync(dbUser.Username, refreshToken, DateTime.UtcNow.AddDays(7));

            // 3. Devolver ambos tokens al cliente
            return Ok(new
            {
                accessToken = accessToken,
                refreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            // 1. Validar si el refresh token existe y sigue activo en la BD
            var username = await _userRepository.GetUsernameByRefreshTokenAsync(request.RefreshToken);

            if (username == null)
            {
                return Unauthorized(new { message = "Refresh token inválido o expirado" });
            }

            var dbUser = await _userRepository.GetByUsernameAsync(username);
            if (dbUser == null) return Unauthorized();

            // 2. Rotación de Refresh Tokens: Revocar el viejo y generar uno nuevo
            await _userRepository.RevokeRefreshTokenAsync(request.RefreshToken);

            var newAccessToken = GenerateJwtToken(dbUser);
            var newRefreshToken = GenerateRefreshToken();
            await _userRepository.SaveRefreshTokenAsync(dbUser.Username, newRefreshToken, DateTime.UtcNow.AddDays(7));

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        private string GenerateJwtToken(UserDto dbUser)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, dbUser.Username),
                new Claim(ClaimTypes.Role, dbUser.Role) // Rol dinámico desde la BD
            }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
