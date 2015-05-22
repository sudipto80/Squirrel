Example #5 (How much money will he accumulate at retirment?)
------
Imagine a person wants to save for his retirement. He decided to invest 300000 INR for the first year and then increase his yearly contribution 5% year over year. Let's also assume that he magically identified an investment scheme that gives him 15% return year over year. He is a proud father of a 1 year old son. He is 30 years old now in 2015. He wants to know during the last 10 years of his retirement what he could possiblly accumulate should everything remains the same. 

The following code using Squirrel framework solves this problem, 
```csharp
//How much money will someone accumulate if he invests 300000 per year 
//And increase that investment by 5% each year 
//and let his total investment grow at a rate of 15% year on year
Table book = new Table();
Dictionary<string, string> initalInvestment = new Dictionary<string, string>(); 
initalInvestment.Add("Year", "2015");
//Assume that his current age is 30 years
initalInvestment.Add("Age", "30");       
//Let's assume he has a son of 1 year age
initalInvestment.Add("SonsAge", "1");
initalInvestment.Add("YearlyContribution", "300000");
initalInvestment.Add("Investment", "300000");//Invest 300000 INR every year.
book.AddRow(initalInvestment);
book.AddRows("Year[1]=Year[0]+1", 30);
//The above line can also be written as 
//book.AddRowsByShortHand("Year++",30);//as done for "Age" field
//he will be 30 year older in 30 years to go
//By then I will be 30 years older. retiring at the age of 60.
book.AddRowsByShortHand("Age++", 30);
book.AddRowsByShortHand("SonsAge++", 30);//his son will be 30 year older
//increase that investment by 5% each year
book.AddRows("YearlyContribution[1]=Round(1.05*YearlyContribution[0],2)", 30);
//This is the equation I will like to use for calculating the investment value 
//This formula involves multiple columns 
book.AddRows("Investment[1] = 1.15 * Investment[0] + YearlyContribution[0]", 30); 
//Transform the column of Investment value to the nearest 1 Crore of INR.
book = book.Transform("Investment", x => Math.Round(Convert.ToDecimal( x)/10000000,4) + " Cr");
string table = book
      .Bottom(10)//Showing records for last 10 years of his retirement
      //for a feel good factor!
      //Chances are that he will retire rich!        
       .ToBasicBootstrapHTMLTable(BootstrapTableDecorators.BootstrapTableClasses
       .Table_Striped);
StreamWriter sw = new StreamWriter("temp.htm");
sw.WriteLine(table);
sw.Close();

System.Diagnostics.Process.Start("temp.htm");
```

This produces the following output. Cr meanse "Crores" . 1 Crore = 10 million

<img src="http://gifyu.com/images/retirement.png" border="0">
