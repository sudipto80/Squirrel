using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Squirrel;
using Squirrel.DataVisualization;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SquirrelUnitTest;
[TestClass]
public class ColorPickerTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var colors =
            DynamicColorPicker.GetColorsForScheme(ColorScheme.Rainbow, 10);
        var hexes = colors.Select(t => t.ToHex()).ToList();
        
    }
}