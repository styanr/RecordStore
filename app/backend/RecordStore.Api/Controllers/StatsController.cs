using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Stats;
using RecordStore.Api.Services.Stats;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/admin/stats")]
[Authorize(Roles = "admin")]
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
        var financialStats = await _statsService.GetFinancialSummaryAsync();
        
        return financialStats;
    }
    
    [HttpGet]
    [Route("orders/{id}/{period}")]
    public async Task<ActionResult<List<OrderDateStats>>> GetOrdersStats(int id, string period)
    {
        var orderStats = await _statsService.GetOrderStatsAsync(id, period);
        
        return orderStats;
    }
    
    [HttpGet]
    [Route("products/{id}/quantity-sold/{period}")]
    public async Task<ActionResult<List<ProductQuantitySoldStats>>>
        GetProductQuantitySoldStats(int id, string period)
    {
        var productQuantitySoldStats = await _statsService.GetProductQuantitySoldStatsAsync(id, period);
        
        return productQuantitySoldStats;
    }
    
    [HttpGet]
    [Route("average-order-value/{period}")]
    public async Task<ActionResult<List<AverageOrderValueStats>>>
        GetAverageOrderValueStats(string period)
    {
        var averageOrderValueStats = await _statsService.GetAverageOrderValueStatsAsync(period);
        
        return averageOrderValueStats;
    }
    
    [HttpGet]
    [Route("orders-per-region")]
    public async Task<ActionResult<List<OrdersPerRegionStats>>> GetOrdersPerRegionStats()
    {
        var ordersPerRegionStats = await _statsService.GetOrdersPerRegionStatsAsync();
        
        return ordersPerRegionStats;
    }
}