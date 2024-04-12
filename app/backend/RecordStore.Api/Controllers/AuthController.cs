using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        await _authService.RegisterAsync(userRegisterDto);
        
        return Ok();
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var token = await _authService.LoginAsync(userLoginDto);
        
        return Ok(new { token });
    }
}