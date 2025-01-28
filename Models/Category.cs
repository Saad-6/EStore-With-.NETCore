namespace EStore.Models;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public IFormFile thumbnail { get; set; }
}
