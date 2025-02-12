using EStore.Models.Basket;
using EStore.Models.Order;
using EStore.Models.User;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "OrderEntity")] 
public class OrderEntity
{
    [PrimaryKey, NotNull] 
    public string Id { get; set; }

    [Column(Name = "Total"), NotNull] 
    public decimal Total { get; set; }

    [Column(Name = "UserId"), Nullable] 
    public string UserId { get; set; }

    [Column(Name = "AddressId"), NotNull] 
    public int AddressId { get; set; }

    [Column(Name = "PaymentMethod"), NotNull] 
    public string PaymentMethod { get; set; }

    [Column(Name = "Created"), NotNull] 
    public DateTime Created { get; set; } = DateTime.Now;

    [Column(Name = "Status"), NotNull] 
    public string Status { get; set; }
    public OrderEntity()
    {
        Id = GenerateRandomId();
        PaymentMethod = "cod";
    }

    public OrderEntity(List<CartItem> cartItems, AddressEntity address, AppUser? user)
    {
        Id = GenerateRandomId();
        AddressId = address.Id;
        UserId = user.Id;
        Created = DateTime.Now;
        Status = "pending";
        PaymentMethod = "cod";
    }
    public void Confirm()
    {
        Status = "confirmed";
    }
    public void Cancel()
    {
        Status = "cancelled";
    }
    public void Ship()
    {
        Status = "shipped";
    }
    private string GenerateRandomId(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
