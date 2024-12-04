using EStore.Models.Products;
using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models;

public class ProductDTO
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string SKU { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [StringLength(100)]
    public string Brand { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(200)]
    public string Slug { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public SEODTO SEO { get; set; }

    [Required]
    public ProductImageDTO PrimaryImage { get; set; }
    public List<VariantDTO> Variants { get; set; }

    public List<ProductImageDTO> Images { get; set; } = new List<ProductImageDTO>();
}

public class SEODTO
{
    [Required]
    [StringLength(60)]
    public string MetaTitle { get; set; }

    [Required]
    [StringLength(160)]
    public string MetaDescription { get; set; }

    [StringLength(200)]
    public string MetaKeywords { get; set; }

    [Required]
    [Url]
    public string CanonicalUrl { get; set; }
}

public class ProductImageDTO
{
    [Required]
    [Url]
    public string Url { get; set; }

    [Required]
    [StringLength(100)]
    public string AltText { get; set; }
}