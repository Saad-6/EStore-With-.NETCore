using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schema;

[Migration(2023111504)]
public class HomePageSettingsSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(HomePageSettingsEntity))
            .WithColumn(nameof(HomePageSettingsEntity.Id)).AsInt32().PrimaryKey().Identity();
    }

    public override void Down()
    {
        Delete.Table(nameof(HomePageSettingsEntity));
    }
}