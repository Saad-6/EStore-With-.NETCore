using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schema;

[Migration(2023111503)]
public class HeroCarouselSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(HeroCarouselEntity))
            .WithColumn(nameof(HeroCarouselEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(HeroCarouselEntity.ImageUrl)).AsString().Nullable()
            .WithColumn(nameof(HeroCarouselEntity.Title)).AsString().Nullable()
            .WithColumn(nameof(HeroCarouselEntity.Subtitle)).AsString().Nullable()
            .WithColumn(nameof(HeroCarouselEntity.ButtonText)).AsString().Nullable()
            .WithColumn(nameof(HeroCarouselEntity.HomePageSettingsId)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(nameof(HeroCarouselEntity));
    }
}