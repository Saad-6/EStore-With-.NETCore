using EStore.Data;
using EStore.DTOs;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Layout;
using Microsoft.EntityFrameworkCore;
using System;
using static EStore.Models.Layout.HomePageLayout;

namespace EStore.Code;

public class LayoutHelper : ILayoutRepository
{
    private readonly AppDbContext _context;
    private readonly ProductHelper _productHelper;
    private readonly CategoryHelper _categoryHelper;
    private readonly ILogger<LayoutHelper> _logger;

    public LayoutHelper(AppDbContext context, CategoryHelper categoryHelper, ProductHelper productHelper,ILogger<LayoutHelper> logger)
    {
        _context = context;
        _productHelper = productHelper;
        _categoryHelper = categoryHelper;
        _logger = logger;
    }

    private async Task<HomePageLayout> GetbyIdAsync(int id)
    {
        return await GetLayoutQuery()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> SaveAsync(HomePageLayout layout, Operation operation = Operation.Add)
    {
        if (layout == null)
        {
            return false;
        }

            HomePageLayout existingLayout;
        try
        {
            if (operation == Operation.Add)
            {

                _context.Add(layout);
            }
            else
            {
                 existingLayout = await GetLayoutQuery()
                  .FirstOrDefaultAsync(m => m.Id == layout.Id);

                if (existingLayout == null) return false;

                // Update simple properties
                _context.Entry(existingLayout).CurrentValues.SetValues(layout);

                // Update complex properties (collections) individually
                UpdateCollection(existingLayout.Settings.HeroCarousel, layout.Settings.HeroCarousel, item => item.Id);
                UpdateCollection(existingLayout.Settings.FeaturedProducts, layout.Settings.FeaturedProducts, item => item.Id);
                UpdateCollection(existingLayout.Settings.Categories, layout.Settings.Categories, item => item.Id);

                // Repeat for other collections as needed

             _context.Update(existingLayout);
            }
            await _context.SaveChangesAsync();

            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is SimpleProductDTO)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    // If databaseValues is null, this might be a new entry that doesn’t exist in the database yet
                    if (databaseValues == null)
                    {
                        // Option 1: If entity does not exist in the database, decide to add it as a new entry
                        // OR handle it according to your business logic
                        Console.WriteLine("Entity does not exist in the database. Treating it as a new entry.");

                        databaseValues = proposedValues.Clone();
                    }

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        // Example condition: If there's a discrepancy, take the database value
                        if (!Equals(proposedValue, databaseValue))
                        {
                            // Keep the database value if there's a conflict
                            proposedValues[property] = databaseValue;
                        }
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
                else
                {
                    throw new NotSupportedException(
                        "Don't know how to handle concurrency conflicts for " + entry.Metadata.Name);
                }
            }
            return false;
        }



    }

    private void UpdateCollection<T>(ICollection<T> existingCollection, ICollection<T> newCollection, Func<T, object> keySelector) where T : class
    {
        if (existingCollection == null)
        {
            existingCollection = new List<T>();
        }
        // Remove items not in the new collection
        var toRemove = existingCollection.Where(e => !newCollection.Any(n => keySelector(n).Equals(keySelector(e)))).ToList();
        foreach (var item in toRemove)
        {
            existingCollection.Remove(item);
        }

        // Update or add new items
        foreach (var newItem in newCollection)
        {
            var existingItem = existingCollection.SingleOrDefault(e => keySelector(e).Equals(keySelector(newItem)));
            if (existingItem != null)
            {
                // Update existing item values
                _context.Entry(existingItem).CurrentValues.SetValues(newItem);
            }
            else
            {
                // Add new item
                existingCollection.Add(newItem);
            }
        }
    }

    public async Task<bool> SeedLayout()
    {
        var layout = new HomePageLayout
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
                FeaturedProducts = (await _productHelper.GetRandomProducts()).Select(product => new SimpleProductDTO
                {
                    Name = product.Name,
                    Id = product.Id,
                    ImageUrl = product.PrimaryImage.Url,
                    Slug = product.Slug,
                    Price = product.Price,
                }).ToList(),
                Categories = (await _categoryHelper.GetAllCategoriesAsync()).Select(category => new SimpleCategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    ImageUrl = category.ThumbNailUrl
                }).ToList(),
                NewArrivals = (await _productHelper.GetRandomProducts()).Select(product => new SimpleProductDTO
                {
                    Name = product.Name,
                    Id = product.Id,
                    ImageUrl = product.PrimaryImage.Url,
                    Slug = product.Slug,
                    Price = product.Price,
                }).ToList(),

            }
        };

        try
        {
            _context.HomePageLayouts.Add(layout);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding layout: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var layout = await GetbyIdAsync(id);
        if (layout == null) return false;

        var layoutCount = await _context.HomePageLayouts.CountAsync();
        if (layoutCount == 1) return false;

        try
        {
            _context.Remove(layout);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting layout: {ex.Message}");
            return false;
        }
    }

    private IQueryable<HomePageLayout> GetLayoutQuery()
    {
        return _context.HomePageLayouts
            .Include(l => l.Settings)
                .ThenInclude(s => s.HeroCarousel)
            .Include(l => l.Settings)
                .ThenInclude(s => s.FeaturedProducts)
            .Include(l => l.Settings)
                .ThenInclude(s => s.Categories)
            .Include(l => l.Settings)
                .ThenInclude(s => s.NewArrivals);
    }

    public async Task<List<HomePageLayout>> GetLayoutsAsync()
    {
        return await GetLayoutQuery().ToListAsync();
    }

    public async Task<HomePageLayout> GetByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public Task<HomePageLayout> GetActiveLayout()
    {
        throw new NotImplementedException();
    }

    public Task<Response> ActivateLayout(int layoutId)
    {
        throw new NotImplementedException();
    }
}