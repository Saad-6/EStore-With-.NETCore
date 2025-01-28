using EStore.Entities;
using EStore.Models;

namespace EStore.Interfaces;
public interface ICategoryRepository
{
    Task<bool> AddCategory(Category category);
    Task<List<CategoryEntity>> GetCategoriesByName(string query);
    Task<bool> DeleteCategory(int id);
    Task<CategoryEntity> GetCategoryById(int id);
    Task<List<CategoryEntity>> GetAllCategoriesAsync();
}
