using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(20283462902)]
public class CartItemSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(CartItemEntity))
            .WithColumn(nameof(CartItemEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(CartItemEntity.ProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(CartItemEntity.OrderEntityId)).AsString().Nullable()
            .WithColumn(nameof(CartItemEntity.Quantity)).AsInt32().NotNullable()
            .WithColumn(nameof(CartItemEntity.SubTotal)).AsDecimal().NotNullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(CartItemEntity));
    }
}


