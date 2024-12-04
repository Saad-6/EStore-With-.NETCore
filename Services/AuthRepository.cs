using EStore.Code;
using EStore.Data;
using EStore.Entities;
using EStore.Interfaces;
using EStore.Models;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EStore.Services;

public class AuthRepository : IAuthRepository
{

    private readonly AltDataContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly ILogRepository _logger;
    public AuthRepository(AltDataContext dataContext, IConfiguration configuration, ILogRepository logger)
    {
        _dataContext = dataContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Response> Login(string email, string password)
    {
        var user = _dataContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return new Response { Success = false, Error = "User is null" };
        }
        if (PasswordHelper.VerifyPassword(password, user.Password))
        {
            return new Response { Success = true, Data = await GenerateJwtToken(user) };
        }
        return new Response { Success = false, Error = "Invalid Credentials" };

    }

    public async Task<Response> SignUp(string username, string email, string password, string confirmPassword)
    {

        if(password != confirmPassword)
        {
            return new Response { Success = false, Error = "Passwords do not match" };
        }

        var userExists = await _dataContext.Users.FirstOrDefaultAsync(m=>m.Email == email);

        if(userExists!= null)
        {
            return new Response { Success = false, Error = "User with that email already exists" };
        }

        UserEntity userEntity = new UserEntity();

        userEntity.SignUp(username, email, password);

        EnsureRoleExists();

        var customerRole = await _dataContext.Roles.FirstOrDefaultAsync(m => m.Role == "Admin");
        userEntity.RoleId = customerRole?.Id;

        try
        {
            await _dataContext.InsertAsync(userEntity);
        }
        catch (Exception ex)
        {
            return new Response { Success = false, Error = ex.Message };
        }

        return new Response { Success = true };

    }
    private async void EnsureRoleExists()
    {
        // Check if the role exists in the database
        var roleCount = await _dataContext.Roles.CountAsync();

        if (roleCount != 2)
        {
            var rolesToInsert = new List<RoleEntity>
              {
                  new RoleEntity { Role = "Customer" },
                  new RoleEntity { Role = "Admin" }
              };

                await _dataContext.BulkCopyAsync(rolesToInsert);
            
        }

    }

    public string GetRole(string email)
    {
        var user = _dataContext.Users.FirstOrDefault(m=>m.Email == email);
        if(user  == null)
        {
            return "";
        }
        var role = _dataContext.Roles.FirstOrDefault(m => m.Id == user.RoleId);
        return role.Role ?? "";
    }
    public async Task<string> GenerateJwtToken(object user)
    {
        if (user is UserEntity appUser)
        {
            var userClaims = new List<Claim>
            {
        new Claim("userId", appUser.Id.ToString()),
        new Claim("email", appUser.Email),
        new Claim("userName", appUser.UserName)
        };


            var role = GetRole(appUser.Email);

            if (role != null)
            {
                userClaims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiResponse:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        else
        {
            return "";
        }

    }
}
