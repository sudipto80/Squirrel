using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Squirrel
{
    /// <summary>
    /// 
    /// </summary>
    public static class BasicStatistics
    {
        public static decimal Median(List<decimal> numbers)
        {
            numbers = numbers.OrderBy(m => m).ToList();
            if (numbers.Count % 2 == 0)
            {
                return (numbers[numbers.Count / 2] + numbers[numbers.Count / 2 - 1]) / 2;
            }
            else
            {
                return numbers[numbers.Count / 2];
            }
        }
        /// <summary>
        /// This must go to the Math API
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static Tuple<decimal, decimal> IQRRange(List<decimal> numbers)
        {
            decimal median = Median(numbers);
            List<decimal> smaller = numbers.Where(n => n < median).ToList();
            List<decimal> bigger = numbers.Where(n => n > median).ToList();
            decimal Q1 = Median(smaller);
            decimal Q3 = Median(bigger);
            decimal IQR = Q3 - Q1;
            return new Tuple<decimal, decimal>(Q1 - 1.5M * IQR, Q3 + 1.5M * IQR);
        }

        /// <summary>
        /// Returns the range of values for the given columns.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static decimal Range(this IEnumerable<decimal> values)
        {
            return values.Max() - values.Min();
        }
        /// <summary>
        /// Returns the kurtosis of a data set. Kurtosis characterizes the relative peakedness or flatness of a 
        /// distribution compared with the normal distribution. 
        /// Positive kurtosis indicates a relatively peaked distribution. Negative kurtosis indicates a relatively 
        /// flat distribution.
        /// </summary>
        /// <param name="values">arguments for which you want to calculate kurtosis. 
        /// You can also use a single array or a reference to an array instead of arguments separated by commas.</param>
        /// <returns></returns>
        public static double Kurtosis(this IList<double> values)
        {
            double count = values.Count;
            double N1 = count + 1;
            double N2 = count;
            double D1 = count - 1;
            double D2 = count - 2;
            double D3 = count - 3;

            double sum = 0;
            double v = values.Average();
            double s = StandardDeviation(values);

            for (int i = 0; i < count; i++)
                sum += Math.Pow((values[i] - v) / s, 4);

            return (N1 * N2 * sum) / (D1 * D2 * D3) - (3 * (Math.Pow(count - 1, 2))) / (D2 * D3);



        }
        public static double StandardDeviation(IList<double> values)
        {
            return 0.0;
            //  return Math.Sqrt(Variance(values));
        }
        public static int AverageCount(this IEnumerable<decimal> values)
        {
            return values.Count(s => s >= values.Average());
        }
        /// <summary>
        /// Returns a number of 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        /// 
        [Description("More than average,Above Average,More than mean")]
        
        public static int AboveAverageCount(this IEnumerable<decimal> values)
        {
            return values.Count(s => s > values.Average());
        }
        /// <summary>
        /// Returns the number of instances that are below average value
        /// </summary>
        /// <param name="values">The values</param>
        /// <returns></returns>
        public static int BelowAverageCount(this IEnumerable<decimal> values)
        {
            return values.Count(s => s < values.Average());
        }
    }
}
