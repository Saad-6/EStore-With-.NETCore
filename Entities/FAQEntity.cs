using EStore.Models;

namespace EStore.Entities;

using LinqToDB.Mapping;

[Table(Name = "FAQEntity")]
public class FAQEntity : BaseEntity
{
    [PrimaryKey, Identity] 
    public new int Id { get; set; }

    [Column(Name = "Question"), NotNull] 
    public string Question { get; set; }

    [Column(Name = "Answer"), NotNull]
    public string Answer { get; set; }

    [Column(Name = "ProductId"), NotNull] 
    public int ProductId { get; set; }
}
