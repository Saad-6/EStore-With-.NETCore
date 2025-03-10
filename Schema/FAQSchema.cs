using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(206311138223)]
public class FAQSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(FAQEntity))
            .WithColumn(nameof(FAQEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(FAQEntity.Question)).AsString(300).Nullable()
            .WithColumn(nameof(FAQEntity.Answer)).AsString(1000).Nullable();
            

    }

    public override void Down()
    {
        Delete.Table(nameof(FAQEntity));
    }
}
