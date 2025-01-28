using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using LinqToDB;
using static System.Net.Mime.MediaTypeNames;

namespace EStore.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly AltDataContext _dataContext;
    private readonly ILogRepository _logger;

    private readonly FileHandler _fileHandler;
    private const string DIRECTORY = "StaticFiles";
    private const string SUB_DIRECTORY = "images/categories";
    private readonly IAppSettingsService _appSettingsService;

    public CategoryRepository(AltDataContext dataContext, ILogRepository logger,FileHandler fileHandler, IAppSettingsService appSettingsService)
    {
        _dataContext = dataContext;
        _logger = logger;
        _fileHandler = fileHandler;
        _appSettingsService = appSettingsService;
    }

    public async Task<bool> AddCategory(Category category)
    {
        try
        {
            var file = await _fileHandler.SaveFileAsync(category?.thumbnail, DIRECTORY, SUB_DIRECTORY);

            var fileName = (string)file.Data;

            var newCategory = new CategoryEntity
            {
                ThumbNailUrl = fileName,
                Description = category.Description,
                Name = category.Name,
            };

            await _dataContext.InsertAsync(newCategory);
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
        return await _dataContext.Categories.Select(m=> new CategoryEntity
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            ThumbNailUrl = m.ThumbNailUrl.Contains("http", StringComparison.OrdinalIgnoreCase) 
            ? m.ThumbNailUrl 
            :  $"{_appSettingsService.BaseUrl}/{m.ThumbNailUrl}",
        }).ToListAsync();
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
