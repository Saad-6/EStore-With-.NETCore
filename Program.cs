using EStore.Models.Order;
using EStore.Models.User;
using EStore.Services;
using EStore.Startup;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Default Services

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

// Our App Specific Services are defined through this wrapper method

builder.Services.AddCustomServices(builder.Configuration);

// Logging

builder.Services.AddLogging(builder =>
{
    builder.AddConsole()
           .SetMinimumLevel(LogLevel.Debug);  
});

// Migrations using Fluent Migrator 

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(runner => runner
        .AddSqlServer()  // Database Provider
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(AppDomain.CurrentDomain.GetAssemblies()).For.Migrations())  // Scan for migrations in the current assembly
    .AddLogging(lb => lb.AddFluentMigratorConsole());

var app = builder.Build();

// Run migrations on startup

using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<MigrationExtension>();
    migrationService.RunMigrations();  
}


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors(o => o
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:3000") // Replace with your frontend URL
    .AllowCredentials() // This is required for credentials
);


app.UseAuthentication();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "StaticFiles/images")),
    RequestPath = "/images" // URL prefix for static files
});

app.UseRouting();

app.UseAuthorization();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Seed roles and admin user
  //  await SeedRolesAndAdminUser(services);
}

app.MapControllers();

app.Run();

async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

    // Add roles if they do not exist
    var roles = new[] { "Admin", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed an admin user if it doesn't already exist
    var adminEmail = "admin@estore.com";
    var adminPassword = "Admin@123"; // Set a secure password in production
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,  // Auto-confirm email for seeded user
            Address = new Address
            {
                FirstName = "Admin",
                LastName = "User",
                City = "AdminCity",
                StreetAddress = "AdminStreet",
                ZipCode = "00000",
                PhoneNumber = "1234567890"
            }
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            // Assign the admin user to the "Admin" role
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}