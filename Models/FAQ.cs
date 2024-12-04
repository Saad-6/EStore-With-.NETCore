using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace EStore.Models;

public class FAQ : BaseEntity
{
    public string Question { get; set; }
    public string Answer { get; set; }
    public Product Product { get; set; }
}
