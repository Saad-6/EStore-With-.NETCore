using EStore.Models.Layout;
using static EStore.Models.Layout.HomePageLayout;

namespace EStore.DTOs;

public class HomePageLayoutDTO : HomePageLayout
{
    public new HomePageSettingsDTO Settings { get; set; } = new HomePageSettingsDTO();
}
public class HomePageSettingsDTO : HomePageSettings
{
    public new List<SimpleProductDTO> FeaturedProducts { get; set; } = new List<SimpleProductDTO>();
    public new List<SimpleCategoryDTO> Categories { get; set; } = new List<SimpleCategoryDTO>();
} 
