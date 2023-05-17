using System.Security.Claims;
using ArzonOL.Dtos.CartDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.CartService.Interfaces;

public interface ICartService
{
    ValueTask<Result<List<CartProductModel>>> GetUserCart(PaginationParams paginationParams,string userId);
    ValueTask<Result> AddToCart(string userId, CreateCartProductDto createCartDto);
    ValueTask<Result> DeleteCartProductById(string userId, Guid productId);
    ValueTask<Result> DeletCartAllProducts(string userId);
}