add
--------------
Returns a new map with the binding added to the given map.
```
: 'Key -> 'T -> Map<'Key,'T> -> Map<'Key,'T>
```
Example
```fsharp
Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
|> Map.add(0)  "zero" 
|> Map.iter ( fun  key value -> printfn  "key: %d value: %s"  key value)
 ```
Output
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
Example
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
Output
```
The specified map contains the key 1.
The specified map does not contain the key 0. 
```

 
exists
--------------
Returns  true  if the given predicate returns true for one of the bindings in the map.
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> bool
```
Example
```fsharp
```
Output
```

// Signature:
Map.exists : ('Key -> 'T -> bool) -> Map  -> bool (requires comparison)

// Usage:
Map.exists predicate table
 
```

 
filter
--------------
Creates a new map containing only the bindings for which the given predicate returns  true .
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> Map<'Key,'T>
```
Example
```fsharp
printfn  "Even numbers and their squares."  
 let  map1 = Map.ofList [ for  i  in  1 .. 10 -> (i, i*i)]
           |> Map.filter ( fun  key _ -> key % 2 = 0)
           |> Map.iter ( fun  key value -> printf  "(%d, %d) "  key value)
printfn  "" 
 ```
Output
```
Even numbers and their squares.
(2, 4) (4, 16) (6, 36) (8, 64) (10, 100)  
```

 
find
--------------
Looks up an element in the map.
```
: 'Key -> Map<'Key,'T> -> 'T
```
Example
```fsharp
```
Output
```

```

 
findKey
--------------
Evaluates the function on each mapping in the collection. Returns the key for the first mapping where the function returns  true .
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> 'Key
```
Example
```fsharp
let  findKeyFromValue findValue map =
    printfn  "With value %A, found key %A."  findValue (Map.findKey ( fun  key value -> value = findValue) map)
 let  map1 = Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
 let  map2 = Map.ofList [  for  i  in  1 .. 10 -> (i, i*i) ]
 try 
    findKeyFromValue  "one"  map1
    findKeyFromValue  "two"  map1
    findKeyFromValue 9 map2
    findKeyFromValue 25 map2
     // The key is not in the map, so the following line throws an exception. 
    findKeyFromValue 0 map2
 with 
     :? System.Collections.Generic.KeyNotFoundException  as  e -> printfn  "%s"  e.Message
 ```
Output
```
With value "one", found key 1.
With value "two", found key 2.
With value 9, found key 3.
With value 25, found key 5.
Exception of type 'System.Collections.Generic.KeyNotFoundException' was thrown. 
```

 
fold
--------------
Folds over the bindings in the map
```
: ('State -> 'Key -> 'T -> 'State) -> 'State -> Map<'Key,'T> -> 'State
```
Example
```fsharp
let  map1 = Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
 // Sum the keys.  
 let  result1 = Map.fold ( fun  state key value -> state + key) 0 map1
printfn  "Result: %d"  result1
 // Concatenate the values.  
 let  result2 = Map.fold ( fun  state key value -> state + value +  " " )  ""  map1
printfn  "Result: %s"  result2
 ```
Output
```
Result: 6
Result: one two three  
```

 
foldBack
--------------
Folds over the bindings in the map.
```
: ('Key -> 'T -> 'State -> 'State) -> Map<'Key,'T> -> 'State -> 'State
```
Example
```fsharp
let  map1 = Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
 // Sum the keys.  
 let  result1 = Map.foldBack ( fun  key value state -> state + key) map1 0
printfn  "Result: %d"  result1
 // Concatenate the values.  
 let  result2 = Map.foldBack ( fun  key value state -> state + value +  " " ) map1  "" 
printfn  "Result: %s"  result2 
 ```
Output
```
Result: 6
Result: three two one 
```

 
forall
--------------
Returns true if the given predicate returns true for all of the bindings in the map.
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> bool
```
Example
```fsharp
let  map1 = Map.ofList [ (1,  "one" ); (2,  "two" ); (3,  "three" ) ]
 let  map2 = Map.ofList [ (-1,  "negative one" ); (2,  "two" ); (3,  "three" ) ]
 let  allPositive = Map.forall ( fun  key value -> key > 0)
printfn  "%b %b"  (allPositive map1) (allPositive map2)
 ```
Output
```
true false 
```

 
isEmpty
--------------
Tests whether the map has any bindings.
```
: Map<'Key,'T> -> bool
```
Example
```fsharp
```
Output
```

// Signature:
Map.isEmpty : Map  -> bool (requires comparison)

// Usage:
Map.isEmpty table
 
```

 
iter
--------------
Applies the given function to each binding in the dictionary
```
: ('Key -> 'T -> unit) -> Map<'Key,'T> -> unit
```
Example
```fsharp
```
Output
```

// Signature:
Map.iter : ('Key -> 'T -> unit) -> Map  -> unit (requires comparison)

// Usage:
Map.iter action table
 
```

 
map
--------------
Creates a new collection whose elements are the results of applying the given function to each of the elements of the collection. The key passed to the function indicates the key of element being transformed.
```
: ('Key -> 'T -> 'U) -> Map<'Key,'T> -> Map<'Key,'U>
```
Example
```fsharp
let  map1 = Map.ofList [ (1,  "One" ); (2,  "Two" ); (3,  "Three" ) ]
 let  map2 = map1 |> Map.map ( fun  key value -> value.ToUpper())
 let  map3 = map1 |> Map.map ( fun  key value -> value.ToLower())
printfn  "%A"  map1
printfn  "%A"  map2
printfn  "%A"  map3
 ```
Output
```
map [(1, "One"); (2, "Two"); (3, "Three")]
map [(1, "ONE"); (2, "TWO"); (3, "THREE")]
map [(1, "one"); (2, "two"); (3, "three")] 
```

 
ofArray
--------------
Returns a new map made from the given bindings.
```
: ('Key * 'T) [] -> Map<'Key,'T>
```
Example
```fsharp
```
Output
```

// Signature:
Map.ofArray : ('Key * 'T) [] -> Map  (requires comparison)

// Usage:
Map.ofArray elements
 
```

 
ofList
--------------
Returns a new map made from the given bindings.
```
: 'Key * 'T list -> Map<'Key,'T>
```
Example
```fsharp
```
Output
```

// Signature:
Map.ofList : 'Key * 'T list -> Map  (requires comparison)

// Usage:
Map.ofList elements
 
```

 
ofSeq
--------------
Returns a new map made from the given bindings.
```
: seq<'Key * 'T> -> Map<'Key,'T>
```
Example
```fsharp
```
Output
```

// Signature:
Map.ofSeq : seq  -> Map  (requires comparison)

// Usage:
Map.ofSeq elements
 
```

 
partition
--------------
Creates two new maps, one containing the bindings for which the given predicate returns  true , and the other the remaining bindings.
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> Map<'Key,'T> * Map<'Key,'T>
```
Example
```fsharp
let  map1 = [  for  i  in  1..10 -> (i, i*i)] |> Map.ofList
 let  (mapEven, mapOdd) = Map.partition ( fun  key value -> key % 2 = 0) map1
printfn  "Evens: %A"  mapEven
printfn  "Odds: %A"  mapOdd
 ```
Output
```
Evens: map [(2, 4); (4, 16); (6, 36); (8, 64); (10, 100)]
Odds: map [(1, 1); (3, 9); (5, 25); (7, 49); (9, 81)] 
```

 
pick
--------------
Searches the map looking for the first element where the given function returns a  Some  value
```
: ('Key -> 'T -> 'U option) -> Map<'Key,'T> -> 'U
```
Example
```fsharp
let  map1 = [  for  i  in  1 .. 100 -> (i, 100 - i) ] |> Map.ofList
 let  result = Map.pick ( fun  key value ->  if  key = value  then  Some(key)  else  None) map1
printfn  "Result where key and value are the same: %d"  result
 ```
Output
```
Result where key and value are the same: 50 
```

 
remove
--------------
Removes an element from the domain of the map. No exception is raised if the element is not present.
```
: 'Key -> Map<'Key,'T> -> Map<'Key,'T>
```
Example
```fsharp
```
Output
```

// Signature:
Map.remove : 'Key -> Map  -> Map  (requires comparison)

// Usage:
Map.remove key table
 
```

 
toArray
--------------
Returns an array of all key/value pairs in the mapping. The array will be ordered by the keys of the map.
```
: Map<'Key,'T> -> ('Key * 'T) []
```
Example
```fsharp
```
Output
```

// Signature:
Map.toArray : Map  -> ('Key * 'T) [] (requires comparison)

// Usage:
Map.toArray table
 
```

 
toList
--------------
Returns a list of all key/value pairs in the mapping. The list will be ordered by the keys of the map.
```
: Map<'Key,'T> -> ('Key * 'T) list
```
Example
```fsharp
```
Output
```

// Signature:
Map.toList : Map  -> ('Key * 'T) list (requires comparison)

// Usage:
Map.toList table
 
```

 
toSeq
--------------
Views the collection as an enumerable sequence of pairs. The sequence will be ordered by the keys of the map.
```
: Map<'Key,'T> -> seq<'Key * 'T>
```
Example
```fsharp
```
Output
```

// Signature:
Map.toSeq : Map  -> seq  (requires comparison)

// Usage:
Map.toSeq table
 
```

 
tryFind
--------------
Looks up an element in the map, returning a  Some  value if the element is in the domain of the map, or  None  if not.
```
: 'Key -> Map<'Key,'T> -> 'T option
```
Example
```fsharp
let  map1 = [  for  i  in  1 .. 100 -> (i, i*i) ] |> Map.ofList
 let  result = Map.tryFind 50 map1
 match  result  with 
| Some x -> printfn  "Found %d."  x
| None -> printfn  "Did not find the specified value." 
 ```
Output
```
Found 2500. 
```

 
tryFindKey
--------------
Returns the key of the first mapping in the collection that satisfies the given predicate, or returns  None  if no such element exists.
```
: ('Key -> 'T -> bool) -> Map<'Key,'T> -> 'Key option
```
Example
```fsharp
let  map1 = [  for  i  in  1 .. 100 -> (i, i*i) ] |> Map.ofList
 let  result = Map.tryFindKey ( fun  key value -> key = value) map1
 match  result  with 
| Some key -> printfn  "Found element with key %d."  key
| None -> printfn  "Did not find any element that matches the condition." 
 ```
Output
```
Found element with key 1. 
```

 
tryPick
--------------
Searches the map looking for the first element where the given function returns a  Some  value.
```
: ('Key -> 'T -> 'U option) -> Map<'Key,'T> -> 'U option
```
Example
```fsharp
let  map1 = [  for  i  in  1 .. 100 -> (i, 100 - i) ] |> Map.ofList
 let  result = Map.tryPick ( fun  key value ->  if  key = value  then  Some(key)  else  None) map1
 match  result  with 
| Some x -> printfn  "Result where key and value are the same: %d"  x
| None -> printfn  "No result satisifies the condition." 
 ```
Output
```
Result where key and value are the same: 50 
```
 
