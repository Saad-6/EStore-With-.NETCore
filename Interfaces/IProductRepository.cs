using EStore.DTOs;
using EStore.Entities;
using EStore.Models;
using EStore.Models.Products;

namespace EStore.Interfaces;

public interface IProductRepository
{
    Task<Response> GiveReviewAsync(int productId, ReviewDTO reviewDTO);
    Task<Response> SaveAsync(ProductAPI product);
    Task<Response> DeleteAsync(int productId);
    Task<Response> UpdateAsync(ProductAPI product);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> Search(string query);
    Task<Product> GetProductById(int id);
    Task<Product> GetProductBySku(string sku);
    Task<Product> GetProductBySlug(string slug);
    Task<List<Product>> GetProductsByName(string name);
    Task<List<Product>> GetProductsByCategoryName(string name);
    Task<List<Product>> GetProductsByCategoryId(int id);
    Task<List<Product>> GetProductsByDescription(string descriptionText);
    Task<List<Product>> GetRandomProducts(int count = 3);
    Task<List<SimpleProductDTO>> GetProductDTOs();
    Task<Response> GetVariantsAndOptions(List<int> variantIds, List<int> optionIds);
}
