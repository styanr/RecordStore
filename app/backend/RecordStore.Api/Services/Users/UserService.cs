using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RecordStore.Api.Context;
using RecordStore.Api.Entities;

namespace RecordStore.Api.Services.Users;

public class UserService : IUserService
{
    private readonly RecordStoreContext _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(IHttpContextAccessor contextAccessor, RecordStoreContext context)
    {
        _contextAccessor = contextAccessor;
        _context = context;
    }

    public async Task<AppUser?> GetCurrentUserAsync()
    {
        var userIdString = _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;

        if (string.IsNullOrEmpty(userIdString))
            throw new InvalidOperationException("User ID not found in claims.");

        var userId = int.Parse(userIdString);

        return await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}