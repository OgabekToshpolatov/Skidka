using ArzonOL.Dtos.ProductDtos;

namespace ArzonOL.Dtos.CategoryDtos;

public class CategoryApproachResponseIdDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ProductResponseDto>? Products { get; set; }
    
}