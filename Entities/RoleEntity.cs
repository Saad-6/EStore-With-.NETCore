using EStore.Models;
using LinqToDB.Mapping;

namespace EStore.Entities;


[Table(Name = "RoleEntity")]
public class RoleEntity : BaseEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "Role"), NotNull] 
    public string Role { get; set; }

    public void SetAdmin() => Role = "Admin";

    public RoleEntity()
    {
        Role = "Customer"; 
    }
}
