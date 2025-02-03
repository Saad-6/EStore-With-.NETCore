namespace EStore.Models.Products;

public class ProductAPI 
{
    public int? Id { get; set; }
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
    public IFormFile? PrimaryImageFile { get; set; }
    public AdditionalImageUpdateDTO? AdditionalImages { get; set; }
    public VariantUpdateDTO? Variants {  get; set; }
}
public class VariantUpdateDTO
{
    public  List<VariantDTO>? NewVariants { get; set; }
    public List<string>? ExistingVariantIds { get; set; }
}
public class AdditionalImageUpdateDTO
{
    // Newly added images
    public List<IFormFile>? Files { get; set; }

    // Urls of the images that were already present and were not removed
    public List<string>? AlreadyPresentUrls { get; set; }
}