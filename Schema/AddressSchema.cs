using EStore.Entities;
using FluentMigrator;

namespace EStore.Schema;


[Migration(2023111403)]
public class AddressSchema : Migration
{
    public override void Up()
    {
        Create.Table(nameof(AddressEntity))
             .WithColumn(nameof(AddressEntity.Id)).AsInt32().NotNullable()
             .WithColumn(nameof(AddressEntity.PhoneNumber)).AsString().NotNullable()
             .WithColumn(nameof(AddressEntity.FirstName)).AsString().Nullable()
             .WithColumn(nameof(AddressEntity.LastName)).AsString().Nullable()
             .WithColumn(nameof(AddressEntity.ZipCode)).AsString().Nullable()
             .WithColumn(nameof(AddressEntity.City)).AsString().NotNullable()
             .WithColumn(nameof(AddressEntity.StreetAddress)).AsString().NotNullable();
    }
    public override void Down() 
    {
        Delete.Table(nameof(AddressEntity));
    }
}
