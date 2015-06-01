PrettyDump
===============
Dumps the table in a pretty format to console. You can choose the header and row colors however the default is 'green' for header and 'white' for rows as shown in [Example #6](https://github.com/sudipto80/Squirrel/edit/master/ScreenCastDemos/example-06.md)

```csharp
        /// <summary>
        /// Dumps the table in a pretty format to console.
        /// </summary>
        /// <param name="tab">The table to be dumped.</param>
        /// <param name="headerColor">The header foreground color</param>
        /// <param name="rowColor">The row color</param>
        /// <param name="header">The header for the table</param>
        /// <param name="align">The alignment. Possible values are left or right</param>
        /// <example>tab.PrettyDump();//The default dump </example>
        /// <example>tab.PrettyDump(header:"Sales Report");//dumping the table with a header</example>
        /// <example>tab.PrettyDump(header:"Sales Report", align:Alignment.Left);//Right alignment is default</example>        
        public static void PrettyDump(
             this Table tab, 
             ConsoleColor headerColor = ConsoleColor.Green, 
             ConsoleColor rowColor = ConsoleColor.White,
             string header = "None", 
             Alignment align = Alignment.Right)
```
