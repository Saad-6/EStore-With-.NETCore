using EStore.Models;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "CartItemEntity")] 
public class CartItemEntity : BaseEntity
{
    [PrimaryKey, Identity] 
    public new int Id { get; set; }

    [Column(Name = "ProductId"), NotNull] 
    public int ProductId { get; set; }

    [Column(Name = "Quantity"), NotNull] 
    public int Quantity { get; set; } = 0;

    [Column(Name = "SubTotal"), NotNull] 
    public decimal SubTotal { get; set; } = decimal.Zero;

    [Column(Name = "OrderEntityId"), Nullable] 
    public int OrderEntityId { get; set; }
}
