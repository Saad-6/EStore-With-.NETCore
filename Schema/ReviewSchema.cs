using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(202828521401)]
public class ReviewSchema : Migration
{

    public override void Up()
    {
        Create.Table(nameof(ReviewEntity))
            .WithColumn(nameof(ReviewEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn((nameof(ReviewEntity.ProductId))).AsInt32().NotNullable()
            .WithColumn(nameof(ReviewEntity.Stars)).AsInt32().NotNullable()
            .WithColumn(nameof(ReviewEntity.Comment)).AsString(4000).Nullable()
            .WithColumn(nameof(ReviewEntity.UserId)).AsString().NotNullable()
            .WithColumn(nameof(ReviewEntity.CreatedAt)).AsDateTime().NotNullable();
    }
    public override void Down()
    {
        Delete.Table(nameof(ReviewEntity));
    }

   
}
