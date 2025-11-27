Filter
==========
Filter helps find out the values sought from different columns of the table. 

```Filter``` has 4 overloaded versions

Here are different overload signatures of the method

```csharp
Table Filter(Func<Dictionary<string,string>,bool> predicate)
```
```csharp
Table Filter(Dictionary<string,List<string>> fieldSearchValuesMap)
```
```csharp
Table Filter(Dictionary<string,string> _fieldSearchValueMap)
```
```csharp
Table Filter(string column , string[] values)
```
