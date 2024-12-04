using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111404)]
public class DiscountSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(DiscountEntity))
            .WithColumn(nameof(DiscountEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(DiscountEntity.DiscountPrice)).AsDecimal().Nullable()
            .WithColumn(nameof(DiscountEntity.DiscountEndDate)).AsDateTime().Nullable()
            .WithColumn(nameof(DiscountEntity.DiscountStartDate)).AsDateTime().Nullable()
            .WithColumn(nameof(DiscountEntity.isActive)).AsBoolean().NotNullable();
       
    }

    public override void Down()
    {
        Delete.Table(nameof(DiscountEntity));
    }
}
