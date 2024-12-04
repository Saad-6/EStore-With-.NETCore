using EStore.Models;
using EStore.Models.Basket;
using EStore.Models.Basket;
using EStore.Models.Layout;
using EStore.Models.Products;
using EStore.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EStore.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {
    }
    public DbSet<HomePageLayout> HomePageLayouts { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<VariantOption> VariantOptions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<UserOrder> Orders { get; set; }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<SelectedVariant> SelectedVariants { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(action: Console.WriteLine,
            minimumLevel: LogLevel.Information);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CartItem>()
            .HasMany(ci => ci.SelectedVariants)
            .WithOne()
            .HasForeignKey("CartItemId");
        // Configure decimal properties for precision and scale
        modelBuilder.Entity<Cart>()
            .Property(c => c.CartTotal)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<CartItem>()
            .Property(ci => ci.SubTotal)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Discount>()
            .Property(d => d.DiscountPrice)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<UserOrder>()
            .Property(o => o.Total)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<VariantOption>()
            .Property(v => v.PriceAdjustment)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<SelectedVariant>()
            .Property(sv => sv.PriceAdjustment)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18, 2)");
    }
}
