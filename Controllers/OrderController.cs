using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderService;

    public OrderController(IOrderRepository orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response =  await _orderService.CreateOrderAsync(orderDto);
            if(!response.Success)
            {
                return BadRequest(response.Error);
            }
            var order = (OrderEntity)response.Data;
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your order.");
        }
    }
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserOrders([FromRoute]string id)
    {
        var orders = await _orderService.GetUserOrders(id);
        return Ok(orders);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var success = await _orderService.UpdateOrderStatus(id, status.Status);
        if (!success)
        {
            return NotFound();
        }
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] Status? status, [FromQuery] int page = 1, [FromQuery] int size = 5)
    {
        var (orders, totalCount) = await _orderService.GetAllAsync(status ?? Status.All, page, size);
        var response = new
        {
            Orders = orders,
            TotalCount = totalCount,
            CurrentPage = page,
            PageSize = size
        };
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var orders = await _orderService.GetOrderByParamsAsync(id);
        if (orders == null)
        {
            return NotFound();
        }
        return Ok(orders);
    }
}
