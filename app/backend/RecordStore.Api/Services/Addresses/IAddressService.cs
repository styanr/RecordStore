using RecordStore.Api.Dto.Address;

namespace RecordStore.Api.Services.Addresses;

public interface IAddressService
{
    public Task AddAddressAsync(AddressRequest request);
    public Task<AddressResponse> UpdateAddressAsync(AddressUpdateRequest request);
    public Task DeleteAddressAsync(int id);
    public Task<List<AddressResponse>> GetAllAsync();
    /*public Task<List<RegionResponse>> GetRegionsAsync(string? name = null);*/
}