using EStore.Code;
using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using EStore.Services;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    
    public CategoryController(ICategoryRepository categoryRepository)
    {
         _categoryRepository = categoryRepository;

    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromForm] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
 

        var response = await _categoryRepository.AddCategory(category);

        if (!response)
        {
            return BadRequest(ModelState);
        }

            return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveCategory(int id)
    {
        if (id == 0)
        {
        return BadRequest();
        }

        await _categoryRepository.DeleteCategory(id);
        return Ok();

    }
}
