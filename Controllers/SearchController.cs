using EStore.Code;
using EStore.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ProductHelper _productHelper;
    private readonly CategoryHelper _categoryHelper;
    private readonly FAQHelper _faqHelper;
    private readonly ReviewHelper _reviewHelper;
    public SearchController(ProductHelper productHelper, CategoryHelper categoryHelper, FAQHelper faqHelper, ReviewHelper reviewHelper)
    {
        _productHelper = productHelper;
        _categoryHelper = categoryHelper;
        _faqHelper = faqHelper;
        _reviewHelper = reviewHelper;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        var products = await _productHelper.GetProductsByName(query);
        var categories = await _categoryHelper.GetCategoriesByName(query);
        var reviews = await _reviewHelper.Search(query);
        var faqs = await _faqHelper.Search(query);

        var searchDto = new SearchDTO
        {
            Products = products,
            Categories = categories,
            Reviews = reviews,
            FAQs = faqs
        };
        return Ok(searchDto);
    }

}
