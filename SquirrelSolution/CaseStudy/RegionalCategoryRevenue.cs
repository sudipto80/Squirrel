namespace CaseStudy;

public class RegionalCategoryRevenue
{
    public string Region { get; set; }
    public string Category { get; set; }
    
    public decimal Revenue { get; set; }

    public RegionalCategoryRevenue(string region, string category, decimal revenue)
    {
        Region = region;
        Category = category;
        Revenue = revenue;
    }
}