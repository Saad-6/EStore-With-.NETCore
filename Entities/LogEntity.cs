using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "LogEntity")]
public class LogEntity
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column(Name = "Message",Length = 4000), NotNull]
    public string Message { get; set; }

    [Column(Name = "DateTime"), NotNull]
    public DateTime DateTime { get; set; }
}
