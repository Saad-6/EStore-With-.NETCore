
using EStore.Models.Basket;
using EStore.Models.Order;
using Microsoft.AspNetCore.Identity;

namespace EStore.Models.User;

public class AppUser : IdentityUser
{
    public Cart Cart { get; set; } = new Cart(); 
    public Address? Address { get; set; } = new Address();
    public List<UserOrder>? Orders { get; set; } = new List<UserOrder>();
}
