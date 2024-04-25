﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Address;
using RecordStore.Api.Entities;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Addresses;

public class AddressService : IAddressService
{
    private readonly RecordStoreContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public AddressService(RecordStoreContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task AddAddressAsync(AddressRequest request)
    {
        var user = await _userService.GetCurrentUserAsync();

        if (user == null) throw new InvalidOperationException($"User not found.");

        var address = _mapper.Map<Address>(request);

        address.UserId = user.Id;

        _context.Addresses.Add(address);

        await _context.SaveChangesAsync();
    }

    public async Task<AddressResponse> UpdateAddressAsync(AddressUpdateRequest request)
    {
        var user = await _userService.GetCurrentUserAsync();

        if (user == null) throw new InvalidOperationException($"User not found.");

        var address = _context.Addresses.Find(request.Id);

        if (address == null) throw new InvalidOperationException($"Address with ID {request.Id} not found.");

        if (address.UserId != user.Id)
            throw new InvalidOperationException(
                $"Address with ID {request.Id} does not belong to user with ID {user.Id}.");

        address = _mapper.Map(request, address);
        address.UserId = user.Id;

        await _context.SaveChangesAsync();

        return _mapper.Map<AddressResponse>(address);
    }

    public async Task DeleteAddressAsync(int id)
    {
        var user = await _userService.GetCurrentUserAsync();

        if (user == null) throw new InvalidOperationException($"User not found.");

        var userId = user.Id;

        var address = await _context.Addresses.FindAsync(id);

        if (address == null) throw new InvalidOperationException($"Address with ID {id} not found.");

        if (address.UserId != userId)
            throw new InvalidOperationException($"Address with ID {id} does not belong to user with ID {userId}.");

        _context.Addresses.Remove(address);
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<AddressResponse>> GetAllAsync()
    {
        var user = await _userService.GetCurrentUserAsync();

        if (user == null) throw new InvalidOperationException($"User not found.");
        
        var userId = user.Id;

        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return _mapper.Map<List<AddressResponse>>(addresses);
    }
    
}