using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squirrel;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Squirrel.Cleansing;

namespace SquirrelUnitTest
{
    [TestClass]
    public class TableAPICoreTests
    {
        /// <summary>
        /// 
        /// </summary>
     
        [TestMethod]
        public void Test_MergeColumns()
        {
            Table test = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("FirstName", "Sam");
            row1.Add("LastName", "Frank");
            row1.Add("Age", "34");
            test.AddRow(row1);
            Assert.AreEqual(3, test.ColumnHeaders.Count);
            Assert.AreEqual(2, test.MergeColumns("Name", ' ',true, "FirstName", "LastName").ColumnHeaders.Count);
            Assert.AreEqual("Sam Frank", test["Name"][0]);

        }

       
        [TestMethod]
        public void Test_SplitOn()
        {

            Table t1 = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Month", "Jan");
            row1.Add("Sales", "3445");

            t1.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Month", "Jan");
            row2.Add("Sales", "3345");

            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Month", "Jan");
            row3.Add("Sales", "4543");
            Dictionary<string, string> row4 = new Dictionary<string, string>();
            row4.Add("Month", "Feb");
            row4.Add("Sales", "3145");
            Dictionary<string, string> row5 = new Dictionary<string, string>();
            row5.Add("Month", "Feb");
            row5.Add("Sales", "3325");

            t1.AddRow(row2);
            t1.AddRow(row3);
            t1.AddRow(row4);

            t1.PrettyDump();
            var splits = t1.SplitOn("Month");
            Assert.AreEqual(3, splits["Jan"].RowCount);
            Assert.AreEqual(1, splits["Feb"].RowCount);
            
        }
        [TestMethod]
        public void Test_Common()
        {
            Table t1 = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("A", "1");
            row1.Add("B", "3");
           
            t1.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("A", "12");
            row2.Add("B", "32");

            t1.AddRow(row2);


            Table t2 = new Table();
            //This is the matching row among two tables
            Dictionary<string, string> row11 = new Dictionary<string, string>();
            row11.Add("A", "1");
            row11.Add("B", "3");

            t2.AddRow(row11);

            Dictionary<string, string> row21 = new Dictionary<string, string>();
            row21.Add("A", "123");
            row21.Add("B", "323");

            t2.AddRow(row21);
            //There is only one common row
            Assert.AreEqual(1, t1.Common(t2).RowCount);
            //Testing if the rows are added in order properly
            Assert.AreEqual("1", t1.Common(t2)[0]["A"]);
            Assert.AreEqual("3", t1.Common(t2)[0]["B"]);
        }
        [TestMethod]
        public void Test_Merge()
        {
            Table t1 = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("A", "1");
            row1.Add("B", "3");

            t1.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("A", "12");
            row2.Add("B", "32");

            t1.AddRow(row2);


            Table t2 = new Table();
            Dictionary<string, string> row11 = new Dictionary<string, string>();
            row11.Add("B", "3");
            row11.Add("A", "1");

            t2.AddRow(row11);

            Dictionary<string, string> row21 = new Dictionary<string, string>();
            row21.Add("A", "123");
            row21.Add("B", "323");

            t2.AddRow(row21);
            //There are three distinct rows in the merged table
            Assert.AreEqual(4, t1.Merge(t2).RowCount);
            Assert.AreEqual(3, t1.Merge(t2,removeDups:true).RowCount);
            

        }
        [TestMethod]
        public void Test_Exclusive()
        {
            Table t1 = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            //This row is exlusive to this table
            row1.Add("Hour", "1");
            row1.Add("Sugar_Level", "High");

            t1.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Hour", "2");
            row2.Add("Sugar_Level", "Low");

            t1.AddRow(row2);

            Table t2 = new Table();            

            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Hour", "1");
            row3.Add("Sugar_Level", "Medium");

            t2.AddRow(row3);

            Dictionary<string, string> row4 = new Dictionary<string, string>();
            row4.Add("Hour", "2");
            row4.Add("Sugar_Level", "Low");

            t2.AddRow(row4);

            Assert.AreEqual(1, t1.Exclusive(t2).RowCount);
        }
        [TestMethod]
        public void Test_Pick()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);
            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Jenny");
            r3.Add("Course", "D");
            r3.Add("Duration", "5");
            t.AddRow(r3);
           

            Table picked = t.Pick("Course","Duration");
            Assert.AreEqual(2, picked.ColumnHeaders.Count);
            Assert.AreEqual("C#", picked["Course"][0]);
            Assert.AreEqual("4", picked["Duration"][0]);
        }
        [TestMethod]
        public void TestAddRows()
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("Year", "2013");
            Table t = new Table();
            t.AddRow(row);

            t.PrettyDump();

            t.AddRows(formula:"Year[1]=Year[0]+1", count:10);

            Assert.AreEqual(11, t.RowCount);
            Assert.AreEqual("2023", t[10]["Year"]);
        }
        [TestMethod]
        public void TestAddColumnCelciusFarenCase()
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("Celcius", "-40");

            Table temps = new Table();
            temps.AddRow(row);

            temps.AddColumn(columnName: "Farenheit", formula: "9*[Celcius]/5 + 32", decimalDigits: 3);
            
            Assert.AreEqual("-40", temps[0]["Farenheit"]);
        }
        [TestMethod]
        public void Test_AddNewColumnFromFormula()
        {
            Dictionary<string, string> row = new Dictionary<string, string>();

            row.Add("Austin", "99");
            row.Add("Tampa", "81");

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Austin", "97");
            row2.Add("Tampa", "82");

            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Austin", "93");
            row3.Add("Tampa", "88");

            Table tempTable = new Table();
            tempTable.AddRow(row);
            tempTable.AddRow(row2);
            tempTable.AddRow(row3);
            //Add a column called "Difference" that will have the differences of temperatures in Austin and Tampa
            tempTable.AddColumn(columnName: "Difference", formula: "[Austin] - [Tampa]", decimalDigits: 3);
            

            //After the column gets added, the name of the column should be available in the column header collection.
            Assert.IsTrue(tempTable.ColumnHeaders.Contains("Difference"));
            //Checking if the calculation is properly done as per the formula.
            Assert.AreEqual("18",tempTable[0]["Difference"]);
            Assert.AreEqual("15",tempTable[1]["Difference"]);
            Assert.AreEqual("5", tempTable[2]["Difference"]);

            tempTable.RemoveColumn("Difference");

            Assert.IsFalse(tempTable.ColumnHeaders.Contains("Difference"));
        }
        [TestMethod]
        public void ColumnHeadersShouldBeExtractedProperly()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            HashSet<string> columns = new HashSet<string>() { "Name", "Course", "Duration" };
            Assert.IsTrue(t.ColumnHeaders.SequenceEqual(columns));
        }
        [TestMethod]
        public void FilterOnSingleColumn()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);
            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Jenny");
            r3.Add("Course", "F#");
            r3.Add("Duration", "5");
            t.AddRow(r3);


            Assert.AreEqual(0, t.Filter("Name", "Samuel").RowCount);
            Assert.AreEqual(2, t.Filter("Course", "F#").RowCount);
        }
        [TestMethod]
        public void Test_Filter()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "5");

            Assert.AreEqual(2, t.RowCount);
            Assert.AreEqual(1, t.Filter(r2).RowCount);
            Assert.AreEqual(1, t.Filter(r).RowCount);
            Assert.AreEqual(0, t.Filter(r3).RowCount);
        }

        [TestMethod]
        public void CheckAllValuesAreProjectedProperly()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "5");
            t.AddRow(r3);

            Assert.IsTrue(t.ValuesOf("Name").SequenceEqual(new string[] { "Sam", "Jen", "Erik" }));
            Assert.IsTrue(t.ValuesOf("Course").SequenceEqual(new string[] { "C#", "F#", "F#" }));
            Assert.IsTrue(t.ValuesOf("Duration").SequenceEqual(new string[] { "4", "5", "5" }));

        }
        [TestMethod]
        public void Test_Histogram()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "03");
            t.AddRow(r3);
            
            Dictionary<string,int> hist = t.Histogram("Course");
            Assert.AreEqual(2, hist.Count);
            Assert.AreEqual(2, hist["F#"]);
            Assert.AreEqual(1, hist["C#"]);
        }

        [TestMethod]
        public void Test_RemoveDuplicates()
        {

            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);
            //This row is a duplicate of the first row
            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Sam");
            r3.Add("Course", "C#");
            r3.Add("Duration", "4");
            t.AddRow(r3);


            Assert.AreEqual(3, t.RowCount);

            Assert.AreEqual(2, t.Filter("Name", "Sam").RowCount);

            t = t.Distinct();

            Assert.AreEqual(2, t.RowCount);
            Assert.AreEqual(1, t.Filter("Name", "Sam").RowCount);
            Assert.AreEqual(1, t.Filter("Name", "Jen").RowCount);
        }


        [TestMethod]
        public void Test_Sort()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4.0");           
            r.Add("Date", "1/2/2013");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5.0");
            r2.Add("Date", "2/02/2013");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "3.0");
            r3.Add("Date", "03/2/2013");
            t.AddRow(r3);

           // Assert.AreEqual("Erik", t.SortBy("Duration",SortDirection.Ascending)[0]["Name"]);
            Assert.AreEqual("Sam", t.SortBy("Name",false,string.Empty,SortDirection.Ascending)[2]["Name"]);
          //  Assert.AreEqual("Jen", t.SortBy("Duration", SortDirection.Ascending)[2]["Name"]);
            Assert.AreEqual("Erik", t.SortBy("Name",false, string.Empty,SortDirection.Descending)[2]["Name"]);
           // Assert.AreEqual("Erik", t.SortBy("Date", SortDirection.Descending)[0]["Name"]);
            
        }
        [TestMethod]
        public void Test_SortInThisOrder()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Course", "C#");
            r.Add("StartsOn", "Tuesday");
            t.AddRow(r);

            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Course", "F#");
            r2.Add("StartsOn", "Sunday");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Course", "Java");
            r3.Add("StartsOn", "Monday");
            t.AddRow(r3);

            List<string> daysOrder = new List<string>()
            { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            Assert.AreEqual("Sunday", t.SortInThisOrder("StartsOn", inThisOrder: daysOrder)[0]["StartsOn"]);
            Assert.AreEqual("Tuesday", t.SortInThisOrder("StartsOn", inThisOrder: daysOrder, how: SortDirection.Descending)[0]["StartsOn"]);
         }
        [TestMethod]
        public void Test_Top_Bottom()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Course", "C#");
            r.Add("StartsOn", "Tuesday");
            t.AddRow(r);

            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Course", "F#");
            r2.Add("StartsOn", "Sunday");
            t.AddRow(r2);

            Assert.AreEqual(1, t.Top(1).RowCount);
            Assert.AreEqual("C#", t.Top(1)["Course"][0]);
            Assert.AreEqual(1, t.Bottom(1).RowCount);
            Assert.AreEqual("F#", t.Bottom(1)["Course"][0]);
            Assert.AreEqual(2, t.Top(8).RowCount);
            Assert.AreEqual(2, t.Bottom(3).RowCount);
        }
        
        [Ignore("The implementations is broken now. being reimplemented. WIP")]
        [TestMethod]
        public void Test_RunSQLQuery()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "03");
            t.AddRow(r3);
            Table result = t.RunSQLQuery(@"select * from [Table] where course = 'F#' and Duration = 5");
            Assert.AreEqual(1, result.RowCount);

        }
        [TestMethod]
        public void Test_LoadARFF()
        {
            Table play = DataAcquisition.LoadArff(@"..\..\..\Data\weather.nominal.arff");
            Assert.AreEqual(14, play.RowCount);
            Assert.IsTrue((new string[] { "outlook", "temperature", "humidity", "windy", "play" }).All(t => play.ColumnHeaders.Contains(t)));
            Assert.IsTrue(play.ValuesOf("outlook").Distinct().All(m => m== "sunny" || m== "overcast" || m == "rainy" ));

        }
        [TestMethod]
        public void Test_GetAs()
        {
            Table t = new Table();
            Dictionary<string, string> r = new Dictionary<string, string>();
            r.Add("Name", "Sam");
            r.Add("Course", "C#");
            r.Add("Duration", "4");
            t.AddRow(r);
            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jen");
            r2.Add("Course", "F#");
            r2.Add("Duration", "5");
            t.AddRow(r2);

            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Erik");
            r3.Add("Course", "F#");
            r3.Add("Duration", "03");
            t.AddRow(r3);
            
            
            
        }
        [TestMethod]
        public void Test_MergeByColumns()
        {
            //To Do
            //Table 1 Columns
            //Name  | Age | Gender 
            //Sam   | 23  | M
            //Jane  | 19  | F
            //Raskin| 14  | M

            //Table 2 Columns
            //Name | Course 
            //Jane | C#
            //Sam  | F#
            //Raskin| Python


            //Merged Columns in the resultant Table.
            //Name | Age | Gender | Course
            //Sam  | 23  | M      | F#
            //Jane | 19  | F      | C#
            //Raskin| 14 | M      | Python

            Table first = new Table();
            Dictionary<string, string> r1 = new Dictionary<string, string>();
            r1.Add("Name", "Sam");
            r1.Add("Age", "23");
            r1.Add("Gender", "M");

            Dictionary<string, string> r2 = new Dictionary<string, string>();
            r2.Add("Name", "Jane");
            r2.Add("Age", "19");
            r2.Add("Gender", "F");


            Dictionary<string, string> r3 = new Dictionary<string, string>();
            r3.Add("Name", "Raskin");
            r3.Add("Age", "14");
            r3.Add("Gender", "M");

            first.Rows.Add(r1);
            first.Rows.Add(r2);
            first.Rows.Add(r3);


            //Table 2 Columns
            //Name | Course 
            //Jane | C#
            //Sam  | F#
            //Raskin| Python

            Table second = new Table();
            Dictionary<string, string> r21 = new Dictionary<string, string>();
            r21.Add("Name", "Jane");
            r21.Add("Course", "C#");

            Dictionary<string, string> r22 = new Dictionary<string, string>();
            r22.Add("Name", "Sam");
            r22.Add("Course", "F#");

            Dictionary<string, string> r23 = new Dictionary<string, string>();
            r23.Add("Name", "Raskin");
            r23.Add("Course", "Python");

            second.Rows.Add(r21);
            second.Rows.Add(r22);
            second.Rows.Add(r23);

            Table merged =  first.MergeByColumns(second).SortBy("Age");

            Assert.AreEqual(4, merged.ColumnHeaders.Count);
            Assert.AreEqual("Name", merged.ColumnHeaders.ElementAt(0));
            Assert.AreEqual("Age", merged.ColumnHeaders.ElementAt(1));
            Assert.AreEqual("Gender", merged.ColumnHeaders.ElementAt(2));
            Assert.AreEqual("Course", merged.ColumnHeaders.ElementAt(3));
            Assert.AreEqual("Raskin", merged["Name",0]);

             //     .ColumnHeaders
               //  .Count
                 
        }
        [TestMethod]
        public void Test_LoadHTML()
        {
            Table accidents = DataAcquisition.LoadHtmlTable(@"..\..\..\Data\roadaccidents.html");
            
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
            Table births = DataAcquisition.LoadCsv(@"..\..\..\Data\births.csv");
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
        [TestMethod]
        [TestCategory("Feature")]
        
        public void Test_ModifyColumnName()
        {
            Table example = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("As Of", "21");
            row1.Add("1Y Chg", "22");
            row1.Add("5Y Ago", "12");

            example.AddRow(row1);

            Assert.IsTrue(example.ColumnHeaders.ElementAt(0) == "As Of");
            Assert.IsTrue(example.ColumnHeaders.ElementAt(1) == "1Y Chg");
            Assert.IsTrue(example.ColumnHeaders.ElementAt(2) == "5Y Ago");

            //replace the spaces from the name of the columns
            example = example.ModifyColumnName("As Of", "AsOf")
                             .ModifyColumnName("1Y Chg", "1YChg")
                             .ModifyColumnName("5Y Ago", "5YAgo");

            Assert.IsTrue(example.ColumnHeaders.ElementAt(0) == "AsOf");
            Assert.IsTrue(example.ColumnHeaders.ElementAt(1) == "1YChg");
            Assert.IsTrue(example.ColumnHeaders.ElementAt(2) == "5YAgo");

        }
        [TestMethod]
        public void Test_CumulativeSum()
        {
            Table population = new Table();
            Dictionary<string, string> firstYearRecord = new Dictionary<string, string>();
            firstYearRecord.Add("Year", "2003");
            firstYearRecord.Add("Population", "30000");

            population.AddRow(firstYearRecord);

            Dictionary<string, string> secondYearRecord = new Dictionary<string, string>();
            secondYearRecord.Add("Year", "2004");
            secondYearRecord.Add("Population", "35000");

            population.AddRow(secondYearRecord);


            Assert.IsTrue(population.CumulativeFold("Year")[1]["Population"] == "65000");
         
        }
        [TestMethod]
        public void Test_Transform()
        {
            Table sales = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Item", "Monitor");
            row1.Add("Quantity", "41000");
            row1.Add("Month", "January");

            sales.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Item", "Keyboard");
            row2.Add("Quantity", "11000");
            row2.Add("Month", "January");

            sales.AddRow(row2);

            Table transformed = sales.Transform("Quantity", x => (Convert.ToInt32(x) / 1000).ToString());

            Assert.AreEqual("41", transformed["Quantity"][0]);
            Assert.AreEqual("11", transformed["Quantity"][1]);
            
        }
        [TestMethod]
        public void Test_Aggregate()
        {
            Table sales = new Table();
            Dictionary<string, string> row1 = new Dictionary<string, string>();
            row1.Add("Item", "Monitor");
            row1.Add("Quantity", "40");
            row1.Add("Month", "January");

            sales.AddRow(row1);

            Dictionary<string, string> row2 = new Dictionary<string, string>();
            row2.Add("Item", "Keyboard");
            row2.Add("Quantity", "11");
            row2.Add("Month", "January");
            
            sales.AddRow(row2);

            Dictionary<string, string> row3 = new Dictionary<string, string>();
            row3.Add("Item", "Monitor");
            row3.Add("Quantity", "50");
            row3.Add("Month", "February");

            sales.AddRow(row3);

            Table aggregatedOverMonth = sales.Aggregate("Month");
            Assert.AreEqual(2, aggregatedOverMonth.RowCount);
            Assert.AreEqual("51",  aggregatedOverMonth[0]["Quantity"]);
        }
    }
}
