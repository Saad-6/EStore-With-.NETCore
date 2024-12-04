using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111501)]
public class FeauturedProductSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(FeauturedProductEntity))
            .WithColumn(nameof(FeauturedProductEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(FeauturedProductEntity.ProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(FeauturedProductEntity.HomePageSettingsId)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(nameof(FeauturedProductEntity));
    }
}
