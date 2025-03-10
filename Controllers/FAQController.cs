using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FAQController : ControllerBase
{
    private readonly IRepository<FAQEntity> _faqEntityRepository;
    const int pageSize = 10;
    public FAQController(IRepository<FAQEntity> faqEntityRepository)
    {
        _faqEntityRepository = faqEntityRepository;
    }
    [HttpGet]
    public async Task<IActionResult> GetFAQs([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var faqs = await _faqEntityRepository.GetAllAsync(page, size);
        return Ok(faqs);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddFAQ([FromBody] FAQDTO faq)
    {
        if (faq == null)
        {
            return BadRequest();
        }

        await _faqEntityRepository.SaveAsync(new FAQEntity
        {
            Question = faq.Question, // Fixed typo from Qusetion to Question
            Answer = faq.Answer,
        });
        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFAQ([FromBody] FAQUpdateDTO faq)
    {
        if (faq == null || faq.Id == 0)
        {
            return BadRequest();
        }

        var qa = await _faqEntityRepository.GetById(faq.Id);
        if (qa == null)
        {
            return BadRequest();
        }
        qa.Answer = faq.Answer;
        qa.Question = faq.Question; // Fixed typo from Qusetion to Question
        await _faqEntityRepository.UpdateAsync(qa);

        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFAQ([FromQuery] int faqId)
    {
        var faq = await _faqEntityRepository.GetById(faqId);
        if (faq == null)
        {
            return BadRequest();
        }
        await _faqEntityRepository.DeleteAsync(faq);
        return Ok();
    }
}
