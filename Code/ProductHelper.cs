using EcommerceAPI.Models;
using EStore.Models.Products;
using EStore.Models;
using EStore.Data;
using Microsoft.EntityFrameworkCore;
using EStore.Models.Layout;
using EStore.DTOs;
using EStore.Interfaces;

namespace EStore.Code;

public class ProductHelper : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductHelper(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Response> SaveAsync(Product product)
    {
        if(product == null) return new Response { Success = false, Error = "Product is null" };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return new Response { Success = true };
    }

    public async Task<Response> UpdateAsync(Product product)
    {
        if (product == null || product.Id == 0) return new Response { Success = false, Error = "Product is null" };

        // Step 1: Load the existing product from the database
        Product existingProduct = await GetProductById(product.Id);
        if (existingProduct == null) return new Response { Success = true, Error = "Product is null" };

        // Step 2: Compare and apply changes
        // This will update existingProduct with the changes from product
        existingProduct = await CompareAndApplyChanges(existingProduct, product);

        // Step 3: Save changes
        await _context.SaveChangesAsync();

        return new Response { Success = true };
    }
    public async Task<List<Product>> Search(string query)
    {
        var products = new HashSet<Product>();
        products.UnionWith(await GetProductsByName(query));
        products.UnionWith(await GetProductsByDescription(query));
        products.UnionWith(await GetProductsByCategoryName(query));
        return products.ToList();
    }
    public async Task<Product> CompareAndApplyChanges(Product existingProduct, Product changedProduct)
    {
        // Update basic properties if they have changed
        if (existingProduct.Name != changedProduct.Name) existingProduct.Name = changedProduct.Name;
        if (existingProduct.Description != changedProduct.Description) existingProduct.Description = changedProduct.Description;
        if (existingProduct.Price != changedProduct.Price) existingProduct.Price = changedProduct.Price;
        if (existingProduct.SKU != changedProduct.SKU) existingProduct.SKU = changedProduct.SKU;
        if (existingProduct.Stock != changedProduct.Stock) existingProduct.Stock = changedProduct.Stock;
        if (existingProduct.IsActive != changedProduct.IsActive) existingProduct.IsActive = changedProduct.IsActive;

        // Update Category (if this should be a reference change)
        if (existingProduct.Category?.Id != changedProduct.Category?.Id)
            existingProduct.Category = changedProduct.Category;

        // Update SEO if changed
        if (existingProduct.SEO == null && changedProduct.SEO != null)
            existingProduct.SEO = changedProduct.SEO;
        else if (existingProduct.SEO != null && changedProduct.SEO != null)
        {
            existingProduct.SEO.MetaTitle = changedProduct.SEO.MetaTitle;
            existingProduct.SEO.MetaDescription = changedProduct.SEO.MetaDescription;
            existingProduct.SEO.MetaKeywords = changedProduct.SEO.MetaKeywords;
            existingProduct.SEO.CanonicalUrl = changedProduct.SEO.CanonicalUrl;
        }

        // Handle Variants
        var variantsToRemove = existingProduct.Variants
            .Where(ev => !changedProduct.Variants.Any(pv => pv.Id == ev.Id)).ToList();
        foreach (var variant in variantsToRemove)
            _context.Entry(variant).State = EntityState.Deleted;

        foreach (var changedVariant in changedProduct.Variants)
        {
            var existingVariant = existingProduct.Variants.FirstOrDefault(ev => ev.Id == changedVariant.Id);
            if (existingVariant == null)
            {
                existingProduct.Variants.Add(changedVariant);
            }
            else
            {
                // Update Variant properties
                existingVariant.Name = changedVariant.Name;
                existingVariant.DisplayType = changedVariant.DisplayType;

                // Handle Variant Options
                var optionsToRemove = existingVariant.Options
                    .Where(ov => !changedVariant.Options.Any(cv => cv.Id == ov.Id)).ToList();
                foreach (var option in optionsToRemove)
                    _context.Entry(option).State = EntityState.Deleted;

                foreach (var changedOption in changedVariant.Options)
                {
                    var existingOption = existingVariant.Options.FirstOrDefault(ov => ov.Id == changedOption.Id);
                    if (existingOption == null)
                    {
                        existingVariant.Options.Add(changedOption);
                    }
                    else
                    {
                        existingOption.Value = changedOption.Value;
                        existingOption.PriceAdjustment = changedOption.PriceAdjustment;
                        existingOption.Stock = changedOption.Stock;
                        existingOption.SKU = changedOption.SKU;

                        // Update Option Images similarly if needed
                    }
                }
            }
        }

        // Handle Images
        var imagesToRemove = existingProduct.Images
            .Where(ev => !changedProduct.Images.Any(pv => pv.Id == ev.Id)).ToList();
        foreach (var image in imagesToRemove)
            _context.Entry(image).State = EntityState.Deleted;

        foreach (var changedImage in changedProduct.Images)
        {
            var existingImage = existingProduct.Images.FirstOrDefault(ei => ei.Id == changedImage.Id);
            if (existingImage == null)
            {
                existingProduct.Images.Add(changedImage);
            }
            else
            {
                existingImage.Url = changedImage.Url;
                existingImage.AltText = changedImage.AltText;
            }
        }

        // Additional fields (Discounts, Reviews, FAQs, etc.) can be updated similarly...

        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<Response> DeleteAsync(int productId)
    {
        var product = await GetProductById(productId);
        if(product == null) return new Response { Success = false, Error = "No Product was found" };
        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return new Response { Success = true };
        }
        catch (Exception ex) 
        {
            return new Response { Success = false, Error = ex.Message };
        }
    }
  
    public async Task<Product> GetProductById(int id)
    {
        if (id == 0) return null;
        var product = await GetProductQuery()
            .Where(m=>m.Id == id)
            .FirstOrDefaultAsync();
        if(product != null) return product;
        return null;
    }

    public async Task<Product> GetProductBySku(string sku)
    {
        if (string.IsNullOrEmpty(sku)) return null;
        var product = await GetProductQuery()
            .Where(m => m.SKU == sku)
            .FirstOrDefaultAsync();
        if (product != null) return product;
        return null;
    }

    public async Task<Product> GetProductBySlug(string slug)
    {
        if (string.IsNullOrEmpty(slug)) return null;
        var product = await GetProductQuery()
            .Where(m => m.Slug == slug)
            .FirstOrDefaultAsync();
        if (product != null) return product;
        return null;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await GetProductQuery()
              .ToListAsync();
    }
    public async Task<Product> GetSimpleProductById(int id)
    {
        return (await _context.Products.FirstOrDefaultAsync(m => m.Id == id));
             
    }
    public async Task<List<SimpleProductDTO>> GetProductDTOs()
    {
        return await _context.Products.Select(product => new SimpleProductDTO
        {
            Id = product.Id,
            ImageUrl = product.PrimaryImage.Url,
            Name = product.Name,
            Slug = product.Slug,
            Price = product.Price,

        })
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var products = await GetProductQuery()
        .Where(m => m.Name.ToLower().Contains(name.ToLower()))
        .ToListAsync();

        return products;
    }

    public async Task<List<Product>> GetProductsByCategoryName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var products = await GetProductQuery()
            .Where(m => m.Category.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();

        return products;
    }

    public async Task<List<Product>> GetProductsByCategoryId(int id)
    {
        if (id == 0) return null;
        var products = await GetProductQuery()
            .Where(m =>m.Category.Id == id)
            .ToListAsync();

        return products;
    }

    public async Task<List<Product>> GetProductsByDescription(string descriptionText)
    {
        if (string.IsNullOrEmpty(descriptionText)) return null;
        var products = await GetProductQuery()
            .Where(m => m.Description.Contains(descriptionText, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();

        return products;
    }

    public async Task<List<Product>> GetRandomProducts(int count = 3)
    {
        // Fetch all available products
        var totalProducts = await GetProductQuery()
            .Include(m=>m.Variants)
            .ToListAsync();

        // If there are no products, return an empty list
        if (totalProducts == null || totalProducts.Count == 0)
        {
            return new List<Product>();
        }
        var randomProducts = totalProducts
            .OrderBy(x => Guid.NewGuid()) 
            .Take(Math.Min(count, totalProducts.Count)) 
            .ToList();

        return randomProducts;
    }

    private IQueryable<Product> GetProductQuery()
    {
        return _context.Products
              .Include(m => m.Category)
        .Include(m => m.SEO)
        .Include(m => m.PrimaryImage)
        .Include(m => m.Images)
        .Include(m => m.Variants)
        .ThenInclude(v => v.Options)
        .ThenInclude(o => o.OptionImages)
        ;

    }

    public Task<Response> SaveAsync(ProductAPI product)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UpdateAsync(ProductAPI product)
    {
        throw new NotImplementedException();
    }
}
