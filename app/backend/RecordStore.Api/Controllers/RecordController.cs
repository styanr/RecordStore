using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.Services;
using RecordStore.Api.Services.Records;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/records")]
public class RecordController
{
    private readonly IRecordService _recordService;

    public RecordController(IRecordService recordService)
    {
        _recordService = recordService;
    }
    
    [HttpGet]
    public async Task<IEnumerable<Record>> GetAll()
    {
        return await _recordService.GetAllAsync();
    }
}