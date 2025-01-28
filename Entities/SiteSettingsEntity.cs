using EStore.Models;
using LinqToDB.Mapping;


namespace EStore.Entities;

[Table(Name = "SiteSettingsEntity")]
public class SiteSettingsEntity : SiteSettings
{
    [PrimaryKey, Identity]
    public new int Id { get; set; }

    [Column(Name = "URL"), NotNull]
    public override string URL {  get; set; }
}
