using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SquirrelUnitTest;


[TestClass]
public class DataAcquisitionTests
{
    [TestMethod]
    public void Test_LoadHTML()
    {
        var crime = DataAcquisition.LoadHtmlTable(@"..\..\..\Data\crimerate.html");
        Assert.AreEqual(10, crime.RowCount);
        Assert.IsTrue(
            crime[crime.RowCount - 1].Values
                .SequenceEqual(["Andhra Pradesh", "3.6"]));

        var olympics = DataAcquisition.LoadHtmlTable(@"..\..\..\Data\olympic_medals.html");
        Assert.AreEqual(146, olympics.RowCount);

        Table accidents = DataAcquisition.LoadHtmlTable(@"..\..\..\Data\roadaccidents.html");
        Assert.AreEqual(186, accidents.RowCount);
        Assert.IsTrue(
            accidents[accidents.RowCount - 1].Values
                .SequenceEqual(["&nbsp;Zimbabwe", "14.6", "212.4", "n/a", "1832", "2010"]));
    }

    [TestMethod]
    public void Test_LoadTSV()
    {
        Table data = DataAcquisition.LoadTsv(@"..\..\..\Data\data.tsv", true);
        Assert.AreEqual(3, data.RowCount);
        Assert.AreEqual("Jane", data["Name"][0]);


    }
    [TestMethod]
    public void Test_LoadCSV()
    {
        Table births = DataAcquisition.LoadCsv(@"C:\Users\Admin\Documents\GitHub\Squirrel\SquirrelTests\Data\births.csv");
        Assert.AreEqual(4, births.ColumnHeaders.Count);
        Assert.AreEqual("year", births.ColumnHeaders.ElementAt(0));
        Assert.AreEqual("state", births.ColumnHeaders.ElementAt(1));
        Assert.AreEqual("sex", births.ColumnHeaders.ElementAt(2));
        Assert.AreEqual("births", births.ColumnHeaders.ElementAt(3));
        Assert.AreEqual(2654, births.RowCount);
        Assert.IsTrue(births["sex"].Distinct().SequenceEqual(new string[] { "boy", "girl" }));            
        Assert.IsTrue(births["births"][0] == births[0]["births"]);
        Assert.AreEqual("4721", births["births"][0]);
    }
}