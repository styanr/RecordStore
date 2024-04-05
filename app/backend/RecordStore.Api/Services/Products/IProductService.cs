using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;

namespace RecordStore.Api.Services.Products;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(GetProductQueryParams queryParams);
    Task<Product> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product entity);
    Task<Product> UpdateAsync(Product entity);
    Task<bool> DeleteAsync(int id);
}