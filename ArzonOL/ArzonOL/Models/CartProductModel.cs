namespace ArzonOL.Models;

public class CartProductModel
{
     public Guid Id {get;set;}
     public ProductModel? Product { get; set; }
     public Guid CartId { get; set; }

}