// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Squirrel;

Console.WriteLine("Hello, World!");

var complex = DataAcquisition.LoadCsv("C:\\Users\\Admin\\Documents\\GitHub\\Squirrel\\SquirrelSolution\\ConsoleDemo\\complex.csv");

complex.PrettyDump(rowColor:ConsoleColor.DarkMagenta);