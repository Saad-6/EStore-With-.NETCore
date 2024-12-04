using EStore.Models;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "VariantOptionEntity")]
public class VariantOptionEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }
    [Column(Name = "Value"), NotNull]
    public string Value { get; set; } // The specific option for the variant (e.g., "Red", "Large")

    [Column(Name = "PriceAdjustment"), NotNull]
    public decimal PriceAdjustment { get; set; } = 0; // Price adjustment for this option

    [Column(Name = "Stock"), NotNull]
    public int Stock { get; set; } = 0; // Stock available for this variant option

    [Column(Name = "SKU"), Nullable]
    public string? SKU { get; set; } // Optional SKU for the variant option

    [Column(Name = "VariantEntityId"), NotNull]
    public int VariantEntityId { get; set; } // Foreign key linking to VariantEntity
}
