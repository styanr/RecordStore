using RecordStore.Api.Dto.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Products;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(GetProductQueryParams queryParams);
    Task<ProductFullResponseDto> GetByIdAsync(int id);
    Task<List<ProductResponseDto>> GetByRecordIdAsync(int recordId, GetRecordProductQueryParams queryParams);
    Task<ProductFullResponseDto> CreateAsync(Product entity);
    Task<ProductFullResponseDto> UpdateAsync(Product entity);
    Task<bool> DeleteAsync(int id);
}