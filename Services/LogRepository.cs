using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using LinqToDB;

namespace EStore.Services;

public class LogRepository : ILogRepository
{
    private readonly AltDataContext _context;
    public LogRepository(AltDataContext context)
    {
        _context = context;
    }
    public async Task Clear()
    {
        await _context.Logs.DeleteAsync();
    }

    public async Task<List<LogEntity>> GetAllAsync()
    {
       return await _context.Logs.ToListAsync();
    }

    public async Task LogAsync(string message)
    {
       var log = new LogEntity
       {
           DateTime = DateTime.Now,
           Message = message
       };
        await _context.InsertAsync(log);
    }

}
