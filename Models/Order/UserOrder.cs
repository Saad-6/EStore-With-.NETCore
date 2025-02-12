using EStore.Models.Basket;
using EStore.Models.Order;
using EStore.Models.User;

namespace EStore.Models;
public class UserOrder
{
    public string Id { get; set; }
    public List<CartItem> CartItems { get; set; }
    public decimal Total { get; set; }
    public AppUser? User { get; set; }
    public Address Address { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public Status Status { get; set; } = Status.Pending;

    public UserOrder()
    {
        Id = GenerateRandomId();
        CartItems = new List<CartItem>();
        Address = new Address();
    }

    public UserOrder(List<CartItem> cartItems, Address address, AppUser? user)
    {
        Id = Guid.NewGuid().ToString();
        CartItems = cartItems;
        Address = address;
        User = user;
        Created = DateTime.Now;
        Status = Status.Pending;
    }
    public static string GenerateRandomId(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; 
        var random = new Random();

        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}