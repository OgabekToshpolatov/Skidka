using System.ComponentModel.DataAnnotations;

namespace ArzonOL.Dtos.CategoryDtos;

public class CreateOrUpdateCategoryApproachDto
{
    [Required]
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ProductCategoryId { get; set; }
}