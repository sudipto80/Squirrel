using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SquirrelUnitTest;


[TestClass]
public class DataAcquisitionTests
{

    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void CsvSpectrum_comma_in_quotes_csv()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\comma_in_quotes.csv");
        var expectedHeaders = new[] { "first", "last", "address", "city", "zip" };
        Assert.AreEqual(5, tab.ColumnHeaders.Count);
        Assert.IsTrue(expectedHeaders.SequenceEqual(tab.ColumnHeaders));
        Assert.AreEqual(1, tab.RowCount);
        //The solitary row is -> John,Doe,120 any st.,"Anytown, WW",08123
        Assert.AreEqual("John", tab[0]["first"]);
        Assert.AreEqual("John", tab["first"][0]);
        Assert.AreEqual("Doe", tab[0]["last"]);
        Assert.AreEqual("120 any st.", tab[0]["address"]);
        Assert.AreEqual("Anytown, WW", tab[0]["city"]);
        Assert.AreEqual("08123", tab[0]["zip"]);
    }

    private static readonly string[] First = ["a", "b", "c"];
    private static readonly string[] Expected = ["2", "3", "4"];

    [TestMethod]
    public void CsvSpectrum_empty_csv()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\empty.csv");

        Assert.AreEqual(2, tab.RowCount);
        Assert.AreEqual(3, tab.ColumnHeaders.Count);
        Assert.IsTrue(First.SequenceEqual(tab.ColumnHeaders));
        Assert.AreEqual(0, tab["b"][0].Length);
        Assert.AreEqual(0, tab["c"][0].Length);

        Assert.AreEqual("2", tab["a"][1]);
        Assert.AreEqual("3", tab["b"][1]);
        Assert.AreEqual("4", tab["c"][1]);
        //Second row value of column "a" is "2"    
        Assert.AreEqual("2", tab["a", 1]);
        //Second row value of column "b" is "3" 
        Assert.AreEqual("3", tab["b", 1]);
        Assert.AreEqual("4", tab["c", 1]);
        //Matching the entire row at once. 
        Assert.IsTrue(tab[1].Values.SequenceEqual(["2", "3", "4"]));


    }

    [TestMethod]
    public void CsvSpectrum_empty_crlf()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\empty_crlf.csv");

        Assert.AreEqual(2, tab.RowCount);
        Assert.AreEqual(3, tab.ColumnHeaders.Count);
        Assert.IsTrue(tab.ColumnHeaders.SequenceEqual(["a", "b", "c"]));

    }
    /// <summary>
    /// 
    /// </summary>
    [TestMethod]
    public void CsvSpectrum_escaped_quotes()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\escaped_quotes.csv");

        Assert.AreEqual(2, tab.RowCount);
        Assert.AreEqual(2, tab.ColumnHeaders.Count);
        Assert.IsTrue(tab.ColumnHeaders.SequenceEqual(["a", "b"]));
        Assert.AreEqual(@"ha ""ha"" ha", tab["b", 0]);
        Assert.AreEqual("3", tab["a", 1]);
        Assert.AreEqual("4", tab["b", 1]);
    }

    [TestMethod]
    public void CsvSpectrum_json()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\json.csv");

        Assert.AreEqual(1, tab.RowCount);
        Assert.AreEqual(tab["val", 0], @"{""type"": ""Point"", ""coordinates"": [102.0, 0.5]}");
    }

    [TestMethod]
    public void CsvSpectrum_location_coordinates()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\location_coordinates.csv");

        Assert.AreEqual(1, tab.RowCount);
        Assert.AreEqual(4, tab.ColumnHeaders.Count);
        Assert.AreEqual("2095257564", tab["Contact Phone Number", 0]);
        Assert.AreEqual(@"37�36'37.8""N 121�2'17.9""W", tab["Location Coordinates", 0]);
        Assert.AreEqual("Modesto", tab["Cities"][0]);
        Assert.AreEqual("Stanislaus", tab["Counties"][0]);

    }

    [TestMethod]
    public void CsvSpectrum_newlines()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\newlines.csv");

        Assert.AreEqual(3, tab.RowCount);


    }

    [TestMethod]
    public void CsvSpectrum_newlines_crlf()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\newlines_crlf.csv");

        Assert.AreEqual(3, tab.RowCount);


    }

    [TestMethod]
    public void CsvSpectrum_quotes_and_newlines()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\quotes_and_newlines.csv");

        Assert.AreEqual(2, tab.RowCount);
        Assert.AreEqual(@"ha ""ha"" ha",tab["b",0]);
        Assert.IsTrue(tab[1].Values.SequenceEqual(["3", "4"]));


    }
    
    [TestMethod]
    public void CsvSpectrum_simple()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\simple.csv");

        Assert.AreEqual(1, tab.RowCount);
        
        Assert.IsTrue(tab[0].Values.SequenceEqual(["1","2","3"]));


    }

    [TestMethod]
    public void CsvSpectrum_simple_crlf()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\simple_crlf.csv");

        Assert.AreEqual(1, tab.RowCount);
        
        Assert.IsTrue(tab[0].Values.SequenceEqual(["1","2","3"]));


    }
    [TestMethod]
    public void CsvSpectrum_utf8()
    {
        var tab =
            DataAcquisition.LoadCsv(@"..\..\..\Data\CsvSpectrum\utf8.csv");

        Assert.AreEqual(2, tab.RowCount);
        
        Assert.IsTrue(tab[1].Values.SequenceEqual(["4","5","ʤ"]));


    }
}