using EStore.Entities;
using FluentMigrator;
using Microsoft.AspNetCore.Http.HttpResults;
using static LinqToDB.Reflection.Methods.LinqToDB;

namespace EStore.Schemal;


[Migration(20241127)] 
public class LogSchema : Migration
{
    public override void Up()
    {

        Create.Table(nameof(LogEntity))
            .WithColumn(nameof(LogEntity.Id)).AsInt32().PrimaryKey().Identity() 
            .WithColumn(nameof(LogEntity.Message)).AsString(4000).NotNullable() 
            .WithColumn(nameof(LogEntity.DateTime)).AsDateTime().NotNullable(); 
    }

    public override void Down()
    {
  
        Delete.Table(nameof(LogEntity));
    }
}