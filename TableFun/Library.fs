namespace Squirrel.FSharp
open Squirrel
open System

module Table =
    //Wrapper for Pick() from Squirrel.Table
    let pick ([<ParamArray>] columns : string array) (x:Table) =
        x.Pick(columns)

    let drop ([<ParamArray>] columns : string array) (x:Table) =
        x.Drop(columns)

    let addColumn(columnName:string)(formula:string)(decimalDigits:int)(x:Table)=
        x.AddColumn(columnName,formula,decimalDigits)

    let aggregate(columnName:string)(how:AggregationMethod)(x:Table)=
        x.Aggregate(columnName,how)

    let roundOffTo(decimalDigits:int)(x:Table) = x.RoundOffTo(decimalDigits)

    let prettyDump (x:Table) = x.PrettyDump()
