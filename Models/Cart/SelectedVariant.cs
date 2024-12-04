namespace EStore.Models.Basket;

public class SelectedVariant : BaseEntity
{
    public virtual string VariantName { get; set; }
    public virtual string OptionValue { get; set; }
    public virtual decimal PriceAdjustment { get; set; }
}
