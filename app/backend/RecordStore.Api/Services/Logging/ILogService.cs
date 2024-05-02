using RecordStore.Api.Dto.Logs;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Logs;

public interface ILogService
{
    Task<List<LogResponse>> GetAllAsync(GetLogQueryParams queryParams);
    
    Task<LogResponse> CreateAsync(LogCreateRequest entity);
    Task LogActionAsync(string action, string description);
}