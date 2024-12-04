using LinqToDB.Mapping;

namespace EStore.Entities;

[Table(Name = "HeroCarouselEntity")]
public class HeroCarouselEntity
{
    [PrimaryKey, Identity]
    [Column(Name = "Id")]
    public int? Id { get; set; }

    [Column(Name = "ImageUrl"), Nullable]
    public string? ImageUrl { get; set; }

    [Column(Name = "Title"), Nullable]
    public string? Title { get; set; }

    [Column(Name = "Subtitle"), Nullable]
    public string? Subtitle { get; set; }

    [Column(Name = "ButtonText"), Nullable]
    public string? ButtonText { get; set; }

    [Column(Name = "HomePageSettingsId"), NotNull]
    public int HomePageSettingsId { get; set; }
}