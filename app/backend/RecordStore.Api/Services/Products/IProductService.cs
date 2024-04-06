using RecordStore.Api.DTO.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;

namespace RecordStore.Api.Services.Products;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(GetProductQueryParams queryParams);
    Task<ProductFullResponseDto> GetByIdAsync(int id);
    Task<ProductFullResponseDto> CreateAsync(Product entity);
    Task<ProductFullResponseDto> UpdateAsync(Product entity);
    Task<bool> DeleteAsync(int id);
}