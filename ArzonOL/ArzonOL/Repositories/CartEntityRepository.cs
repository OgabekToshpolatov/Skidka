using ArzonOL.Data;
using ArzonOL.Entities;
using ArzonOL.Repositories.Interfaces;

namespace ArzonOL.Repositories;

public class CartEntityRepository : GenericRepository<CartEntity>, ICartEntityRepository
{
    public CartEntityRepository(AppDbContext context) : base(context)
    {
    }
}