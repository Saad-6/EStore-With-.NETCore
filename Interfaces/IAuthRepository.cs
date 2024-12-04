using EStore.Entities;
using EStore.Models;
using EStore.Models.User;

namespace EStore.Interfaces;

public interface IAuthRepository
{
    Task<Response> SignUp(string username, string email, string password, string confirmPassword);
    Task<Response> Login(string email, string password);
    Task<string> GenerateJwtToken(object user);
}
