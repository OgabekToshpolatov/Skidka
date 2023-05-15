using System.ComponentModel.DataAnnotations.Schema;

namespace ArzonOL.Entities;

public class CartProduct:BaseEntity
{
    public Guid ProductId { get; set; }
    public virtual BaseProductEntity? Product { get; set; }
    public Guid CartId { get; set; }
    public virtual CartEntity? Cart { get; set; }


}