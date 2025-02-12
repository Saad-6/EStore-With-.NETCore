using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2025386839)]
public class SelectedVariantSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(SelectedVariantEntity))
            .WithColumn(nameof(SelectedVariantEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(SelectedVariantEntity.VariantEntityId)).AsInt32().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.OptionEntityId)).AsString().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.CartItemEntityId)).AsInt32().NotNullable()
            .WithColumn(nameof(SelectedVariantEntity.PriceAdjustment)).AsDouble().Nullable()
           ;


    }

    public override void Down()
    {
        Delete.Table(nameof(SelectedVariantEntity));
    }
}

