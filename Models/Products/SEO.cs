namespace EStore.Models.Products;

public class SEO : BaseEntity
{
    public virtual string? MetaTitle { get; set; } // Custom title for SEO purposes
    public virtual string? MetaDescription { get; set; } // Meta description for search engines
    public virtual string? MetaKeywords { get; set; } // Keywords relevant to SEO
    public virtual string? CanonicalUrl { get; set; } // Canonical URL for SEO

}
