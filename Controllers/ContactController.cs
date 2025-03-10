using EStore.Entities;
using EStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;
    //private readonly IEmailService _emailService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
       // _emailService = emailService;
    }

    // POST: api/Contact
    [HttpPost]
    public async Task<IActionResult> SubmitContact([FromBody] ContactSubmissionDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var contactSubmission = new UserQueryEntity
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Subject = model.Subject,
                Message = model.Message,
                Category = model.Category,
                IsRead = false,
                IsResolved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _contactService.CreateSubmissionAsync(contactSubmission);

            // Send notification to admin
            //await _emailService.SendAdminNotificationAsync(
            //    "New Contact Form Submission",
            //    $"A new contact form submission has been received from {model.Name}. Subject: {model.Subject}"
            //);

            //// Send confirmation to user
            //await _emailService.SendEmailAsync(
            //    model.Email,
            //    "Thank you for contacting us",
            //    "We have received your message and will get back to you as soon as possible."
            //);

            return Ok(new { message = "Your message has been submitted successfully" });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    // GET: api/Contact
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSubmissions([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string status = "all", [FromQuery] string search = null)
    {
        try
        {
            var result = await _contactService.GetSubmissionsAsync(page, size, status, search);
            return Ok(new { items = result.Items, totalCount = result.TotalCount });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while retrieving submissions" });
        }
    }

    // GET: api/Contact/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSubmission(int id)
    {
        try
        {
            var submission = await _contactService.GetSubmissionByIdAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            return Ok(submission);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while retrieving the submission" });
        }
    }

    // PATCH: api/Contact/{id}/read
    [HttpPatch("{id}/read")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            var success = await _contactService.MarkAsReadAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while updating the submission" });
        }
    }

    // PATCH: api/Contact/{id}/resolve
    [HttpPatch("{id}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleResolved(int id, [FromBody] ResolveDTO model)
    {
        try
        {
            var success = await _contactService.ToggleResolvedAsync(id, model.IsResolved);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while updating the submission" });
        }
    }

    // POST: api/Contact/{id}/respond
    [HttpPost("{id}/respond")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RespondToSubmission(int id, [FromBody] ResponseDTO model)
    {
        if (string.IsNullOrEmpty(model.Response))
        {
            return BadRequest(new { message = "Response cannot be empty" });
        }

        try
        {
            var submission = await _contactService.GetSubmissionByIdAsync(id);
            if (submission == null)
            {
                return NotFound();
            }

            // Save the response
            await _contactService.SaveResponseAsync(id, model.Response);

            //// Send email to the user
            //await _emailService.SendEmailAsync(
            //    submission.Email,
            //    $"Re: {submission.Subject}",
            //    model.Response
            //);

            // Mark as resolved
            await _contactService.ToggleResolvedAsync(id, true);

            return Ok(new { message = "Response sent successfully" });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An error occurred while sending the response" });
        }
    }
}

public class ContactSubmissionDTO
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public string? Category { get; set; }
}

public class ResolveDTO
{
    public bool IsResolved { get; set; }
}

public class ResponseDTO
{
    public string Response { get; set; }
}