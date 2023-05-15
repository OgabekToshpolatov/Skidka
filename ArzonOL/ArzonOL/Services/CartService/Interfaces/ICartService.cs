using System.Security.Claims;
using ArzonOL.Dtos.CartDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.CartService.Interfaces;

public interface ICartService
{
    ValueTask<List<CartProductModel>> GetUserCart(PaginationParams paginationParams, ClaimsPrincipal claims);
    ValueTask AddToCart(ClaimsPrincipal claims, CreateCartProductDto createCartDto);
    ValueTask DeleteCartProductById(ClaimsPrincipal claims, Guid productId);
    ValueTask DeletCartAllProducts(ClaimsPrincipal claims);
}