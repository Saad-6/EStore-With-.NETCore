using EStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly ILogRepository _logger;
    public LogController(ILogRepository logger)
    {
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var logs =  await _logger.GetAllAsync();
        return Ok(logs);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> ClearLogs()
    {
        await _logger.Clear();
        return Ok();
    }
}
