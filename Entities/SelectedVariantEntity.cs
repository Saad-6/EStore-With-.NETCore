using EStore.Models.Basket;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "SelectedVariantEntity")]
public class SelectedVariantEntity : SelectedVariant
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "CartItemEntityId"), NotNull]
    public int CartItemEntityId { get; set; }
    [Column(Name = "VariantName"), NotNull]
    public override string VariantName { get; set; }

    [Column(Name = "OptionValue"), NotNull]
    public override string OptionValue { get; set; }

    [Column(Name = "PriceAdjustment"), NotNull]
    public override decimal PriceAdjustment { get; set; }
}
