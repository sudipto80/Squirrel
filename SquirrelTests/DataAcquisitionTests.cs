using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;
using TableAPI;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SquirrelUnitTest;


[TestClass]
public class DataAcquisitionTests
{
    private static readonly string TestDataPath = 
        Path.Combine("..","..","..","Data");

    [TestMethod]
    public void Test_LoadARFF()
    {
        Table play = DataAcquisition.LoadArff( Path.Combine(TestDataPath, "weather.nominal.arff"));
        Assert.AreEqual(14, play.RowCount);
        Assert.IsTrue((new string[] { "outlook", "temperature", "humidity", "windy", "play" }).All(t => play.ColumnHeaders.Contains(t)));
        Assert.IsTrue(play.ValuesOf("outlook").Distinct().All(m => m== "sunny" || m== "overcast" || m == "rainy" ));

    }
    [TestMethod]
    public void Test_AnonLoading()
    {
        // A funny dataset but it proves the point
        var names = new []{"Sam", "Jenny", "Helmut" };
        var recTab = names.Select(t => new { Name = t, Age = 20 + t.Length })
                                                .ToRecordTableFromAnonList();
      
        Assert.AreEqual(3, recTab.RowCount);
        Assert.AreEqual("Sam", recTab[0].Name);
        Assert.AreEqual("Jenny", recTab[1].Name);
        Assert.AreEqual("Helmut", recTab[2].Name);
        Assert.AreEqual(23, recTab[0].Age);
        Assert.AreEqual(25, recTab[1].Age);
        Assert.AreEqual(26, recTab[2].Age);
    }
    [TestMethod]
    public void Test_LoadHTML()
    {
        var crime = DataAcquisition.LoadHtmlTable(Path.Combine(TestDataPath, "crimerate.html"));
        Assert.AreEqual(10, crime.RowCount);
        Assert.IsTrue(
            crime[crime.RowCount - 1].Values
                .SequenceEqual(["Andhra Pradesh", "3.6"]));

        var olympics = DataAcquisition.LoadHtmlTable(Path.Combine(TestDataPath, "olympic_medals.html"));
        Assert.AreEqual(146, olympics.RowCount);

        var accidents = DataAcquisition.LoadHtmlTable(Path.Combine(TestDataPath, "roadaccidents.html"));
        Assert.AreEqual(186, accidents.RowCount);
        Assert.IsTrue(
            accidents[accidents.RowCount - 1].Values
                .SequenceEqual(["&nbsp;Zimbabwe", "14.6", "212.4", "n/a", "1832", "2010"]));
    }

    [TestMethod]
    public void Test_LoadTSV()
    {
        Table data = DataAcquisition.LoadTsv(Path.Combine(TestDataPath, "data.tsv"), true);
        Assert.AreEqual(3, data.RowCount);
        Assert.AreEqual("Jane", data["Name"][0]);


    }

    [TestMethod]
    public void Test_LoadCSV()
    {

        Table births =
            DataAcquisition.LoadCsv(Path.Combine(TestDataPath, "births.csv"));
        Assert.AreEqual(4, births.ColumnHeaders.Count);
        Assert.IsTrue(births.ColumnHeaders.SequenceEqual(["year", "state", "sex", "births"]));
      
        Assert.AreEqual(2654, births.RowCount);
        Assert.IsTrue(births["sex"].Distinct().SequenceEqual(["boy", "girl"]));
        Assert.IsTrue(births["births"][0] == births[0]["births"]);
        Assert.AreEqual("4721", births["births"][0]);
    }
}