﻿namespace EStore.DTOs;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }

}

public class SignUpDTO
{
    public string? Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}