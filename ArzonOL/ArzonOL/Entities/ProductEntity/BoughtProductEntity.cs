using System.ComponentModel.DataAnnotations.Schema;

namespace ArzonOL.Entities;

public class BoughtProductEntity : BaseEntity
{
    [ForeignKey(nameof(UserEntity))]
    public string? UserId {get; set;}
    public virtual UserEntity? UserEntity {get; set;}
    [ForeignKey(nameof(ProductEntity))]
    public Guid? ProductId {get; set;}
    public virtual BaseProductEntity? ProductEntity {get; set;}
}