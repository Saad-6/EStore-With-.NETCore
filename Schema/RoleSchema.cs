using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111408)]
public class RoleSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(RoleEntity))
            .WithColumn(nameof(RoleEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(RoleEntity.Role)).AsString().NotNullable();


    }

    public override void Down()
    {
        Delete.Table(nameof(RoleEntity));
    }
}
