using EStore.Code;
using EStore.Data;
using EStore.Interfaces;
using EStore.Models.User;
using EStore.Services;
using EStore.Utility;
using FluentMigrator.Runner;
using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EStore.Startup;
public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

        // Identity
        services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<AppDbContext>();


        //Linq2Db
        var connectionString = GetConnectionString(configuration);
        var options = new DataOptions()
                       .UseSqlServer(SqlServerVersion.v2017, SqlServerProvider.MicrosoftDataSqlClient)
                       .UseConnectionString(connectionString);

        // Register AltDataContext with the configured options
        services.AddScoped<AltDataContext>(_ => new AltDataContext(options));


        // Services or Helpers using linq2db

        services.AddScoped<Mapper>();
        services.AddScoped<FileHandler>();
        services.AddTransient<MigrationExtension>(); 
        services.AddScoped<ILogRepository,LogRepository>();
        services.AddScoped<IAuthRepository,AuthRepository>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IOrderRepository,OrderRepository>();
        services.AddScoped<ILayoutRepository,LayoutRepository>();
        services.AddScoped<IProductRepository,ProductRepository>();
        services.AddScoped<ICategoryRepository,CategoryRepository>();
        services.AddScoped<IAppSettingsService, AppSettingsService>();
        services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

        // CORS

        services.AddCors();

        // JWT Authentication

        ConfigureJwtAuthentication(services, configuration);
    }
    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        //var claims = new List<Claim>()
        //{
        //    new Claim(ClaimTypes.Name,"userId"),
        //    new Claim(ClaimTypes.Email,"userId"),
        //    new Claim(ClaimTypes.Role,"userId"),
        //    new Claim(ClaimTypes.Name,"userId"),

        //};

        //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey"));
        //var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.Sha256);

        //var token = new JwtSecurityToken(
        //      issuer: "domain",
        //      audience: "domain",
        //      claims: claims,
        //      expires: DateTime.UtcNow.AddHours(10),
        //      signingCredentials: credentials
        //    );

        //string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var key = configuration.GetValue<string>("ApiResponse:SecretKey");
        services.AddAuthentication(u =>
        {
            u.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            u.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(u =>
        {
            u.SaveToken = true;
            u.RequireHttpsMetadata = false;
            u.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
    public static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("DefaultConnection");
    }
}
