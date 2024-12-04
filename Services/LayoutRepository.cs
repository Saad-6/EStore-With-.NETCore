using LinqToDB;
using EStore.Models.Layout;
using EStore.Entities;
using EStore.DTOs;
using EStore.Data;
using EStore.Interfaces;
using static EStore.Models.Layout.HomePageLayout;
using EStore.Models;
using EStore.Code;
using EStore.Utility;


public class LayoutRepository : ILayoutRepository
{
    private readonly AltDataContext _context;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogRepository _logger;

    public LayoutRepository(AltDataContext context, IProductRepository productRepository, ICategoryRepository categoryRepository,ILogRepository logger)
    {
        _context = context;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var transaction = _context.BeginTransaction())
        {
            try
            {
                var layout = await _context.HomePageLayouts
                    .Where(l => l.Id == id)
                    .Select(l => new { l.Id, l.HomePageSettingsId })
                    .FirstOrDefaultAsync();

                if (layout == null)
                    return false;

                // Delete associated HomePageCarousels
                await _context.HomePageCarousels
                    .Where(h => h.HomePageSettingsId == layout.HomePageSettingsId)
                    .DeleteAsync();

                // Delete associated FeaturedProducts
                await _context.FeaturedProducts
                    .Where(fp => fp.HomePageSettingsId == layout.HomePageSettingsId)
                    .DeleteAsync();

                // Delete associated HomePageCategories
                await _context.HomePageCategories
                    .Where(sc => sc.HomePageSettingsId == layout.HomePageSettingsId)
                    .DeleteAsync();

                // Delete associated NewArrivals
                await _context.NewArrivals
                    .Where(na => na.HomePageSettingsId == layout.HomePageSettingsId)
                    .DeleteAsync();

                // Delete the HomePageSettings
                await _context.HomePageSettings
                    .Where(s => s.Id == layout.HomePageSettingsId)
                    .DeleteAsync();

                // Delete the HomePageLayout
                await _context.HomePageLayouts
                    .Where(l => l.Id == id)
                    .DeleteAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _logger.LogAsync(ex.Message);
                return false;
            }
        }
    }

    public async Task<HomePageLayout> GetActiveLayout()
    {
        
        var activeLayoutId = (await _context.HomePageLayouts.Where(m => m.IsActive == true).FirstOrDefaultAsync())?.Id ?? 0;

        // For the first time, when no layout exists ; we seed a default layout
        if(activeLayoutId == 0)
        {
            await _logger.LogAsync("Seeding Layout");
            await SeedLayout();
            await GetActiveLayout();
        }
        return await GetByIdAsync(activeLayoutId);
    }
    private async Task<HomePageSettings> MapSettingsQueryAsync(int homePageSettingsId)
    {
        var settings = await _context.HomePageSettings
            .Where(s => s.Id == homePageSettingsId)
            .FirstOrDefaultAsync();

        if (settings == null)
            return null;

        var heroCarousel = await _context.HomePageCarousels
            .Where(h => h.HomePageSettingsId == settings.Id)
            .Select(h => new HeroCarouselSlide
            {
                Id = h.Id,
                Image = h.ImageUrl,
                Title = h.Title,
                Subtitle = h.Subtitle,
                ButtonText = h.ButtonText
            })
            .ToListAsync();

        var featuredProducts = await _context.FeaturedProducts
            .Where(fp => fp.HomePageSettingsId == settings.Id)
            .Join(_context.Products, fp => fp.ProductId, p => p.Id, (fp, p) => new SimpleProductDTO
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

        var categories = await _context.HomePageCategories
            .Where(sc => sc.HomePageSettingsId == settings.Id)
            .Join(_context.Categories, sc => sc.CategoryId, c => c.Id, (sc, c) => new SimpleCategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ThumbNailUrl
            })
            .ToListAsync();

        var newArrivals = await _context.NewArrivals
            .Where(na => na.HomePageSettingsId == settings.Id)
            .Join(_context.Products, na => na.ProductId, p => p.Id, (na, p) => new SimpleProductDTO
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

        return new HomePageSettings
        {
            Id = settings.Id,
            HeroCarousel = heroCarousel,
            FeaturedProducts = featuredProducts,
            Categories = categories,
            NewArrivals = newArrivals
        };
    }



    public async Task<HomePageLayout> GetByIdAsync(int id)
    {
        var layout = await _context.HomePageLayouts
            .Where(l => l.Id == id)
            .Select(l => new HomePageLayout
            {
                Id = l.Id,
                Name = l.Name,
                IsActive = l.IsActive,
                Settings = new HomePageSettings
                {
                    Id = l.HomePageSettingsId
                }
            })
            .FirstOrDefaultAsync();
        if (layout != null)
        {
            layout.Settings = await MapSettingsQueryAsync(layout.Settings.Id.Value);
        
        }

        return layout;
    }

    public async Task<List<HomePageLayout>> GetLayoutsAsync()
    {
        var layouts = await _context.HomePageLayouts
            .Select(l => new HomePageLayout
            {
                Id = l.Id,
                Name = l.Name,
                IsActive = l.IsActive,
                Settings = new HomePageSettings{
                    Id = l.HomePageSettingsId 
                }
            })
            .ToListAsync();

        foreach (var layout in layouts)
        {
            if (layout.Settings.Id.HasValue)
            {
                layout.Settings = await MapSettingsQueryAsync(layout.Settings.Id.Value);
            }
        }

        if (layouts.Count == 0)
        {
            await SeedLayout();
            return await GetLayoutsAsync();
        }

        return layouts;
    }

    public async Task<bool> SaveAsync(HomePageLayout layout, Operation operation = Operation.Add)
    {
        using (var transaction = _context.BeginTransaction())
        {
            try
            {
                if (operation == Operation.Add)
                {
                    var settingsEntity = new HomePageSettingsEntity();
                    var settingsId =  _context.InsertWithInt32Identity(settingsEntity);

                    var layoutEntity = new HomePageLayoutEntity
                    {
                        Name = layout.Name,
                        IsActive = layout.IsActive,
                        HomePageSettingsId = settingsId
                    };
                    await _context.InsertAsync(layoutEntity);

                    foreach(var fp in layout?.Settings?.FeaturedProducts)
                    {
                        var feautedProduct = new FeauturedProductEntity
                        {
                            ProductId = fp.Id,
                            HomePageSettingsId = settingsId,
                        };
                       await  _context.InsertAsync(feautedProduct);
                    }
                    foreach (var na in layout?.Settings?.NewArrivals)
                    {
                        var newArrivals = new NewArrivalsEntity
                        {
                            ProductId = na.Id,
                            HomePageSettingsId = settingsId,
                        };
                        await _context.InsertAsync(newArrivals);
                    }
                    foreach (var cat in layout?.Settings?.Categories)
                    {
                        var categories = new SimpleCategoryEntity
                        {
                            CategoryId = cat.Id,
                            HomePageSettingsId = settingsId,
                        };
                        await _context.InsertAsync(categories);
                    }
                    foreach (var hc in layout?.Settings?.HeroCarousel)
                    {
                        var carousel = new HeroCarouselEntity
                        {
                            ButtonText = hc.ButtonText,
                            HomePageSettingsId = settingsId,
                            Subtitle = hc.Subtitle,
                            ImageUrl = hc.Image,
                            Title = hc.Title,
                        };
                        await _context.InsertAsync(carousel);
                    }

                }
                else
                {
                    await _context.HomePageLayouts
                        .Where(l => l.Id == layout.Id)
                        .Set(l => l.Name, layout.Name)
                        .Set(l => l.IsActive, layout.IsActive)
                        .UpdateAsync();

                    var settingsId = await _context.HomePageLayouts
                        .Where(l => l.Id == layout.Id)
                        .Select(l => l.HomePageSettingsId)
                        .FirstOrDefaultAsync();

                    if (settingsId.HasValue)
                    {
                        // Update HeroCarousel
                        await _context.HomePageCarousels
                            .Where(h => h.HomePageSettingsId == settingsId.Value)
                            .DeleteAsync();

                        foreach (var slide in layout.Settings.HeroCarousel)
                        {
                            await _context.InsertAsync(new HeroCarouselEntity
                            {
                                ImageUrl = slide.Image,
                                Title = slide.Title,
                                Subtitle = slide.Subtitle,
                                ButtonText = slide.ButtonText,
                                HomePageSettingsId = settingsId.Value
                            });
                        }

                        // Update FeaturedProducts
                        await _context.FeaturedProducts
                            .Where(fp => fp.HomePageSettingsId == settingsId.Value)
                            .DeleteAsync();

                        foreach (var product in layout.Settings.FeaturedProducts)
                        {
                            await _context.InsertAsync(new FeauturedProductEntity
                            {
                                ProductId = product.Id,
                                HomePageSettingsId = settingsId.Value
                            });
                        }

                        // Update Categories
                        await _context.HomePageCategories
                            .Where(sc => sc.HomePageSettingsId == settingsId.Value)
                            .DeleteAsync();

                        foreach (var category in layout.Settings.Categories)
                        {
                            await _context.InsertAsync(new SimpleCategoryEntity
                            {
                                CategoryId = category.Id,
                                HomePageSettingsId = settingsId.Value
                            });
                        }

                        // Update NewArrivals
                        await _context.NewArrivals
                            .Where(na => na.HomePageSettingsId == settingsId.Value)
                            .DeleteAsync();

                        foreach (var product in layout.Settings.NewArrivals)
                        {
                            await _context.InsertAsync(new NewArrivalsEntity
                            {
                                ProductId = product.Id,
                                HomePageSettingsId = settingsId.Value
                            });
                        }
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex) 
            {
                await transaction.RollbackAsync();
                await _logger.LogAsync(ex.Message);
                return false;
            }
        }
    }

    public async Task<bool> SeedLayout()
    {
        if (await _context.HomePageLayouts.AnyAsync())
        {
            return false; // Layout already seeded
        }

        var defaultLayout = new HomePageLayout
        {
            Name = "Default Layout",
            IsActive = true,
            Settings = new HomePageSettings
            {
                HeroCarousel = new List<HeroCarouselSlide>
                {
                    new HeroCarouselSlide { Image = "https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da", Title = "Welcome to EStore", Subtitle = "Discover amazing deals on top brands", ButtonText = "Shop Now" },
                    new HeroCarouselSlide { Image = "https://images.unsplash.com/photo-1607082349566-187342175e2f", Title = "Summer Sale", Subtitle = "Up to 50% off on selected items", ButtonText = "View Deals" }
                },
                FeaturedProducts = (await _productRepository.GetRandomProducts()).Select(product => new SimpleProductDTO
                {
                    Name = product.Name,
                    Id = product.Id,
                    ImageUrl = product.PrimaryImage.Url,
                    Slug = product.Slug,
                    Price = product.Price,
                }).ToList(),
                Categories = (await _categoryRepository.GetAllCategoriesAsync()).Select(category => new SimpleCategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImageUrl = category.ThumbNailUrl
                }).ToList(),
                NewArrivals = (await _productRepository.GetRandomProducts()).Select(product => new SimpleProductDTO
                {
                    Name = product.Name,
                    Id = product.Id,
                    ImageUrl = product.PrimaryImage.Url,
                    Slug = product.Slug,
                    Price = product.Price,
                }).ToList(),

            }
        };

        return await SaveAsync(defaultLayout, Operation.Add);
    }

    public async Task<Response> ActivateLayout(int layoutId)
    {
        using var transaction = await _context.BeginTransactionAsync();

        try
        {
            // Step 1: Set all layouts to inactive
            await _context.HomePageLayouts
                .Where(l => l.IsActive)
                .Set(l => l.IsActive, false)
                .UpdateAsync();

            // Step 2: Set the specified layout to active
            int affectedRows = await _context.HomePageLayouts
                .Where(l => l.Id == layoutId)
                .Set(l => l.IsActive, true)
                .UpdateAsync();

            // Step 3: Check if the layout ID was valid and the update was successful
            if (affectedRows == 0)
            {
                await transaction.RollbackAsync();
                return new Response { Success = false, Error = "Invalid layout ID or layout not found" };
            }

            // Step 4: Commit transaction
            await transaction.CommitAsync();
            return new Response { Success = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await _logger.LogAsync(ex.Message); // Log the error
            return new Response { Success = false, Error = ex.Message };
        }
    }

}