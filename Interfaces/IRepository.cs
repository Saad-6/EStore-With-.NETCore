using EStore.Models;

namespace EStore.Interfaces;

public interface IRepository<T> where T : class
{
    Task<Response> SaveAsync(T entity);
    Task<int> SaveAndGetIdAsync(T entity);
    Task<Response> BulkSaveAsync(List<T> entities);
    Task<List<T>> GetAllAsync();
    Task<T> GetById(int id);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);

}
