using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "ProductImageEntity")] 
public class ProductImageEntity : ProductImage
{
    [PrimaryKey, Identity] 
    [Column(Name = "Id")]
    public new int Id { get; set; }

    [Column(Name = "Url",Length = 2000), NotNull]
    public override string Url { get; set; }

    [Column(Name = "AltText"), Nullable] 
    public override string AltText { get; set; }

    [Column(Name = "ProductId"), Nullable] 
    public int? ProductId { get; set; }

    [Column(Name = "VariantOptionsEntityId"), Nullable] 
    public int? VariantOptionsEntityId { get; set; }
}
