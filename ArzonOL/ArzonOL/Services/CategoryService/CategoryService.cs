using System;
using ArzonOL.Entities;
// using ArzonOL.Exceptions;
using ArzonOL.Dtos.CategoryDtos;
using ArzonOL.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ArzonOL.Services.CategoryService.Interfaces;
using ArzonOL.Models;

namespace ArzonOL.Services.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(
        ILogger<CategoryService> logger,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger ;
        _unitOfWork = unitOfWork ;
    }
    public async ValueTask<Result<CategoryResponseDto>> CreateAsync(CreateOrUpdateCategoryDto model)
    {
        try
        {
             var categoryNames = _unitOfWork.CategoryRepository.GetAll().Where(c => c.Name == model.Name);

            foreach(var categoryName in categoryNames)
            {
            if(categoryName.Name!.ToLower() == model.Name!.ToLower())
                    return new Result<CategoryResponseDto>(isSuccess:false, errorMessage: "Category already exists"){Data = null};
            }

            var category = new ProductCategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var createCategory =await  _unitOfWork.CategoryRepository.AddAsync(category);
            return new(true) { Data = createCategory.Adapt<CategoryResponseDto>()};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<List<CategoryResponseDto>>> GetAll()
    {
        try
        {
            var categories = _unitOfWork.CategoryRepository.GetAll();

            if(categories is null)
            {
                return new Result<List<CategoryResponseDto>>(isSuccess:false, errorMessage: " This product does not exist "){Data = null};
            }
        
            var categoriestDto =await categories.Select(category => category.Adapt<CategoryResponseDto>()).ToListAsync();
            
            return new(true) { Data = categoriestDto };
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }

           
    }

    public async ValueTask<Result<CategoryResponseIdDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var existingCategory = _unitOfWork.CategoryRepository.GetById(id);
        
            if(existingCategory is null)
                    return new Result<CategoryResponseIdDto>(isSuccess:false, errorMessage: " Category with given Id not found."){Data = null};
                    
            var categoryApproachList = await _unitOfWork.CategoryApproachRepository
                                                .GetAll().Where(x => x.ProductCategoryId == id).ToListAsync();
            
            var categoryApproachListView = categoryApproachList
                        .Select(approach => approach.Adapt<CategoryApproachResponseDto>()).ToList();

            var categoryResponseIdDto = new CategoryResponseIdDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                CreatedAt = existingCategory.CreatedAt,
                UpdatedAt = existingCategory.UpdatedAt,
                Approaches = categoryApproachListView
            };

            return new(true) { Data = categoryResponseIdDto };
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<CategoryResponseDto>> Remove(Guid id)
    {
        try
        {
            var existingCategory =  _unitOfWork.CategoryRepository.GetById(id);
                    
            if(existingCategory is null)
                    return new Result<CategoryResponseDto>(isSuccess:false, errorMessage: " Category with given Id not found."){Data = null};
                
            var removeCategory = await _unitOfWork.CategoryRepository.Remove(existingCategory);

            if(removeCategory is null)
                   return new Result<CategoryResponseDto>(isSuccess:false, errorMessage: "Removing the quiz failed. Contact support"){Data = null};
                   
                
            return new(true) { Data = removeCategory.Adapt<CategoryResponseDto>()};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
;
    }

    public async ValueTask<Result<CategoryResponseDto>> Update(Guid id, CreateOrUpdateCategoryDto model)
    {
        try
        {
            var existingCategory =  _unitOfWork.CategoryRepository.GetById(id);
                    
            if(existingCategory is null)
                    return new Result<CategoryResponseDto>(isSuccess:false, errorMessage: "Category with given Id not found."){Data = null};

            var categoryNames = _unitOfWork.CategoryRepository.GetAll().Where(c => c.Name == model.Name);

            foreach(var categoryName in categoryNames)
            {
                if(categoryName.Name!.ToLower() == model.Name!.ToLower())
                    return new Result<CategoryResponseDto>(isSuccess:false, errorMessage: "Category already exists"){Data = null};
            }
            
            existingCategory.Name = model.Name;
            existingCategory.Description = model.Description;
            existingCategory.UpdatedAt = DateTime.Now;

            var updateCategory = await _unitOfWork.CategoryRepository.Update(existingCategory);

            return new(true) { Data =updateCategory.Adapt<CategoryResponseDto>()};
        }
        catch (System.Exception e)
        {       
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }

    }
}