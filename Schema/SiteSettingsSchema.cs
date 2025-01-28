using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2026111503)]
public class SiteSettingsSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(SiteSettingsEntity))
             .WithColumn(nameof(SiteSettingsEntity.Id)).AsInt32().PrimaryKey().Identity()
             .WithColumn(nameof(SiteSettingsEntity.URL)).AsString().NotNullable();
    }
    public override void Down()
    {
        Delete.Table(nameof(SiteSettingsEntity));
    }
}
