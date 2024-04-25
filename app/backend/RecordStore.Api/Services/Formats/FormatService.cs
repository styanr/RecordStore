using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Formats;

namespace RecordStore.Api.Services.Formats;

public class FormatService : IFormatService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public FormatService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<FormatResponseDto>> GetFormatsAsync(string name)
    {
        var normalizedName = name.ToLower().Trim();
        
        var formats = await _context.Formats.Where(f => f.FormatName.ToLower().StartsWith(normalizedName)).ToListAsync();
        
        return _mapper.Map<List<FormatResponseDto>>(formats);
    }
}