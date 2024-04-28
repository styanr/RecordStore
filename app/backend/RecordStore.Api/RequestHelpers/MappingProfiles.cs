using AutoMapper;
using RecordStore.Api.Dto.Address;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Cart;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Dto.Genres;
using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Dto.Records;
using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.Dto.Tracks;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;

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
            .ForMember(s => s.ReleaseDate, opt => opt.MapFrom(d => d.Record.ReleaseDate));
        
        CreateMap<OrderStatus, OrderStatusDto>();
        CreateMap<OrderLine, OrderLineResponse>()
            .ForMember(s => s.Product, opt => opt.MapFrom(d => d.Product));

        CreateMap<ShopOrder, OrderResponse>()
            .ForMember(s => s.Items, opt => opt.MapFrom(d => d.OrderLines));
        
        CreateMap<ShoppingCartProduct, CartItemResponse>()
            .ForMember(s => s.Product, opt => opt.MapFrom(d => d.Product));
        
        CreateMap<ShoppingCart, CartResponse>()
            .ForMember(s => s.Items, opt => 
                opt.MapFrom(d => d.ShoppingCartProducts))
            .ForMember(s => s.TotalPrice, opt => 
                opt.MapFrom(d => d.ShoppingCartProducts.Sum(p => p.Product.Price * p.Quantity)));
        
        CreateMap<AddressRequest, Address>();
        CreateMap<Address, AddressResponse>();
        CreateMap<AddressUpdateRequest, Address>();
            
        
        CreateMap<Review, ReviewResponse>()
            .ForMember(s => s.UserFullName, opt => opt.MapFrom(d => d.User.FirstName + " " + d.User.LastName));
        
        CreateMap<CreateReviewRequest, Review>();
        
        CreateMap<PagedResult<Product>, PagedResult<ProductResponseDto>>();
        CreateMap<PagedResult<ShopOrder>, PagedResult<OrderResponse>>();
        
        CreateMap<AppUser, UserResponse>().ForMember(s => s.Role, opt => opt.MapFrom(d => d.Role.RoleName));

        CreateMap<Supplier, SupplierResponse>();
        
        CreateMap<PurchaseOrderLine, PurchaseOrderLineResponse>();
        CreateMap<PurchaseOrder, PurchaseOrderResponse>();
        
        CreateMap<PurchaseOrderLineCreateRequest, PurchaseOrderLine>();
        CreateMap<PurchaseOrderCreateRequest, PurchaseOrder>().AfterMap((request, order) =>
        {
            order.PurchaseOrderLines.ToList().ForEach(p => p.PurchaseOrderId = order.Id);
        });
        CreateMap<PagedResult<PurchaseOrder>, PagedResult<PurchaseOrderResponse>>();
    }
}