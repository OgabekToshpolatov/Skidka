using ArzonOL.Dtos.ProductDtos;
using ArzonOL.Entities;
using ArzonOL.Models;
using ArzonOL.Repositories.Interfaces;
using ArzonOL.Services.ProductServeice.Interfaces;
using Mapster;


namespace ArzonOL.Services.ProductServeice;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async ValueTask<Result<ProductModel>> CreateProductAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Product creating");
        try
        {
            if(createProductDto.OldPrice <= createProductDto.NewPrice)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"NewPrice must lower from OldPrice"){Data = null};
            
            var isExistUserId = _unitOfWork.ProductRepository.Find(x => x.UserEntityId == createProductDto.UserId.ToString());
            if(isExistUserId.Count() == 0)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"UserId hato kiritildi");

            var isExistCategoryId = _unitOfWork.ProductRepository.Find(x => x.ProductCategoryApproachId == createProductDto.ProductCategoryApproachId);
            if(isExistCategoryId.Count() == 0)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"ProductCategoryApproachId hato kiritildi");
            
            if(createProductDto is null)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"Product can't be null here"){Data = null};

            var newProduct = new BaseProductEntity()
            {
                Name = createProductDto.Name,
                OldPrice = createProductDto.OldPrice,
                NewPrice = createProductDto.NewPrice,
                VideoUrl = createProductDto.VideoUrl,
                Description = createProductDto.Description,
                Brand = createProductDto.Brand,
                Latitudes = createProductDto.Latitudes,
                Longitudes = createProductDto.Longitudes,
                Region = createProductDto.Region,
                Destrict = createProductDto.Destrict,
                PhoneNumber = createProductDto.PhoneNumber,
                StartDate = createProductDto.StartDate,
                EndDate = createProductDto.EndDate,
                CreatedAt = DateTime.Now,
                ProductCategoryApproachId = createProductDto.ProductCategoryApproachId,
                ProductCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(new Guid(createProductDto.ProductCategoryApproachId.ToString()!)),
                UserEntityId = createProductDto.UserId.ToString(),
                UserEntity = _unitOfWork.UserRepository.GetAll().FirstOrDefault(x => x.Id == createProductDto.UserId.ToString()),
                Discount = ((float)(createProductDto.OldPrice - createProductDto.NewPrice))/((float)createProductDto.OldPrice)*100
            };
        var createdProduct = await _unitOfWork.ProductRepository.AddAsync(newProduct);

        if(createdProduct is null)
        return new Result<ProductModel>(isSuccess:false, errorMessage:"Failed while creating Product"){Data = null};
        
        var voteResult = GetProductVoteResult(createdProduct.Id);
        var boughtCount = GetBoughtCount(createdProduct.Id);
        var productPhotos = GetProductPhotos(createdProduct.Id);
        var productVoters = GetProductVoters(createdProduct.Id);

        var returnModel = new ProductModel
        {
            Id = createdProduct.Id,
            CreatedAt = createdProduct.CreatedAt,
            Name = createdProduct.Name,
            OldPrice = createdProduct.OldPrice,
            NewPrice = createdProduct.NewPrice,
            VideoUrl = createdProduct.VideoUrl,
            Description = createdProduct.Description,
            Brand = createdProduct.Brand,
            Latitudes = createdProduct.Latitudes,
            Longitudes = createdProduct.Longitudes,
            Region = createdProduct.Region,
            Destrict = createdProduct.Destrict,
            PhoneNumber = createdProduct.PhoneNumber,
            StartDate = createdProduct.StartDate,
            EndDate = createdProduct.EndDate,
            ProductPhotos = productPhotos,
            VotesResult = voteResult,
            BoughtCount = boughtCount,
            Discount = createdProduct.Discount,
            Voters = productVoters
        };

        return new Result<ProductModel>(isSuccess:true){Data = returnModel};
     }
     catch(Exception e)
     {
        _logger.LogInformation("Error while creating product");
        throw new Exception(e.Message);
     }

    }

    private List<ProductVoterModel> GetProductVoters(Guid id)
    => _unitOfWork.VoterRepository.GetAll().Where(x => x.ProductId == id).Select(x => 
    new ProductVoterModel
    {
      Vote = x.Vote,
      Comment = x.Comment,
      User = new PublicUserModel
      {
        Id = _unitOfWork.VoterRepository.GetAll().FirstOrDefault(id => x.Id == x.Id)!.User!.Id,
        Name = _unitOfWork.VoterRepository.GetAll().FirstOrDefault(id => x.Id == x.Id)!.User!.UserName,
      }
    }).ToList();

    private  List<string>? GetProductPhotos(Guid id)
    =>  _unitOfWork.ProductMediaRepository.GetAll().Where(x => x.ProductId == id).Select(p => p.ImageBase64String).ToList()!;
    
    private long GetBoughtCount(Guid id)
    => _unitOfWork.BoughtProductRepository.GetAll().Where(x => x.ProductId == id).Count();

    private float GetProductVoteResult(Guid productId)
    {
        var productVoters = _unitOfWork.VoterRepository.GetAll().Where(x => x.ProductId == productId).ToList();

        if(productVoters.Count == 0)
        return 0;
        
        return productVoters.Select(x => x.Vote).Sum()/productVoters.Count();
    }

    public async ValueTask<Result<ICollection<ProductModel>>> GetAllAsync()
    {
        try
        {
            var products = _unitOfWork.ProductRepository.GetAll().Select( x =>
            new ProductModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                OldPrice = x.OldPrice,
                NewPrice = x.NewPrice,
                VideoUrl = x.VideoUrl,
                Description = x.Description,
                Brand = x.Brand,
                Latitudes = x.Latitudes,
                Longitudes = x.Longitudes,
                Region = x.Region,
                Destrict = x.Destrict,
                PhoneNumber = x.PhoneNumber,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Discount = x.Discount,     
            }).ToList();

            products.ForEach(x => 
            {
                x.ProductPhotos =  GetProductPhotos(x.Id);
                x.VotesResult =  GetProductVoteResult(x.Id);
                x.BoughtCount =  GetBoughtCount(x.Id);
                x.Voters =  GetProductVoters(x.Id);
            });

            return new Result<ICollection<ProductModel>>(isSuccess:true){Data = products.ToList()};
        }
        catch(Exception e)  
        {
          throw new Exception(e.Message);
        }
        
    }

    public async ValueTask<Result<ProductModel>> GetById(Guid id)
    {
        _logger.LogInformation("Taking product with Id = ", id);
        try
        {
            if(string.IsNullOrEmpty(id.ToString()))
            return new Result<ProductModel>(isSuccess: false, errorMessage: "This Id is not availabel"){Data = null};

            var product = _unitOfWork.ProductRepository.GetById(id);

            if(product is null)
            return new Result<ProductModel>(isSuccess:false, errorMessage: "Product didn't found with id = "+id.ToString()){Data = null};

            return new Result<ProductModel>(isSuccess:true){Data = new ProductModel
            {
                Id = product.Id,
                CreatedAt = product.CreatedAt,
                Name = product.Name,
                OldPrice = product.OldPrice,
                NewPrice = product.NewPrice,
                VideoUrl = product.VideoUrl,
                Description = product.Description,
                Brand = product.Brand,
                Latitudes = product.Latitudes,
                Longitudes = product.Longitudes,
                Region = product.Region,
                Destrict = product.Destrict,
                PhoneNumber = product.PhoneNumber,
                StartDate = product.StartDate,
                EndDate = product.EndDate,
                Discount = product.Discount,
                ProductPhotos = GetProductPhotos(product.Id),
                VotesResult = GetProductVoteResult(product.Id),
                BoughtCount = GetBoughtCount(product.Id),
                Voters = GetProductVoters(product.Id)
            }};
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<ICollection<ProductModel>>> GetWithPaginationAsync(int page, int limit)
    {
       try
       {
            var products = _unitOfWork.ProductRepository
            .GetAll()
            .Skip((page - 1) * limit)
            .Take(limit).Select(x => new ProductModel
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Name = x.Name,
                OldPrice = x.OldPrice,
                NewPrice = x.NewPrice,
                VideoUrl = x.VideoUrl,
                Description = x.Description,
                Brand = x.Brand,
                Latitudes = x.Latitudes,
                Longitudes = x.Longitudes,
                Region = x.Region,
                Destrict = x.Destrict,
                PhoneNumber = x.PhoneNumber,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Discount = x.Discount,    
            }).ToList();

            products.ForEach(x => 
            {
                x.ProductPhotos =  GetProductPhotos(x.Id);
                x.VotesResult =  GetProductVoteResult(x.Id);
                x.BoughtCount =  GetBoughtCount(x.Id);
                x.Voters =  GetProductVoters(x.Id);
            });

            return new Result<ICollection<ProductModel>>(isSuccess:true){Data = products.ToList()};
       }
       catch (System.Exception e)
       {
        _logger.LogInformation(e.Message);
        throw new Exception(e.Message);
       }
    }

    public async ValueTask<Result> Remove(Guid id)
    {
        try
        {
            if(string.IsNullOrEmpty(id.ToString()))
            return new Result<ProductModel>(isSuccess:false, errorMessage: "This Id is not availabel"){Data = null};

            var product = _unitOfWork.ProductRepository.GetById(id);

            if(product is null)
            return new Result<ProductModel>(isSuccess:false, errorMessage: "Product didn't found with id = "+id.ToString()){Data = null};

           await _unitOfWork.ProductRepository.Remove(product);

           return new Result(isSuccess:true);
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async ValueTask<Result<ProductModel>> UpdateAsync(UpdateProductDto updateProductDto)
    {
        try
        {
            var isExistUserId = _unitOfWork.ProductRepository.Find(x => x.UserEntityId == updateProductDto.UserId.ToString());
            if(isExistUserId.Count() == 0)
            return new Result<ProductModel>(isSuccess: false, errorMessage:"UserId hato kiritildi");

            var isExistCategoryId = _unitOfWork.ProductRepository.Find(x => x.ProductCategoryApproachId == updateProductDto.ProductCategoryApproachId);
            if(isExistCategoryId.Count() == 0)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"ProductCategoryApproachId hato kiritildi");

            if(string.IsNullOrEmpty(updateProductDto.Id.ToString()))
            return new Result<ProductModel>(isSuccess:false, errorMessage: "This Id is not availabel"){Data = null}; 

            var product = _unitOfWork.ProductRepository.GetById(updateProductDto.Id);

            if(product is null)
            return new Result<ProductModel>(isSuccess:false, errorMessage: "Product didn't found with id = "+updateProductDto.Id){Data = null};
            
            product.Name = updateProductDto.Name;
            product.OldPrice = updateProductDto.OldPrice;
            product.NewPrice = updateProductDto.NewPrice;
            product.VideoUrl = updateProductDto.VideoUrl;
            product.Description = updateProductDto.Description;
            product.Brand = updateProductDto.Brand;
            product.Latitudes = updateProductDto.Latitudes;
            product.Longitudes = updateProductDto.Longitudes;
            product.Region = updateProductDto.Region;
            product.Destrict = updateProductDto.Destrict;
            product.PhoneNumber = updateProductDto.PhoneNumber;
            product.StartDate = updateProductDto.StartDate;
            product.EndDate = updateProductDto.EndDate;
            product.Discount = ((float)(updateProductDto.OldPrice - updateProductDto.NewPrice))/((float)updateProductDto.OldPrice)*100;
            product.ProductCategoryApproachId = updateProductDto.ProductCategoryApproachId;
            product.ProductCategoryApproach = _unitOfWork.CategoryApproachRepository.GetById(new Guid(updateProductDto.ProductCategoryApproachId.ToString()!));
            product.UserEntityId = updateProductDto.UserId.ToString();
            product.UserEntity = _unitOfWork.UserRepository.GetAll().FirstOrDefault(x => x.Id == updateProductDto.UserId.ToString());
                
            var updatedProduct = await _unitOfWork.ProductRepository.Update(product);

            if(updatedProduct is null)
            return new Result<ProductModel>(isSuccess:false, errorMessage:"Failed while updating product"){Data = null};
            
            var productModel = new ProductModel
                {
                    Id = updatedProduct.Id,
                    CreatedAt = updatedProduct.CreatedAt,
                    Name = updatedProduct.Name,
                    OldPrice = updatedProduct.OldPrice,
                    NewPrice = updatedProduct.NewPrice,
                    VideoUrl = updatedProduct.VideoUrl,
                    Description = updatedProduct.Description,
                    Brand = updatedProduct.Brand,
                    Latitudes = updatedProduct.Latitudes,
                    Longitudes = updatedProduct.Longitudes,
                    Region = updatedProduct.Region,
                    Destrict = updatedProduct.Destrict,
                    PhoneNumber = updatedProduct.PhoneNumber,
                    StartDate = updatedProduct.StartDate,
                    EndDate = updatedProduct.EndDate,
                    Discount = updatedProduct.Discount,
                    ProductPhotos = GetProductPhotos(product.Id),
                    VotesResult = GetProductVoteResult(product.Id),
                    BoughtCount = GetBoughtCount(product.Id),
                    Voters = GetProductVoters(product.Id)
                };
            return new Result<ProductModel>(isSuccess:true){Data = productModel};
            
        }
        catch (System.Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }
}