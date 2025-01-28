namespace EStore.Models.Products;

public class ProductAPI 
{
    public virtual string? Name { get; set; }
    public virtual string? Description { get; set; }
    public virtual decimal Price { get; set; }
    public virtual string? SKU { get; set; }
    public virtual int Stock { get; set; }
    public virtual string? Brand { get; set; }
    public virtual string? Slug { get; set; }
    public virtual bool IsActive { get; set; } = true; 
    public virtual SEO? SEO { get; set; }
    public int CategoryId { get; set; }
    public IFormFile PrimaryImageFile { get; set; }
    public List<IFormFile>? AdditionalImageFiles { get; set; }
    public virtual List<VariantDTO>? Variants { get; set; }
}
