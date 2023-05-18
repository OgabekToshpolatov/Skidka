using ArzonOL.Dtos.CategoryDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.CategoryService.Interfaces;

public interface ICategoryService
{
    ValueTask<Result<CategoryResponseDto>> CreateAsync(CreateOrUpdateCategoryDto model);
    ValueTask<Result<CategoryResponseDto>> Update(Guid id, CreateOrUpdateCategoryDto model);
    ValueTask<Result<CategoryResponseDto>> Remove(Guid id);
    ValueTask<Result<List<CategoryResponseDto>>> GetAll();
    ValueTask<Result<CategoryResponseIdDto>> GetByIdAsync(Guid id);
}