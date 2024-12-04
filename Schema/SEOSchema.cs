using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111410)]
public class SEOSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(SEOEntity))
            .WithColumn(nameof(SEOEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(SEOEntity.MetaKeywords)).AsString().NotNullable()
            .WithColumn(nameof(SEOEntity.MetaTitle)).AsString().NotNullable()
            .WithColumn(nameof(SEOEntity.CanonicalUrl)).AsString().NotNullable()
            .WithColumn(nameof(SEOEntity.MetaDescription)).AsString().NotNullable();
    
    }

    public override void Down()
    {
        Delete.Table(nameof(SEOEntity));
    }
}
