namespace EStore.Models;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ThumbNailUrl { get; set; }
}
