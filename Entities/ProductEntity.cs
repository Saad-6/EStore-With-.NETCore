using EStore.Models;
using EStore.Models.Products;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "ProductEntity")] 
public class ProductEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "Name"), Nullable] 
    public string? Name { get; set; }

    [Column(Name = "Description", Length = 1500), Nullable] 
    public string? Description { get; set; }

    [Column(Name = "Price"), NotNull] 
    public decimal Price { get; set; }

    [Column(Name = "SKU"), Nullable] 
    public string? SKU { get; set; }

    [Column(Name = "Stock"), NotNull] 
    public int Stock { get; set; }

    [Column(Name = "Brand"), Nullable] 
    public string? Brand { get; set; }

    [Column(Name = "CategoryId"), NotNull] 
    public int CategoryId { get; set; }

    [Column(Name = "Slug"), Nullable] 
    public string? Slug { get; set; }

    [Column(Name = "IsActive"), NotNull] 
    public bool IsActive { get; set; } = true;

    [Column(Name = "SEOId"), Nullable] 
    public int? SEOId { get; set; }

    [Column(Name = "PrimaryImageId"), Nullable] 
    public int? PrimaryImageId { get; set; }

    [Column(Name = "DiscountId"), Nullable] 
    public int? DiscountId { get; set; }
}