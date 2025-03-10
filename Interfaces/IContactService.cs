using EStore.DTOs;
using EStore.Entities;

namespace EStore.Interfaces;

public interface IContactService
{
    Task<UserQueryEntity> CreateSubmissionAsync(UserQueryEntity submission);
    Task<(IEnumerable<UserQueryEntity> Items, int TotalCount)> GetSubmissionsAsync(int page, int size, string status, string search);
    Task<UserQueryEntity> GetSubmissionByIdAsync(int id);
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> ToggleResolvedAsync(int id, bool isResolved);
    Task<bool> SaveResponseAsync(int id, string response);
}