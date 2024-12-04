using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023141400)]
public class CategorySchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(CategoryEntity))
            .WithColumn(nameof(CategoryEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(CategoryEntity.Name)).AsString().NotNullable()
            .WithColumn(nameof(CategoryEntity.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(CategoryEntity.ThumbNailUrl)).AsString().Nullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(CategoryEntity));
    }
}

