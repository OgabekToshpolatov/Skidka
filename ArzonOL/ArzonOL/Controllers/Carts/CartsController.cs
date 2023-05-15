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
        return Ok(await _cartService.GetUserCart(paginationParams, User));
    }
}