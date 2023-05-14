namespace ArzonOL.Dtos.CategoryDtos;

public class CategoryResponseIdDto
{
   public Guid Id { get; set; }
   public string? Name { get; set; }
   public string? Description { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime UpdatedAt { get; set; } = DateTime.Now;
   public ICollection<CategoryApproachResponseDto>? Approaches { get; set; }
}