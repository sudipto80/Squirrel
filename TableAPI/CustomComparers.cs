using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squirrel
{
    public class CustomComparers
    {
        
        public static List<string> sortInThisOrder = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public class TwentyFourHourTimeCustomSorter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                //20:00 
                return 0;
                //19:35
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class TwelveHourTimeFormatCustomSorter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                //8:00 PM
                //10:00 AM
                return 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class CurrencyCustomSorter : IComparer<string>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="amount1"></param>
            /// <param name="amount2"></param>
            /// <returns></returns>
            public int Compare(string amount1, string amount2)
            {
                //$3,000.00
                //£ 402.10
                amount1 = amount1.Replace(" ", string.Empty);
                amount2 = amount2.Replace(" ", string.Empty);
                return Convert.ToDecimal(amount1.Substring(1).Replace(",", string.Empty))
                    .CompareTo(Convert.ToDecimal(amount2.Substring(1).Replace(",", string.Empty)));


            }
        }
        /// <summary>
        /// Generic plain comparer that compares two strings.
        /// </summary>
        public class CustomSorter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return sortInThisOrder.IndexOf(x).CompareTo(
                sortInThisOrder.IndexOf(y));
            }
        }
        /// <summary>
        /// Comparer to compare two dates.
        /// </summary>
        public class DateComparer : IComparer<string> 
        {
            public int Compare(string x, string y)
            {
                return Convert.ToDateTime(x).CompareTo(Convert.ToDateTime(y));
            }
        }
    
        /// <summary>
        /// Numeric comparer to compare numbers
        /// </summary>
        public class NumericComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return Convert.ToDecimal(x).CompareTo(Convert.ToDecimal(y));
            }
        }
      
    }
}
