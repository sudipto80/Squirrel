Aggregate
====
```Aggregate``` has two overloaded versions and it is used to flatten a given table as per the given scheme. 
```csharp
public Table Aggregate(string columnName, AggregationMethod how = AggregationMethod.Sum)
```

```Aggregate``` does flatten a table as per the given rule. It takes has two parameters. The first one is the name of the non-numeric column that has to be present in the final result and a scheme depicted as an enum that will be used to flatten the table.

Consider you have the following table.

<img src="http://gifyu.com/images/iris.gif" border="0">

And you want to find range of values for all the fields. Since there is only one non-numeric field, that's the obvious choice to be used for flattening the table. So basically if you want to find out the range for each numeric columns like "SepalLength", "PetalLength" and such you can use the following call to ```Aggregate``` 

```csharp
iris.Aggregate("Name",AggregationMethod.Range)
    .PrettyDump();
```

There following Aggregation schemes are now supported
```csharp
   public enum AggregationMethod 
    {
        /// <summary>
        /// Summation of the sequence
        /// </summary>
        [Description("by summing")]
        Sum, 
        /// <summary>
        /// Average of the sequence
        /// </summary>
        [Description("by average,by mean")]        
        Average, 
        /// <summary>
        /// Maximum value of the sequence
        /// </summary>
        [Description("by max,by maximum value")]
        Max,
        /// <summary>
        /// Minimum value of the sequence
        /// </summary>
        [Description("by min,by minimum value")]
        Min, 
        /// <summary>
        /// Total count of the sequence
        /// </summary>
        [Description("by number of entries,by count")]
        Count,
        /// <summary>
        /// Standard deviation of the sequence      
        /// </summary>
        [Description("by standard deviation")]        
        StandardDeviation, 
        /// <summary>
        /// Calculates the variance of the sequence 
        /// </summary>
        [Description("by variance")]
        Variance, 
        /// <summary>
        /// Number of instance above average
        /// </summary>
        [Description("more than average")]
        AboveAverageCount,  
        /// <summary>
        /// Number of instances which are below the average value
        /// </summary>
        [Description("less than average")]
        BelowAverageCount,
        /// <summary>
        /// Number of instances that has the value equal to average
        /// </summary>
        [Description("just average,average")]
        AverageCount,
        /// <summary>
        /// Measures the skewness of the given sequence
        /// </summary>
        [Description("skewed")]
        Skew, 
        /// <summary>
        /// Measures the Kurtosis of the given sequence
        /// </summary>
        [Description("kurtosis")]       
        Kurtosis, 
        /// <summary>
        /// Measures the range of values of the given sequence.
        /// </summary>
        [Description("by range")]
        Range 
    }
```

The default scheme is to sum all the values for all the numeric columns. 

```Aggregate ``` has another overloaded version that takes a user defined scheme instead of pre-defined ones from the AggregationMethod enum. Here is how that overload looks 

```csharp
public Table Aggregate(string columnName, Func<List<string>, string> how)
```
Using this version users can provide the a flattening scheme. 
