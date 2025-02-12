using EStore.Models.Basket;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "SelectedVariantEntity")]
public class SelectedVariantEntity : SelectedVariant
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "VariantEntityId"), NotNull]
    public new int VariantEntityId { get; set; }

    [Column(Name = "OptionEntityId"), NotNull]
    public new int OptionEntityId { get; set; }

    [Column(Name = "CartItemEntityId"), NotNull]
    public new int CartItemEntityId { get; set; }

}
