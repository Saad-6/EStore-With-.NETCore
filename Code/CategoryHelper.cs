using EStore.Data;
using EStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EStore.Code;

public class CategoryHelper 
{
    private readonly AppDbContext _context;

    public CategoryHelper(AppDbContext context)
    {
        _context = context;
    }
    public async Task<bool> AddCategory(Category category)
    {
        if (category == null) return false;
        _context.Categories.Add(category);
        await SaveChangesAsync();
        return true;
    }
    public async Task<List<Category>> GetCategoriesByName(string query)
    {
    
        var categories = await _context.Categories
            .Where(c => c.Name.ToLower().Contains(query.ToLower()))  
            .ToListAsync();                     

        return (new HashSet<Category>(categories)).ToList();
    }

    public async Task<bool> DeleteCategory(int id)
    {
        var category = await GetCategoryById(id);
        if (category == null) return false;
        _context.Categories.Remove(category);
        await SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Category> GetCategoryById(int id) 
    {
        return await _context.Categories.Where(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }


}
