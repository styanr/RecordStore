using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Services.Products;
using RecordStore.Api.Services.Records;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RecordStoreContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MasterConnection"));
});

builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();