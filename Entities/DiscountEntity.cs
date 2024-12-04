using EStore.Models.Products;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "DiscountEntity")] 
public class DiscountEntity : Discount
{
    [PrimaryKey, Identity] 
    public new int Id { get; set; }

    [Column(Name = "isActive"), NotNull] 
    public override bool isActive { get; set; }

    [Column(Name = "DiscountPrice"), Nullable] 
    public override decimal? DiscountPrice { get; set; }

    [Column(Name = "DiscountStartDate"), Nullable] 
    public override DateTime? DiscountStartDate { get; set; }

    [Column(Name = "DiscountEndDate"), Nullable] 
    public override DateTime? DiscountEndDate { get; set; }
}
