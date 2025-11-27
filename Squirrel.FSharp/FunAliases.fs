namespace Squirrel.FSharp

open Squirrel


module TableFunAliases = 
    let ``pick these columns`` = Table.pick
    let ``load from this csv`` = DataAcquisition.LoadCsv 
    
    let ``aggregate numeric columns by sum and pick`` : string -> Table -> Table = Table.aggregate AggregationMethod.Sum

    let ``round off all decimal places to `` = Table.roundOffTo
    
    let ``add column with the formula`` = Table.addColumn
    
    let ``pretty print to console`` = Table.prettyDump