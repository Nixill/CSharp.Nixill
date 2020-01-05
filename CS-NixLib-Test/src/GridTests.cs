using NUnit.Framework;
using Nixill.Collections.Grid;

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
    }
  }
}