using NUnit.Framework;
using Nixill.Utils;

namespace CS_NixLib_Test
{
  public class NumbersTests
  {
    [Test]
    public void Test1()
    {
      Assert.AreEqual(Numbers.IntToString(5, 2), "101");
    }
  }
}