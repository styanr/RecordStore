using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecordStore.Api.Context;
using RecordStore.Api.Extensions;
using RecordStore.Api.Filters;
using RecordStore.Api.Services.Addresses;
using RecordStore.Api.Services.Artists;
using RecordStore.Api.Services.Carts;
using RecordStore.Api.Services.Formats;
using RecordStore.Api.Services.Genres;
using RecordStore.Api.Services.Orders;
using RecordStore.Api.Services.Products;
using RecordStore.Api.Services.PurchaseOrders;
using RecordStore.Api.Services.Records;
using RecordStore.Api.Services.Reviews;
using RecordStore.Api.Services.Stats;
using RecordStore.Api.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<EntityNotFoundExceptionFilter>();
    options.Filters.Add<PostgresExceptionFilter>();
    options.Filters.Add<UnauthorizedExceptionFilter>();
    options.Filters.Add<InvalidOperationExceptionFilter>();
    options.Filters.Add<UnauthorizedAccessFilter>();
    // options.Filters.Add<GenericExceptionFilter>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new List<string>()
        }
    });
});
builder.Services.AddDbContext<RecordStoreContext>((services, optionsBuilder) =>
{
    var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
    var role = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;


    var connectionString = role switch
    {
        "user" => builder.Configuration.GetConnectionString("UserConnection"),
        "employee" => builder.Configuration.GetConnectionString("EmployeeConnection"),
        "admin" => builder.Configuration.GetConnectionString("MasterConnection"),
        _ => builder.Configuration.GetConnectionString("GuestConnection")
    };
    
    optionsBuilder.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFormatService, FormatService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();

builder.Services.AddCors(opt =>
    {
    opt.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.UseCors();

app.Run();
