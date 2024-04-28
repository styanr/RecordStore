using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;

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
}