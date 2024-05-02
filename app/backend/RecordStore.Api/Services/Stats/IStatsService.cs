using RecordStore.Api.Dto.Stats;

namespace RecordStore.Api.Services.Stats;

public interface IStatsService
{
    public Task<List<OrderDateStats>> GetOrderStatsAsync(string period);
    public Task<List<FinancialDateStats>> GetFinancialStatsAsync(string period);
    public Task<FinancialStats> GetFinancialSummaryAsync();
    public Task<List<OrderDateStats>> GetOrderStatsAsync(int id, string period);
    public Task<List<ProductQuantitySoldStats>> GetProductQuantitySoldStatsAsync(int id, string period);
    public Task<List<AverageOrderValueStats>> GetAverageOrderValueStatsAsync(string period);
    
    public Task<List<OrdersPerRegionStats>> GetOrdersPerRegionStatsAsync();
}