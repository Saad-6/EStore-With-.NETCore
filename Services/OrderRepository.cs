using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Order;
using LinqToDB;

namespace EStore.Services;

public class OrderRepository : IOrderRepository
{
    private readonly AltDataContext _dataContext;
    private readonly ILogRepository _logger;
    public OrderRepository(AltDataContext dataContext, ILogRepository logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }
    public async Task<Response> CreateOrderAsync(OrderCreateDto orderDto)
    {
        var response = new Response();
        try
        {
            // Save address 
        var addressEntity = new AddressEntity()
        {
            City = orderDto.Address.City,
            ZipCode = orderDto.Address.ZipCode,
            StreetAddress = orderDto.Address.StreetAddress,
            FirstName = orderDto.Address.FirstName,
            LastName = orderDto.Address.LastName,
            PhoneNumber = orderDto.Address.PhoneNumber,
        };

        var addressId = await _dataContext.InsertWithInt32IdentityAsync(addressEntity);

            // Save order with address Id
            var orderEntity = new OrderEntity()
            {
                AddressId = addressId,
                Created = DateTime.UtcNow,
                Total = orderDto.Total,
                Status = "pending",
                PaymentMethod = orderDto.PaymentMethod,
                UserId = orderDto?.UserId 
            };

        var orderId = await _dataContext.InsertWithInt32IdentityAsync(orderEntity);

            // Save cartItems with OrderId'
            var cartItems = new List<CartItemEntity>();
            foreach(var ci in orderDto.CartItems)
            {
                cartItems.Add(new CartItemEntity
                {
                    OrderEntityId = orderId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                
                });
            }



            response.Data = orderEntity;

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = ex.Message;
            await _logger.LogAsync(ex.Message);
        }

        return response;
    }

    public Task<List<UserOrder>> GetAllAsync(Status status = Status.All)
    {
        throw new NotImplementedException();
    }

    public Task<UserOrder> GetOrderByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrder(UserOrder order)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateOrderStatus(string orderId, Status status)
    {
        throw new NotImplementedException();
    }
}
