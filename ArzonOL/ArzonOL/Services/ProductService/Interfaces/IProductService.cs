using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.ProductServeice.Interfaces;

public interface IProductService
{
    public ValueTask<Result<ProductModel>> CreateProductAsync(CreateProductDto createProductDto);
    public ValueTask<Result<ProductModel>> UpdateAsync(UpdateProductDto updateProductDto);
    public ValueTask<Result<ProductModel>> GetById(Guid id);
    public ValueTask<Result<ICollection<ProductModel>>> GetAllAsync();
    public ValueTask<Result<ICollection<ProductModel>>> GetWithPaginationAsync(int page, int limit);
    public ValueTask<Result> Remove(Guid id);
    
}