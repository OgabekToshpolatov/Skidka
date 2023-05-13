using System.ComponentModel.DataAnnotations.Schema;

namespace ArzonOL.Entities;

public class BoughtProductEntity : BaseEntity
{
    [ForeignKey(nameof(UserEntity))]
    public string? UserId {get; set;}
    public UserEntity? UserEntity {get; set;}
    [ForeignKey(nameof(ProductEntity))]
    public Guid? ProductId {get; set;}
    public BaseProductEntity? ProductEntity {get; set;}
}