
//Home for all Table functionalities that 
//have a default value for one or more of the arguments
namespace Squirrel.FSharp.Defaults
open Squirrel


module Table =

//When no argument is provided to Aggregate it uses "Sum" 
//For that we needed this function 
let aggregate(columnName:string)(x:Table)=
      x.Aggregate(columnName,AggregationMethod.Sum)

