using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Services.Formats;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/formats")]
public class FormatController
{
    private readonly IFormatService _formatService;

    public FormatController(IFormatService formatService)
    {
        _formatService = formatService;
    }
    [HttpGet]
    public async Task<ActionResult<List<FormatResponseDto>>> GetFormats(string name)
    {
        return await _formatService.GetFormatsAsync(name);
    }
}