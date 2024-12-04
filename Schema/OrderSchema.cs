using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111406)]
public class OrderSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(OrderEntity))
            .WithColumn(nameof(OrderEntity.Id)).AsString().PrimaryKey()
            .WithColumn(nameof(OrderEntity.AddressId)).AsInt32().NotNullable()
            .WithColumn(nameof(OrderEntity.Status)).AsString().Nullable()
            .WithColumn(nameof(OrderEntity.Created)).AsDateTime().NotNullable()
            .WithColumn(nameof(OrderEntity.PaymentMethod)).AsString().NotNullable()
            .WithColumn(nameof(OrderEntity.Total)).AsDecimal().NotNullable()
            .WithColumn(nameof(OrderEntity.UserId)).AsString().Nullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(OrderEntity));
    }
}
