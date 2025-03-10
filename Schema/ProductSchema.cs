using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schema;

[Migration(202311643401)]
public class ProductSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(ProductEntity))
            .WithColumn(nameof(ProductEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(ProductEntity.Name)).AsString().NotNullable()
            .WithColumn(nameof(ProductEntity.Description)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(ProductEntity.Brand)).AsString().Nullable()
            .WithColumn(nameof(ProductEntity.Slug)).AsString().Nullable()
            .WithColumn(nameof(ProductEntity.Price)).AsDecimal().NotNullable()
            .WithColumn(nameof(ProductEntity.IsActive)).AsBoolean().Nullable()
            .WithColumn(nameof(ProductEntity.CategoryId)).AsInt32().Nullable()
            .WithColumn(nameof(ProductEntity.PrimaryImageId)).AsInt32().NotNullable()
            .WithColumn(nameof(ProductEntity.SEOId)).AsInt32().Nullable()
            .WithColumn(nameof(ProductEntity.DiscountId)).AsInt32().Nullable()
            .WithColumn(nameof(ProductEntity.SKU)).AsString().Nullable()
            .WithColumn(nameof(ProductEntity.Stock)).AsInt32().NotNullable();

 
    }

    public override void Down()
    {
        Delete.Table(nameof(ProductEntity));
    }
}
