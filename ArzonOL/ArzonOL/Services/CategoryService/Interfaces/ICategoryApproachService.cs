using ArzonOL.Dtos.CategoryDtos;

namespace ArzonOL.Services.CategoryService.Interfaces;

public interface ICategoryApproachService
{
    ValueTask<CategoryApproachResponseDto> CreateAsync(CreateOrUpdateCategoryApproachDto model);
    ValueTask<CategoryApproachResponseDto> Update(Guid id, CreateOrUpdateCategoryApproachDto model);
    ValueTask<CategoryApproachResponseDto> Remove(Guid id);
    ValueTask<List<CategoryApproachResponseDto>> GetAll();
    ValueTask<CategoryApproachResponseIdDto> GetByIdAsync(Guid id);
}