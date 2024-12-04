using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111412)]
public class VariantSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(VariantEntity))
            .WithColumn(nameof(VariantEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(VariantEntity.Name)).AsString().NotNullable()
            .WithColumn(nameof(VariantEntity.DisplayType)).AsString().NotNullable()
            .WithColumn(nameof(VariantEntity.ProductId)).AsInt32().NotNullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(VariantEntity));
    }
}
