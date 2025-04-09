using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squirrel;
using System.Data;


using Squirrel.Cleansing;

namespace SquirrelUnitTest
{
    [TestClass]
    public class DataCleansingTests
    {

        [TestMethod]
        public void Test_RemoveIfBetween()
        {
            Table test = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Name", "Sam");
            row1.Add("Experience", "10");



            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Name", "Jen");
            row2.Add("Experience", "1");


            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Name", "Raul");
            row3.Add("Experience", "5");

            Dictionary<string, string> row4 = new Dictionary<string, string>();
            row4.Add("Name", "Samuel");
            row4.Add("Experience", "15");


            test.Rows.Add(row1);
            test.Rows.Add(row2);
            test.Rows.Add(row3);
            test.Rows.Add(row4);

            Assert.AreEqual(4, test.RowCount);
            Assert.AreEqual(2, test.RemoveIfBetween("Experience", 0, 6).RowCount);
        }
        [TestMethod]
        public void Test_RemoveIfBefore()
        {
            Table test = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Date", DateTime.Today.AddDays(-1).ToShortDateString());
            row1.Add("Sales", "22");
            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Date", DateTime.Today.AddDays(1).ToShortDateString());
            row2.Add("Sales", "24");
            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Date", DateTime.Today.ToShortDateString());
            row3.Add("Sales", "25");

            test.Rows.Add(row1);
            test.Rows.Add(row2);
            test.Rows.Add(row3);

            Assert.AreEqual(3, test.RowCount);
            Assert.AreEqual(2, test.RemoveIfBefore("Date", DateTime.Today).RowCount);

        }
        [TestMethod]
        public void Test_RemoveIfAfter()
        {
            Table test = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Date", DateTime.Today.AddDays(-1).ToShortDateString());
            row1.Add("Sales", "22");
            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Date", DateTime.Today.AddDays(1).ToShortDateString());
            row2.Add("Sales", "24");
            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Date", DateTime.Today.ToShortDateString());
            row3.Add("Sales", "25");

            test.Rows.Add(row1);
            test.Rows.Add(row2);
            test.Rows.Add(row3);

            Assert.AreEqual(3, test.RowCount);
            Assert.AreEqual(2, test.RemoveIfAfter("Date", DateTime.Today).RowCount);
        }

        [TestMethod]
        public void Test_ExtractOutliers()
        {
            Table messageSent = new Table();
            //1, 1,2,3,5,18,26,32,40,58,100
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Sent", "1");
            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Sent", "1");
            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Sent", "2");
            Dictionary<string, string> row4 = new Dictionary<string, string>();
            row4.Add("Sent", "3");
            Dictionary<string, string> row5 = new Dictionary<string, string>();
            row5.Add("Sent", "5");
            Dictionary<string, string> row6 = new Dictionary<string, string>();
            row6.Add("Sent", "18");
            Dictionary<string, string> row7 = new Dictionary<string, string>();
            row7.Add("Sent", "26");
            Dictionary<string, string> row8 = new Dictionary<string, string>();
            row8.Add("Sent", "32");
            Dictionary<string, string> row9 = new Dictionary<string, string>();
            row9.Add("Sent", "40");
            Dictionary<string, string> row10 = new Dictionary<string, string>();
            row10.Add("Sent", "58");
            Dictionary<string, string> row11 = new Dictionary<string, string>();
            row11.Add("Sent", "100");
            messageSent.AddRow(row1);
            messageSent.AddRow(row2);
            messageSent.AddRow(row3);
            messageSent.AddRow(row4);
            messageSent.AddRow(row5);
            messageSent.AddRow(row6);
            messageSent.AddRow(row7);
            messageSent.AddRow(row8);
            messageSent.AddRow(row9);
            messageSent.AddRow(row10);
            messageSent.AddRow(row11);
            //var outliers = messageSent.ExtractOutliers("Sent", OutlierDetectionAlgorithm.IQR_Interval);
            //Assert.AreEqual(1, outliers.RowCount);
            //Assert.AreEqual("100", outliers[0]["Sent"]);
            Assert.Inconclusive("WIP");

        }
        [TestMethod]
        public void Test_RemoveOutliers()
        {
            Table messageSent = new Table();
            //1, 1,2,3,5,18,26,32,40,58,100
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Sent", "1");
            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Sent", "1");
            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Sent", "2");
            Dictionary<string, string> row4 = new Dictionary<string, string>();
            row4.Add("Sent", "3");
            Dictionary<string, string> row5 = new Dictionary<string, string>();
            row5.Add("Sent", "5");
            Dictionary<string, string> row6 = new Dictionary<string, string>();
            row6.Add("Sent", "18");
            Dictionary<string, string> row7 = new Dictionary<string, string>();
            row7.Add("Sent", "26");
            Dictionary<string, string> row8 = new Dictionary<string, string>();
            row8.Add("Sent", "32");
            Dictionary<string, string> row9 = new Dictionary<string, string>();
            row9.Add("Sent", "40");
            Dictionary<string, string> row10 = new Dictionary<string, string>();
            row10.Add("Sent", "58");
            Dictionary<string, string> row11 = new Dictionary<string, string>();
            row11.Add("Sent", "100");
            messageSent.AddRow(row1);
            messageSent.AddRow(row2);
            messageSent.AddRow(row3);
            messageSent.AddRow(row4);
            messageSent.AddRow(row5);
            messageSent.AddRow(row6);
            messageSent.AddRow(row7);
            messageSent.AddRow(row8);
            messageSent.AddRow(row9);
            messageSent.AddRow(row10);
            messageSent.AddRow(row11);
            Assert.AreEqual(11, messageSent.RowCount);
            Assert.AreEqual(10, messageSent.RemoveOutliers("Sent").RowCount);
            Assert.IsFalse(messageSent.ValuesOf("Sent").Contains("100"));
        }
        [TestMethod]
        public void Test_RemoveIfNotBetweenNumericValues()
        {
            Table test = new Table();

            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Name", "Sam");
            row1.Add("Age", "23");

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Name", "Jane");
            row2.Add("Age", "123");

            test.Rows.Add(row1);
            test.Rows.Add(row2);

            Assert.AreEqual(2, test.Rows.Count);
            Assert.AreEqual(1, test.RemoveIfNotBetween("Age", 0, 100).RowCount);

        }
    }
}
