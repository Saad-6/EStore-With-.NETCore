using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111713)]
public class SimpleCategorySchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(SimpleCategoryEntity))
            .WithColumn(nameof(SimpleCategoryEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(SimpleCategoryEntity.CategoryId)).AsInt32().NotNullable()
            .WithColumn(nameof(SimpleCategoryEntity.HomePageSettingsId)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(nameof(SimpleCategoryEntity));
    }
}
