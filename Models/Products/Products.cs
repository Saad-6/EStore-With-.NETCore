using EStore.Models;
using EStore.Models.Products;

public class Product : BaseEntity
{
    public virtual string? Name { get; set; } 
    public virtual string? Description { get; set; } 
    public virtual decimal Price { get; set; } 
    public virtual string? SKU { get; set; } 
    public virtual int Stock { get; set; } 
    public virtual string? Brand { get; set; } 
    public virtual Category? Category { get; set; }
    public virtual string? Slug { get; set; } 
    public virtual bool IsActive { get; set; } = true;
    public virtual SEO? SEO { get; set; }
    public virtual ProductImage? PrimaryImage { get; set; }
    public virtual List<ProductImage>? Images { get; set; } 
    public virtual Discount? Discount { get; set; }
    public virtual List<Review> Reviews { get; set; }
    public virtual List<FAQ> FAQs { get; set; }
    public virtual List<Variant> Variants { get; set; }
    public Product()
    {
        Images = new List<ProductImage>();
        SEO = GenerateSEOFields();
        Category = new Category();
        Discount = new Discount();
        PrimaryImage = new ProductImage();
        Reviews = new List<Review>();
        FAQs = new List<FAQ>();
        Variants = new List<Variant>();
        Slug = GenerateSlug();
    }
    public string GenerateSlug()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return Name;
        }

        return Name.Replace(' ', '-');
    }
    public SEO GenerateSEOFields()
    {
        return new SEO
        {
            MetaTitle = string.IsNullOrWhiteSpace(this.SEO?.MetaTitle) ? $"{Name} - Buy {Name} Online" : this.SEO.MetaTitle,
            MetaDescription = string.IsNullOrWhiteSpace(this.SEO?.MetaDescription) ? $"Purchase {Name} for {Price:C}. {Description}" : this.SEO.MetaDescription,
            CanonicalUrl = string.IsNullOrWhiteSpace(this.SEO?.CanonicalUrl) ? $"/{Slug}" : this.SEO.CanonicalUrl,
        };
    }

    public decimal GetAverageReview()
    {
        if (Reviews == null || !Reviews.Any())
        {
            return 0;
        }
        return (decimal)Reviews.Average(r => r.Stars);
    }
}
public class ProductImage
{
    public virtual int Id { get; set; }
    public virtual string Url { get; set; } 
    public virtual string AltText { get; set; } 
}
