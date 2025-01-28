using EStore.Data;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Basket;
using EStore.Models.Order;
using EStore.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class OrderHelper : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public OrderHelper(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> UpdateOrderStatus(string orderId, Status status)
    {
        var order = await GetOrderByIdAsync(orderId);
        if (order is null)
        {
            return false;
        }
        order.Status = status;
        await UpdateOrder(order);
        return true;

    }

    public async Task UpdateOrder(UserOrder order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Response> CreateOrderAsync(OrderCreateDto orderDto)
    {
        var response = new Response();
        try
        {

            AppUser? user = null;
            if (!string.IsNullOrEmpty(orderDto.UserId))
            {
                user = await _userManager.FindByIdAsync(orderDto.UserId);
            }

            var cartItems = new List<CartItem>();

            foreach (var ci in orderDto.CartItems)
            {
                var product = await _context.Products
                    .Include(p => p.Variants)
                    .ThenInclude(v => v.Options)
                    .FirstOrDefaultAsync(p => p.Id == ci.ProductId);

                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {ci.ProductId} not found.");
                }

                var selectedVariants = ci.SelectedVariants.Select(sv => new SelectedVariant
                {
                    VariantName = sv.Key,
                    OptionValue = sv.Value.Value,
                    PriceAdjustment = sv.Value.PriceAdjustment
                }).ToList();

                var subTotal = ci.Quantity * (product.Price + selectedVariants.Sum(sv => sv.PriceAdjustment));

                var cartItem = new CartItem
                {
                    Product = product,
                    Quantity = ci.Quantity,
                    SubTotal = subTotal,
                    SelectedVariants = selectedVariants
                };

                cartItems.Add(cartItem);
            }

            var address = new Address
            {
                FirstName = orderDto.Address.FirstName,
                LastName = orderDto.Address.LastName,
                StreetAddress = orderDto.Address.StreetAddress,
                City = orderDto.Address.City,
                ZipCode = orderDto.Address.ZipCode,
                PhoneNumber = orderDto.Address.PhoneNumber,
            };

            var order = new UserOrder(cartItems, address, user)
            {
                Total = orderDto.Total
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            response.Data = order;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
            response.Success = false;
        }

        return response;
    }

    public async Task<UserOrder> GetOrderByIdAsync(string id)
    {
        return await GetOrderQuery()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<UserOrder>> GetAllAsync(Status status = Status.All)
    {
        var query = GetOrderQuery();
        if (status != Status.All)
        {
            query = query.Where(m => m.Status == status);
        }

        return await query
            .ToListAsync();
    }

    private IQueryable<UserOrder> GetOrderQuery()
    {
        return _context.Orders
            .Include(m => m.User)
            .Include(m => m.Address)
            .Include(m => m.CartItems)
                .ThenInclude(m => m.Product)
            .Include(m => m.CartItems)
                .ThenInclude(m => m.SelectedVariants);

    }
}