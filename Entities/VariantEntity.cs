using EStore.Models;
using EStore.Models.Products;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "VariantEntity")]
public class VariantEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "Name"), NotNull]
    public string Name { get; set; } // Name of the variant (e.g., "Color", "Size")

    [Column(Name = "DisplayType"), NotNull]
    public string DisplayType { get; set; } // Display type (e.g., dropdown, radio button)

    [Column(Name = "ProductId"), NotNull]
    public int ProductId { get; set; } // Foreign key linking to ProductEntity
}