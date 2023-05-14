using ArzonOL.Dtos.CategoryDtos;
using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Entities;
using ArzonOL.Exceptions;
using ArzonOL.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ArzonOL.Services.CategoryService.Interfaces;

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
    public async ValueTask<CategoryApproachResponseDto> CreateAsync(CreateOrUpdateCategoryApproachDto model)
    {
        var categoryApproachNames = _unitOfWork.CategoryApproachRepository.GetAll().Where(c => c.Name ==  model.Name);

        foreach(var categoryApproachName in categoryApproachNames)
        {
            if(categoryApproachName.Name!.ToLower() == model.Name!.ToLower())
                  throw new BadRequestException("category already exists");
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
 
        return createCategoryApproach.Adapt<CategoryApproachResponseDto>();
    }

    public async ValueTask<List<CategoryApproachResponseDto>> GetAll()
    {
        var categoryAppproachs = _unitOfWork.CategoryApproachRepository.GetAll();

        if(categoryAppproachs.Count() == 0)
            throw new("There are no categories yet");
    
        var categoriestApproachDto = await categoryAppproachs
                            .Select(categoryApproach => categoryApproach.Adapt<CategoryApproachResponseDto>()).ToListAsync();
    
        return categoriestApproachDto; 
    }

    public async ValueTask<CategoryApproachResponseIdDto> GetByIdAsync(Guid id)
    {
        var existingCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(id);
        
        if(existingCategoryApproach is null)
                throw new BadRequestException("CategoryApproach with given Id not found.");

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

        return categoryApproachDtoView;
    }

    public async ValueTask<CategoryApproachResponseDto> Remove(Guid id)
    {
        var existingCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(id);
        
        if(existingCategoryApproach is null)
                throw new BadRequestException("CategoryApproach with given Id not found.");
            
        var removeCategoryApproach = await _unitOfWork.CategoryApproachRepository.Remove(existingCategoryApproach);

        if(removeCategoryApproach is null)
                throw new BadHttpRequestException("Removing the categoryApproach failed. Contact support.");
            
        return removeCategoryApproach.Adapt<CategoryApproachResponseDto>();
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