using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111409)]
public class SelectedVariantSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(SelectedVariantEntity))
            .WithColumn(nameof(SelectedVariantEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(SelectedVariantEntity.VariantName)).AsString().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.OptionValue)).AsString().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.PriceAdjustment)).AsDecimal().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.CartItemEntityId)).AsInt32().NotNullable();


    }

    public override void Down()
    {
        Delete.Table(nameof(SelectedVariantEntity));
    }
}

