using ArzonOL.Data;
using ArzonOL.Entities;
using ArzonOL.Repositories.Interfaces;

namespace ArzonOL.Repositories;

public class BoughtProductRepository : GenericRepository<BoughtProductEntity>, IBoughtProductRepository
{
    public BoughtProductRepository(AppDbContext context) : base(context)
    {
    }
}