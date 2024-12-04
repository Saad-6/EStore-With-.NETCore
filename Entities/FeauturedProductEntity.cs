using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "FeauturedProductEntity")]
public class FeauturedProductEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public int Id { get; set; }

    [Column(Name = "ProductId"), NotNull]
    public int ProductId { get; set; }

    [Column(Name = "HomePageSettingsId"), NotNull]
    public int HomePageSettingsId { get; set; }
}

