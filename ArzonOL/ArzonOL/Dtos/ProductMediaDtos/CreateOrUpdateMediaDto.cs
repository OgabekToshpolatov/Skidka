namespace ArzonOL.Dtos.ProductMediaDtos;

public class CreateOrUpdateMediaDto
{
    public Guid? Id { get; set; }
    public Guid? ProductId { get; set; }
    public string? ImageBase64String { get; set; }
}