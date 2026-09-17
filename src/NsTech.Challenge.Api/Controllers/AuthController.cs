namespace NsTech.Challenge.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using NsTech.Challenge.Api.Services;

public record LoginRequest(string Username, string Password);

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Mock simples de autenticação para o teste técnico
        if (request.Username == "nstech" && request.Password == "admin123")
        {
            var secretKey = _configuration["Jwt:SecretKey"] ?? "NsTechSuperSecretJwtKey2026_Minimum32Bytes!";
            var token = TokenService.GenerateToken(request.Username, secretKey);
            return Ok(new { token });
        }

        return Unauthorized(new { message = "Invalid credentials." });
    }
}