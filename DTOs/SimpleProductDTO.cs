using EStore.Models;

namespace EStore.DTOs;

public class SimpleProductDTO : BaseEntity
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string Slug { get; set; }
    public int Stock { get; set; }

    public string CategoryName {  get; set; }
    public decimal? Price { get; set; }
}
public class SimpleCategoryDTO : BaseEntity
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }

}