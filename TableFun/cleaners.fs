namespace Squirrel.Cleansing.FSharp
open Squirrel
open Squirrel.Cleansing
open System

module Table =
     let distinct (x:Table) = 
         x.Distinct()

     let convertOnesAndZerosToBoolean(column:string)(tab:Table) = 
         tab.ConvertOnesAndZerosToBoolean(column)

     let removeRowsWithImpossibleValues(columnName:string)(possibleValues:string array)(tab : Table) = 
         tab.RemoveRowsWithImpossibleValues(columnName,possibleValues)

     let removeIncompleteRows(tab:Table)=
        tab.RemoveIncompleteRows()

     let transform(tab:Table)(columnName:string)(transformer:System.Func<string,string>)=
        tab.Transform(columnName,transformer) 

     let removeIfAfter(tab:Table)(columnName:string)(date:DateTime) = 
         tab.RemoveIfAfter(columnName,date)
    
     let removeIfBefore(tab:Table)(columnName:string)(date:DateTime) = 
         tab.RemoveIfAfter(columnName,date)

     let removeIfBetween(tab:Table)(columnName:string)(startDate:DateTime)(endDate:DateTime) = 
         tab.RemoveIfBetween(columnName,startDate,endDate)

     