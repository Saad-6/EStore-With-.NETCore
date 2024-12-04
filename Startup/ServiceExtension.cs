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
using System.Reflection;
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

        // Helpers
    //    services.AddScoped<ProductHelper>();
    //    services.AddScoped<CategoryHelper>();
    //    services.AddScoped<ReviewHelper>();
    //    services.AddScoped<FAQHelper>();
    //    services.AddScoped<OrderHelper>();
    ////   services.AddScoped<AuthHelper>();
    //    services.AddScoped<LayoutHelper>();

        services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
        
        services.AddScoped<Mapper>();
        services.AddScoped<ILogRepository,LogRepository>();
        services.AddScoped<IOrderRepository,OrderRepository>();
        services.AddScoped<ILayoutRepository,LayoutRepository>();
        services.AddScoped<IProductRepository,ProductRepository>();
        services.AddScoped<ICategoryRepository,CategoryRepository>();
        services.AddScoped<IAuthRepository,AuthRepository>();
        services.AddTransient<MigrationExtension>();

        //services.AddFluentMigratorCore()
        //.ConfigureRunner(runner => runner
        //    .AddSqlServer()  // Add the appropriate database provider
        //    .WithGlobalConnectionString(configuration.GetConnectionString("DefaultConnection"))
        //    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())  // Scan for migrations in the current assembly
        //.AddLogging(lb => lb.AddFluentMigratorConsole());

        // services.AddFluentMigratorCore()
        //.ConfigureRunner(runner => runner
        //    .AddSqlServer()
        //    .WithGlobalConnectionString(connectionString)
        //    .ScanIn(AppDomain.CurrentDomain.GetAssemblies()).For.Migrations()); // Specify the assembly that contains your migrations

        //services.AddFluentMigratorCore();

        // CORS
        services.AddCors();

        // JWT Authentication
        ConfigureJwtAuthentication(services, configuration);
    }
    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
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
