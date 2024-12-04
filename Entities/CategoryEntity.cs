using EStore.Models;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "CategoryEntity")]
public class CategoryEntity : BaseEntity
{
    [PrimaryKey, Identity] 
    public new int Id { get; set; }

    [Column(Name = "Name")]
    public string Name { get; set; }

    [Column(Name = "Description", CanBeNull = true, Length = 1000)]
    public string? Description { get; set; }

    [Column(Name = "ThumbNailUrl", CanBeNull = true)]
    public string? ThumbNailUrl { get; set; }

}