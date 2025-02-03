using EStore.Code;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.Products;
using EStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
   
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }
    [HttpGet("simple")]
    public async Task<IActionResult> GetSimpleProducts()
    {
        var products = await _productRepository.GetProductDTOs();
        return Ok(products);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var products = await _productRepository.GetProductBySlug(slug);
        return Ok(products);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var products = await _productRepository.GetProductById(id);
        return Ok(products);
    }

    [HttpGet("recommendations/{slug}")]
    public async Task<IActionResult> GetProductRecommendations(string slug)
    {
        var products = await _productRepository.GetRandomProducts();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> AddOrUpdateProduct([FromForm] ProductAPI product, [FromQuery] string operation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Response response;

        if (operation == Operation.Add.ToString().ToLower())
        {
            response = await _productRepository.SaveAsync(product);
        }
        else
        {
            response = await _productRepository.UpdateAsync(product);
        }
        if (response.Success)
        {
            return Ok(product);
        }
        return BadRequest(response.Error);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
  
        Response response = await _productRepository.DeleteAsync(productId);
        if (response.Success) 
        {
            return Ok();
        }
        return BadRequest(response.Error);
    }
}
