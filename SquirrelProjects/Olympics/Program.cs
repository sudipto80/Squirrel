using Squirrel;
using Squirrel.Cleansing;

namespace Olympics;

class Program
{
    static void Main(string[] args)
    {
        var olympic_medals =
            DataAcquisition.LoadHtmlTable(
                    @"/Users/sudiptamukherjee/Documents/GitHub/Squirrel/SquirrelTests/Data/olympic_medals.html")
                .Pick("Country", "Gold Olympic Medals")
                .Transform("Gold Olympic Medals", x => x.Replace(",", string.Empty).Trim())
                .ModifyColumnName("Gold Olympic Medals", "Gold")
                .RemoveIncompleteRows("Gold")
                .SortBy("Gold", how: SortDirection.Descending)
                //Skipping the bottom total row
                .Skip(1)
                .Top(10);
              

        var usa = olympic_medals.Filter("Country", "United States")[0];
        string html = 
            olympic_medals
            .ToPieByGoogleDataVisualization
             (column:"Country", 
              title: "Top Gold Winning Nations in Olympics",
              type: GoogleDataVisualization.PieChartType.Pie3D);
        File.WriteAllText("olympics.html", html);
    }
}