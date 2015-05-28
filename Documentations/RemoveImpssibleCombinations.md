RemoveImpssibleCombinations
===============
Removes impossible combinations from data like gender:male,pregnant
For referrence visit : http://en.wikipedia.org/wiki/Data_pre-processing 

The method takes impossible combinations as a dictionary. Here is the method signature
```csharp
public static Table RemoveImpssibleCombinations(this Table tab, Dictionary<string,string> impossibleCombos)
```


Let's say you have the following table 

|Name|Gender|Pregnant|
|:---|:-----|:-----|
|Sam|M|Y|
|Jenny|F|Y|
|Gabriel|M|NA

Now sure enough "Sam" being a male can't be pregnant and that's a data error. Using ```RemoveImpossibleCombinations``` you can fix these sort of issues. 

The impossible combiations are wrapped inside the dictionary to be filled and used like this.
Let's say that the previous table is loaded in a table instance called ```tab```.

```csharp

Dictionary<string, string> impComb = new Dictionary<string, string>();
impComb.Add("Gender", "M");
impComb.Add("Pregnant", "Y");
tab = tab.RemoveImpossibleCombinations(impossibleCombos:impComb);
```

This will remove the row which has the impossible combination. So in this case tab will have the following rows


|Name|Gender|Pregnant|
|:---|:-----|:-----|
|Jenny|F|Y|
|Gabriel|M|NA
