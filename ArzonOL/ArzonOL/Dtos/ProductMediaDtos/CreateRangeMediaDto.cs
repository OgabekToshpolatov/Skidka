namespace ArzonOL.Dtos.ProductMediaDtos;

public class CreateRangeMediaDto
{
    public Guid? ProductId { get; set; }
    public List<string>? ImageBase64Strings { get; set; }
}