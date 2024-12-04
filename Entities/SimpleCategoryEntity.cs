using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "SimpleCategoryEntity")]
public class SimpleCategoryEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public int Id { get; set; }

    [Column(Name = "CategoryId"), NotNull]
    public int CategoryId { get; set; }

    [Column(Name = "HomePageSettingsId"), NotNull]
    public int HomePageSettingsId { get; set; }
}