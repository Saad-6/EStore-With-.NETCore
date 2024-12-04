using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using LinqToDB;

namespace EStore.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly AltDataContext _dataContext;
    private readonly ILogRepository _logger;
    public CategoryRepository(AltDataContext dataContext, ILogRepository logger)
    {
        _dataContext = dataContext;
        _logger = logger;
    }

    public async Task<bool> AddCategory(CategoryEntity category)
    {
        try
        {
            await _dataContext.InsertAsync(category);
            return true;
        }
        catch(Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteCategory(int id)
    {
        try
        {
            var category = await _dataContext.Categories.FirstOrDefaultAsync(m=>m.Id == id);
            if (category == null) return false;

            await _dataContext.DeleteAsync(category);
            return true;
        }
        catch(Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return false;
        }
    }

    public async Task<List<CategoryEntity>> GetAllCategoriesAsync()
    {
        return await _dataContext.Categories.ToListAsync();
    }

    public async Task<List<CategoryEntity>> GetCategoriesByName(string query)
    {
        return await _dataContext.Categories
            .Where(c => c.Name.Contains(query))
            .ToListAsync();
    }

    public async Task<CategoryEntity> GetCategoryById(int id)
    {
        return await _dataContext.Categories.FirstOrDefaultAsync(m=>m.Id == id);
    }

}
