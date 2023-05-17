using ArzonOL.Dtos.CartDtos;
using ArzonOL.Helpers;
using ArzonOL.Models;
using ArzonOL.Services.CartService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArzonOL.Controllers.Carts;


[Route("api/[controller]")]
[ApiController]
public class CartsController:ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService ;
    }

    [ProducesResponseType(typeof(List<CartProductModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<IActionResult> GetUserCart([FromQuery] PaginationParams paginationParams)
    {
        var userId = TokenHelper.GetUserIdFromToken(HttpContext, "AuthToken");
        if(userId == null) return Unauthorized();
        
        var a = await _cartService.GetUserCart(paginationParams,userId);

        return Ok(a);
    }

    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<IActionResult> AddToCart(CreateCartProductDto createCartDto)
    {
        
        var userId = TokenHelper.GetUserIdFromToken(HttpContext, "AuthToken");
        System.Console.WriteLine("################################################3"+userId);
        if(userId == null) return Unauthorized();
        return Ok(await _cartService.AddToCart(userId, createCartDto));
    }

    [HttpDelete("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        if(Guid.Empty == productId)
        return BadRequest("Id can't be empty");

        var userId = TokenHelper.GetUserIdFromToken(HttpContext, "AuthToken");
        if(userId == null) return Unauthorized();
        return Ok(await _cartService.DeleteCartProductById(userId,productId));
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
    public async Task<IActionResult> DeleteFromCartAllProducts()
    {
        var userId = TokenHelper.GetUserIdFromToken(HttpContext, "AuthToken");
        if(userId == null) return Unauthorized();
        return Ok(await _cartService.DeletCartAllProducts(userId));
    }
}