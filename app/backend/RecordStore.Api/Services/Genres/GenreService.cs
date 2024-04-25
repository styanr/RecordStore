using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Genres;

namespace RecordStore.Api.Services.Genres;

public class GenreService : IGenreService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public GenreService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<GenreResponseDto>> GetGenresAsync(string name)
    {
        var normalizedName = name.ToLower().Trim();
        
        var formats = await _context.Genres.Where(f => f.Name.ToLower().StartsWith(normalizedName)).ToListAsync();
        
        return _mapper.Map<List<GenreResponseDto>>(formats);
    }
}