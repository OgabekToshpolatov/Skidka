using ArzonOL.Dtos.CategoryDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.CategoryService.Interfaces;

public interface ICategoryApproachService
{
    ValueTask<Result<CategoryApproachResponseDto>> CreateAsync(CreateOrUpdateCategoryApproachDto model);
    ValueTask<CategoryApproachResponseDto> Update(Guid id, CreateOrUpdateCategoryApproachDto model);
    ValueTask<Result<CategoryApproachResponseDto>> Remove(Guid id);
    ValueTask<Result<List<CategoryApproachResponseDto>>> GetAll();
    ValueTask<Result<CategoryApproachResponseIdDto>> GetByIdAsync(Guid id);
}