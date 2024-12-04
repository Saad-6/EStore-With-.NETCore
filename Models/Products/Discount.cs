namespace EStore.Models.Products;

public class Discount : BaseEntity
{
    public virtual bool isActive { get; set; } = false;
    public virtual decimal? DiscountPrice { get; set; }
    public virtual DateTime? DiscountStartDate { get; set; }
    public virtual DateTime? DiscountEndDate { get; set; }
}
