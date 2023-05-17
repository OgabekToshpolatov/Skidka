using System.ComponentModel.DataAnnotations.Schema;

namespace ArzonOL.Entities;

public class CartEntity : BaseEntity
{
    public string? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual UserEntity? User { get; set; }
    public virtual ICollection<CartProduct>? CartProducts { get; set; }
}