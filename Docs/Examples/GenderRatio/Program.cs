using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Squirrel;

namespace GenderRatio
{
    class Program
    {
        static void Main(string[] args)
        {
            Table births = DataAcquisition.LoadCSV(@"..\..\births.csv");
            var splits = births.SplitOn("sex");

            var boys = splits["boy"].Aggregate("state").Drop("year");
            var girls = splits["girl"].Aggregate("state").Drop("year");

            Table combined = new Table();

            combined.AddColumn("State", boys["state"]);
            combined.AddColumn("Boys", boys["births"]);
            combined.AddColumn("Girls", girls["births"]);
            combined.AddColumn("Difference", "[Boys]-[Girls]", 4);
            combined.AddColumn("GenderRatio", "[Girls]/[Boys]", 4);
            //Showing 5 stats with lowest gender ratio at the end of 2006.
            string tab = combined.Pick("State", "GenderRatio")
                    .SortBy("GenderRatio")
                    .Top(5)
                    .ToHTMLTable();

            StreamWriter sw = new StreamWriter("temp.htm");
            sw.WriteLine("<html><h2>Gender Ratio in North America by the end of 2006</h2>" + tab + "</html>");
            sw.Close();
            System.Diagnostics.Process.Start("temp.htm");

        }
    }
}
