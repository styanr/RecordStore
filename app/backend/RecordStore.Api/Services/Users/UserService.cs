using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;

namespace RecordStore.Api.Services.Users;

public class UserService : IUserService
{
    private readonly RecordStoreContext _context;
    private readonly IConfiguration _configuration;

    public UserService(RecordStoreContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
        
        var user = new AppUser
        {
            Email = userRegisterDto.Email,
            Password = hashedPassword,
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
        };
        
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task<string> LoginAsync(UserLoginDto userLoginDto)
    {
        
        var user = await _context
            .AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);
            
        
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            throw new Exception("Invalid password");
        }
        
        var token = JwtGenerator.GenerateJwtToken(user, _configuration);
        
        return token;
    }
}