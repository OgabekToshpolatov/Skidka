namespace ArzonOL.Entities;


public class ProductMediaEntity : BaseEntity
{
    public string? ImageBase64String {get; set;}
    public Guid? ProductId { get; set; }
    public virtual BaseProductEntity? Product { get; set; }
}