using EStore.Data;
using EStore.Interfaces;
using EStore.Models;
using EStore.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EStore.Code;

public class AuthHelper : IAuthRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    public AuthHelper(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;

    }
    public async Task<Response> SignUp(string userName,string email, string password, string confirmPassword)
    {
        if(password != confirmPassword)
        {
            return new Response { Success = false, Error = "Passwords do not match" };
        }
        var user = new AppUser
        {
            UserName = userName,
            Email = email
        };
        try
        {

        var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new Response { Success = false, Error = "An Error occured while creating the user" };
            }
        }
        catch(Exception ex)
        {
            return new Response { Success = false, Error = ex.Message };
        }
      
        await _userManager.AddToRoleAsync(user, "Customer");

        return new Response { Success = true };
    }


    public async Task<Response?> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }
        var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
             return new Response { Success = true, Data = await GenerateJwtToken(user) };
        }

        return null;
    }

    public async Task<string> GenerateJwtToken(object user)
    {
        if(user is AppUser appUser)
        {

            var userClaims = new List<Claim>
        {
        new Claim("userId", appUser.Id),
        new Claim("email", appUser.Email),
        new Claim("userName", appUser.UserName)
        };

            var roles = await _userManager.GetRolesAsync(appUser);
            var role = roles.FirstOrDefault();

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
