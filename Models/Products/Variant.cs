namespace EStore.Models.Products;

public class Variant : BaseEntity
{
    public string Name { get; set; }
    public string DisplayType { get; set; }
    public List<VariantOption> Options { get; set; }
}

public enum DisplayType
{
    Dropdown,
    Checkbox
}

public class VariantOption : BaseEntity
{
    public string Value { get; set; }
    public List<ProductImage>? OptionImages { get; set; }
    public decimal PriceAdjustment { get; set; } = 0; 
    public int Stock { get; set; } = 0; 
    public string? SKU { get; set; }
}

public class VariantDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<VariantOptionDTO> Options { get; set; }
}

public class VariantOptionDTO
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<ProductImage>? OptionImages { get; set; }
    public decimal PriceAdjustment { get; set; } = 0;
    public int Stock { get; set; } = 0;
    public string? SKU { get; set; }
}