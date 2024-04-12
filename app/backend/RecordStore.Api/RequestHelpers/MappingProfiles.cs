using AutoMapper;
using RecordStore.Api.Dto.Address;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Cart;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Dto.Genres;
using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Dto.Records;
using RecordStore.Api.Dto.Tracks;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;

namespace RecordStore.Api.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Artist, ArtistResponseDto>();
        CreateMap<Artist, ArtistFullResponseDto>();
        
        CreateMap<Genre, GenreResponseDto>();
        
        CreateMap<Format, FormatResponseDto>()
            .ForMember(s => s.Name, opt => opt.MapFrom(d => d.FormatName));
        CreateMap<TrackProduct, TrackResponseDTO>()
            .ForMember(s => s.Title, opt => opt.MapFrom(d => d.Track.Title))
            .ForMember(s => s.DurationSeconds, opt => opt.MapFrom(d => d.Track.DurationSeconds));

        CreateMap<Record, RecordResponseDto>();
        CreateMap<Record, RecordFullResponseDto>()
            .ForMember(s => s.Artists, opt => opt.MapFrom(d => d.Artists))
            .ForMember(s => s.Genres, opt => opt.MapFrom(d => d.Genres));
        
        CreateMap<Product, ProductResponseDto>()
            .ForMember(s => s.Title, opt => opt.MapFrom(d => d.Record.Title))
            .ForMember(s => s.Artists, opt => opt.MapFrom(d => d.Record.Artists))
            .ForMember(s => s.Genres, opt => opt.MapFrom(d => d.Record.Genres))
            .ForMember(s => s.Format, opt => opt.MapFrom(d => d.Format))
            .ForMember(s => s.ReleaseDate, opt => opt.MapFrom(d => d.Record.ReleaseDate));

        CreateMap<Product, ProductFullResponseDto>()
            .ForMember(s => s.Title, opt => opt.MapFrom(d => d.Record.Title))
            .ForMember(s => s.Artists, opt => opt.MapFrom(d => d.Record.Artists))
            .ForMember(s => s.Genres, opt => opt.MapFrom(d => d.Record.Genres))
            .ForMember(s => s.Format, opt => opt.MapFrom(d => d.Format))
            .ForMember(s => s.ReleaseDate, opt => opt.MapFrom(d => d.Record.ReleaseDate))
            .ForMember(s => s.Tracklist, opt => opt.MapFrom(d => d.TrackProducts));

        CreateMap<OrderStatus, OrderStatusResponse>();
        CreateMap<OrderLine, OrderLineResponse>()
            .ForMember(s => s.Product, opt => opt.MapFrom(d => d.Product));

        CreateMap<ShopOrder, OrderResponse>();
        
        CreateMap<ShoppingCartProduct, CartItemResponse>()
            .ForMember(s => s.Product, opt => opt.MapFrom(d => d.Product));
        
        CreateMap<ShoppingCart, CartResponse>()
            .ForMember(s => s.Items, opt => 
                opt.MapFrom(d => d.ShoppingCartProducts))
            .ForMember(s => s.TotalPrice, opt => 
                opt.MapFrom(d => d.ShoppingCartProducts.Sum(p => p.Product.Price * p.Quantity)));

        CreateMap<Region, RegionResponse>();
        
        CreateMap<AddressRequest, Address>();
        CreateMap<Address, AddressResponse>();
        CreateMap<AddressUpdateRequest, Address>()
            .ForMember(s => s.RegionId, opt => opt.MapFrom(d => d.RegionId));
    }
}