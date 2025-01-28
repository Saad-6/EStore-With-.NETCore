using EStore.Models.Order;
using EStore.Models;

namespace EStore.Interfaces;

public interface IOrderRepository
{
    Task<List<UserOrder>> GetAllAsync(Status status = Status.All);
    Task<UserOrder> GetOrderByIdAsync(string id);
    Task<Response> CreateOrderAsync(OrderCreateDto orderDto);
    Task UpdateOrder(UserOrder order);
    Task<bool> UpdateOrderStatus(string orderId, Status status);
}
