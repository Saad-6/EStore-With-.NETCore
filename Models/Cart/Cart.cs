namespace EStore.Models.Basket;

public class Cart : BaseEntity 
{
    public IList<CartItem> Items { get; set; } = new List<CartItem>();
    public decimal CartTotal {  get; set; } = 0;
    public void EmptyCart()
    {
        Items.Clear();
        CartTotal = 0;
    }

}
