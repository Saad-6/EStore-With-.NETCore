using EStore.Models.Order;
using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "AddressEntity")] 
public class AddressEntity : Address
{
    [PrimaryKey, Identity] 
    public new int Id { get; set; }

    [Column(Name = "PhoneNumber"), NotNull]
    public override string? PhoneNumber { get; set; }

    [Column(Name = "FirstName"), Nullable] 
    public override string? FirstName { get; set; }

    [Column(Name = "LastName"), Nullable] 
    public override string? LastName { get; set; }

    [Column(Name = "ZipCode"), Nullable] 
    public override string? ZipCode { get; set; }

    [Column(Name = "City"), NotNull] 
    public override string? City { get; set; }

    [Column(Name = "StreetAddress"), NotNull] 
    public override string? StreetAddress { get; set; }
}
