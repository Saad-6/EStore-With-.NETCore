using EStore.Models.Order;

namespace EStore.DTOs;

public class AddToCartDTO
{
    public List<CartItemDto> CartItems { get; set; }
    public string UserId { get; set; }
    public double Total { get; set; }
}
public class UpdateCartDTO : AddToCartDTO
{
}

