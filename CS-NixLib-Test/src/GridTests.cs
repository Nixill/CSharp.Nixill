using System;
using NUnit.Framework;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;

namespace Nixill.Test {
  public class GridTests {
    [Test]
    public void GridManipTest() {
      Grid<string> testingGrid = new Grid<string>();

      testingGrid.AddColumn();
      testingGrid.AddColumn();
      testingGrid.AddColumn();

      Assert.AreEqual(0, testingGrid.Size);
      Assert.AreEqual(0, testingGrid.Height);
      Assert.AreEqual(3, testingGrid.Width);

      testingGrid.AddRow();
      testingGrid.AddRow();

      Assert.AreEqual(6, testingGrid.Size);
      Assert.AreEqual(2, testingGrid.Height);
      Assert.AreEqual(3, testingGrid.Width);

      testingGrid["A1"] = "hello";
      testingGrid["r2c1"] = "world";
      testingGrid["B1"] = "there";
      testingGrid["c1"] = "beautiful";
      testingGrid[1, 1] = "lovely";
      testingGrid[1, 2] = "day";

      Assert.AreEqual(testingGrid[(GridReference)new Tuple<int, int>(1, 0)], "world");

      string toCSV = CSVParser.GridToString(testingGrid);

      Assert.AreEqual(toCSV, "hello,there,beautiful\nworld,lovely,day");

      Grid<string> toGrid = CSVParser.StringToGrid(toCSV);
      string toCSVAgain = CSVParser.GridToString(toGrid);

      Assert.AreEqual(toCSV, toCSVAgain);
    }
  }
}