using EStore.Models.Basket;

namespace EStore.Models.Basket;

public class CartItem : BaseEntity
{
    public Product Product { get; set; } = new Product();
    public int Quantity { get; set; } = 0;
    public decimal SubTotal { get; set; } = decimal.Zero;
    public List<SelectedVariant> SelectedVariants { get; set; } = new List<SelectedVariant>();
}

