using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;

namespace RecordStore.Api.Services.Users;

public interface IUserService
{
    Task<UserResponse> GetCurrentUserAsync();
}