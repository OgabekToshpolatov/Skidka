using ArzonOL.Dtos.CategoryDtos;
using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Entities;
using ArzonOL.Exceptions;
using ArzonOL.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ArzonOL.Services.CategoryService.Interfaces;
using ArzonOL.Models;

namespace ArzonOL.Services.CategoryService;

public class CategoryApproachService : ICategoryApproachService
{
    private readonly ILogger<CategoryApproachService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryApproachService(
        ILogger<CategoryApproachService> logger,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger ;
        _unitOfWork = unitOfWork ;
    }
    public async ValueTask<Result<CategoryApproachResponseDto>> CreateAsync(CreateOrUpdateCategoryApproachDto model)
    {
        try
        {
             var categoryApproachNames = _unitOfWork.CategoryApproachRepository.GetAll().Where(c => c.Name ==  model.Name);

            foreach(var categoryApproachName in categoryApproachNames)
            {
                if(categoryApproachName.Name!.ToLower() == model.Name!.ToLower())
                    return new Result<CategoryApproachResponseDto>(isSuccess:false, errorMessage: "CategoryApproach already exists"){ Data = null };
            }
            
            var categoryApproach = new ProductCategoryApproachEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ProductCategoryId = model.ProductCategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var createCategoryApproach = await  _unitOfWork.CategoryApproachRepository.AddAsync(categoryApproach);
    
            return new(true) { Data = createCategoryApproach.Adapt<CategoryApproachResponseDto>()};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
;
    }

    public async ValueTask<Result<List<CategoryApproachResponseDto>>> GetAll()
    {
        try
        {
            var categoryAppproachs = _unitOfWork.CategoryApproachRepository.GetAll();

            if(categoryAppproachs.Count() == 0)
                     return new Result<List<CategoryApproachResponseDto>>(isSuccess:false, errorMessage: " There are no categories yet"){Data = null};
        
            var categoriestApproachDto = await categoryAppproachs
                                .Select(categoryApproach => categoryApproach.Adapt<CategoryApproachResponseDto>()).ToListAsync();
        
            return new(true) { Data = categoriestApproachDto };
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
 
    }

    public async ValueTask<Result<CategoryApproachResponseIdDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var existingCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(id);
        
            if(existingCategoryApproach is null)
                    return new Result<CategoryApproachResponseIdDto>(isSuccess:false, errorMessage: " CategoryApproach with given Id not found."){Data = null};;

            var categoryProductList = await _unitOfWork.ProductRepository
                                            .GetAll().Where(x => x.ProductCategoryApproachId == id).ToListAsync();

            var categoryProductListView = categoryProductList
                        .Select(product => product.Adapt<ProductResponseDto>()).ToList();
            
            var categoryApproachDtoView = new CategoryApproachResponseIdDto
            {
                Id = existingCategoryApproach.Id,
                Name = existingCategoryApproach.Name,
                Description = existingCategoryApproach.Description,
                ProductCategoryId = existingCategoryApproach.ProductCategoryId,
                CreatedAt = existingCategoryApproach.CreatedAt,
                UpdatedAt = existingCategoryApproach.UpdatedAt,
                Products = categoryProductListView
            };

            return new(true) { Data = categoryApproachDtoView};
        }
        catch (System.Exception e)
        {
             _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }

    }

    public async ValueTask<Result<CategoryApproachResponseDto>> Remove(Guid id)
    {
        try
        {
            var existingCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(id);
            System.Console.WriteLine("########################################################################################");
            if(existingCategoryApproach is null)
                   return new Result<CategoryApproachResponseDto>(isSuccess:false, errorMessage: " CategoryApproach with given Id not found."){Data = null};
            System.Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");    
            var removeCategoryApproach = await _unitOfWork.CategoryApproachRepository.Remove(existingCategoryApproach);
            System.Console.WriteLine(")))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))");
            if(removeCategoryApproach is null)
                    return new Result<CategoryApproachResponseDto>(isSuccess:false, errorMessage: "Removing the categoryApproach failed. Contact support"){Data = null};
             System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");       
                
            return new(true) { Data = removeCategoryApproach.Adapt<CategoryApproachResponseDto>()};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }

    }

    public async ValueTask<CategoryApproachResponseDto> Update(Guid id, CreateOrUpdateCategoryApproachDto model)
    {
        var existingCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(id);
        
        if(existingCategoryApproach is null)
                throw new BadRequestException("CategoryApproach with given Id not found.");

        var categoryApproachNames = _unitOfWork.CategoryApproachRepository.GetAll().Where(c => c.Name ==  model.Name);

        foreach(var categoryApproachName in categoryApproachNames)
        {
            if(categoryApproachName.Name!.ToLower() == model.Name!.ToLower())
                  throw new BadRequestException("category already exists");
        }

        var categoryApproachBaseCategories = _unitOfWork.CategoryRepository.GetAll().Select(c => c.Id);

        var count = categoryApproachBaseCategories.Count();
        var k =0;

        foreach(var categoryApproachBaseCategory in categoryApproachBaseCategories)
        {
            if(categoryApproachBaseCategory != model.ProductCategoryId){k++;};
        }
        if(count == k ) throw new BadRequestException("No such category exists");
        
        existingCategoryApproach.Name = model.Name;
        existingCategoryApproach.Description = model.Description;
        existingCategoryApproach.ProductCategoryId = model.ProductCategoryId;
        existingCategoryApproach.UpdatedAt = DateTime.Now;

        var updateCategoryApproach = await _unitOfWork.CategoryApproachRepository.Update(existingCategoryApproach);

        return updateCategoryApproach.Adapt<CategoryApproachResponseDto>();
    }
}