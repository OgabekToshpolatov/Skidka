namespace ArzonOL.Entities;
public class WishListEntity : BaseEntity
{
    public virtual UserEntity? User { get; set; }
    public virtual ICollection<BaseProductEntity>? Products { get; set; }
}
