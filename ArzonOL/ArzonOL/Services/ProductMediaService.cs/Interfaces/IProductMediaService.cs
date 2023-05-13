using ArzonOL.Dtos.ProductMediaDtos;
using ArzonOL.Models;

namespace ArzonOL.Services.ProductMediaService;

public interface IProductMediaService
{
    ValueTask<Result<ProductMediaModel>> GetById(Guid id);
    ValueTask<Result<ProductMediaModel>> CreateAsync(CreateOrUpdateMediaDto createMediaDto);
    ValueTask<Result<ProductMediaModel>> UpdateAsync(CreateOrUpdateMediaDto updateMediaDto);
    ValueTask<Result<List<ProductMediaModel>>> GetMediasWithPagination(int page, int limit);
    ValueTask<Result<List<ProductMediaModel>>> GetAll();
    ValueTask<Result> RemoveAsync(Guid id);
    ValueTask<Result<List<ProductMediaModel>>> CreateRangeProductMedia(CreateRangeMediaDto createRangeMediaDto);
}