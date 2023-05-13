using ArzonOL.Dtos.ProductMediaDtos;
using ArzonOL.Entities;
using ArzonOL.Models;
using ArzonOL.Repositories.Interfaces;

namespace ArzonOL.Services.ProductMediaService;

public class ProductMediaService : IProductMediaService
{
    private readonly ILogger<ProductMediaService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public ProductMediaService(ILogger<ProductMediaService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async ValueTask<Result<ProductMediaModel>> CreateAsync(CreateOrUpdateMediaDto createMediaDto)
    {
        _logger.LogInformation($"Started creating Product media");
        try
        {
            if(createMediaDto is null)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage: "Data can't be null for create"){Data = null};

            if(createMediaDto.ImageBase64String is null || createMediaDto.ProductId == Guid.Empty)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage:"imagstring or ProductId is null"){Data = null}; 
            
            var product = _unitOfWork.ProductRepository.Find(x => x.Id == createMediaDto.ProductId); 

            if(product.Count() == 0)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage:$"Product didn't found with Id {createMediaDto.ProductId}");

            var createdMedia = await _unitOfWork.ProductMediaRepository.AddAsync(new ProductMediaEntity
            {
                Id = Guid.NewGuid(),
                ImageBase64String = createMediaDto.ImageBase64String,
                ProductId = createMediaDto.ProductId,
                Product = product.FirstOrDefault()
            });
            
            var mediaModel = new ProductMediaModel
            {
               Id = createdMedia.Id,
               ImageBase64String = createdMedia.ImageBase64String,
               ProductId = createdMedia.ProductId
            };

            return new Result<ProductMediaModel>(isSuccess:true){Data = mediaModel}; 
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<List<ProductMediaModel>>> CreateRangeProductMedia(CreateRangeMediaDto createRangeMediaDto)
    {
        try
        {
            var product = _unitOfWork.ProductRepository.Find(x => x.Id == createRangeMediaDto.ProductId).FirstOrDefault(); 

            if(product is null)
            return new(isSuccess:false, errorMessage:$"Product didn't found with Id {createRangeMediaDto.ProductId}");
            
            var productMediaEntities = new List<ProductMediaEntity>();

            foreach (var imageBase64String in createRangeMediaDto.ImageBase64Strings!)
            {
                productMediaEntities.Add(new ProductMediaEntity
                {
                    Id = Guid.NewGuid(),
                    ImageBase64String = imageBase64String,
                    ProductId = createRangeMediaDto.ProductId,
                    Product = product
                });
            }
            var createdMedias = _unitOfWork.ProductMediaRepository.AddRange(productMediaEntities);

            return new(true); 
        }
        catch (System.Exception e)
        {
            
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<List<ProductMediaModel>>> GetAll()
    {
        try
        {
            var mediaModels = _unitOfWork.ProductMediaRepository.GetAll().ToList().Select(x => 
              new ProductMediaModel
              {
                Id = x.Id,
                ProductId = x.ProductId,
                ImageBase64String = x.ImageBase64String
              }).ToList();

            return new Result<List<ProductMediaModel>>(isSuccess:true){Data = mediaModels};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<ProductMediaModel>> GetById(Guid id)
    {
        _logger.LogInformation($"Started Getting Product media Id With {id}");
        try
        {
            var productMedia = _unitOfWork.ProductMediaRepository.GetById(id);

            if(productMedia is null)
            return new Result<ProductMediaModel>(isSuccess:false,errorMessage:$"ProductMedia not found with Id {id}"){Data = null};

            return new Result<ProductMediaModel>(isSuccess:true){Data = new ProductMediaModel
            {
              Id = productMedia.Id,
              ProductId = productMedia.ProductId,
              ImageBase64String = productMedia.ImageBase64String
            }};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<List<ProductMediaModel>>> GetMediasWithPagination(int page, int limit)
    {
        try
        {
            var mediaModels = _unitOfWork.ProductMediaRepository
            .GetAll()
            .Skip((page - 1) * limit)
            .Take(limit).Select(x => 
              new ProductMediaModel
              {
                Id = x.Id,
                ProductId = x.ProductId,
                ImageBase64String = x.ImageBase64String
              }).ToList();

            return new Result<List<ProductMediaModel>>(isSuccess:true){Data = mediaModels};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result> RemoveAsync(Guid id)
    {
        _logger.LogInformation($"Started remove ProductMedia with Id {id}");
        try
        {
            var productMedia = _unitOfWork.ProductMediaRepository.GetById(id);

            if(productMedia is null)
            return new Result<ProductMediaModel>(isSuccess:false,errorMessage:$"ProductMedia not found with Id {id}"){Data = null};
            
            await _unitOfWork.ProductMediaRepository.Remove(productMedia);

            return new Result(isSuccess:true);
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<ProductMediaModel>> UpdateAsync(CreateOrUpdateMediaDto updateMediaDto)
    {
        try
        {
            if(updateMediaDto is null)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage: "Data can't be null for create"){Data = null};

            if(updateMediaDto.ImageBase64String is null || updateMediaDto.ProductId == Guid.Empty)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage:"imagstring or ProductId is null"){Data = null}; 
            
            var product = _unitOfWork.ProductRepository.Find(x => x.Id == updateMediaDto.ProductId); 
            if(product.Count() == 0)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage:$"Product didn't found with Id {updateMediaDto.ProductId}");
            
            var productMedia = _unitOfWork.ProductMediaRepository.Find(x => x.Id == updateMediaDto.Id).FirstOrDefault();
            if(productMedia is null)
            return new Result<ProductMediaModel>(isSuccess:false, errorMessage:$"ProductMedia didn't found with Id {updateMediaDto.Id}");
            
            productMedia.ImageBase64String = updateMediaDto.ImageBase64String;
            productMedia.ProductId = updateMediaDto.ProductId;
            productMedia.Product = product.FirstOrDefault();

            var updatedProductMedia = await _unitOfWork.ProductMediaRepository.Update(productMedia);

            return new Result<ProductMediaModel>(isSuccess:true){Data = new ProductMediaModel
            {
                Id = updatedProductMedia.Id,
                ImageBase64String = updatedProductMedia.ImageBase64String,
                ProductId = updatedProductMedia.ProductId
            }};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }
}