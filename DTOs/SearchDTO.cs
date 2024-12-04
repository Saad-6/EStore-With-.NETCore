using EStore.Models;

namespace EStore.DTOs;

public class SearchDTO
{
    public List<Product> Products { get; set; }
    public List<Category> Categories { get; set; }
    public List<Review> Reviews { get; set; }
    public List<FAQ> FAQs { get; set; }
}
