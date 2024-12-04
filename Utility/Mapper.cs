using EStore.Data;
using EStore.DTOs;
using EStore.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EStore.Models.Layout.HomePageLayout;

namespace EStore.Utility;

public class Mapper
{
    private readonly AltDataContext _context;

    public Mapper(AltDataContext context)
    {
        _context = context;
    }

    public async Task<HomePageSettings> MapSettingsAsync(int layoutId)
    {
        var settingsId = await _context.HomePageLayouts
            .Where(l => l.Id == layoutId)
            .Select(l => l.HomePageSettingsId)
            .FirstOrDefaultAsync();

        if (settingsId == 0) return null;

        return new HomePageSettings
        {
            Id = settingsId,
            HeroCarousel = await MapHeroCarouselAsync(settingsId.Value),
            FeaturedProducts = await MapFeaturedProductsAsync(settingsId.Value),
            Categories = await MapCategoriesAsync(settingsId.Value),
            NewArrivals = await MapNewArrivalsAsync(settingsId.Value)
        };
    }
    private async Task<List<HeroCarouselSlide>> MapHeroCarouselAsync(int settingsId)
    {
        return await _context.HomePageCarousels
            .Where(h => h.HomePageSettingsId == settingsId)
            .Select(h => new HeroCarouselSlide
            {
                Id = h.Id,
                Image = h.ImageUrl,
                Title = h.Title,
                Subtitle = h.Subtitle,
                ButtonText = h.ButtonText
            })
            .ToListAsync();
    }

    private async Task<List<SimpleProductDTO>> MapFeaturedProductsAsync(int settingsId)
    {
        return await _context.FeaturedProducts
            .Where(fp => fp.HomePageSettingsId == settingsId)
            .Join(_context.Products,
                fp => fp.ProductId,
                p => p.Id,
                (fp, p) => new SimpleProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = _context.ProductImages
                        .Where(pi => pi.Id == p.PrimaryImageId)
                        .Select(pi => pi.Url)
                        .FirstOrDefault(),
                    Slug = p.Slug,
                    Price = p.Price
                })
            .ToListAsync();
    }

    private async Task<List<SimpleCategoryDTO>> MapCategoriesAsync(int settingsId)
    {
        return await _context.HomePageCategories
            .Where(sc => sc.HomePageSettingsId == settingsId)
            .Join(_context.Categories,
                sc => sc.CategoryId,
                c => c.Id,
                (sc, c) => new SimpleCategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ThumbNailUrl
                })
            .ToListAsync();
    }

    private async Task<List<SimpleProductDTO>> MapNewArrivalsAsync(int settingsId)
    {
        return await _context.NewArrivals
            .Where(na => na.HomePageSettingsId == settingsId)
            .Join(_context.Products,
                na => na.ProductId,
                p => p.Id,
                (na, p) => new SimpleProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = _context.ProductImages
                        .Where(pi => pi.Id == p.PrimaryImageId)
                        .Select(pi => pi.Url)
                        .FirstOrDefault(),
                    Slug = p.Slug,
                    Price = p.Price
                })
            .ToListAsync();
    }

}
