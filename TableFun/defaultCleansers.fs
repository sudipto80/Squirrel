namespace Squirrel.Cleansing.Default.FSharp
open Squirrel
open Squirrel.Cleansing
open System

module Table =

      let extractOutliers(x:Table)(columnName:string)=
          x.ExtractOutliers(columnName)