using EStore.Models;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "ReviewEntity")]
public class ReviewEntity 
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column(Name = "ProductId"), NotNull]
    public int ProductId { get; set; }

    [Column(Name = "Stars"), NotNull]
    public int Stars { get; set; }

    [Column(Name = "UserId"), NotNull]
    public string UserId { get; set; }

    [Column(Name = "Comment"), NotNull]
    public string? Comment {  get; set; }

    [Column(Name = "CreatedAt"), NotNull]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
