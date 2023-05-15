using System.Security.Claims;
using ArzonOL.Dtos.CartDtos;
using ArzonOL.Entities;
using ArzonOL.Helpers;
using ArzonOL.Models;
using ArzonOL.Repositories.Interfaces;
using ArzonOL.Services.CartService.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ArzonOL.Services.CartService;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ;
    }
    public ValueTask AddToCart(ClaimsPrincipal claims, CreateCartProductDto createCartDto)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeletCartAllProducts(ClaimsPrincipal claims)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteCartProductById(ClaimsPrincipal claims, Guid productId)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<List<CartProductModel>> GetUserCart(PaginationParams paginationParams, ClaimsPrincipal claims)
    {
        var userId = Guid.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        System.Console.WriteLine(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Console.WriteLine(userId);
        var cart = await _unitOfWork.CartRepository.GetAll().FirstOrDefaultAsync(c => c.UserId == userId);

        if(cart is null)
        {
            cart = new Entities.CartEntity()
            {
                UserId = userId,
            };

            await _unitOfWork.CartRepository.AddAsync(cart);
        }

        var pagedList = cart.CartProducts?.AsQueryable().ToPagedList(paginationParams);
        cart.CartProducts ??= new List<CartProduct>();
        var cartProductList = pagedList.Adapt<List<CartProductModel>>();

        return cartProductList;
    }
}