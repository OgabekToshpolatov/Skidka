namespace ArzonOL.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICategoryApproachRepository CategoryApproachRepository { get; }
    IVoterRepository VoterRepository { get; }
    IProductMediaRepository ProductMediaRepository { get; }
    IProductRepository ProductRepository { get; }
    IUserRepository UserRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IBoughtProductRepository BoughtProductRepository {get;}
    ICartEntityRepository CartRepository {get;}
    ICartProductRepository CartProductRepository {get;}
    Task<int> SaveAsync();
}