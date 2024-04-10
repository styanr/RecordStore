using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        await _userService.RegisterAsync(userRegisterDto);
        
        return Ok();
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var token = await _userService.LoginAsync(userLoginDto);
        
        return Ok(new { token });
    }
}