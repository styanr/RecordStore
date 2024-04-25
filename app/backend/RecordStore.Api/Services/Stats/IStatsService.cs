using RecordStore.Api.Dto.Stats;

namespace RecordStore.Api.Services.Stats;

public interface IStatsService
{
    public Task<List<OrderDateStats>> GetOrderStatsAsync(string period);
    public Task<List<FinancialDateStats>> GetFinancialStatsAsync(string period);
    public Task<FinancialStats> GetFinancialStatsAsync();
}