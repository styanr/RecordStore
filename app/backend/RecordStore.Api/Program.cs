using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RecordStore.Api.Context;
using RecordStore.Api.Extensions;
using RecordStore.Api.Filters;
using RecordStore.Api.Services.Artists;
using RecordStore.Api.Services.Products;
using RecordStore.Api.Services.Records;
using RecordStore.Api.Services.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<EntityNotFoundExceptionFilter>();
    // options.Filters.Add<GenericExceptionFilter>();
});

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
builder.Services.AddDbContext<RecordStoreContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MasterConnection"));
});

builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IUserService, UserService>();

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

app.Run();
