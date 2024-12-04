
namespace EStore.Models;

public enum Operation
{
    Add,
    Update
}
public enum OrderType
{
    all,
    pending,
    confirmed,
    shipped,
    delivered,
    cancelled
}