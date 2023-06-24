using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Models;
using ArzonOL.Services.ProductServeice.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArzonOL.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("getAllWithPagination")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<IEnumerable<ProductModel>>))]
        public async Task<IActionResult> GetProductWithPagination(int page, int limit)
        {
            if (page <= 0 || limit <= 0)
                return BadRequest("Page or limit cannot be less than or equal to zero");

            var paginatedProducts = await _productService.GetWithPaginationAsync(page, limit);
            return Ok(paginatedProducts);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            if (createProductDto == null)
                return BadRequest("Product cannot be null");

            var createdProduct = await _productService.CreateProductAsync(createProductDto);
            return Ok(createdProduct);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                return BadRequest("Product cannot be null");

            var updatedProduct = await _productService.UpdateAsync(updateProductDto);
            return Ok(updatedProduct);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
        public async Task<IActionResult> DeleteProduct([FromForm]Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be empty");

            var result = await _productService.Remove(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductModel>))]
        public async Task<IActionResult> GetWithId(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be null");

            var product = await _productService.GetById(id);
            return Ok(product);
        }
    }
}
