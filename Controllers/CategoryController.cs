using EStore.Code;
using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly AltDataContext _context;

    public CategoryController(ICategoryRepository categoryRepository,AltDataContext context)
    {
         _categoryRepository = categoryRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var categoryToSave = new CategoryEntity
        {
            Description = category.Description,
            Name = category.Name,
            ThumbNailUrl = category.ThumbNailUrl
        };

        try
        {
            // Use the mapped entity to insert into the database
            await _context.InsertAsync(categoryToSave);
            return Ok(categoryToSave);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }


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
