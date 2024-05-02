using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Users;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync();
    
    Task<PagedResult<UserResponse>> GetUsersAsync(GetUserQueryParams queryParams);
    
    // get roles
    Task<List<RoleResponse>> GetRolesAsync();
    
    Task<UserResponse> UpdateUserRoleAsync(int userId, UserUpdateRoleRequest request);
    
    Task DeleteUserAsync(int userId);
}