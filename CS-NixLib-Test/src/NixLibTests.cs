using System;
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

    [Test]
    public void LZSTest() {
      Assert.AreEqual(26, Numbers.LeadingZeroStringToInt("00", 26));
      Assert.AreEqual("00", Numbers.IntToLeadingZeroString(26, 26));

      Assert.AreEqual(27, Numbers.LeadingZeroStringToInt("01", 26));
      Assert.AreEqual("01", Numbers.IntToLeadingZeroString(27, 26));

      Assert.AreEqual(0, Numbers.LeadingZeroStringToInt("0", 26));
      Assert.AreEqual("0", Numbers.IntToLeadingZeroString(0, 26));

      Assert.AreEqual(10, Numbers.LeadingZeroStringToInt("00", 10));
      Assert.AreEqual("00", Numbers.IntToLeadingZeroString(10, 10));

      Assert.AreEqual(20, Numbers.LeadingZeroStringToInt("10", 10));
      Assert.AreEqual("10", Numbers.IntToLeadingZeroString(20, 10));

      Assert.Throws(typeof(ArgumentOutOfRangeException), () => { Numbers.LeadingZeroStringToInt("0", 40); });
    }
  }
}