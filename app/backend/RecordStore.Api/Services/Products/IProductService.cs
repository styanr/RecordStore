using RecordStore.Api.Dto.Labels;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Products;

public interface IProductService
{
    Task<PagedResult<ProductShortResponseDto>> GetAllAsync(GetProductQueryParams queryParams);
    Task<ProductResponseDto> GetByIdAsync(int id);
    Task<List<ProductShortResponseDto>> GetByRecordIdAsync(int recordId, GetRecordProductQueryParams queryParams);
    Task<ProductFullResponseDto> CreateAsync(ProductCreateRequest entity);
    Task<ProductFullResponseDto> UpdateAsync(int id, ProductUpdateRequest entity);
    
    Task<PriceMinMaxResponse> GetPriceMinMaxAsync();
    
    Task<List<LabelResponse>> GetLabelsAsync(string name);
    Task<bool> DeleteAsync(int id);
}