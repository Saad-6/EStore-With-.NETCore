using EStore.Models.Order;
using EStore.Models;
using EStore.DTOs;

namespace EStore.Interfaces;

public interface IOrderRepository
{
    Task<List<UserOrder>> GetAllAsync(Status status = Status.All, string userId = "");
    Task<UserOrder> GetOrderByIdAsync(string id);
    Task<Response> CreateOrderAsync(OrderCreateDto orderDto);

    Task<List<UserOrder>> GetUserOrders(string userId);
    Task<bool> UpdateOrderStatus(string orderId, Status status);
}
