using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "HomePageLayoutEntity")]
public class HomePageLayoutEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public int Id { get; set; }

    [Column(Name = "Name"), NotNull]
    public string Name { get; set; }

    [Column(Name = "IsActive"), NotNull]
    public bool IsActive { get; set; }

    [Column(Name = "HomePageSettingsId"), Nullable]
    public virtual int? HomePageSettingsId { get; set; }
}