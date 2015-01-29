using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Squirrel
{
    /// <summary>
    /// Class for functionalities that help telling a story with data.
    /// This is the place to add a method if you think it should be 
    /// used from the outer-most layer.
    /// </summary>
    public static class Story
    {
        /// <summary>
        /// Returns the  HTML content for a quick dashboard for a given table
        /// </summary>
        /// <param name="tab">The given table</param>
        /// <returns></returns>
        /// <remarks>This is a work in progress version.</remarks>
        [Description("Show dashboard")]
        public static List<string> QuickDashboard(this Table tab)
        {
            List<string> dashboardContents = new List<string>();
            dashboardContents.Add(tab.Gist().ToTable().ToBasicBootstrapHTMLTable());
            List<string> stringColumns = tab.ColumnHeaders.Except(tab.NumericColumns).ToList();
            foreach (var stringCol in stringColumns)
                dashboardContents.Add(tab.ToPieByGoogleDataVisualization(stringCol,"Distribution of " + stringCol));
            //
            return dashboardContents;
        }
        /// <summary>
        /// Calculates the gist of values for numeric and currency columns.
        /// </summary>
        /// <param name="tab">The table</param>
        /// <returns>A list of tuples with max,min,average and range for all the numeric columns.</returns>
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
                
                double max = tab.ValuesOf(m).Select(t => Convert.ToDouble(t)).Max();
                double min = tab.ValuesOf(m).Select(t => Convert.ToDouble(t)).Min();
                double avg = tab.ValuesOf(m).Select(t => Convert.ToDouble(t)).Average();
                decimal range = tab.ValuesOf(m).Select(t => Convert.ToDecimal(t)).Range();

                gistValues.Add(new Tuple<string, string, string>(m, "Max", max.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Min", min.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Average", avg.ToString()));
                gistValues.Add(new Tuple<string, string, string>(m, "Range", range.ToString()));
                
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
