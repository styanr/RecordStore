using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
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
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetUsers([FromQuery] GetUserQueryParams queryParams)
    {
        var users = await _userService.GetUsersAsync(queryParams);
        
        return users;
    }
    
    [HttpGet]
    [Route("roles")]
    public async Task<ActionResult<List<RoleResponse>>> GetRoles()
    {
        var roles = await _userService.GetRolesAsync();
        
        return roles;
    }
    
    [HttpPut]
    [Route("{userId}/role")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserResponse>> UpdateUserRole(int userId, [FromBody] UserUpdateRoleRequest request)
    {
        var user = await _userService.UpdateUserRoleAsync(userId, request);
        
        return user;
    }
    
    [HttpDelete]
    [Route("{userId}")]
    [Authorize(Roles="admin")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        await _userService.DeleteUserAsync(userId);
        
        return NoContent();
    }
}