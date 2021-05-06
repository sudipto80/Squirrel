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

    let randomSample(sampleSize:int)(x:Table) = x.RandomSample(sampleSize)

    let top(n:int) (x:Table) = x.Top(n)

    let bottom(n:int) (x:Table) = x.Bottom(n)

    let middle(skip:int)(take:int) (x:Table) = x.Middle(skip,take)

    let topNPercent(n:int) (x:Table) = x.TopNPercent(n)

    let bottmNPercent(n:int) (x:Table) = x.BottomNPercent(n)
    
    let shuffle(x:Table) = x.Shuffle()

    let common(anotherTable:Table)(x:Table) = x.Common(anotherTable)

    let isSubSet(anotherTable:Table)(x:Table) = x.IsSubset(anotherTable)
    
    let modifyColumnName(oldName:string)(newName:string)(x:Table) = x.ModifyColumnName(oldName,newName)
    
    let addRowsInPlace(formula:string)(count:int) (x:Table) = x.AddRows(formula,count)

    let addRows(formula:string)(count:int) (thisTable:Table) = 
        thisTable.AddRows(formula,count)
        thisTable 

    let removeColumn(columnName:string)(thisTable:Table) = 
        thisTable.RemoveColumn(columnName)
        thisTable

