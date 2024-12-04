using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111405)]
public class FAQSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(FAQEntity))
            .WithColumn(nameof(FAQEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(FAQEntity.Question)).AsString().Nullable()
            .WithColumn(nameof(FAQEntity.Answer)).AsString().Nullable()
            .WithColumn(nameof(FAQEntity.ProductId)).AsInt32().Nullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(FAQEntity));
    }
}
