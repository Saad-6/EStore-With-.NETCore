using EStore.Entities;
using LinqToDB;
using LinqToDB.Data;

namespace EStore.Data;

public class AltDataContext : DataConnection
{
    public AltDataContext(DataOptions options) : base(options) { }
    public ITable<LogEntity> Logs => this.GetTable<LogEntity>();
    public ITable<FAQEntity> FAQs => this.GetTable<FAQEntity>();
    public ITable<SEOEntity> SEOs => this.GetTable<SEOEntity>();
    public ITable<UserEntity> Users => this.GetTable<UserEntity>();
    public ITable<RoleEntity> Roles => this.GetTable<RoleEntity>();
    public ITable<OrderEntity> Orders => this.GetTable<OrderEntity>();
    public ITable<ReviewEntity> Reviews => this.GetTable<ReviewEntity>();
    public ITable<ProductEntity> Products => this.GetTable<ProductEntity>();
    public ITable<VariantEntity> Variants => this.GetTable<VariantEntity>();
    public ITable<AddressEntity> Addresses => this.GetTable<AddressEntity>();
    public ITable<DiscountEntity> Discounts => this.GetTable<DiscountEntity>();
    public ITable<CartItemEntity> CartItems => this.GetTable<CartItemEntity>();
    public ITable<CategoryEntity> Categories => this.GetTable<CategoryEntity>();
    public ITable<NewArrivalsEntity> NewArrivals => this.GetTable<NewArrivalsEntity>();
    public ITable<ProductImageEntity> ProductImages => this.GetTable<ProductImageEntity>();
    public ITable<VariantOptionEntity> VariantOptions => this.GetTable<VariantOptionEntity>();
    public ITable<HeroCarouselEntity> HomePageCarousels => this.GetTable<HeroCarouselEntity>();
    public ITable<HomePageLayoutEntity> HomePageLayouts => this.GetTable<HomePageLayoutEntity>();
    public ITable<SimpleCategoryEntity> HomePageCategories => this.GetTable<SimpleCategoryEntity>();
    public ITable<SelectedVariantEntity> SelectedVariants => this.GetTable<SelectedVariantEntity>();
    public ITable<FeauturedProductEntity> FeaturedProducts => this.GetTable<FeauturedProductEntity>();
    public ITable<HomePageSettingsEntity> HomePageSettings => this.GetTable<HomePageSettingsEntity>();
}
