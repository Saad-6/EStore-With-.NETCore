using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;

[Migration(8513628562)]
public class UserQuerySchema : Migration
{
    public override void Down()
    {
       Delete.Table(nameof(UserQueryEntity));
    }

    public override void Up()
    {
        Create.Table(nameof(UserQueryEntity))
            .WithColumn(nameof(UserQueryEntity.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(UserQueryEntity.Name)).AsString(100).NotNullable()
            .WithColumn(nameof(UserQueryEntity.Email)).AsString(150).NotNullable()
            .WithColumn(nameof(UserQueryEntity.Phone)).AsString(20).Nullable()
            .WithColumn(nameof(UserQueryEntity.Category)).AsString(40).Nullable()
            .WithColumn(nameof(UserQueryEntity.Subject)).AsString(400).NotNullable()
            .WithColumn(nameof(UserQueryEntity.Message)).AsString(1500).NotNullable()
            .WithColumn(nameof(UserQueryEntity.Response)).AsString(1500).Nullable()
            .WithColumn(nameof(UserQueryEntity.IsRead)).AsBoolean().Nullable()
            .WithColumn(nameof(UserQueryEntity.IsResolved)).AsBoolean().Nullable()
            .WithColumn(nameof(UserQueryEntity.CreatedAt)).AsDateTime().Nullable()
            ;
    }
}
