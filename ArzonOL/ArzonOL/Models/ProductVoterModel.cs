namespace ArzonOL.Models;

public class ProductVoterModel
{
     public int Vote { get; set; }
    public string? Comment { get; set; }
    public Guid? UserId { get; set; }
    public virtual PublicUserModel? User { get; set; }
}