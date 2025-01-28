using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SiteSettingsController : ControllerBase
{
    private readonly IRepository<SiteSettingsEntity> _siteSettingsRepository;
    public SiteSettingsController(IRepository<SiteSettingsEntity> siteSettingsRepository)
    {
        _siteSettingsRepository = siteSettingsRepository;
    }
     async Task<SiteSettingsEntity> GetSettings()
    {
        var resposne = await _siteSettingsRepository.GetAllAsync();

        if (resposne.Count != 1)
        {
            return new SiteSettingsEntity();
        }
        return resposne.FirstOrDefault();
    }
    [HttpGet]
    public async Task<IActionResult> GetSiteName()
    {
        var settings = await GetSettings();
        if(settings == null)
        {
            return NotFound();
        }
 
        return Ok(settings);
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateSiteUrl(SiteSettingsDTO siteSettingsDTO)
    {
        if (string.IsNullOrEmpty(siteSettingsDTO.URL))
        {
            return BadRequest(new
            {
                message = "Invalid Url"
            });
        }

        var currentSettings = await GetSettings();

        if(currentSettings.Id == 0)
        {
            currentSettings = new SiteSettingsEntity();
            currentSettings.URL = siteSettingsDTO.URL;
            await _siteSettingsRepository.SaveAsync(currentSettings);
            return Ok();
        }
        currentSettings.URL = siteSettingsDTO.URL;
        await _siteSettingsRepository.UpdateAsync(currentSettings);
        return Ok();
    }
}
