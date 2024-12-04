namespace EStore.Models.Order;


public class OrderCreateDto
{
    public List<CartItemDto> CartItems { get; set; }
    public AddressDto Address { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Total { get; set; }
    public string? UserId { get; set; }
}
public class CartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Dictionary<string, VariantOptionDto> SelectedVariants { get; set; }
}

public class VariantOptionDto
{
    public string Value { get; set; }
    public decimal PriceAdjustment { get; set; }
}
public class AddressDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string PhoneNumber { get; set; }
}