
using EStore.Data;
using EStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EStore.Code;

public class FAQHelper
{
    private readonly AppDbContext _context;
    public FAQHelper(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<FAQ>> Search(string query)
    {
        var faqs = await _context.FAQs
     .Where(c => c.Question.ToLower().Contains(query.ToLower()) || c.Answer.Contains(query))  
     .ToListAsync();

        return (new HashSet<FAQ>(faqs)).ToList();
    }
}
