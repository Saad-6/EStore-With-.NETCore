using EStore.Data;
using EStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EStore.Code;

public class ReviewHelper
{
    private readonly AppDbContext _context;
    public ReviewHelper(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Review>> Search(string query)
    {
        var reviews = await _context.Reviews
        .Where(c => c.Comment.ToLower().Contains(query.ToLower()))
        .ToListAsync();

        return (new HashSet<Review>(reviews)).ToList();
    }
}
