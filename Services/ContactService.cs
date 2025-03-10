using EStore.Data;
using EStore.DTOs;
using EStore.Entities;
using EStore.Interfaces;
using LinqToDB;

namespace EStore.Services;


public class ContactService : IContactService
{
    private readonly AltDataContext _context;

    public ContactService(AltDataContext context)
    {
        _context = context;
    }

    public async Task<UserQueryEntity> CreateSubmissionAsync(UserQueryEntity submission)
    {
        await _context.InsertAsync(submission);
        //await _context.SaveChangesAsync();
        return submission;
    }

    public async Task<(IEnumerable<UserQueryEntity> Items, int TotalCount)> GetSubmissionsAsync(int page, int size, string status, string search)
    {
        var query = _context.UserQueries.AsQueryable();

        // Apply status filter
        switch (status?.ToLower())
        {
            case "unread":
                query = query.Where(s => !s.IsRead);
                break;
            case "read":
                query = query.Where(s => s.IsRead);
                break;
            case "resolved":
                query = query.Where(s => s.IsResolved);
                break;
            case "unresolved":
                query = query.Where(s => !s.IsResolved);
                break;
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search) ||
                s.Subject.ToLower().Contains(search)
            );
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<UserQueryEntity> GetSubmissionByIdAsync(int id)
    {
        return await _context.UserQueries.FirstOrDefaultAsync(m=>m.Id == id);
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var submission = await GetSubmissionByIdAsync(id);
        if (submission == null)
        {
            return false;
        }

        submission.IsRead = true;
        _context.Update(submission);
        //await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleResolvedAsync(int id, bool isResolved)
    {
        var submission = await GetSubmissionByIdAsync(id);
        if (submission == null)
        {
            return false;
        }

        submission.IsResolved = isResolved;
        _context.Update(submission);
        //await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SaveResponseAsync(int id, string response)
    {
        var submission = await GetSubmissionByIdAsync(id);
        if (submission == null)
        {
            return false;
        }

        submission.Response = response;
        submission.IsRead = true;
        _context.Update(submission);
        return true;
    }
}