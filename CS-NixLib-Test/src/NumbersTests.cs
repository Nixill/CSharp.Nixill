using NUnit.Framework;
using Nixill.Utils;

namespace Nixill.Testing {
  public class NumbersTests {
    [Test]
    public void Test1() {
      Assert.AreEqual(Numbers.IntToString(5, 2), "101");
    }
  }
}