using NUnit.Framework;
using Nixill.Utils;

namespace Nixill.Test {
  public class NixLibTests {
    [Test]
    public void Test1() {
      Assert.AreEqual(Numbers.IntToString(5, 2), "101");
    }

    [Test]
    public void CipherTest() {
      Cipher noVowels = new Cipher("aeiouAEIOU", "\0\0\0\0\0\0\0\0\0\0");
      Assert.AreEqual(noVowels.Apply("The quick brown fox jumps over the lazy dog!"), "Th qck brwn fx jmps vr th lzy dg!");
      Assert.False(noVowels.Reversible);

      Cipher swapCase = new Cipher("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM", "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm");
      Assert.AreEqual(swapCase.Apply("Hello world!"), "hELLO WORLD!");
      Assert.True(swapCase.Reversible);

      Cipher stringMask = new Cipher("xx", "hs");
      Assert.AreEqual(stringMask.Apply("ax txe wind blows"), "as the wind blows");
      Assert.False(stringMask.Reversible);
    }
  }
}