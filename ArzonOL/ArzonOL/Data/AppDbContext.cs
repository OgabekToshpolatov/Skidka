using ArzonOL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArzonOL.Data;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<BaseProductEntity>? Products { get; set; }
    public DbSet<CartEntity>? Carts { get; set; }
    public DbSet<WishListEntity>? WishLists { get; set; }
    public DbSet<ProductVoterEntity>? ProductVoters { get; set; }
    public DbSet<ProductCategoryApproachEntity>? ProductCategoryApproaches {get; set;}
    public DbSet<ProductCategoryEntity>? ProductCategories {get; set;}
    public DbSet<ProductMediaEntity>? ProductMedias {get; set;}
    public DbSet<BoughtProductEntity>? BoughtProducts {get; set;}
    public DbSet<CartProduct>? CartProducts { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // comment
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategoryEntity>()
                    .HasMany(c => c.Approaches)
                    .WithOne(p => p.ProductCategory)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductCategoryApproachEntity>()
                    .HasMany(c => c.Products)
                    .WithOne(p => p.ProductCategoryApproach)
                    .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);


        
    }

}