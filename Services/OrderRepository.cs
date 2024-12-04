using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Order;

namespace EStore.Services;

public class OrderRepository : IOrderRepository
{
    public Task<UserOrder> CreateOrderAsync(OrderCreateDto orderDto)
    {
        throw new NotImplementedException();
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
