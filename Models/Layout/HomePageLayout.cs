using EStore.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EStore.Models.Layout;

public class HomePageLayout
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public virtual HomePageSettings? Settings { get; set; }


public class HomePageSettings
{
    [Key]
    public int? Id { get; set; }
    public List<HeroCarouselSlide>? HeroCarousel { get; set; } = new List<HeroCarouselSlide>();
    public virtual List<SimpleProductDTO>? FeaturedProducts { get; set; } = new List<SimpleProductDTO>();
    public virtual List<SimpleProductDTO>? NewArrivals { get; set; } = new List<SimpleProductDTO>();
    public virtual List<SimpleCategoryDTO>? Categories { get; set; } = new List<SimpleCategoryDTO>();

}

public class HeroCarouselSlide
{
    [Key]
    public int? Id { get; set; }
    public string? Image { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? ButtonText { get; set; }
}

}