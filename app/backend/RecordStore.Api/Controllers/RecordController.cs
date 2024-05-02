using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Records;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;
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
    public async Task<PagedResult<RecordResponseDto>> GetAll([FromQuery] GetRecordQueryParams queryParams)
    {
        return await _recordService.GetAllAsync(queryParams);
    }

    [HttpGet("{id}")]
    public async Task<RecordFullResponseDto> GetById(int id)
    {
        return await _recordService.GetByIdAsync(id);
    }
    
    [Route("~/api/artists/{artistId}/records")]
    [HttpGet]
    public async Task<IEnumerable<RecordResponseDto>> GetByArtistId(int artistId, [FromQuery] GetRecordQueryParams queryParams)
    {
        return await _recordService.GetByArtistIdAsync(artistId, queryParams);
    }
    
    [HttpPost]
    public async Task<RecordFullResponseDto> Create([FromBody] RecordCreateRequest request)
    {
        return await _recordService.CreateAsync(request);
    }
    
    [HttpPut("{id}")]
    public async Task<RecordFullResponseDto> Update(int id, [FromBody] RecordUpdateRequest request)
    {
        return await _recordService.UpdateAsync(id, request);
    }
    
    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        await _recordService.DeleteAsync(id);
    }
}