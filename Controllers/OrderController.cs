using EStore.DTOs;
using EStore.Interfaces;
using EStore.Models.Order;
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
            var order = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while processing your order.");
        }
    }

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

    [HttpGet]
    public async Task<IActionResult> GetAllOrders([FromQuery] Status? status)
    {
        var orders = await _orderService.GetAllAsync(status ?? Status.All);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }
}
