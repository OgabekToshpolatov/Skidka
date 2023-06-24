using ArzonOL.Dtos.ProductMediaDtos;
using ArzonOL.Models;
using ArzonOL.Services.ProductMediaService;
using Microsoft.AspNetCore.Mvc;

namespace ArzonOL.Controllers.ProductMedia;

[ApiController]
[Route("api/[controller]")]
public class ProductMediaController : ControllerBase
{
    private readonly IProductMediaService _producMediaService;

    public ProductMediaController(IProductMediaService producMediaService)
    {
        _producMediaService = producMediaService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductMediaModel>))]
    public async Task<IActionResult> CreateMedia(CreateOrUpdateMediaDto creteOrUpdate)
    => Ok(await _producMediaService.CreateAsync(creteOrUpdate));

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<ProductMediaModel>))]
    public async Task<IActionResult> UpdateMedia(CreateOrUpdateMediaDto updateMediaDto)
    => Ok(await _producMediaService.UpdateAsync(updateMediaDto));

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
    public async Task<IActionResult> RemoveMedia(Guid id)
    => Ok(await _producMediaService.RemoveAsync(id));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<ProductMediaModel>>))]
    public async Task<IActionResult> GetAllMedias()
    => Ok(await _producMediaService.GetAll());

    [HttpGet("getAllWithPagination")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<List<ProductMediaModel>>))]
    public async Task<IActionResult> GetAllMedias(int page, int limit)
    => Ok(await _producMediaService.GetMediasWithPagination(page, limit));

    [HttpPost("createRange")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
    public async Task<IActionResult> CreateRange(CreateRangeMediaDto createRangeMediaDto)
    => Ok(await _producMediaService.CreateRangeProductMedia(createRangeMediaDto));
}