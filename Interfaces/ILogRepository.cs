using EStore.Entities;

namespace EStore.Interfaces;

public interface ILogRepository
{
    Task<List<LogEntity>> GetAllAsync();
    Task LogAsync(string message);
    Task Clear();
}
