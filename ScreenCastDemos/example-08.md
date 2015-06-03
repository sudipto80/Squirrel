Baby name popularity
--------------------------

<img src="http://www.babypregnancycare.com/wp-content/uploads/2013/04/Baby-Names.jpg" border="0"/>

What are you going to name your newborn ? Are you going to rely on social media to suggest a name for your child? or are you going to lookup the history of baby names. We recommend the latter. Here is a file that lists all the baby names along with their popularity peercentage for the given year. 

|year|name|percent|sex|
-----|----|-------|---|
|1880|John|0.081541000 |boy|
|1880|William|0.080511000 |boy|
|1880|James|0.050057000 |boy|
|1880|Charles|0.045167000 |boy|
|1880|George|0.043292000 |boy|

***data*** for this example is available at ***../Data/baby-names.csv***

And here are few questions that I posed for myself

1. What is all time favorite "boy" name in history?
2. What is all time favorite "girl" name in history?
3. What are the top 10 boy names in 2008 (or in any particular year)
4. What are the top 10 girl names in 2008 (or in any particular year)

The following code answers all of these questions using Squirrel

```csharp

Table babyNames = DataAcquisition.LoadCSV(@"C:\Personal\Loose Files\baby-names.csv");
var splits = babyNames.SplitOn("year")["2008"]
                      .SplitOn("sex");            

            
var splitsBySex = babyNames.SplitOn("sex");
//All time favorite Boy name
splitsBySex["boy"]
             .Aggregate("name", AggregationMethod.Average)
             .Top(1)
             .PrettyDump(header:"All time favorite boy name");
//All time favorite girl name
splitsBySex["girl"]
             .Aggregate("name", AggregationMethod.Average)
             .Top(1)
             .PrettyDump(header: "All time favorite girl name");
    
//Most popular "girl" names in 2008;            
splits["girl"]
             .Drop("year")//Aggregating over the year doesn't make sense
             .Aggregate("name",AggregationMethod.Average)
             .SortBy("percent",how:SortDirection.Descending)
             .Top(10)
             .PrettyDump(header:"10 Most popular girl names in 2008",
                         align:Alignment.Left);

//Most popular "boy" names in 2008
splits["boy"]
            .Drop("year")//aggregating over the year doesn't make sense
            .Aggregate("name",AggregationMethod.Average)
            .SortBy("percent", how: SortDirection.Descending)
            .Top(10)
            .PrettyDump(header: "10 Most popular boy names in 2008", 
                       align: Alignment.Left);
```

This produces the following output

<img src="http://gifyu.com/images/baby.png" border="0">

add
--------------
Returns a new map with the binding added to the given map.
```
: 'Key -> 'T -> Map<'Key,'T> -> Map<'Key,'T>
```
```fsharp
Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
|> Map.add(0)  "zero" 
|> Map.iter ( fun  key value -> printfn  "key: %d value: %s"  key value)
 
```
key: 0 value: zero
key: 1 value: one
key: 2 value: two
key: 3 value: three 
```

 
containsKey
--------------
Tests if an element is in the domain of the map.
```
: 'Key -> Map<'Key,'T> -> bool
```
```fsharp
let  map1 = Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
 let  findKeyAndPrint key map =
     if  (Map.containsKey key map)  then 
        printfn  "The specified map contains the key %d."  key
     else 
        printfn  "The specified map does not contain the key %d."  key
findKeyAndPrint 1 map1
findKeyAndPrint 0 map1
 
```
The specified map contains the key 1.
The specified map does not contain the key 0. 
```
 
