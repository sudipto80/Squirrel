using Squirrel;
using System.Diagnostics;

Stopwatch w = new Stopwatch();
w.Start();
var flights = DataAcquisition.LoadCsv(@"C:\personal\flights.csv");

//(@"C:\Users\Admin\Downloads\customers-100000.csv");//
//@"C:\Users\Admin\Downloads\titanic.csv");
w.Stop();
//Console.WriteLine("Last customer id " +  titanic["Customer Id"][titanic.RowCount - 1]);


Console.WriteLine(flights.RowCount + " "  + w.Elapsed.Seconds + " " + w.Elapsed.Milliseconds);