open Squirrel

let tips = DataAcquisition.LoadCsv("./tips.csv")

//Add a new column based on the formula
tips.AddColumn(columnName="tip%", formula="[tip]*100/[total_bill]", decimalDigits=3)

tips
    //Pick only these columns
    .Pick("sex", "tip%")
    //Aggregate the Tip% values by calculating the average
    .Aggregate("sex", AggregationMethod.Average)
    //Round off the result till 2 decimal points
    .RoundOffTo(2)
     //Dump the result to console.
    .PrettyDump()