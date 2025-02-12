using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Layout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LayoutController : ControllerBase
{
    private readonly ILayoutRepository _layoutRepository;
    public LayoutController(ILayoutRepository layoutRepository)
    {
        _layoutRepository = layoutRepository;
    }

    [HttpGet]
    public async Task<IActionResult>GetHomePageLayouts()
    {
        var layouts = await _layoutRepository.GetLayoutsAsync();

        return Ok(layouts);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<HomePageLayout>> GetHomePageLayout(int id)
    {
        return null;
    }
    [HttpGet("/api/Layout/active")]
    public async Task<IActionResult> GetActiveHomePageLayout()
    {
        var activeLayout =  await _layoutRepository.GetActiveLayout();
        return Ok(activeLayout);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<HomePageLayout>> CreateHomePageLayout (HomePageLayout layout)
    {
        if (await _layoutRepository.SaveAsync(layout, Operation.Add)) return Ok();
        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/api/Layout/update")]
    public async Task<IActionResult> UpdateHomePageLayout (HomePageLayout layout)
    {
        if (await _layoutRepository.SaveAsync(layout, Operation.Update)) return Ok();
        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHomePageLayout(int id)
    {
        if (await _layoutRepository.DeleteAsync(id)) return Ok();
        return BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateHomePageLayout(int id)
    {
        var result = await _layoutRepository.ActivateLayout(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Error);
    }

}
