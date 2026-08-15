using LibraryManagementSystem.Application.DTOs.Auth;
using LibraryManagementSystem.Application.IServices.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> Refresh([FromBody] RefreshRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(result);
    }

    [HttpPost("revoke")]
    [Authorize] 
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto request)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken);
        return NoContent();
    }
}