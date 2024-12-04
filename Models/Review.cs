using EStore.Models.User;

namespace EStore.Models;

public class Review : BaseEntity
{
    public int Stars { get; set; }
    public AppUser? GivenBy { get; set; }
    public string? Comment { get; set; }
}
