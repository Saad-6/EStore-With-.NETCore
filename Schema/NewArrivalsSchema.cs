using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schema;

[Migration(2023111502)]
public class NewArrivalsSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(NewArrivalsEntity))
            .WithColumn(nameof(NewArrivalsEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NewArrivalsEntity.ProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(NewArrivalsEntity.HomePageSettingsId)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table(nameof(NewArrivalsEntity));
    }
}