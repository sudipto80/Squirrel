using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Squirrel;

namespace IrisDataSetAggregation
{
    class Program
    {
        static void Main(string[] args)
        {

            Table iris = DataAcquisition.LoadCSV(@"..\..\iris.csv");
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("<html>");

            builder.AppendLine("<h2>Range</h2>");
            builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Range).ToHTMLTable());

            builder.AppendLine("<h2>Average</h2>");
            builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Average).ToHTMLTable());

            builder.AppendLine("<h2>Max</h2>");
            builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Max).ToHTMLTable());

            builder.AppendLine("<h2>Min</h2>");
            builder.AppendLine(iris.Aggregate("Name", AggregationMethod.Min).ToHTMLTable());

            builder.AppendLine("</html>");
            StreamWriter writer = new StreamWriter("temp.html");
            writer.WriteLine(builder.ToString());
            writer.Close();

            System.Diagnostics.Process.Start("temp.html");

        }
    }
}
