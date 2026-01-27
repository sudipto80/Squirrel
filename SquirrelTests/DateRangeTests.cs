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
        var tsData = DataAcquisition.LoadCsv(@"/Users/sudiptamukherjee/Downloads/timeseries_data (1).csv");
        var mes = tsData.Resample("timestamp", DateTimeFrequency.Weekly, AggregationMethod.Average);
        var mbs = tsData.Resample("timestamp", DateTimeFrequency.MonthStart, AggregationMethod.Average);

    }
}