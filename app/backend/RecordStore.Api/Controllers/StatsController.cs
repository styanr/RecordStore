using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Stats;
using RecordStore.Api.Services.Stats;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/admin/stats")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }
    
    [HttpGet]
    [Route("orders/{period}")]
    public async Task<ActionResult<List<OrderDateStats>>> GetOrdersStats(string period)
    {
        var orderStats = await _statsService.GetOrderStatsAsync(period);
        
        return Ok(orderStats);
    }
    
    [HttpGet]
    [Route("financial/{period}")]
    public async Task<ActionResult<List<FinancialDateStats>>> GetFinancialStats(string period)
    {
        var financialStats = await _statsService.GetFinancialStatsAsync(period);
        
        return Ok(financialStats);
    }
    
    [HttpGet]
    [Route("financial")]
    public async Task<ActionResult<FinancialStats>> GetFinancialStats()
    {
        var financialStats = await _statsService.GetFinancialStatsAsync();
        
        return Ok(financialStats);
    }
}