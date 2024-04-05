using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.DTO.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.Services.Products;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDTO>>> GetAll([FromQuery] GetProductQueryParams queryParams)
    {
        var products = await _productService.GetAllAsync(queryParams);
        var productsResponse = _mapper.Map<List<ProductResponseDTO>>(products);
        
        return productsResponse;
    }
}