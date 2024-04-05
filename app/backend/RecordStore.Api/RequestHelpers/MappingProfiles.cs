using AutoMapper;
using RecordStore.Api.DTO.Artists;
using RecordStore.Api.DTO.Formats;
using RecordStore.Api.DTO.Genres;
using RecordStore.Api.DTO.Products;
using RecordStore.Api.Entities;

namespace RecordStore.Api.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Artist, ArtistResponseDto>();
        CreateMap<Genre, GenreResposeDto>();
        CreateMap<Format, FormatResponseDto>();
        CreateMap<Product, ProductResponseDTO>()
            .ForMember(s => s.Title, opt => opt.MapFrom(d => d.Record.Title))
            .ForMember(s => s.Artists, opt => opt.MapFrom(d => d.Record.Artists))
            .ForMember(s => s.Genres, opt => opt.MapFrom(d => d.Record.Genres))
            .ForMember(s => s.Format, opt => opt.MapFrom(d => d.Format))
            .ForMember(s => s.ReleaseDate, opt => opt.MapFrom(d => d.Record.ReleaseDate));
    }
}