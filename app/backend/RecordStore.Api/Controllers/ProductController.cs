using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Products;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll([FromQuery] GetProductQueryParams queryParams)
    {
        var products = await _productService.GetAllAsync(queryParams);
        
        return products;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ProductFullResponseDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        
        return product;
    }
}