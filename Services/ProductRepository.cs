using EStore.Data;
using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Products;
using LinqToDB;
using LinqToDB.Data;



namespace EStore.Services;

public class ProductRepository : IProductRepository
{
    private readonly AltDataContext _dbContext;
    private readonly ILogRepository _logger;
    public ProductRepository(AltDataContext dataContext, ILogRepository logger)
    {
        _dbContext = dataContext;
        _logger = logger;
    }

    public async Task<Response> DeleteAsync(int productId)
    {
        var transaction = _dbContext.BeginTransaction();
        try
        {
            // Step 1: Delete Variants and their Options
            var variants = await _dbContext.Variants
                .Where(v => v.ProductId == productId)
                .ToListAsync();

            foreach (var variant in variants)
            {
                // Delete Variant Options
                await _dbContext.VariantOptions
                    .Where(vo => vo.VariantEntityId == variant.Id)
                    .DeleteAsync();

                // Delete Variant
                await _dbContext.Variants
                    .Where(v => v.Id == variant.Id)
                    .DeleteAsync();
            }

            // Step 2: Delete Additional Images
            await _dbContext.ProductImages
                .Where(pi => pi.ProductId == productId)
                .DeleteAsync();

            // Step 3: Delete SEO
            var product = await _dbContext.Products
                .Where(p => p.Id == productId)
                .FirstOrDefaultAsync();

            if (product?.SEOId != null)
            {
                await _dbContext.SEOs
                    .Where(seo => seo.Id == product.SEOId)
                    .DeleteAsync();
            }

            // Step 4: Delete Discount
            if (product?.DiscountId != null)
            {
                await _dbContext.Discounts
                    .Where(discount => discount.Id == product.DiscountId)
                    .DeleteAsync();
            }

            // Step 5: Delete Primary Image
            if (product?.PrimaryImageId != null)
            {
                await _dbContext.ProductImages
                    .Where(image => image.Id == product.PrimaryImageId)
                    .DeleteAsync();
            }

            // Step 6: Delete Product
            await _dbContext.Products
                .Where(p => p.Id == productId)
                .DeleteAsync();

            // Commit transaction
            await transaction.CommitAsync();
            return new Response { Success = true };
        }
        catch (Exception ex)
        {
            // Rollback transaction
            await transaction.RollbackAsync();
            await _logger.LogAsync(ex.Message);
            return new Response { Success = false, Error = ex.Message };
        }
        finally
        {
            transaction.Dispose();
        }
    }

    private IQueryable<Product> GetProductQuery()
    {
        return from product in _dbContext.Products
               join category in _dbContext.Categories
               on product.CategoryId equals category.Id
               join seo in _dbContext.SEOs
               on product.SEOId equals seo.Id into seoJoin
               from seo in seoJoin.DefaultIfEmpty()  // Left join to include products without SEO
               join image in _dbContext.ProductImages
               on product.PrimaryImageId equals image.Id into imageJoin
               from image in imageJoin.DefaultIfEmpty()  // Left join to include products without image
               join discount in _dbContext.Discounts
               on product.DiscountId equals discount.Id into discountJoin
               from discount in discountJoin.DefaultIfEmpty()  // Left join to include products without discount
               select new Product
               {
                   Id = product.Id,
                   Name = product.Name,
                   Description = product.Description,
                   Price = product.Price,
                   SKU = product.SKU,
                   Stock = product.Stock,
                   Brand = product.Brand,
                   Slug = product.Slug,
                   IsActive = product.IsActive,
                   Category = new Category
                   {
                       Id = category.Id,
                       Name = category.Name,
                       Description = category.Description,
                       ThumbNailUrl = category.ThumbNailUrl
                   },
                   SEO = seo == null ? null : new SEO
                   {
                       Id = seo.Id,
                       MetaTitle = seo.MetaTitle,
                       MetaDescription = seo.MetaDescription,
                       MetaKeywords = seo.MetaKeywords,
                       CanonicalUrl = seo.CanonicalUrl
                   },
                   Images = _dbContext.ProductImages
                   .Where(pi => pi.ProductId == product.Id ) 
                   .Select(pi => new ProductImage
                   {
                       Id = pi.Id,
                       Url = pi.Url,
                       AltText = pi.AltText
                   })
                   .ToList(),
                   PrimaryImage = image == null ? null : new ProductImage
                   {
                       Id = image.Id,
                       Url = image.Url,
                       AltText = image.AltText
                   },
                   
                   Discount = discount == null ? null : new Discount
                   {
                       Id = discount.Id,
                       DiscountPrice = discount.DiscountPrice,
                       DiscountStartDate = discount.DiscountStartDate,
                       DiscountEndDate = discount.DiscountEndDate
                   },
                   Variants = _dbContext.Variants
               .Where(v => v.ProductId == product.Id)
               .Select(v => new Variant
               {
                   Id = v.Id,
                   Name = v.Name,
                   DisplayType = v.DisplayType,
                   Options = _dbContext.VariantOptions
                       .Where(vo => vo.VariantEntityId == v.Id)
                       .Select(vo => new VariantOption
                       {
                           Id = vo.Id,
                           Value = vo.Value,
                           PriceAdjustment = vo.PriceAdjustment,
                           Stock = vo.Stock,
                           SKU = vo.SKU,
                           OptionImages = _dbContext.ProductImages
                               .Where(pi => pi.VariantOptionsEntityId == vo.Id)
                               .Select(oi => new ProductImage
                               {
                                   AltText = oi.AltText,
                                   Id = oi.Id,
                                   Url = oi.Url,

                               })
                               .ToList()
                       })
                       .ToList()
               })
               .ToList()
               };
    }
    public async Task<List<Product>> GetAllAsync()
    {
       
        return await GetProductQuery().ToListAsync();
    }

    public Task<Product> GetProductById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> GetProductBySku(string sku)
    {
        return await GetProductQuery().FirstOrDefaultAsync(m=>m.SKU == sku);
    }

    public async Task<Product> GetProductBySlug(string slug)
    {
        return await GetProductQuery().FirstOrDefaultAsync(m => m.Slug == slug);
    }

    public Task<List<Product>> GetProductsByCategoryId(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsByCategoryName(string name)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsByDescription(string descriptionText)
    {
        throw new NotImplementedException();
    }

    public Task<List<Product>> GetProductsByName(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Product>> GetRandomProducts(int count = 3)
    {
        return await GetProductQuery()
     .ToListAsync();
    }

    public async Task<Response> SaveAsync(Product product)
    {
        ProductEntity productEntity;
        var transaction = _dbContext.BeginTransaction();
        try
        {
            // Step 1: Add SEO
            int? seoId = null;
            if (product.SEO != null)
            {
                seoId = await _dbContext.InsertWithInt32IdentityAsync(new SEOEntity
                {
                    MetaTitle = product.SEO.MetaTitle,
                    MetaDescription = product.SEO.MetaDescription,
                    MetaKeywords = product.SEO.MetaKeywords,
                    CanonicalUrl = product.SEO.CanonicalUrl,
                });
            }

            // Step 2: Add Primary Image
            int? primaryImageId = null;
            if (product.PrimaryImage != null)
            {
                primaryImageId = await _dbContext.InsertWithInt32IdentityAsync(new ProductImageEntity
                {
                    Url = product.PrimaryImage.Url,
                    AltText = product.PrimaryImage.AltText
                });
            }

            // Step 3: Add Discount
            int? discountId = null;
            if (product.Discount != null)
            {
                discountId = await _dbContext.InsertWithInt32IdentityAsync(new DiscountEntity
                {
                    isActive = product.Discount.isActive,
                    DiscountPrice = product.Discount.DiscountPrice,
                    DiscountStartDate = product.Discount.DiscountStartDate,
                    DiscountEndDate = product.Discount.DiscountEndDate
                });
            }

            // Step 4: Get Category
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == product.Category.Id);

            if (category == null)
            {
                return new Response { Success = false, Error = "Invalid category ID (Category doesn't exist or was deleted)" };
            }


            // Step 5: Add Product
            productEntity = new ProductEntity
            {
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                CategoryId = category.Id,
                Brand = product.Brand,
                IsActive = true,
                SKU = product.SKU,
                Stock = product.Stock,
                Slug = product.Slug,
                SEOId = seoId,
                PrimaryImageId = primaryImageId,
                DiscountId = discountId
            };

            int productId = await _dbContext.InsertWithInt32IdentityAsync(productEntity);

            // Step 6: Add Images
            if (product.Images != null && product.Images.Any())
            {
                var imageEntities = product.Images.Select(image => new ProductImageEntity
                {
                    Url = image.Url,
                    AltText = image.AltText,
                    ProductId = productId
                }).ToList();

                // Perform bulk copy
                await _dbContext.BulkCopyAsync(imageEntities);
            }

            // Step 7: Add Variants
            if (product.Variants != null && product.Variants.Any())
            {
         
                foreach (var variant in product.Variants)
                {
                    var variantId = await _dbContext.InsertWithInt32IdentityAsync(new VariantEntity
                    {
                        Name = variant.Name,
                        DisplayType = variant.DisplayType,
                        ProductId = productId
                    });

                    // Add Variant Options
                    if (variant.Options != null && variant.Options.Any())
                    {
                        foreach (var option in variant.Options)
                        {
                           var variantOptionId = await _dbContext.InsertWithInt32IdentityAsync(new VariantOptionEntity
                            {
                                Value = option.Value,
                                PriceAdjustment = option.PriceAdjustment,
                                SKU = option.SKU,
                                Stock = option.Stock,
                                VariantEntityId = variantId
                            });
                            if (option.OptionImages != null && option.OptionImages.Any())
                            {
                                foreach (var optionImage in option.OptionImages)
                                {
                                    await _dbContext.InsertAsync(new ProductImageEntity
                                    {
                                        Url = optionImage.Url,
                                        AltText = optionImage.AltText,
                                        VariantOptionsEntityId = variantOptionId
                                    });
                                }
                            }




                        }
                    }
                }
            }

            // Commit transaction
            await transaction.CommitAsync();
            return new Response { Success = true};
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            await _logger.LogAsync(ex.Message);
            return new Response { Success = true, Error = ex.Message };
        }
        finally
        {
            transaction.Dispose();
        }

    }

    public Task<List<Product>> Search(string query)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> UpdateAsync(Product product)
    {
        using var transaction = await _dbContext.BeginTransactionAsync();
        try
        {
            // Step 1: Update or Add SEO
            if (product.SEO != null)
            {
                if (product.SEO.Id > 0)
                {
                    await _dbContext.SEOs
                        .Where(s => s.Id == product.SEO.Id)
                        .Set(s => s.MetaTitle, product.SEO.MetaTitle)
                        .Set(s => s.MetaDescription, product.SEO.MetaDescription)
                        .Set(s => s.MetaKeywords, product.SEO.MetaKeywords)
                        .Set(s => s.CanonicalUrl, product.SEO.CanonicalUrl)
                        .UpdateAsync();
                }
                else
                {
                    product.SEO.Id = await _dbContext.InsertWithInt32IdentityAsync(new SEOEntity
                    {
                        MetaTitle = product.SEO.MetaTitle,
                        MetaDescription = product.SEO.MetaDescription,
                        MetaKeywords = product.SEO.MetaKeywords,
                        CanonicalUrl = product.SEO.CanonicalUrl
                    });
                }
            }

            // Step 2: Update or Add Primary Image
            if (product.PrimaryImage != null)
            {
                if (product.PrimaryImage.Id > 0)
                {
                    await _dbContext.ProductImages
                        .Where(pi => pi.Id == product.PrimaryImage.Id)
                        .Set(pi => pi.Url, product.PrimaryImage.Url)
                        .Set(pi => pi.AltText, product.PrimaryImage.AltText)
                        .UpdateAsync();
                }
                else
                {
                    product.PrimaryImage.Id = await _dbContext.InsertWithInt32IdentityAsync(new ProductImageEntity
                    {
                        Url = product.PrimaryImage.Url,
                        AltText = product.PrimaryImage.AltText
                    });
                }
            }

            // Step 3: Update Product
            await _dbContext.Products
                .Where(p => p.Id == product.Id)
                .Set(p => p.Name, product.Name)
                .Set(p => p.Description, product.Description)
                .Set(p => p.Price, product.Price)
                .Set(p => p.SKU, product.SKU)
                .Set(p => p.Stock, product.Stock)
                .Set(p => p.Brand, product.Brand)
                .Set(p => p.CategoryId, product.Category.Id)
                .Set(p => p.Slug, product.Slug)
                .Set(p => p.IsActive, product.IsActive)
                .Set(p => p.SEOId, product.SEO?.Id)
                .Set(p => p.PrimaryImageId, product.PrimaryImage?.Id)
                .UpdateAsync();

            // Step 4: Update Variants
            var existingVariants = await _dbContext.Variants
                .Where(v => v.ProductId == product.Id)
                .ToListAsync();

            foreach (var variant in product.Variants)
            {
                if (variant.Id > 0)
                {
                    await _dbContext.Variants
                        .Where(v => v.Id == variant.Id)
                        .Set(v => v.Name, variant.Name)
                        .Set(v => v.DisplayType, variant.DisplayType)
                        .UpdateAsync();

                    // Update Variant Options
                    var existingOptions = await _dbContext.VariantOptions
                        .Where(vo => vo.VariantEntityId == variant.Id)
                        .ToListAsync();

                    foreach (var option in variant.Options)
                    {
                        if (option.Id > 0)
                        {
                            await _dbContext.VariantOptions
                                .Where(vo => vo.Id == option.Id)
                                .Set(vo => vo.Value, option.Value)
                                .Set(vo => vo.PriceAdjustment, option.PriceAdjustment)
                                .Set(vo => vo.SKU, option.SKU)
                                .Set(vo => vo.Stock, option.Stock)
                                .UpdateAsync();

                            // Update Option Images
                            var existingOptionImages = await _dbContext.ProductImages
                                .Where(pi => pi.VariantOptionsEntityId == option.Id)
                                .ToListAsync();

                            foreach (var optionImage in option.OptionImages)
                            {
                                if (optionImage.Id > 0)
                                {
                                    await _dbContext.ProductImages
                                        .Where(pi => pi.Id == optionImage.Id)
                                        .Set(pi => pi.Url, optionImage.Url)
                                        .Set(pi => pi.AltText, optionImage.AltText)
                                        .UpdateAsync();
                                }
                                else
                                {
                                    await _dbContext.InsertAsync(new ProductImageEntity
                                    {
                                        Url = optionImage.Url,
                                        AltText = optionImage.AltText,
                                        VariantOptionsEntityId = option.Id
                                    });
                                }
                            }

                            // Remove Option Images no longer associated
                            var optionImagesToRemove = existingOptionImages
                                .Where(eoi => !option.OptionImages.Any(oi => oi.Id == eoi.Id))
                                .ToList();
                            await _dbContext.ProductImages.DeleteAsync(pi => optionImagesToRemove.Select(i => i.Id).Contains(pi.Id));
                        }
                        else
                        {
                            var newOptionId = await _dbContext.InsertWithInt32IdentityAsync(new VariantOptionEntity
                            {
                                Value = option.Value,
                                PriceAdjustment = option.PriceAdjustment,
                                SKU = option.SKU,
                                Stock = option.Stock,
                                VariantEntityId = variant.Id
                            });

                            foreach (var optionImage in option.OptionImages)
                            {
                                await _dbContext.InsertAsync(new ProductImageEntity
                                {
                                    Url = optionImage.Url,
                                    AltText = optionImage.AltText,
                                    VariantOptionsEntityId = newOptionId
                                });
                            }
                        }
                    }

                    // Remove options that are no longer associated
                    var optionsToRemove = existingOptions
                        .Where(eo => !variant.Options.Any(vo => vo.Id == eo.Id))
                        .ToList();
                    await _dbContext.VariantOptions.DeleteAsync(vo => optionsToRemove.Select(o => o.Id).Contains(vo.Id));
                }
                else
                {
                    var newVariantId = await _dbContext.InsertWithInt32IdentityAsync(new VariantEntity
                    {
                        Name = variant.Name,
                        DisplayType = variant.DisplayType,
                        ProductId = product.Id
                    });

                    foreach (var option in variant.Options)
                    {
                        var newOptionId = await _dbContext.InsertWithInt32IdentityAsync(new VariantOptionEntity
                        {
                            Value = option.Value,
                            PriceAdjustment = option.PriceAdjustment,
                            SKU = option.SKU,
                            Stock = option.Stock,
                            VariantEntityId = newVariantId
                        });

                        foreach (var optionImage in option.OptionImages)
                        {
                            await _dbContext.InsertAsync(new ProductImageEntity
                            {
                                Url = optionImage.Url,
                                AltText = optionImage.AltText,
                                VariantOptionsEntityId = newOptionId
                            });
                        }
                    }
                }
            }

            // Remove variants that are no longer associated
            var variantsToRemove = existingVariants
                .Where(ev => !product.Variants.Any(v => v.Id == ev.Id))
                .ToList();
            await _dbContext.Variants.DeleteAsync(v => variantsToRemove.Select(vr => vr.Id).Contains(v.Id));

            await transaction.CommitAsync();
            return new Response { Success = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await _logger.LogAsync(ex.Message);
            return new Response { Success = false, Error = ex.Message };
        }
    }


    public async Task<List<SimpleProductDTO>> GetProductDTOs()
    {
        var query = from product in _dbContext.Products
                    join image in _dbContext.ProductImages
                    on product.PrimaryImageId equals image.Id into imageJoin
                    from image in imageJoin.DefaultIfEmpty()
                    select new SimpleProductDTO
                    {
                        Id = product.Id,
                        ImageUrl = image != null ? image.Url : null, // Handle null cases
                        Name = product.Name,
                        Slug = product.Slug,
                        Price = product.Price
                    };

        return await query.ToListAsync();
    }

}
