using EStore.Data;
using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Products;
using EStore.Models.User;
using FluentMigrator;
using LinqToDB;
using LinqToDB.Data;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;



namespace EStore.Services;

public class ProductRepository : IProductRepository
{
    private readonly AltDataContext _dbContext;
    private readonly ILogRepository _logger;

    private readonly FileHandler _fileHandler;
    private const string DIRECTORY = "StaticFiles";
    private const string SUB_DIRECTORY = "images/products";
    private readonly IAppSettingsService _appSettingsService;
    public ProductRepository(AltDataContext dataContext, ILogRepository logger, FileHandler fileHandler, IAppSettingsService appSettingsService)
    {
        _dbContext = dataContext;
        _logger = logger;
        _fileHandler = fileHandler;
        _appSettingsService = appSettingsService;
 
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
            await DeleteVariants(variants);

            // Step 2: Delete Additional Images
            var imagesToBeDeleted = await _dbContext.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
            // Get URLs and Delete  Images from the server
            var UrlsImagesToBeDeleted = imagesToBeDeleted.Select(m=>m.Url).ToList();
            UrlsImagesToBeDeleted.ForEach(m => _fileHandler.DeleteFile(m));

            // Finall delete the Image Entites from the database
            var imageToBeDeleteIds = imagesToBeDeleted.Select(m => m.Id).ToList();
            await _dbContext.ProductImages.Where(m=>imageToBeDeleteIds.Contains(m.Id)).DeleteAsync();

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
                var primaryImage = await _dbContext.ProductImages
                    .Where(image => image.Id == product.PrimaryImageId)
                    .FirstOrDefaultAsync();
                 _fileHandler.DeleteFile(primaryImage.Url);
                await _dbContext.DeleteAsync(primaryImage);
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

    public async Task<List<Product>> GetAllAsync()
    {
       
        return await GetProductQuery().ToListAsync();
    }

    public async Task<Product> GetProductBySku(string sku)
    {
        return await GetProductQuery().FirstOrDefaultAsync(m=>m.SKU == sku);
    }

    public async Task<Product> GetProductBySlug(string slug)
    {
        return await GetProductQuery().FirstOrDefaultAsync(m => m.Slug == slug);
    }

    public async Task<List<Product>> GetProductsByCategoryId(int id)
    {
        return await GetProductQuery().Where(m => m.Category.Id == id).ToListAsync();
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
  
    public Task<List<Product>> Search(string query)
    {
        throw new NotImplementedException();
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
                        ImageUrl = image != null ? _appSettingsService.BaseUrl+"/"+ image.Url : null, // Handle null cases
                        Name = product.Name,
                        Slug = product.Slug,
                        Price = product.Price,
                        Stock = product.Stock,
                        CategoryName = _dbContext.Categories.FirstOrDefault(m=>m.Id == product.CategoryId).Name ?? "No Category Found"
                    };

        return await query.ToListAsync();
    }

    public async Task<Product> GetProductById(int id)
    {
        return await GetProductQuery().FirstOrDefaultAsync(m => m.Id == id);
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
                       //Description = category.Description,
                       //ThumbNailUrl = category.ThumbNailUrl
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
                       Url = image.Url.Contains("http", StringComparison.OrdinalIgnoreCase)
                       ? pi.Url
                       : $"{_appSettingsService.BaseUrl}/{pi.Url}",
                       AltText = pi.AltText
                   })
                   .ToList(),
                   PrimaryImage = image == null ? null : new ProductImage
                   {
                       Id = image.Id,
                       Url = image.Url.Contains("http", StringComparison.OrdinalIgnoreCase)
                       ? image.Url
                       : $"{_appSettingsService.BaseUrl}/{image.Url}",
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
                                   Url = oi.Url.Contains("http", StringComparison.OrdinalIgnoreCase)
                                   ? oi.Url
                                   : $"{_appSettingsService.BaseUrl}/{oi.Url}",

                               })
                               .ToList()
                       })
                       .ToList()
               })
               .ToList(),
                      Reviews = _dbContext.Reviews.Where(m => m.ProductId == product.Id).Select(n => new Review
                      {
                          Comment = n.Comment,
                          Stars = n.Stars,
                          PostedAt = n.CreatedAt,
                          GivenBy = _dbContext.Users.FirstOrDefault(b => b.Id.ToString() == n.UserId).UserName
                      }).ToList()
               };
    }

    public async Task<Response> SaveAsync(ProductAPI product)
    {
        ProductEntity productEntity;
        var transaction = _dbContext.BeginTransaction();
        Response response;
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
            if (product.PrimaryImageFile != null)
            {
                response = await _fileHandler.SaveFileAsync(product.PrimaryImageFile, DIRECTORY,SUB_DIRECTORY);
                if (!response.Success)
                {
                    return response;
                }
                var filePath = (string)response.Data;
                primaryImageId = await _dbContext.InsertWithInt32IdentityAsync(new ProductImageEntity
                {
                    Url = filePath,
                    AltText = filePath,
                });
            }

            // Step 3: Add Discount
            //int? discountId = null;
            //if (product.Discount != null)
            //{
            //    discountId = await _dbContext.InsertWithInt32IdentityAsync(new DiscountEntity
            //    {
            //        isActive = product.Discount.isActive,
            //        DiscountPrice = product.Discount.DiscountPrice,
            //        DiscountStartDate = product.Discount.DiscountStartDate,
            //        DiscountEndDate = product.Discount.DiscountEndDate
            //    });
            //}

            // Step 4: Get Category
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

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
                DiscountId = 0
            };

            int productId = await _dbContext.InsertWithInt32IdentityAsync(productEntity);

            // Step 6: Add Additional Product Images
            if (product?.AdditionalImages?.Files != null && product.AdditionalImages.Files.Any())
            {
                var listOfImages = new List<ProductImageEntity>();
                foreach(var image in product.AdditionalImages.Files)
                {
                    response = await _fileHandler.SaveFileAsync(image, DIRECTORY,SUB_DIRECTORY);
                    if (!response.Success)
                    {
                        await _logger.LogAsync(response.Error);
                        return response;
                    }
                    var imagePath = (string)response.Data;
                    var imageEntity = new ProductImageEntity
                    {
                        Url = imagePath,
                        ProductId = productId,
                        AltText = imagePath,

                    };
                    listOfImages.Add(imageEntity);

                }


                // Perform bulk copy
                await _dbContext.BulkCopyAsync(listOfImages);

            }


            // Step 7: Add Variants
            if (product?.Variants?.NewVariants != null && product.Variants.NewVariants.Any())
            {
                response = await AddNewVariant(product.Variants.NewVariants, productId);
          
            }

            // Commit transaction
            await transaction.CommitAsync();
            return new Response { Success = true };
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            await _logger.LogAsync(ex.Message);
            return new Response { Success = false, Error = ex.Message };
        }
        finally
        {
            transaction.Dispose();
        }
    }

    async Task<Response> DeleteVariants(List<VariantEntity> variantsToBeDeleted)
    {
        Response response = new();
        try
        {
        foreach (var variant in variantsToBeDeleted)
        {
            // For each variant to be deleted, get it's option Ids
            var variantOptionIds = await _dbContext.VariantOptions.Where(m => m.VariantEntityId == variant.Id).Select(m => m.Id).ToListAsync();

            // Get all Images which contain those option ids
            var imagesToBeDeleted = await _dbContext.ProductImages.Where(m => variantOptionIds.Contains(m.VariantOptionsEntityId ?? 0)).ToListAsync();

            // Get their Ids
            var imagesToBeDeletedIds = imagesToBeDeleted.Select(m => m.Id).ToList();

            // Delete them on server
            imagesToBeDeleted.ForEach(m => _fileHandler.DeleteFile(m.Url));

            // Then Delete the Image Entities from the Database
            await _dbContext.ProductImages.Where(m => imagesToBeDeletedIds.Contains(m.Id)).DeleteAsync();

            // Now delete the options themselves
            await _dbContext.VariantOptions.Where(m => variantOptionIds.Contains(m.Id)).DeleteAsync();

        }
        // And finally, delete the variants
        await _dbContext.Variants.Where(m => variantsToBeDeleted.Select(n => n.Id).Contains(m.Id)).DeleteAsync();

        }
        catch(Exception ex)
        {
            response.Error = ex.Message;
            response.Success = false;
            await _logger.LogAsync(ex.Message);
        }
        return response;
    }

    async Task<Response> AddNewVariant(List<VariantDTO> variants ,int productId)
    {
        Response response = new();
        var variantIdsList = variants.Select(m => m.Id).ToList();

        // Get List of variants which are not included (Removed on the frontend) to be soon deleted
        var variantsToBeDeleted = await _dbContext.Variants.Where(m => m.ProductId == productId && !variantIdsList.Contains(m.Id)).ToListAsync();
        await DeleteVariants(variantsToBeDeleted);
 

        foreach (var variant in variants)
        {      
            int variantId;

            // Variant Exists
            if(variant.Id != 0)
            {
                var existingVariant = _dbContext.Variants.FirstOrDefault(m=>m.Id == variant.Id);
                variantId = variant.Id;

                existingVariant.Name = variant.Name;
                existingVariant.DisplayType = variant.DisplayType;
                existingVariant.ProductId = productId;

                await _dbContext.UpdateAsync(existingVariant);
            }
            // New Variant
            else
            {

            variantId = await _dbContext.InsertWithInt32IdentityAsync(new VariantEntity
            {
                Name = variant.Name,
                DisplayType = variant.DisplayType,
                ProductId = productId
            });
            }

            // Add Variant Options
            if (variant.NewOptions != null && variant.NewOptions.Any())
            {
                foreach (var option in variant.NewOptions)
                {
                    int optionId;
                    var optiondImageIdsToKeepString = option?.ExistingOptionImageIds?.FirstOrDefault()?.Split(',')
                    .Select(url => url.Trim('"'))
                    .Select(url => TrimUrl(url))
                    .ToList() ?? new List<string>();

                    var optiondImageIdsToKeep = ParseInt(optiondImageIdsToKeepString);



                    // Optioon already exists
                    if(option?.Id != 0)
                    {
                        optionId = option.Id;

                        var existingOption = _dbContext.VariantOptions.FirstOrDefault(m => m.Id == option.Id) ?? new VariantOptionEntity();
                       
                        existingOption.SKU = option.SKU;
                        existingOption.PriceAdjustment = option.PriceAdjustment;
                        existingOption.Stock = option.Stock;
                        existingOption.Value = option.Value;
                        await _dbContext.UpdateAsync(existingOption);

                    }
                    // Option is new
                    else
                    {
                    optionId = await _dbContext.InsertWithInt32IdentityAsync(new VariantOptionEntity
                    {
                        Value = option.Value,
                        PriceAdjustment = option.PriceAdjustment,
                        SKU = option.SKU,
                        Stock = option.Stock,
                        VariantEntityId = variantId
                    });

                    }
                    await _dbContext.ProductImages.Where(m => m.VariantOptionsEntityId == optionId && !optiondImageIdsToKeep.Contains(m.Id)).DeleteAsync();
                    if (option.NewOptionImages != null && option.NewOptionImages.Any())
                    {
                        var listOfImages = new List<ProductImageEntity>();
                        foreach (var optionImage in option.NewOptionImages)
                        {

                            response = await _fileHandler.SaveFileAsync(optionImage, DIRECTORY, SUB_DIRECTORY);
                            if (!response.Success)
                            {
                                await _logger.LogAsync(response.Error);
                                return response;
                            }
                            var imagePath = (string)response.Data;
                            var imageEntity = new ProductImageEntity
                            {
                                Url = imagePath,
                                AltText = imagePath,
                                VariantOptionsEntityId = optionId
                            };
                            listOfImages.Add(imageEntity);

                        }
                        // Perform bulk copy
                        await _dbContext.BulkCopyAsync(listOfImages);
                    }

                }
            }
        }
        return response;
    }

    public async Task<Response> UpdateAsync(ProductAPI product)
    {
        var transaction = _dbContext.BeginTransaction();
        Response response = new();

        try
        {
            var productEntity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (productEntity == null)
            {
                return new Response { Success = false, Error = "Product not found" };
            }

            // Step 1: Update SEO
            if (product.SEO != null)
            {
                if (productEntity.SEOId.HasValue)
                {
                    var seoEntity = await _dbContext.SEOs.FirstOrDefaultAsync(s => s.Id == productEntity.SEOId);
                    if (seoEntity != null)
                    {
                        seoEntity.MetaTitle = product.SEO.MetaTitle;
                        seoEntity.MetaDescription = product.SEO.MetaDescription;
                        seoEntity.MetaKeywords = product.SEO.MetaKeywords;
                        seoEntity.CanonicalUrl = product.SEO.CanonicalUrl;
                        await _dbContext.UpdateAsync(seoEntity);
                    }
                }
                else
                {
                    productEntity.SEOId = await _dbContext.InsertWithInt32IdentityAsync(new SEOEntity
                    {
                        MetaTitle = product.SEO.MetaTitle,
                        MetaDescription = product.SEO.MetaDescription,
                        MetaKeywords = product.SEO.MetaKeywords,
                        CanonicalUrl = product.SEO.CanonicalUrl,
                    });
                }
            }
            // End

            // Step 2: Update Primary Image
            if (product.PrimaryImageFile != null)
            {
                // Get current primary Image
                var fileName = (await _dbContext.ProductImages.FirstOrDefaultAsync(m => m.Id == productEntity.PrimaryImageId)).Url;

                // Delete it from the server
                 _fileHandler.DeleteFile(fileName);

                // Add new Image to the server
                response = await _fileHandler.SaveFileAsync(product.PrimaryImageFile, DIRECTORY, SUB_DIRECTORY);

                var filePath = (string)response.Data;

                if (productEntity.PrimaryImageId.HasValue)
                {
                    var imageEntity = await _dbContext.ProductImages.FirstOrDefaultAsync(i => i.Id == productEntity.PrimaryImageId);
                    if (imageEntity != null)
                    {
                        imageEntity.Url = filePath;
                        imageEntity.AltText = filePath;
                        await _dbContext.UpdateAsync(imageEntity);
                    }
                }
                else
                {
                    productEntity.PrimaryImageId = await _dbContext.InsertWithInt32IdentityAsync(new ProductImageEntity
                    {
                        Url = filePath,
                        AltText = filePath,
                        ProductId = product.Id
                    });
                }
            }
            // End


            // Step 3: Update Category
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            if (category == null)
            {
                await _logger.LogAsync("Inavlid Category Id while updating product");
                return new Response { Success = false, Error = "Invalid category ID" };
            }
            productEntity.CategoryId = category.Id;

            // Step 4: Update Product Details
            productEntity.Price = product.Price;
            productEntity.Description = product.Description;
            productEntity.Name = product.Name;
            productEntity.Brand = product.Brand;
            productEntity.SKU = product.SKU;
            productEntity.Stock = product.Stock;
            productEntity.Slug = product.Slug;
            await _dbContext.UpdateAsync(productEntity);

            // End

            var existingImages = await _dbContext.ProductImages      // All Images related to the product (minus primary image)
                .Where(img => img.ProductId == productEntity.Id)
                .ToListAsync();

            existingImages.Select(m => m.Url = _appSettingsService.BaseUrl + "/" + m.Url).ToList(); // Append the URL before imgages to make valid comparisons (Its stored as relative path in DB and as a url when fetched from the frontend)

            var imagesToDelete = new List<ProductImageEntity>();  // Initialize the images to be deleted list 

            var imageUrlsToKeep = new List<string>(); // Initialize the images to be kept list 

            var existingUrls = product.AdditionalImages?.AlreadyPresentUrls?
                .FirstOrDefault() // Get the single string
                ?.Split(',') // Split by comma
                .Select(url => url.Trim('"')) // Trim extra quotes
                .Select(url => TrimUrl(url)) // Apply your trim function
                .ToList() ?? new List<string>();
            // Trim and prepare exisitng URLs to be added to the keep list


            imageUrlsToKeep.AddRange(existingUrls);  // Concat the alreadyPresent and newlyAdded lists


            // Step 5.1 : Add newly added Additional Images
            if (product.AdditionalImages?.Files != null && product.AdditionalImages.Files.Any())
            {
                var newImages = new List<ProductImageEntity>();

                foreach (var image in product.AdditionalImages.Files)
                {
                    var imgResponse = await _fileHandler.SaveFileAsync(image, DIRECTORY, SUB_DIRECTORY);
                    if (!imgResponse.Success)
                    {
                        return imgResponse;
                    }

                    newImages.Add(new ProductImageEntity
                    {
                        Url = (string)imgResponse.Data,
                        ProductId = product.Id,
                        AltText = (string)imgResponse.Data
                    });
                }

                // Newly added images will be added to the keep list
                 var newImageUrls = newImages.Select(img => img.Url).ToList();
                imageUrlsToKeep = imageUrlsToKeep.Concat(newImageUrls).ToList();

                // Add only new images
                await _dbContext.BulkCopyAsync(newImages);
            }

            // End



            // Step 5.2 : Delete removed Images

            // Delete any image that isn't part of either (meaning it was removed during the update process)
            imagesToDelete = existingImages.Where(img => !imageUrlsToKeep.Contains(img.Url)).ToList();
            foreach (var image in imagesToDelete)
            {
                _fileHandler.DeleteFile(image.Url);
            }
            if (imagesToDelete.Any())
            { 
    
                var imageIdsToDelete = imagesToDelete.Select(img => img.Id).ToList();

                await _dbContext.ProductImages
                       .Where(img => imageIdsToDelete.Contains(img.Id))
                       .DeleteAsync();

            }


            // Step 6: Update Variants

            var variantIdsString = product?.Variants?.ExistingVariantIds?.FirstOrDefault()
                ?.Split(',') 
                .Select(url => url.Trim('"')) 
                .Select(url => TrimUrl(url)) 
                .ToList() ?? new List<string>();
            var variantIds = ParseInt(variantIdsString);
            
            await _dbContext.Variants
            .Where(v => v.ProductId == product.Id && !variantIds.Contains(v.Id)) // Exclude variants in the list
            .DeleteAsync();

            if (product?.Variants?.NewVariants != null && product.Variants.NewVariants.Any())
            {
                response = await AddNewVariant(product.Variants.NewVariants, product.Id ?? 0);
            }

            // Commit transaction
            await transaction.CommitAsync();
            return new Response { Success = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await _logger.LogAsync(ex.Message);
            return new Response { Success = false, Error = ex.Message };
        }
        finally
        {
            transaction.Dispose();
        }
    }

    string TrimUrl(string url)
    {
        return url.Trim(new char[] { '[', ']', '"' });
    }

    List<int> ParseInt(List<string> str)
    {
        List<int> list = new();
        foreach(var c in str)
        {
            if (int.TryParse(c, out int num))
            {
                list.Add(num);
            }
        }
        return list;
    }

    public Task<Response> SaveAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task<Response> UpdateAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> GiveReviewAsync(int productId, ReviewDTO reviewDTO)
    {
        Response response = new();

        var product =  await GetProductById(productId);

        if(product == null || string.IsNullOrEmpty(reviewDTO.UserId))
        {
            response.Success = false;
            response.Error = "Product does not exist";
            return response;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id.ToString() == reviewDTO.UserId);

        if(user == null)
        {
            response.Success = false;
            response.Error = "User does not exist";
            return response;
        }

        var existingReview = await _dbContext.Reviews.Where(m => m.UserId == user.Id.ToString() && m.ProductId == productId).ToListAsync();
        if(existingReview!= null && existingReview.Count > 0)
        {
            response.Success = false;
            response.Error = "You have already sumbitted a review for this order";
            return response;
        }

        try
        {
            await _dbContext.InsertAsync(new ReviewEntity
            {
                Comment = reviewDTO.Comment,
                CreatedAt = DateTime.Now,
                UserId = reviewDTO.UserId,
                ProductId = productId,
                Stars = reviewDTO.Stars
            });
            response.Success = true;
        }
        catch(Exception ex)
        {
            response.Success = false;
            response.Error =ex.Message;
        }
        return response;
    }


    public async Task<Response> GetVariantsAndOptions(List<int> variantIds, List<int> optionIds)
    {
        var response = new Response();

        try
        {
            var variants = await _dbContext.Variants
                .Where(v => variantIds.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id, v => v.Name);

            var options = await _dbContext.VariantOptions
                .Where(o => optionIds.Contains(o.Id))
                .ToDictionaryAsync(o => o.Id, o => o.Value);

            response.Success = true;
            response.Data = new { variants, options };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Error = $"Failed to get variants and options: {ex.Message}";
            await _logger.LogAsync(ex.Message);
        }

        return response;
    }

}
