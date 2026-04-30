using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;

namespace SquirrelUnitTest;

[TestClass]
public class DateRangeTests
{
    [TestMethod]
    public void Test_MondaysFrom()
    {
        var mondays = 5.MondaysFrom(DateTime.Today);
        Assert.AreEqual(5, mondays.Count);
        Assert.IsTrue(mondays.All(d => d.DayOfWeek == DayOfWeek.Monday));
    }
    
    [TestMethod]
    public void Test_TimeSeries()
    {
        var tsData = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Downloads/timeseries_data.csv");
        var dows = tsData["timestamp"].Select(t => DateTime.Parse(t).DayOfWeek.ToString()).ToList();
        tsData.AddColumn("DOW", dows);
        var allFridays = tsData.Filter("DOW", "Friday");
        var start =  DateTime.Parse(tsData["timestamp"][0]);
        
        
        var mondayData  = tsData.FilterByDates("timestamp", 10.MondaysFrom(start));
        var fridayData  = tsData.FilterByDates("timestamp", 10.FridaysFrom(start));
        var bizData     = tsData.FilterByDates("timestamp", 10.BusinessDaysFrom(start));
 
   
        
        var mes = tsData.Resample("timestamp",["volume"], DateTimeFrequency.Weekly, AggregationMethod.Average);
        var mbs = tsData.Resample("timestamp",["volume"], DateTimeFrequency.QuarterEnd, AggregationMethod.Average);

    }
}