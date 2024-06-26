﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.Services.Logs;

namespace RecordStore.Api.Services.Users;

public class AuthService : IAuthService
{
    private readonly RecordStoreContext _context;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public AuthService(RecordStoreContext context, IConfiguration configuration, IMapper mapper, ILogService logService)
    {
        _context = context;
        _configuration = configuration;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<string> RegisterUserAsync(UserRegisterDto userRegisterDto)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
        var roleId = _context.Roles.FirstOrDefault(r => r.RoleName == "user")?.Id;

        if (roleId == null)
        {
            throw new InvalidOperationException("Role not found");
        }

        var emailExists = await _context.AppUsers.AnyAsync(u => u.Email == userRegisterDto.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new AppUser
        {
            Email = userRegisterDto.Email,
            Password = hashedPassword,
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
            RoleId = roleId.Value
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var token = JwtGenerator.GenerateJwtToken(user, _configuration);

        // Log the registration of a new user
        await _logService.LogActionAsync("Register User", $"New user registered with email: {userRegisterDto.Email}");

        return token;
    }

    public async Task<string> LoginAsync(UserLoginDto userLoginDto)
    {
        var user = await _context
            .AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == userLoginDto.Email);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
        {
            throw new InvalidOperationException("Invalid password");
        }

        var token = JwtGenerator.GenerateJwtToken(user, _configuration);

        // Log the user login
        await _logService.LogActionAsync("Login", $"User with email {userLoginDto.Email} logged in.");

        return token;
    }

    public async Task CreateUserAsync(UserCreateRequest request)
    {
        var user = _mapper.Map<AppUser>(request);

        var role = _context.Roles.FirstOrDefault(r => r.Id == request.RoleId);

        if (role is null)
        {
            throw new EntityNotFoundException("Role not found.");
        }

        user.Role = role;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        user.Password = hashedPassword;

        _context.AppUsers.Add(user);

        await _context.SaveChangesAsync();

        // Log the creation of a new user
        await _logService.LogActionAsync("Create User", $"New user created with email: {user.Email}");
    }
}