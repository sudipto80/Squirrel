using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;

namespace SquirrelUnitTest;

[TestClass]
public class DateExtensionTests
{
    [TestMethod]
    public void TestBusinessDayFrequency()
    {
       var businessDays = Squirrel.DateExtensions.GenerateDateRange(DateTime.Today, DateTime.Today.AddMonths(1),
            DateTimeFrequency.BusinessDay);
        
       Assert.IsTrue(businessDays.Count > 0);
       Assert.IsTrue(businessDays.All(z => z.DayOfWeek != DayOfWeek.Saturday && z.DayOfWeek != DayOfWeek.Sunday));
    }

    [TestMethod]
    public void TestMonthEndDaysFrequency()
    {
        var monthEndDays =
            DateExtensions.GenerateDateRange(DateTime.Today, DateTime.Today.AddMonths(10), DateTimeFrequency.BusinessMonthEnd);
        Assert.IsTrue(monthEndDays.Count > 0);
        //Assert.IsTrue(monthEndDays.All(z => z.Day == DateTime.DaysInMonth(z.Year, z.Month)));
    }
}
