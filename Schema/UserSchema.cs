
using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(2023111411)]
public class UserSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(UserEntity))
            .WithColumn(nameof(UserEntity.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(UserEntity.UserName)).AsString().NotNullable()
            .WithColumn(nameof(UserEntity.Email)).AsString().NotNullable()
            .WithColumn(nameof(UserEntity.EmailConfirmed)).AsBoolean().NotNullable()
            .WithColumn(nameof(UserEntity.Password)).AsString().NotNullable()
            .WithColumn(nameof(UserEntity.RoleId)).AsInt32().NotNullable();

    }

    public override void Down()
    {
        Delete.Table(nameof(UserEntity));
    }
}
