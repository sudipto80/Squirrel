using System.Data;
using Squirrel;
using Squirrel.Cleansing;

namespace CaseStudy;

class Program
{
    static void Main(string[] args)
    {
        //Question 1: Regional Performance Analysis
        // "Which region generates the highest revenue,
        // and what are the top 3 product categories in each region?"

        var ecommerceData =
            DataAcquisition.LoadCsv(
                @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelSolution/CaseStudy/ecommerce_sales.csv");

        var regions = ecommerceData.SplitOn("Region");

        
        
        
        
        
        
        
        regions.ToList()
            .ForEach(t => t.Value.AddColumn("Revenue", formula: "[Unit_Price] * [Quantity]", decimalDigits: 2));

        
        
        
        
        
        
        
        var regionwiseSales = regions
            .Select(t => new
            {
                Region = t.Key,
                Revenue = t.Value["Revenue"].Select(Convert.ToDecimal).Sum()
            })
            .ToTableFromAnonList()
            .SortBy("Revenue", how: SortDirection.Descending);

        regionwiseSales.PrettyDump(header: "Regionwise Sales", rowColor: ConsoleColor.Blue);


        //Top product categories by revenue in each region
        var topCatPerRegion = regions.Select(regionGroup => new
            {
                Region = regionGroup.Key,
                Categories = regionGroup.Value
                    .SplitOn("Product_Category")
                    .Select(categoryGroup => new
                    {
                        Category = categoryGroup.Key,
                        Revenue = categoryGroup.Value["Revenue"]
                            .Select(Convert.ToDecimal)
                            .Sum()
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(3)
                    .Select(x => $"{x.Category} (${x.Revenue:N2})")
                    .Aggregate((a, b) => a + " | " + b)
            })
            .ToTableFromAnonList()
            .Explode("Categories", delim: '|');


        topCatPerRegion.AddColumn("Category",
            topCatPerRegion["Categories"].Select(x => x.Substring(0, x.IndexOf('('))).ToList());
        topCatPerRegion.AddColumn("Revenue",
            topCatPerRegion["Categories"]
                .Select(x =>
                    x.Substring(x.IndexOf('(') + 1, x.IndexOf(')')
                                                    - x.IndexOf('(') - 1)).ToList());
        //topCatPerRegion = topCatPerRegion.Drop("Categories");
        topCatPerRegion.Explode("Categories", delim: '|')
            .PrettyDump(header: "Top 3 Product Categories by Revenue in Each Region",
                rowColor: ConsoleColor.Magenta, align: Alignment.Left);

        Console.WriteLine("CASE 2");
        var sqlTab = RecordTable<RegionalCategoryRevenue>.FromTable(topCatPerRegion).ToSqlTable();

        var createScript = sqlTab.CreateTableScript;
        var insertScript = sqlTab.RowInsertCommands;
        //


        //"Compare purchasing patterns between Premium and Standard customers.
        // What's the average order value for each segment,
        // and which payment methods do they prefer?"
        // Squirrel features showcased: Filter, SplitOn, grouping, multiple aggregations


        var customers = ecommerceData.SplitOn("Customer_Segment");

        customers.ToList().ForEach(t =>
            t.Value.AddColumn("Order_Value", formula: "[Unit_Price] * [Quantity]", decimalDigits: 2));
        var custSect = customers.Select(t => new
            {
                Segment = t.Key,
                OrderValue = Math.Round(t.Value["Order_Value"].Select(Convert.ToDecimal).Average(), 2)
            })
            .ToTableFromAnonList();
        custSect.SortBy("OrderValue", how: SortDirection.Descending);

        // Part 2: Payment Method Preferences WITH PERCENTAGES
        var preferredPaymentMethods = customers
            .Select(segmentGroup => new
            {
                Segment = segmentGroup.Key,
                //TotalOrders = segmentGroup.Value.RowCount,
                PaymentMethods = segmentGroup.Value
                    .SplitOn("Payment_Method")
                    .Select(paymentGroup => new
                    {
                        PaymentMethod = paymentGroup.Key,
                        Count = paymentGroup.Value.RowCount,
                        Percentage = Math.Round(
                            (paymentGroup.Value.RowCount * 100.0) / segmentGroup.Value.RowCount,
                            1)
                    })
                    .OrderByDescending(x => x.Count)
                    .Select(x => $"{x.PaymentMethod} ({x.Percentage}%)")
                    .Aggregate((a, b) => a + ", " + b)
            })

            .ToTableFromAnonList()
            .Explode("PaymentMethods", delim: ',');
        custSect.PrettyDump(header: "Customer Group", rowColor: ConsoleColor.Blue);
        preferredPaymentMethods.PrettyDump(header: "Preferred Payment Methods", rowColor: ConsoleColor.Magenta);
        ;


        //═══════════════════════════════════════════════════════════════════════════
        // Question 3: Time-Based Trend Analysis
        // "Show monthly sales trends. Which month had the highest revenue, 
        //  and how many unique customers made purchases each month?"
        //═══════════════════════════════════════════════════════════════════════════

        // Add Month column for grouping
        ecommerceData.AddColumn("Month",
            ecommerceData["Order_Date"]
                .Select(x => DateTime.Parse(x).ToString("MMM"))
                .ToList());



        // Part A: Monthly Revenue Trends
        var monthlySales = ecommerceData
            .SplitOn("Month")
            .Select(monthGroup => new
            {
                Month = monthGroup.Key,
                Revenue = monthGroup.Value["Revenue"]
                    .Select(Convert.ToDecimal)
                    .Sum(),
                OrderCount = monthGroup.Value.RowCount
            })
            .ToTableFromAnonList()
            .SortBy("Revenue", how: SortDirection.Descending);

        Console.WriteLine("\n📈 MONTHLY SALES TRENDS\n");
        monthlySales.PrettyDump(
            header: "Revenue by Month (Highest to Lowest)",
            rowColor: ConsoleColor.Blue);

        // Part B: Unique Customers Per Month
        var uniqueCustomers = ecommerceData
            .SplitOn("Month")
            .Select(monthGroup => new
            {
                Month = monthGroup.Key,
                UniqueCustomers = monthGroup.Value["Customer_ID"].Distinct().Count(),
                TotalOrders = monthGroup.Value.RowCount,
                AvgOrdersPerCustomer = Math.Round(
                    monthGroup.Value.RowCount * 1.0 /
                    monthGroup.Value["Customer_ID"].Distinct().Count(), 1)
            })
            .ToTableFromAnonList()
            .SortBy("UniqueCustomers", how: SortDirection.Descending);

        Console.WriteLine("\n👥 CUSTOMER ENGAGEMENT BY MONTH\n");
        uniqueCustomers.PrettyDump(
            header: "Unique Customers Per Month",
            rowColor: ConsoleColor.Magenta);

        // Key Insights
        Console.WriteLine("\n🔍 KEY INSIGHTS:");
        var topMonth = monthlySales[0]["Month"];
        var topRevenue = monthlySales[0]["Revenue"];
        var mostActiveMonth = uniqueCustomers[0]["Month"];
        var mostCustomers = uniqueCustomers[0]["UniqueCustomers"];

        Console.WriteLine($"• Highest revenue month: {topMonth} (${topRevenue})");
        Console.WriteLine($"• Most active customer month: {mostActiveMonth} ({mostCustomers} unique customers)");

        //═══════════════════════════════════════════════════════════════════════════
        // Question 4: Product Category Deep Dive
        // "For the Electronics category, find the top 5 best-selling products by 
        //  revenue. Then identify which customers purchased multiple electronics items."
        //═══════════════════════════════════════════════════════════════════════════

        // Ensure Revenue column exists
        if (!ecommerceData.ColumnHeaders.Contains("Revenue"))
        {
            ecommerceData.AddColumn("Revenue",
                formula: "[Unit_Price] * [Quantity]",
                decimalDigits: 2);
        }

        // Part A: Top 5 Best-Selling Electronics Products by Revenue
        var electronicsProducts = ecommerceData
            .Filter("Product_Category", "Electronics")
            .SplitOn("Product_Name")
            .Select(productGroup => new
            {
                Product = productGroup.Key,
                TotalRevenue = productGroup.Value["Revenue"]
                    .Select(Convert.ToDecimal)
                    .Sum(),
                UnitsSold = productGroup.Value["Quantity"]
                    .Select(Convert.ToDecimal)
                    .Sum(),
                OrderCount = productGroup.Value.RowCount
            })
            .ToTableFromAnonList()
            .SortBy("TotalRevenue", how: SortDirection.Descending)
            .Top(5);

        Console.WriteLine("\n💻 TOP 5 ELECTRONICS PRODUCTS\n");
        electronicsProducts.PrettyDump(
            header: "Best-Selling Electronics by Revenue",
            rowColor: ConsoleColor.Blue);

// Part B: Customers Who Purchased Multiple Electronics Items
        var electronicsOnly = ecommerceData.Filter("Product_Category", "Electronics");

        var customerPurchases = electronicsOnly
            .SplitOn("Customer_ID")
            .Select(customerGroup => new
            {
                CustomerID = customerGroup.Key,
                CustomerName = customerGroup.Value["Customer_Name"].First(),
                ItemsPurchased = customerGroup.Value.RowCount,
                TotalSpent = customerGroup.Value["Revenue"]
                    .Select(Convert.ToDecimal)
                    .Sum(),
                Products = customerGroup.Value["Product_Name"]
                    .Distinct()
                    .Aggregate((a, b) => a + ", " + b)
            })
            .Where(x => x.ItemsPurchased > 1) // Filter for multiple purchases
            .OrderByDescending(x => x.ItemsPurchased)
            .ToTableFromAnonList();

        Console.WriteLine("\n🛒 REPEAT ELECTRONICS BUYERS\n");
        customerPurchases.PrettyDump(
            header: "Customers with Multiple Electronics Purchases",
            rowColor: ConsoleColor.Magenta);

// Part C: Detailed Purchase History for Repeat Buyers (using set operations)
        if (customerPurchases.RowCount > 0)
        {
            Console.WriteLine("\n📋 DETAILED PURCHASE HISTORY\n");

            var repeatBuyerIDs = customerPurchases["CustomerID"].ToList();

            var detailedHistory = electronicsOnly
                .Filter(row => repeatBuyerIDs.Contains(row["Customer_ID"]))
                .Pick("Customer_Name", "Product_Name", "Order_Date", "Revenue")
                .SortBy("Customer_Name");

            detailedHistory.PrettyDump(
                header: "Order History - Repeat Electronics Buyers",
                rowColor: ConsoleColor.Cyan);
        }

        // Key Insights
        Console.WriteLine("\n🔍 KEY INSIGHTS:");
        var topProduct = electronicsProducts[0]["Product"];
        var topRevenue4 = electronicsProducts[0]["TotalRevenue"];
        var repeatBuyerCount = customerPurchases.RowCount;

        Console.WriteLine($"• Top electronics product: {topProduct} (${topRevenue4} revenue)");
        Console.WriteLine($"• {repeatBuyerCount} customers purchased multiple electronics items");
        if (customerPurchases.RowCount > 0)
        {
            var mostLoyal = customerPurchases[0]["CustomerName"];
            var itemCount = customerPurchases[0]["ItemsPurchased"];
            Console.WriteLine($"• Most active buyer: {mostLoyal} ({itemCount} electronics orders)");
        }

        //═══════════════════════════════════════════════════════════════════════════
// Question 5: Geographic Expansion Strategy
// "Identify countries with only 1-2 orders (emerging markets). 
//  For these countries, what product categories were purchased, 
//  and what's the total potential if we extrapolate based on 
//  established market averages?"
//═══════════════════════════════════════════════════════════════════════════

// Ensure Revenue column exists
        if (!ecommerceData.ColumnHeaders.Contains("Revenue"))
        {
            ecommerceData.AddColumn("Revenue",
                formula: "[Unit_Price] * [Quantity]",
                decimalDigits: 2);
        }

// Part A: Identify Emerging vs Established Markets
        var marketsByCountry = ecommerceData
            .SplitOn("Country")
            .Select(countryGroup => new
            {
                Country = countryGroup.Key,
                OrderCount = countryGroup.Value.RowCount,
                TotalRevenue = countryGroup.Value["Revenue"]
                    .Select(Convert.ToDecimal)
                    .Sum(),
                UniqueCustomers = countryGroup.Value["Customer_ID"].Distinct().Count(),
                AvgOrderValue = Math.Round(
                    countryGroup.Value["Revenue"]
                        .Select(Convert.ToDecimal)
                        .Average(), 2),
                MarketType = countryGroup.Value.RowCount <= 2 ? "Emerging" : "Established"
            })
            .ToTableFromAnonList()
            .SortBy("OrderCount", how: SortDirection.Descending);

        Console.WriteLine("\n🌍 MARKET CLASSIFICATION\n");
        marketsByCountry.PrettyDump(
            header: "Markets by Order Volume",
            rowColor: ConsoleColor.Blue);

// Part B: Focus on Emerging Markets (1-2 orders)
        var emergingMarkets = marketsByCountry
            .Filter(row => Convert.ToInt32(row["OrderCount"]) <= 2);

        Console.WriteLine("\n🚀 EMERGING MARKETS (≤2 orders)\n");
        emergingMarkets.PrettyDump(
            header: "Countries with Growth Potential",
            rowColor: ConsoleColor.Magenta);

// Part C: Product Categories Purchased in Emerging Markets
        var emergingCountries = emergingMarkets["Country"].ToList();

        var emergingMarketPurchases = ecommerceData
            .Filter(row => emergingCountries.Contains(row["Country"]))
            .SplitOn("Country")
            .Select(countryGroup => new
            {
                Country = countryGroup.Key,
                Categories = countryGroup.Value["Product_Category"]
                    .Distinct()
                    .Aggregate((a, b) => a + ", " + b),
                TopCategory = countryGroup.Value
                    .SplitOn("Product_Category")
                    .OrderByDescending(cat => cat.Value.RowCount)
                    .First().Key
            })
            .ToTableFromAnonList();

        Console.WriteLine("\n📦 PRODUCT PREFERENCES IN EMERGING MARKETS\n");
        emergingMarketPurchases.PrettyDump(
            header: "Categories Purchased by Emerging Markets",
            rowColor: ConsoleColor.Cyan);

// Part D: Calculate Established Market Benchmarks
        var establishedMarkets = marketsByCountry
            .Filter(row => row["MarketType"] == "Established");

        var establishedAvgRevenue = establishedMarkets["TotalRevenue"]
            .Select(Convert.ToDecimal)
            .Average();

        var establishedAvgOrders = establishedMarkets["OrderCount"]
            .Select(Convert.ToDecimal)
            .Average();

        Console.WriteLine("\n📊 ESTABLISHED MARKET BENCHMARKS\n");
        Console.WriteLine($"Average Orders per Country: {establishedAvgOrders:F1}");
        Console.WriteLine($"Average Revenue per Country: ${establishedAvgRevenue:N2}");

// Part E: Extrapolate Potential for Emerging Markets
        var potentialAnalysis = emergingMarkets.Rows
            .Select(row => new
            {
                Country = row["Country"],
                CurrentRevenue = Convert.ToDecimal(row["TotalRevenue"]),
                CurrentOrders = Convert.ToInt32(row["OrderCount"]),
                ProjectedOrders = Math.Round(establishedAvgOrders, 0),
                ProjectedRevenue = Math.Round(establishedAvgRevenue, 2),
                RevenueGap = Math.Round(establishedAvgRevenue -
                                        Convert.ToDecimal(row["TotalRevenue"]), 2),
                GrowthMultiplier = Math.Round(
                    establishedAvgRevenue / Convert.ToDecimal(row["TotalRevenue"]), 1)
            })
            .ToTableFromAnonList()
            .SortBy("RevenueGap", how: SortDirection.Descending);

        Console.WriteLine("\n💰 REVENUE POTENTIAL ANALYSIS\n");
        potentialAnalysis.PrettyDump(
            header: "Extrapolated Growth Opportunity",
            rowColor: ConsoleColor.Yellow);

// Part F: Total Opportunity Calculation
        var totalCurrentRevenue = emergingMarkets["TotalRevenue"]
            .Select(Convert.ToDecimal)
            .Sum();

        var totalProjectedRevenue = emergingMarkets.RowCount * establishedAvgRevenue;

        var totalOpportunity = totalProjectedRevenue - totalCurrentRevenue;

        Console.WriteLine("\n🎯 TOTAL EXPANSION OPPORTUNITY\n");
        Console.WriteLine($"Current Revenue (Emerging Markets): ${totalCurrentRevenue:N2}");
        Console.WriteLine($"Projected Revenue (at Established Avg): ${totalProjectedRevenue:N2}");
        Console.WriteLine($"Total Growth Opportunity: ${totalOpportunity:N2}");
        Console.WriteLine($"Potential ROI: {(totalOpportunity / totalCurrentRevenue * 100):F0}%");

        // Part G: Strategic Recommendations by Category
        var categoryPerformance = ecommerceData
            .Filter(row => !emergingCountries.Contains(row["Country"])) // Established markets
            .SplitOn("Product_Category")
            .Select(catGroup => new
            {
                Category = catGroup.Key,
                AvgRevenue = Math.Round(
                    catGroup.Value["Revenue"]
                        .Select(Convert.ToDecimal)
                        .Average(), 2),
                TotalOrders = catGroup.Value.RowCount
            })
            .OrderByDescending(x => x.AvgRevenue)
            .ToTableFromAnonList();

        Console.WriteLine("\n📈 CATEGORY PERFORMANCE (Established Markets)\n");
        categoryPerformance.PrettyDump(
            header: "Best-Performing Categories to Target",
            rowColor: ConsoleColor.Green);

        // Key Insights & Recommendations
        Console.WriteLine("\n🔍 STRATEGIC INSIGHTS:\n");
        Console.WriteLine($"• {emergingMarkets.RowCount} countries identified as emerging markets");
        Console.WriteLine($"• Combined growth opportunity: ${totalOpportunity:N2}");
        Console.WriteLine(
            $"• Average growth multiplier: {potentialAnalysis["GrowthMultiplier"].Select(Convert.ToDecimal).Average():F1}x");

        var topOpportunity = potentialAnalysis[0];
        Console.WriteLine($"\n🎯 TOP PRIORITY: {topOpportunity["Country"]}");
        Console.WriteLine(
            $"   Current: ${topOpportunity["CurrentRevenue"]} ({topOpportunity["CurrentOrders"]} orders)");
        Console.WriteLine(
            $"   Potential: ${topOpportunity["ProjectedRevenue"]} ({topOpportunity["ProjectedOrders"]} orders)");
        Console.WriteLine($"   Gap to close: ${topOpportunity["RevenueGap"]}");

        Console.WriteLine($"\n💡 RECOMMENDATION:");
        var topCategory = categoryPerformance[0]["Category"];
        Console.WriteLine($"   Focus marketing efforts on {topCategory} category");
        Console.WriteLine($"   which shows ${categoryPerformance[0]["AvgRevenue"]} avg revenue in established markets");


        //═══════════════════════════════════════════════════════════════════════════
// Case Study Question 7: Customer Loyalty & Retention Analysis
// 
// Business Questions:
// 1. Which customers bought in Q1 but NOT in Q2? (churn risk)
// 2. Which customers are consistent across quarters? (loyal customers)
// 3. Are Premium customers a subset of multi-category shoppers?
// 4. Which customers bought Electronics but nothing else? (upsell opportunity)
// 5. Compare payment method preferences across customer segments
//
// Squirrel Features: Common(), Exclusive(), IsSubset(), Filter(), SplitOn()
//═══════════════════════════════════════════════════════════════════════════

        

// Add Revenue column if not exists
        if (!ecommerceData.ColumnHeaders.Contains("Revenue"))
        {
            ecommerceData.AddColumn("Revenue",
                formula: "[Unit_Price] * [Quantity]",
                decimalDigits: 2);
        }

        Console.WriteLine("\n🔍 CUSTOMER LOYALTY & RETENTION ANALYSIS\n");
        Console.WriteLine(new string('═', 80));

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// PART 1: Quarterly Customer Retention (COMMON & EXCLUSIVE)
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n📅 QUARTERLY RETENTION ANALYSIS\n");

// Segment customers by quarter
        var q1Customers = ecommerceData
            .Filter(row =>
            {
                var date = DateTime.Parse(row["Order_Date"]);
                return date.Month >= 1 && date.Month <= 3;
            })
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var q2Customers = ecommerceData
            .Filter(row =>
            {
                var date = DateTime.Parse(row["Order_Date"]);
                return date.Month >= 4 && date.Month <= 6;
            })
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var q3Customers = ecommerceData
            .Filter(row =>
            {
                var date = DateTime.Parse(row["Order_Date"]);
                return date.Month >= 7 && date.Month <= 9;
            })
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        Console.WriteLine($"Q1 Customers: {q1Customers.RowCount}");
        Console.WriteLine($"Q2 Customers: {q2Customers.RowCount}");
        Console.WriteLine($"Q3 Customers: {q3Customers.RowCount}");

// Find customers who bought in Q1 but NOT in Q2 (Churn Risk!)
        var q1NotQ2 = q1Customers.Exclusive(q2Customers);

        Console.WriteLine("\n⚠️  CHURN RISK: Customers who bought in Q1 but NOT in Q2\n");
        q1NotQ2.PrettyDump(
            header: "At-Risk Customers (Q1 buyers who didn't return in Q2)",
            rowColor: ConsoleColor.Red);

// Find loyal customers (bought in BOTH Q1 AND Q2)
        var loyalCustomers = q1Customers.Common(q2Customers);

        Console.WriteLine("\n✅ LOYAL CUSTOMERS: Bought in both Q1 and Q2\n");
        loyalCustomers.PrettyDump(
            header: "Retained Customers",
            rowColor: ConsoleColor.Green);

// Calculate retention rate
        var retentionRate = (loyalCustomers.RowCount * 100.0) / q1Customers.RowCount;
        Console.WriteLine($"\n📊 Q1→Q2 Retention Rate: {retentionRate:F1}%");

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// PART 2: Cross-Category Shopping Patterns (COMMON & EXCLUSIVE)
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n\n🛍️  CROSS-CATEGORY SHOPPING PATTERNS\n");
        Console.WriteLine(new string('─', 80));

// Get customers by product category
        var electronicsCustomers = ecommerceData
            .Filter("Product_Category", "Electronics")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var clothingCustomers = ecommerceData
            .Filter("Product_Category", "Clothing")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var booksCustomers = ecommerceData
            .Filter("Product_Category", "Books")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var homeGardenCustomers = ecommerceData
            .Filter("Product_Category", "Home & Garden")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        Console.WriteLine("\n📊 Category Reach:");
        Console.WriteLine($"Electronics:    {electronicsCustomers.RowCount} customers");
        Console.WriteLine($"Clothing:       {clothingCustomers.RowCount} customers");
        Console.WriteLine($"Books:          {booksCustomers.RowCount} customers");
        Console.WriteLine($"Home & Garden:  {homeGardenCustomers.RowCount} customers");

// Find customers who bought Electronics AND Clothing (cross-sell success!)
        var electronicsAndClothing = electronicsCustomers.Common(clothingCustomers);

        Console.WriteLine("\n🎯 CROSS-SELL SUCCESS: Electronics + Clothing buyers\n");
        electronicsAndClothing.PrettyDump(
            header: "Customers Who Bought Both Electronics and Clothing",
            rowColor: ConsoleColor.Cyan);

// Find customers who bought Electronics but NOT Clothing (upsell opportunity!)
        var electronicsOnlyNotClothing = electronicsCustomers.Exclusive(clothingCustomers);

        Console.WriteLine("\n💡 UPSELL OPPORTUNITY: Electronics buyers who haven't bought Clothing\n");
        electronicsOnlyNotClothing.PrettyDump(
            header: "Target for Clothing Promotions",
            rowColor: ConsoleColor.Yellow);

// Find customers who bought ALL categories (super shoppers!)
        var electronicAndBooks = electronicsCustomers.Common(booksCustomers);
        var threeCategories = electronicAndBooks.Common(clothingCustomers);
        var superShoppers = threeCategories.Common(homeGardenCustomers);

        Console.WriteLine($"\n⭐ SUPER SHOPPERS: Bought from all 4 categories: {superShoppers.RowCount} customers\n");
        if (superShoppers.RowCount > 0)
        {
            superShoppers.PrettyDump(
                header: "VIP Multi-Category Shoppers",
                rowColor: ConsoleColor.Magenta);
        }

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// PART 3: Customer Segment Analysis (ISSUBSET)
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n\n👥 CUSTOMER SEGMENT VALIDATION\n");
        Console.WriteLine(new string('─', 80));

// Get Premium vs Standard customers
        var premiumCustomers = ecommerceData
            .Filter("Customer_Segment", "Premium")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var standardCustomers = ecommerceData
            .Filter("Customer_Segment", "Standard")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        Console.WriteLine($"\nPremium Customers:  {premiumCustomers.RowCount}");
        Console.WriteLine($"Standard Customers: {standardCustomers.RowCount}");

// Are all loyal customers Premium? (IsSubset check)
        var areLoyalCustomersPremium = loyalCustomers.IsSubset(premiumCustomers);

        Console.WriteLine($"\n🔍 Are all retained customers Premium? {areLoyalCustomersPremium}");

        if (!areLoyalCustomersPremium)
        {
            // Find which loyal customers are NOT Premium
            var loyalStandard = loyalCustomers.Exclusive(premiumCustomers);
            Console.WriteLine($"\n💎 UPGRADE OPPORTUNITY: {loyalStandard.RowCount} loyal Standard customers\n");
            loyalStandard.PrettyDump(
                header: "Loyal Standard Customers (Upgrade Targets)",
                rowColor: ConsoleColor.Blue);
        }

// Are all multi-category shoppers Premium?
        var areMultiCategoryShoppersPremium = electronicsAndClothing.IsSubset(premiumCustomers);

        Console.WriteLine($"\n🔍 Are all Electronics+Clothing buyers Premium? {areMultiCategoryShoppersPremium}");

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// PART 4: Payment Method Segmentation (COMMON on different criteria)
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n\n💳 PAYMENT METHOD ANALYSIS\n");
        Console.WriteLine(new string('─', 80));

// Get customers by payment method
        var creditCardCustomers = ecommerceData
            .Filter("Payment_Method", "Credit Card")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var paypalCustomers = ecommerceData
            .Filter("Payment_Method", "PayPal")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        var debitCardCustomers = ecommerceData
            .Filter("Payment_Method", "Debit Card")
            .Pick("Customer_ID", "Customer_Name")
            .Distinct();

        Console.WriteLine($"\nCredit Card users:  {creditCardCustomers.RowCount} customers");
        Console.WriteLine($"PayPal users:       {paypalCustomers.RowCount} customers");
        Console.WriteLine($"Debit Card users:   {debitCardCustomers.RowCount} customers");

// Find overlap between Premium customers and Credit Card users
        var premiumCreditCard = premiumCustomers.Common(creditCardCustomers);
        var premiumPayPal = premiumCustomers.Common(paypalCustomers);

        Console.WriteLine("\n📊 Premium Customer Payment Preferences:");
        Console.WriteLine($"Premium + Credit Card: {premiumCreditCard.RowCount} customers");
        Console.WriteLine($"Premium + PayPal:      {premiumPayPal.RowCount} customers");

// Are all Premium customers using Credit Card?
        var allPremiumUseCreditCard = premiumCustomers.IsSubset(creditCardCustomers);
        Console.WriteLine($"\n🔍 Do all Premium customers use Credit Card? {allPremiumUseCreditCard}");

        if (!allPremiumUseCreditCard)
        {
            var premiumNotCreditCard = premiumCustomers.Exclusive(creditCardCustomers);
            Console.WriteLine($"\n💡 {premiumNotCreditCard.RowCount} Premium customers NOT using Credit Card\n");
            premiumNotCreditCard.PrettyDump(
                header: "Premium Customers Without Credit Card",
                rowColor: ConsoleColor.Yellow);
        }

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// PART 5: Regional Retention Analysis
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n\n🌍 REGIONAL RETENTION PATTERNS\n");
        Console.WriteLine(new string('─', 80));

// Analyze retention by region
     
        foreach (var region in regions)
        {
            var regionQ1 = q1Customers
                .Rows
                .Where(row =>
                {
                    var customerId = row["Customer_ID"];
                    return ecommerceData
                        .Filter("Customer_ID", customerId)
                        .Filter("Region", region.Key)
                        .RowCount > 0;
                })
                .ToList();

            var regionQ2 = q2Customers
                .Rows
                .Where(row =>
                {
                    var customerId = row["Customer_ID"];
                    return ecommerceData
                        .Filter("Customer_ID", customerId)
                        .Filter("Region", region.Key)
                        .RowCount > 0;
                })
                .ToList();

            if (regionQ1.Count > 0)
            {
                var regionRetention = (regionQ2.Count * 100.0) / regionQ1.Count;
                Console.WriteLine(
                    $"{region,-20} Q1: {regionQ1.Count}  Q2: {regionQ2.Count}  Retention: {regionRetention:F1}%");
            }
        }

//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// SUMMARY: Actionable Insights
//━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        Console.WriteLine("\n" + new string('═', 80));
        Console.WriteLine("📋 KEY INSIGHTS & RECOMMENDED ACTIONS");
        Console.WriteLine(new string('═', 80) + "\n");

        Console.WriteLine("🎯 RETENTION:");
        Console.WriteLine($"   • Retention rate Q1→Q2: {retentionRate:F1}%");
        Console.WriteLine($"   • {q1NotQ2.RowCount} customers at churn risk");
        Console.WriteLine($"   → Action: Launch re-engagement campaign for at-risk customers\n");

        Console.WriteLine("🛍️  CROSS-SELL:");
        Console.WriteLine($"   • {electronicsAndClothing.RowCount} customers buy across Electronics + Clothing");
        Console.WriteLine($"   • {electronicsOnlyNotClothing.RowCount} Electronics-only customers (upsell target)");
        Console.WriteLine($"   → Action: Promote Clothing to Electronics buyers\n");

        if (superShoppers.RowCount > 0)
        {
            Console.WriteLine("⭐ VIP SEGMENT:");
            Console.WriteLine($"   • {superShoppers.RowCount} customers shop all categories");
            Console.WriteLine($"   → Action: Create VIP loyalty program for super shoppers\n");
        }

        if (!areLoyalCustomersPremium)
        {
            Console.WriteLine("💎 UPGRADE OPPORTUNITY:");
            Console.WriteLine($"   • {loyalCustomers.Exclusive(premiumCustomers).RowCount} loyal Standard customers");
            Console.WriteLine($"   → Action: Offer Premium upgrade with exclusive benefits\n");
        }

        Console.WriteLine(new string('═', 80) + "\n");
    }
}