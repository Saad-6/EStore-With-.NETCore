using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schema;

[Migration(2023111505)]
public class HomePageLayoutSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(HomePageLayoutEntity))
            .WithColumn(nameof(HomePageLayoutEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(HomePageLayoutEntity.Name)).AsString().NotNullable()
            .WithColumn(nameof(HomePageLayoutEntity.IsActive)).AsBoolean().Nullable()
            .WithColumn(nameof(HomePageLayoutEntity.HomePageSettingsId)).AsInt32().Nullable();
    }

    public override void Down()
    {
        Delete.Table(nameof(HomePageLayoutEntity));
    }
}