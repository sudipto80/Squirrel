using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace Squirrel
{
    public static class Story
    {
        private static Dictionary<string, List<string>> _methodTagMap = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> _mapping = 
            new Dictionary<string, string>();
        public static List<string> ColumnNamesInvolved( Table tab, string nql)
        {
            return tab.ColumnHeaders.ToList();
            //List<string> cols = new List<string>();
            //cols = tab.ColumnHeaders.Select(t => t.ToLower()).Where(col => nql.Split(' ').Select(t => t.Trim()).Contains(col)).ToList();
            //string[] toks = nql.Split(' ');
            //foreach (var col in tab.ColumnHeaders)
            //    if (toks.Any(t => tab.ValuesOf(col).Contains(t)))
            //        cols.Add("columnName:" + col);
        //    return cols;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="nql"></param>
        /// <returns></returns>
        public static string MethodInvolved(Table tab, string nql)
        {
            //(new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Reflection.CustomAttributeTypedArgument>((new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Reflection.CustomAttributeData>(cus)).Items[0].m_typedCtorArgs as System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)).Items[0].Value
            List<MethodInfo> matchingMethods = typeof(BasicStatistics).GetMethods().Where(t => t.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0).ToList();
            List<CustomAttributeData> cus = matchingMethods[0].CustomAttributes.ToList();
            //cus.First( t=> nql.Contains(t.ConstructorArguments[0].Value.ToString()))
           return nql;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this System.Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        public static string ToChart(Table tab, string columnName)
        {
            return tab.ToPieByGoogleDataVisualization(columnName, "Distribution of " + columnName);
        }
        public static Table HandleIt(Table tab, string commnd, params string[] parameters)
        {
            switch (commnd)
            {
                case ".SortBy":
                case "[SortBy]":
                    return tab.SortBy(parameters[0]);
                case "[Range]":
                case ".Range":
                    return tab.Aggregate(parameters[0], AggregationMethod.Range);
                case "[Max]":
                case ".Max":
                    return tab.Aggregate(parameters[0],AggregationMethod.Max);
                case "[Min]":
                case ".Min":
                    return tab.Aggregate(parameters[0], AggregationMethod.Max);

                case "[Average]":
                case ".Average":
                    return tab.Aggregate(parameters[0], AggregationMethod.Average);
                case "[AboveAverageCount]":
                case ".AboveAverageCount":
                    return tab.Aggregate(parameters[0], AggregationMethod.AboveAverageCount);
                case ".LessThanAverage":
                    return tab.Aggregate(parameters[0], AggregationMethod.BelowAverageCount);
                
                case ".Pick":
                case "[Pick]":
                    return tab.Pick(parameters);
                 case ".Drop":
                    return tab.Drop(parameters);
                 
                default:
                    return tab;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="command"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Table HandleIt(Table tab, string command, int n)
        {
            switch (command)
            {
                case ".RandomSample":
                    return tab.RandomSample(n);
                case ".Top":
                    return tab.Top(n);
                case ".Bottom":
                    return tab.Bottom(n);
                    break;

                default:
                    return tab;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string[]> GetCommand(string command, Stack<string> values)
        {
            return new KeyValuePair<string, string[]>(command, values.Select(t => t).ToArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static KeyValuePair<string, int> GetCommand(string command, int n)
        {
            return new KeyValuePair<string, int>(command, n);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="storyScript"></param>
        /// <returns></returns>
        public static Table PseudoNaturalQuery(this Table tab,string storyScript)
        {
            _methodTagMap.Clear();
            Table result = tab;
            typeof(Table).GetMembers().Select(t =>
                {
                    try
                    {
                        return new KeyValuePair<string, List<string>>(t.Name, t.CustomAttributes.ElementAt(0).ConstructorArguments[0].Value.ToString().Split(',').ToList());
                    }
                    catch { return new KeyValuePair<string, List<string>>(Guid.NewGuid().ToString(), new List<string>()); }
                })
                .ToList()
                .ForEach(m => _methodTagMap.Add(m.Key, m.Value));

            _methodTagMap = _methodTagMap.Where(t => !Regex.Match(t.Key, "[a-zA-Z0-9-]{36}").Success).ToDictionary(t => t.Key, t => t.Value);

            Dictionary<string, string> trasformedMap = new Dictionary<string, string>();

            foreach (var m in _methodTagMap.Keys)
            {
                foreach (var z in _methodTagMap[m])
                {
                    try
                    {
                        trasformedMap.Add(z.ToLower(), m);
                    }
                    catch { }
                }
            }

           List<AggregationMethod> vals =  typeof(AggregationMethod).GetEnumValues().Cast<AggregationMethod>().ToList();
           foreach (var val in vals)
           {
               string desc = val.ToDescription();
               if (desc.Contains(","))
               {
                   string[] allDescriptions = desc.Split(',');
                   foreach (var z in allDescriptions)
                       trasformedMap.Add(z, val.ToString());
               }
               else
                   trasformedMap.Add(val.ToDescription(), val.ToString());

           }
            foreach (var m in trasformedMap.Keys)
            {
                storyScript = storyScript.Replace(m.ToLower(), "[" + trasformedMap[m] +"]");
            }

            

            List<string> cols =  ColumnNamesInvolved(tab, storyScript);

            Stack<string> methodStack = new Stack<string>();
            Stack<string> operandStack = new Stack<string>();
            List<string> calls = new List<string>();
          //  do
           // {
                string[] inToks = storyScript.Split(' ');
                foreach (var v in inToks)
                {
                    if (v.StartsWith("[") && v.EndsWith("]"))
                    {
                        if (methodStack.Count == 0)
                        {
                            methodStack.Push("."+ v.Trim(new char[]{'[',']'}));
                            continue;
                        }

                        if (methodStack.Count == 1 || inToks.ToList().IndexOf(v)==inToks.Length-1)
                        {
                            var topElement = methodStack.Peek();
                            KeyValuePair<string,string[]> command  = new KeyValuePair<string,string[]> ();
                            KeyValuePair<string, int> commandNumeric = new KeyValuePair<string,int> ();
                            if (!operandStack.All(t => t.StartsWith("numeric:")))
                            {
                                command = GetCommand(topElement, operandStack);
                                result = HandleIt(result, command.Key, command.Value);
                            }
                            else
                            {
                                commandNumeric = GetCommand(topElement, Convert.ToInt32(operandStack.Peek().Replace("numeric:",string.Empty)));
                                result = HandleIt(result, commandNumeric.Key, commandNumeric.Value);
                            }
                            calls.Add("."+methodStack.Pop().Trim(new char[]{'[',']'}) + "(");
                            if (operandStack.All(t => t.StartsWith("numeric:")))
                                calls.Add(operandStack.Select(m => m.Replace("numeric:", string.Empty)).Aggregate((a, b) => a + "," + b));
                            else
                                calls.Add(operandStack.Select(t => "\"" + t + "\"").Aggregate((a, b) => a + "," + b));
                            while (operandStack.Count != 0)
                                operandStack.Pop();
                            
                            calls.Add(")");
                            

                            methodStack.Push(v);
                            continue;
                        }

                    }
                    else
                    {
                    var suspectedCol = string.Empty;
                    foreach (var z in tab.ColumnHeaders)
                    {
                        if (tab.ValuesOf(z).Contains(v))
                        {
                            suspectedCol = z; 
                            operandStack.Push(suspectedCol);
                            break;
                        }
                    }
                    if (cols.Contains(v) || Regex.Match(v, "[0-9]+").Success)
                    {
                        if (Regex.Match(v, "[0-9]+").Success)
                        {
                            operandStack.Push("numeric:" + v);
                        }
                        
                       
                        else
                        {
                            operandStack.Push(v);
                            if (storyScript.Contains("[Histogram]"))
                                return tab;// ToChart(tab, operandStack.Peek());
                        }
                        if (inToks.ToList().IndexOf(v) == inToks.Length - 1)
                        {
                            var topElement = methodStack.Peek();
                            KeyValuePair<string, string[]> command = new KeyValuePair<string, string[]>();
                            KeyValuePair<string, int> commandNumeric = new KeyValuePair<string, int>();
                            if (!operandStack.All(t => t.StartsWith("numeric:")))
                            {
                                command = GetCommand(topElement, operandStack);
                                result = HandleIt(result, command.Key, command.Value);
                            }
                            else
                            {
                                commandNumeric = GetCommand(topElement, Convert.ToInt32(operandStack.Peek().Replace("numeric:", string.Empty)));
                                result = HandleIt(result, commandNumeric.Key, commandNumeric.Value);
                            }

                            calls.Add("." + methodStack.Pop().Trim(new char[] { '[', ']' }) + "(");
                            if (operandStack.All(t => t.StartsWith("numeric:")))
                                calls.Add(operandStack.Select(t => t.Replace("numeric:", string.Empty)).Aggregate((a, b) => a + "," + b));
                            else
                                calls.Add(operandStack.Select(t => "\"" + t + "\"").Aggregate((a, b) => a + "," + b));
                            while (operandStack.Count != 0)
                                operandStack.Pop();
                            calls.Add(")");
                        }
                        continue;
                    }
                    if (!cols.Contains(v) && inToks.ToList().IndexOf(v) == inToks.Length - 1)
                    {

                        var topElement = methodStack.Peek();
                        KeyValuePair<string, string[]> command = new KeyValuePair<string, string[]>();
                        KeyValuePair<string, int> commandNumeric = new KeyValuePair<string, int>();
                        if (operandStack.Count > 0)
                        {
                            if (!operandStack.All(t => t.StartsWith("numeric:")))
                            {
                                command = GetCommand(topElement, operandStack);
                                result = HandleIt(result, command.Key, command.Value);
                            }
                            else
                            {
                                commandNumeric = GetCommand(topElement, Convert.ToInt32(operandStack.Peek().Replace("numeric:", string.Empty)));
                                result = HandleIt(result, "." + commandNumeric.Key.Replace("[", string.Empty).Replace("]", string.Empty), commandNumeric.Value);
                            }
                        }
                    }

                    }
                }
                if (methodStack.Count == 1)//still an operation needs to be performed
                {
                   KeyValuePair<string, string[]> command   = GetCommand(methodStack.Peek(), operandStack);
                    result = HandleIt(result, command.Key, command.Value);
                    
                }
            return result;//.ToHTMLTable();
                //string gen =  calls.Aggregate((a, b) => a + b).Replace("..",".");
                //gen = "tab." + gen;
                //gen = gen.Replace("..", ".");
                //List<string> tops = Regex.Matches(gen, "[Top](\"[0-9]+\")").Cast<Match>().Select(t => t.Value).ToList();

                //ScriptEngine engine = new ScriptEngine();
                //Roslyn.Scripting.Session session = engine.CreateSession(tab);
               
                //session.AddReference(typeof(Table).Assembly);
                
                //session.ImportNamespace("Squirrel");
                //var x = session.Execute("tab");
                //var tg = session.Execute("tab");
            
                //session.Execute("Table thisTable = tab;");
                //Table gt = (Table) session.Execute(gen);
              //  int someHow = 0;

          //  } while (Regex.Match(storyScript, "[[a-zA-Z]]+").Success);
            //Load "C:\quotes.csv"
            //Pick-> Show me only
            //Drop-> Don't show 
            //Aggregate-> Collapse Month by {average}
            ////Column to pick is the one for which "Physics" is a value
            ////and the function to choose is "{AboveAverage}".
            //Aggregate-> Who scored {more than average} in {Physics} 
            //SortBy-> sort by {colum} in {Descending} order
            ////default is Assending order
            //Top-> show top/first {10} entries 
            //Bottom-> show last/bottom {10} entries
            //TopNPercent-> show first {5}%
            //BottomNPercent-> show bottom {5}%
            //NameTable-> Call it {Name}
            //Merge -> Merge {Name1} and {Name2} :-) => Name1.Merge(Name2);
            //RemoveOutliers -> Clean/Delete Outliers/Remove Outliers
            //ShowOutliers-> Show outliers/show bad data
            //CumulativeFold -> Show cumulative {sum/average}
            //AddToReport-> Add to report (Adds the last one)
            //RemoveFromReport-> Remove from report (removes the last one)
            //Shuffle-> Please shuffle. Shuffle the data
        //    mapping.Add("Show me only", "Pick");
          //  mapping.Add("Don't show", "Drop");
           // mapping.Add("Collapse {ColName} by {Function}", "Aggregate");
            
            //Find jobs processed by iPhone and Android
            //how many jobs were processed by iPhone and Android over the weekend. 
            
            //{show me only} {sub-category} and {item} for {top} {5} items which were bought {more than average} in {Snacks}

            //A really deep example
            //show me only 
            //NQL : "{show me only} {name} and {marks} for {top} {10} person who scored {more than average} in {Physics}"
            //               Pick Name Marks  Top 10 Aggregate how:AggregationMethod.AboveAverage columnName:Subject
            //        
            // columnName:Subject                         __ arg
            // AggregationMethod.AboveAverage             __ arg
            // Aggregate                                  __ op -----> At this point-> tab.Aggregate(how:AggregationMethod.AboveAverage,columnName:Subject)
            // 10                                         __ arg
            // Top                                        __ op -----> At this point -> tab.Aggregate(how:AggregationMethod.AboveAverage,columnName:Subject)
                                                                                      //   .Top(10)
            // Marks                                      __ arg
            // Name                                       __ arg
            // Pick                                       __ op ------> At this point      
                                                                                     //  tab.Aggregate(how:AggregationMethod.AboveAverage,columnName:Subject)
                                                                                      //    .Top(10)
                                                                                      //    .Pick(new string[]{"Name","Marks"});
            //a LINQ compiler/Roslyn can be used to evaluate this generated statement.
            //Evaluant.Linq.Compiler 

            //
            //
            //As we encounter an "op" then use the "args" so far and call and put it to the result 

            //Depiction

            //Top(10)
            //AggregationMethod.AboveAverage
            //Aggreagte on {Subject}

            //In natural language query the flow is from left to right
            //In the computer model, the flow is from right to left
            //From the given query we figure out that {Physics} is a value of let's say the subject column.
            //Then the columns involved is "Subject". 
            //t.Aggregate("Subject",AggregationMethod.AboveAverage).Top(10)

            //string pickPattern = "show me only [a-zA-Z0-9_,% ]+";
            //string dropPattern = "don't show [a-zA-Z0-9_,% ]+";
            //string aggregatePattern = "collapse [a-zA-Z0-9_% ]+ by* [a-zA-Z0-9_% ]*";

            //Match pickMatch = Regex.Match(storyScript, pickPattern);
            //if (pickMatch.Success)
            //{
            //    string[] columnNamesToPick = pickMatch.Value.Substring(12).Split(new string[]{",","and"},StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            //    return tab.Pick(columnNamesToPick);            
            //}

            //Match dropMatch = Regex.Match(storyScript, dropPattern);
            //if (dropMatch.Success)
            //{
            //    string[] columnNamesToPick = dropMatch.Value.Substring(12).Split(new string[] { ",", "and" }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            //    return tab.Drop(columnNamesToPick);
            //}

            //Match aggregateMatch = Regex.Match(storyScript, aggregatePattern);
            //if (aggregateMatch.Success)
            //{
            //    AggregationMethod method = AggregationMethod.Sum;
            //    List<string> values = Regex.Matches(aggregateMatch.Value, "[a-zA-Z0-9_% ]+").Cast<Match>().Select(m => m.Value).ToList();
            //    if (values.Count == 2)
            //    {
            //        string columnName = values[0];
            //        string functionName = values[1];
            //        if (functionName == "sum" || functionName == "summation")
            //            method = AggregationMethod.Sum;
            //        if (functionName == "average" || functionName == "mean")
            //            method = AggregationMethod.Average;

            //        return tab.Aggregate(columnName, method);
            //    }
            //    else
            //    {
            //        return tab.Aggregate(values[0], AggregationMethod.Sum);
            //    }
            //}

            






            

            //return new Table ();
        }
        /// <summary>
        /// Calculates the gist of values for numeric and currency columns.
        /// </summary>
        /// <param name="tab">The table</param>
        /// <returns></returns>
        /// 
        [Description("Gist,show me the gist,nutshell,in a nutshell")]
        public static List<Tuple<string, string, string>> Gist(this Table tab)
        {
            List<Tuple<string, string, string>> gistValues = new List<Tuple<string, string, string>>();
            List<string> numericColumns = new List<string> ();
            List<string> currencyColumns = new List<string> ();
            List<string> dateColumns = new List<string> ();
            List<string> allOtherStringColumns = new List<string>();
            Dictionary<string,string> gist = new Dictionary<string, string>();
            //Find all numeric columns            
            string numericRegex = @"^-?[0-9]\d*(\.\d+)?$";//matches decimals with negative 
            string currencyRegex = @"[$£€¥][0-9]\d*(\,.\d+)?$";//matches currencies
            foreach (var colName in tab.ColumnHeaders)
            {            
               if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, numericRegex)))
                   numericColumns.Add(colName);
               if (tab.ValuesOf(colName).All(m => Regex.IsMatch(m, currencyRegex)))
                   currencyColumns.Add(colName);
               else
                   allOtherStringColumns.Add(colName);
            }

            foreach (var m in numericColumns)
            {
                
                double max = tab.ValuesOf(m).Select(Convert.ToDouble).Max();
                double min = tab.ValuesOf(m).Select(Convert.ToDouble).Min();
                double avg = tab.ValuesOf(m).Select(Convert.ToDouble).Average();
                decimal range = tab.ValuesOf(m).Select(Convert.ToDecimal).Range();

                gistValues.Add(new Tuple<string, string, string>(m, "Max", max.ToString(CultureInfo.InvariantCulture)));
                gistValues.Add(new Tuple<string, string, string>(m, "Min", min.ToString(CultureInfo.InvariantCulture)));
                gistValues.Add(new Tuple<string, string, string>(m, "Average", avg.ToString(CultureInfo.InvariantCulture)));
                gistValues.Add(new Tuple<string, string, string>(m, "Range", range.ToString(CultureInfo.InvariantCulture)));
                
            }
           
            Func<string, string> replaceOddChars = x => x.Replace("$", string.Empty)
                                                         .Replace("£", string.Empty)
                                                         .Replace("€", string.Empty)
                                                         .Replace("¥", string.Empty)
                                                         .Replace(",", string.Empty);
           
            foreach (var m in currencyColumns)
            {
               
                double max = tab.ValuesOf(m).Select(z => Convert.ToDouble(replaceOddChars(z)))
                                            .Select(t => Convert.ToDouble(t)).Max();
                double min = tab.ValuesOf(m).Select(z => Convert.ToDouble(replaceOddChars(z)))
                                            .Select(t => Convert.ToDouble(t)).Min();
                double avg = tab.ValuesOf(m).Select(z => Convert.ToDouble(replaceOddChars(z)))
                                            .Select(t => Convert.ToDouble(t)).Average();
                decimal range = tab.ValuesOf(m).Select(z => Convert.ToDouble(replaceOddChars(z)))
                                               .Select(t => Convert.ToDecimal(t)).Range();

                gistValues.Add(new Tuple<string, string, string>(m, "Max", max.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Min", min.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Average", avg.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Range", range.ToString()));
            }
            
            return gistValues;
        }
    }
}
