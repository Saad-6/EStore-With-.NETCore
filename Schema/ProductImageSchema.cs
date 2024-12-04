using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;


[Migration(24023111407)]
public class ProductImageSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(ProductImageEntity))
            .WithColumn(nameof(ProductImageEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(ProductImageEntity.AltText)).AsString().NotNullable()
            .WithColumn(nameof(ProductImageEntity.ProductId)).AsInt32().Nullable()
            .WithColumn(nameof(ProductImageEntity.VariantOptionsEntityId)).AsInt32().Nullable()
            .WithColumn(nameof(ProductImageEntity.Url)).AsString(2000).NotNullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(ProductImageEntity));
    }
}
