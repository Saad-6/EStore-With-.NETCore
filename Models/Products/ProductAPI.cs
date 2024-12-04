namespace EStore.Models.Products;

public class ProductAPI : Product
{
    public IFormFile PrimaryImageFile { get; set; }
    public List<IFormFile> AdditionalImageFiles { get; set; }
}
