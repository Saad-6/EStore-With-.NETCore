using EStore.Models.User;

namespace EStore.Models;

public class Review : BaseEntity
{
    public int Stars { get; set; }
    public string GivenBy { get; set; }
    public string? Comment { get; set; }
    public DateTime PostedAt { get; set; }
}
public class ReviewDTO
{
    public int Stars { get; set; }
    public string UserId { get; set; }
    public string? Comment { get; set; }
    
}
