using System.ComponentModel.DataAnnotations;

namespace EStore.DTOs;

public class ContactDTOs
{
}
public class ContactSubmission
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; }

    [Required]
    public string Message { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; }

    public bool IsRead { get; set; }

    public bool IsResolved { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Response { get; set; }
}
