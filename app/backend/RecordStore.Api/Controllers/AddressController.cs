using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Address;
using RecordStore.Api.Entities;
using RecordStore.Api.Services.Addresses;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/user/address")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAddress(AddressRequest request)
    {
        await _addressService.AddAddressAsync(request);
        
        return Created();
    }
    
    [HttpPut]
    public async Task<ActionResult<AddressResponse>> UpdateAddress(AddressUpdateRequest request)
    {
        var response = await _addressService.UpdateAddressAsync(request);
        
        return response;
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _addressService.DeleteAddressAsync(id);

        return NoContent();
    }
    
    [HttpGet]
    public async Task<List<AddressResponse>> GetAddresses()
    {
        var addresses = await _addressService.GetAllAsync();
        
        return addresses;
    }
    
    [HttpGet]
    [Route("/regions")]
    public async Task<List<RegionResponse>> GetRegions(string? name = null)
    {
        var regions = await _addressService.GetRegionsAsync(name);
        
        return regions;
    }
}