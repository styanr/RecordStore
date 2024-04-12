using RecordStore.Api.Entities;

namespace RecordStore.Api.Services.Users;

public interface IUserService
{
    Task<AppUser?> GetCurrentUserAsync();
}