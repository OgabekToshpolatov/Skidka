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
    private readonly ILogger<CartService> _logger;

    public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger)
    {
        _unitOfWork = unitOfWork ;
        _logger = logger;
    }

    public async ValueTask<Result> AddToCart(string userId, CreateCartProductDto createCartDto)
    {
        try
        {
            var cart = await _unitOfWork.CartRepository.GetAll().FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart is null)
            {
                cart = new Entities.CartEntity()
                {
                    UserId = userId,
                };

                await _unitOfWork.CartRepository.AddAsync(cart);
            }

            var product = new CartProduct()
            {
                ProductId = createCartDto.ProductId,
            };

            cart.CartProducts ??= new List<CartProduct>();
            cart.CartProducts.Add(product);

            await _unitOfWork.CartRepository.Update(cart);

            return new Result(isSuccess:true);
        }
        catch (System.Exception e)
        {
             _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
        
    }

    public async ValueTask<Result> DeletCartAllProducts(string userId)
    {
        try
        {
            var cart = await _unitOfWork.CartRepository.GetAll().FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                return new Result<CartProductModel>(isSuccess:false, errorMessage: " Cart Not Found "){Data = null};
            }
            if (cart.CartProducts is not null)
            {
                cart.CartProducts.Clear();
                await _unitOfWork.SaveAsync();
            }

            return new Result(isSuccess:true);
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result> DeleteCartProductById(string userId, Guid productId)
    {
        try
        {
            var cart = await _unitOfWork.CartRepository.GetAll().FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart is null)
            {
                return new Result<CartProductModel>(isSuccess:false, errorMessage: " Cart Not Found "){Data = null};
            }

            var product = await _unitOfWork.CartProductRepository.GetAll().FirstOrDefaultAsync(x => x.ProductId == productId);
            
            if (product is null)
            {
                return new Result<CartProductModel>(isSuccess:false, errorMessage: " This product does not exist "){Data = null};
            }
            
            await _unitOfWork.CartProductRepository.Remove(product);
            await _unitOfWork.CartRepository.Update(cart);

            return new Result(isSuccess:true); 
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<List<CartProductModel>>> GetUserCart(PaginationParams paginationParams,string userId)
    {
        try
        {
            var cart = await _unitOfWork.CartRepository.GetAll().FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart is null)
            {
                cart = new Entities.CartEntity()
                {
                    UserId = userId,
                };

                await _unitOfWork.CartRepository.AddAsync(cart);
            }

            var pagedList = cart.CartProducts?.AsQueryable().ToPagedList(paginationParams);
            cart.CartProducts ??= new List<CartProduct>();
            var cartProductList = pagedList!.Adapt<List<CartProductModel>>();

            return new(true) { Data = cartProductList};
        }
        catch (System.Exception e)
        {
             _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    private CartProductModel ToModel(CartProduct x)
    {
        var result = new CartProductModel()
        {
            Id = x.Id,
            CartId = x.CartId,
            Product = _unitOfWork.ProductRepository.GetById(x.ProductId)!.Adapt<ProductModel>()
        };

        return result;
    }

    
}