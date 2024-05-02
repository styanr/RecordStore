using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Users;

public class UserService : IUserService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(IHttpContextAccessor contextAccessor, RecordStoreContext context, IMapper mapper)
    {
        _contextAccessor = contextAccessor;
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetCurrentUserAsync()
    {
        var userIdString = _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

        if (string.IsNullOrEmpty(userIdString))
            throw new InvalidOperationException("User ID not found in claims.");

        var userId = int.Parse(userIdString);
        
        var user = await _context.AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
            throw new UserNotFoundException();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<PagedResult<UserResponse>> GetUsersAsync(GetUserQueryParams queryParams)
    {
        var query = _context.AppUsers.ApplyIncludes().ApplyFiltersAndOrderBy(queryParams);
        
        var pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        return _mapper.Map<PagedResult<UserResponse>>(pagedResult);
    }

    public async Task<List<RoleResponse>> GetRolesAsync()
    {
        var roles = await _context.Roles.ToListAsync();
        
        return _mapper.Map<List<RoleResponse>>(roles);
    }

    public async Task<UserResponse> UpdateUserRoleAsync(int userId, UserUpdateRoleRequest request)
    {
        var user = await _context.AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new UserNotFoundException();
        }
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId);
        
        if (role is null)
        {
            throw new EntityNotFoundException("Role not found.");
        }
        
        user.Role = role;
        
        await _context.SaveChangesAsync();
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user is null)
        {
            throw new UserNotFoundException();
        }
        
        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync();
    }
}