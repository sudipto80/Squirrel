namespace IvyFashion;

public class FashionProduct
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;

    public FashionProduct(int userId, int productId, string productName, string brand, string category, decimal price,
        double rating, string color, string size)
    {
        UserId = userId;
        ProductId = productId;
        ProductName = productName;
        Brand = brand;
        Category = category;
        Price = price;
        Rating = rating;
        Color = color;
        Size = size;
    }
}