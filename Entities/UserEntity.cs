

using EStore.Code;
using EStore.Models;
using System.Text.RegularExpressions;

namespace EStore.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool EmailConfirmed { get; set; }
    public int? RoleId { get; set; }

    public Response SignUp(string userName, string email, string password)
    {
        Id = Guid.NewGuid();
        // Null or whitespace validation
        if (string.IsNullOrWhiteSpace(userName))
        {
            return new Response { Success = false, Error = "Username is required." };
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return new Response { Success = false, Error = "Email is required." };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new Response { Success = false, Error = "Password is required." };
        }

        // Email validation
        if (!IsValidEmail(email))
        {
            return new Response { Success = false, Error = "Invalid email format." };
        }

        // Password length validation
        if (password.Length < 6)
        {
            return new Response { Success = false, Error = "Password must be at least 6 characters long." };
        }

        // Assign properties
        UserName = userName;
        Email = email;
        HashPassword(password);
        EmailConfirmed = false;
        return new Response { Success = true, Error = "User registered successfully." };
    }

    public void HashPassword(string password)
    {
        Password = PasswordHelper.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHelper.VerifyPassword(password, Password);
    }

    private bool IsValidEmail(string email)
    {
        const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

}
