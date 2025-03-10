using EStore.Models.Order;
using EStore.Models;
using EStore.DTOs;

namespace EStore.Interfaces;

public interface IOrderRepository
{
    Task<(List<UserOrder>, int)> GetAllAsync(Status status = Status.All, int page = 1, int size = 5, string userId = "");
    Task<List<UserOrder>> GetOrderByParamsAsync(string param);
    Task<Response> CreateOrderAsync(OrderCreateDto orderDto);

    Task<List<UserOrder>> GetUserOrders(string userId);
    Task<bool> UpdateOrderStatus(string orderId, Status status);
}
