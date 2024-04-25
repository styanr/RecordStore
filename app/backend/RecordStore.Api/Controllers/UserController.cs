using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Route("current")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var user = await _userService.GetCurrentUserAsync();
        
        return user;
    }
}