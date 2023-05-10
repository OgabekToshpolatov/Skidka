namespace ArzonOL.ViewModels.Category;

public class CategoryView
{
   public Guid Id { get; set; }
   public string? Name { get; set; }
   public string? Description { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime UpdatedAt { get; set; } = DateTime.Now;
   public ICollection<CategoryApproachDtoView>? Approaches { get; set; }
}