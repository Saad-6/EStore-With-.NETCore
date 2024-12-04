using EStore.Models.Products;
using LinqToDB.Mapping;

namespace EStore.Entities;

public class SEOEntity : SEO
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "MetaTitle"), Nullable]
    public override string? MetaTitle { get; set; }

    [Column(Name = "MetaDescription"), Nullable]
    public override string? MetaDescription { get; set; } 

    [Column(Name = "MetaKeywords"), Nullable]
    public override string? MetaKeywords { get; set; } 

    [Column(Name = "CanonicalUrl"), Nullable]
    public override string? CanonicalUrl { get; set; }
}
