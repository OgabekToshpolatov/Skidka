using ArzonOL.Dtos.CategoryDtos;

namespace ArzonOL.Services.CategoryService.Interfaces;

public interface ICategoryService
{
    ValueTask<CategoryResponseDto> CreateAsync(CreateOrUpdateCategoryDto model);
    ValueTask<CategoryResponseDto> Update(Guid id, CreateOrUpdateCategoryDto model);
    ValueTask<CategoryResponseDto> Remove(Guid id);
    ValueTask<List<CategoryResponseDto>> GetAll();
    ValueTask<CategoryResponseIdDto> GetByIdAsync(Guid id);
}