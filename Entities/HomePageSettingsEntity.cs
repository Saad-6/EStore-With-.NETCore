using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "HomePageSettingsEntity")]
public class HomePageSettingsEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public int Id { get; set; }
}
