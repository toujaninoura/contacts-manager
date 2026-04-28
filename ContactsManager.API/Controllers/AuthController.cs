using ContactsManager.Application.DTOs.Auth;
using ContactsManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        _logger.LogInformation("POST /api/auth/register called for {Email}", dto.Email);

        var response = await _authService.RegisterAsync(dto);

        if (!response.Success)
            return BadRequest(response);

        return CreatedAtAction(nameof(Register), response);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("POST /api/auth/login called for {Email}", dto.Email);

        var response = await _authService.LoginAsync(dto);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }
}
