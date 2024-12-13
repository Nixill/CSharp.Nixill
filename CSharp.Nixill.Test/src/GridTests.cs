using System.IO;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Nixill.Collections;

namespace Nixill.Test
{
  public class GridTests
  {
    [Test]
    public void GridManipTest()
    {
      Grid<string> testingGrid = new Grid<string>();

      testingGrid.AddColumn(default(string));
      testingGrid.AddColumn(default(string));
      testingGrid.AddColumn(default(string));

      Assert.AreEqual(0, testingGrid.Size);
      Assert.AreEqual(0, testingGrid.Height);
      Assert.AreEqual(3, testingGrid.Width);

      testingGrid.AddRow(default(string));
      testingGrid.AddRow(default(string));

      Assert.AreEqual(6, testingGrid.Size);
      Assert.AreEqual(2, testingGrid.Height);
      Assert.AreEqual(3, testingGrid.Width);

      // please don't actually do this it makes me cry ;-;
      testingGrid["A1"] = "hello";
      testingGrid["r2c1"] = "world";
      testingGrid[GridReference.XY(1, 0)] = "there";
      testingGrid["c1"] = "beautiful";
      testingGrid[1, 1] = "lovely";
      testingGrid[GridReference.XY(2, 1)] = "day";

      Assert.AreEqual(testingGrid[new GridReference("R2C1")], "world");

      string toCSV = testingGrid.Serialize();

      Assert.AreEqual("hello,there,beautiful\nworld,lovely,day", toCSV);

      Grid<string> toGrid = Grid.Deserialize(toCSV);
      string toCSVAgain = toGrid.Serialize();

      Assert.AreEqual(toCSV, toCSVAgain);

      testingGrid.AddRow(new List<string> { "add", "some", "values" });
      testingGrid.InsertColumn(2, new List<string> { "commas,", "\"quotes\"", "new\nlines" });

      toCSV = testingGrid.Serialize();

      Assert.AreEqual("hello,there,\"commas,\",beautiful\nworld,lovely,\"\"\"quotes\"\"\",day\nadd,some,\"new\nlines\",values", toCSV);

      toGrid = Grid.Deserialize(toCSV);
      toCSVAgain = toGrid.Serialize();

      Assert.AreEqual(toCSV, toCSVAgain);

      // Now test files
      string tmp = Path.GetTempPath();
      string file = tmp + "test.csv";

      testingGrid.SerializeToFile(file);
      toGrid = Grid.DeserializeFromFile(file);
      toCSVAgain = toGrid.Serialize();

      Assert.AreEqual(toCSV, toCSVAgain);
    }

    [Test]
    public void GridColumnsTest()
    {
      // Let's test the Columns thing! :D
      decimal[][] divisionArray = Enumerable.Range(1, 10).Select(x => (decimal)x).Select(
        num => Enumerable.Range(1, 5).Select(x => (decimal)x).Select(den => num / den).ToArray()
      ).ToArray();

      Grid<decimal> divisionTable = new(divisionArray);
      Grid<decimal> divisionTableTransposed = new(divisionTable.Columns);

      foreach (int r in Enumerable.Range(0, 10))
      {
        foreach (int c in Enumerable.Range(0, 5))
        {
          Assert.AreEqual(divisionTable[r, c], divisionTableTransposed[c, r]);
        }
      }
    }
  }
}