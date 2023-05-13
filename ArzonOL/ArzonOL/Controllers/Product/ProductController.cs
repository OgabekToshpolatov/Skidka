using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Models;
using ArzonOL.Services.ProductServeice.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArzonOL.Controllers.Product;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
    public async Task<IActionResult> GetAllProducts()
    => Ok(await _productService.GetAllAsync());

    [HttpGet("GetWithPagination")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
    public async Task<IActionResult> GetProductWithPagination(int page, int limit)
    {
        if(page <= 0 || limit <= 0)
        return BadRequest("Page or limit can't be minus or zero");

        return Ok(await _productService.GetWithPaginationAsync(page, limit));
    }
   
   [HttpPost("createProduct")]
   [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
   public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
   {
     if(createProductDto is null)
     return BadRequest("Product can't be null");

     return Ok(await _productService.CreateProductAsync(createProductDto));
   }

   [HttpPut("updateProduct")]
   [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
   public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
   {
    if(updateProductDto is null)
    return BadRequest("Product can't be null here");

    return Ok(await _productService.UpdateAsync(updateProductDto));
   }
   
   [HttpDelete("deleteProduct")]
   [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
   public async Task<IActionResult> DeleteProduct([FromForm]Guid id)
   {
     if(Guid.Empty == id)
     return BadRequest("Id can't be empty");

     return Ok(await _productService.Remove(id));
   }

   [HttpGet("getWithId")]
   [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
   public async Task<IActionResult> GetWithId(Guid id)
   {
     if(Guid.Empty == id)
     return BadRequest("Id cant't be null");

     return Ok(await _productService.GetById(id));
   }
    
}