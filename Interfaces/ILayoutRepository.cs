using EStore.Models;
using EStore.Models.Layout;

namespace EStore.Interfaces;

public interface ILayoutRepository
{
    Task<HomePageLayout> GetByIdAsync(int id);
    Task<bool> SaveAsync(HomePageLayout layout, Operation operation = Operation.Add);
    Task<bool> DeleteAsync(int id);
    Task<List<HomePageLayout>> GetLayoutsAsync();
    Task<HomePageLayout> GetActiveLayout();
    Task<Response> ActivateLayout(int layoutId);
}
