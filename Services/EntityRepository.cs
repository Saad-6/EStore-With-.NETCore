using EStore.Data;
using EStore.Interfaces;
using EStore.Models;
using LinqToDB;
using LinqToDB.Data;

namespace EStore.Services;

public class EntityRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AltDataContext _dbContext;
    private readonly ILogRepository _logger;
    public EntityRepository(AltDataContext dbContext, ILogRepository logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Response> BulkSaveAsync(List<T> entities)
    {
        try
        {
            await _dbContext.BulkCopyAsync(entities);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return new Response { Success = false, Error = ex.Message };
        }
        return new Response { Success = true };
    }

    public async Task<Response> DeleteAsync(T entity)
    {
        try
        {
            await _dbContext.DeleteAsync(entity);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return new Response { Error = ex.Message, Success = false };

        }
        return new Response { Success = true };
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.GetTable<T>().ToListAsync();
    }
    public async Task<List<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        int index = (pageNumber - 1) * pageSize;
        return await _dbContext.GetTable<T>().Skip(index).Take(pageSize).ToListAsync();
    }

    public async Task<T> GetById(int id)
    {
        return await _dbContext.GetTable<T>().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Response> SaveAsync(T entity)
    {
        
        try
        {
            await _dbContext.InsertAsync(entity);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return new Response { Error = ex.Message, Success = false };

        }
        return new Response { Success = true };
    }

    public async Task<int> SaveAndGetIdAsync(T entity)
    {
        return await _dbContext.InsertWithInt32IdentityAsync(entity);
    }

    public async Task<Response> UpdateAsync(T entity)
    {
        try
        {
            await _dbContext.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            await _logger.LogAsync(ex.Message);
            return new Response { Error = ex.Message, Success = false };

        }
        return new Response { Success = true };
    }
}
