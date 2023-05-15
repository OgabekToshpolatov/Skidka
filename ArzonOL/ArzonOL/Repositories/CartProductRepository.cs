using ArzonOL.Data;
using ArzonOL.Entities;
using ArzonOL.Repositories.Interfaces;

namespace ArzonOL.Repositories;

public class CartProductRepository : GenericRepository<CartProduct>, ICartProductRepository
{
    public CartProductRepository(AppDbContext context) : base(context)
    {
    }
}