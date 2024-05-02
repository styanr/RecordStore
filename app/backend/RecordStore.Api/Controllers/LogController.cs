using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Logs;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Logs;

namespace RecordStore.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/logs")]
public class LogController
{
    private readonly ILogService _logService;

    public LogController(ILogService logService)
    {
        _logService = logService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<LogResponse>>> GetAll([FromQuery] GetLogQueryParams queryParams)
    {
        var logs = await _logService.GetAllAsync(queryParams);

        return logs;
    }
}