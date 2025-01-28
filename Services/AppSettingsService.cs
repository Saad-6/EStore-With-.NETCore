using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;

namespace EStore.Services;

public class AppSettingsService : IAppSettingsService
{
    public string BaseUrl { get; private set; }

    public AppSettingsService(IRepository<SiteSettingsEntity> siteSettings)
    {
        var baseUrl = siteSettings.GetAllAsync().Result; // Fetch once during initialization
        BaseUrl = baseUrl.FirstOrDefault()?.URL ?? throw new Exception("Base URL not configured");
    }
}