using EStore.Entities;
using EStore.Models.Products;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111413)]
public class VariantOptionSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(VariantOptionEntity))
            .WithColumn(nameof(VariantOptionEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(VariantOptionEntity.VariantEntityId)).AsInt32().NotNullable()
            .WithColumn(nameof(VariantOptionEntity.Value)).AsString().NotNullable()
            .WithColumn(nameof(VariantOptionEntity.PriceAdjustment)).AsDecimal().Nullable()
            .WithColumn(nameof(VariantOptionEntity.SKU)).AsString().NotNullable()
            .WithColumn(nameof(VariantOptionEntity.Stock)).AsInt16().NotNullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(VariantOptionEntity));
    }
}
