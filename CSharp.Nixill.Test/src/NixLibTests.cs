using System;
using NUnit.Framework;
using Nixill.Utils;
using Nixill.Objects;
using System.Text.RegularExpressions;
using Nixill.Collections;
using System.Linq;

namespace Nixill.Test {
  public class NixLibTests {
    [Test]
    public void Test1() {
      Assert.AreEqual(NumberUtils.IntToString(5, 2), "101");
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
      Assert.AreEqual(26, NumberUtils.LeadingZeroStringToInt("00", 26));
      Assert.AreEqual("00", NumberUtils.IntToLeadingZeroString(26, 26));

      Assert.AreEqual(27, NumberUtils.LeadingZeroStringToInt("01", 26));
      Assert.AreEqual("01", NumberUtils.IntToLeadingZeroString(27, 26));

      Assert.AreEqual(0, NumberUtils.LeadingZeroStringToInt("0", 26));
      Assert.AreEqual("0", NumberUtils.IntToLeadingZeroString(0, 26));

      Assert.AreEqual(10, NumberUtils.LeadingZeroStringToInt("00", 10));
      Assert.AreEqual("00", NumberUtils.IntToLeadingZeroString(10, 10));

      Assert.AreEqual(20, NumberUtils.LeadingZeroStringToInt("10", 10));
      Assert.AreEqual("10", NumberUtils.IntToLeadingZeroString(20, 10));

      Assert.Throws(typeof(ArgumentOutOfRangeException), () => { NumberUtils.LeadingZeroStringToInt("0", 40); });
    }

    [Test]
    public void RegexTest() {
      Match mtc = null;
      Regex rgx = new Regex(@"(...).*\1");

      Assert.True(rgx.TryMatch("allochirally", out mtc));
      Assert.True(mtc.TryGroup(1, out string val));
      Assert.AreEqual("all", val);

      Assert.True(rgx.TryMatch("mathematic", out mtc));
      Assert.True(mtc.TryGroup(1, out val));
      Assert.AreEqual("mat", val);
      Assert.False(mtc.TryGroup(2, out val));

      Assert.False(rgx.TryMatch("nonimitative", out mtc));
    }

    [Test]
    public void AVLSearchAroundTest() {
      var set = new AVLTreeSet<int> { 16, 2, 18, 4, 20, 6, 22, 8, 24, 10, 26, 12, 28, 14, 30 };

      TestValues(set.SearchAround(5), 4, null, 6);
      TestValues(set.SearchAround(1), null, null, 2);
      TestValues(set.SearchAround(10), 8, 10, 12);
      TestValues(set.SearchAround(30), 28, 30, null);
      TestValues(set.SearchAround(16), 14, 16, 18);
    }

    [Test]
    public void ChunkWhileTest() {
      string[] words = EnumerableUtils
        .Of('a', 'b', 'c', ' ', 'd', 'e', ' ', 'f')
        .ChunkWhile(chr => chr != ' ')
        .Select(chunk => chunk.FormString())
        .ToArray();

      Assert.AreEqual(words.Length, 3);
      Assert.AreEqual(words[0], "abc");
      Assert.AreEqual(words[1], "de");
      Assert.AreEqual(words[2], "f");
    }

    public void TestValues(NodeTriplet<int> ints, int? lower, int? equal, int? higher) {
      if (lower.HasValue) {
        Assert.True(ints.HasLesserValue);
        Assert.AreEqual(ints.LesserValue, lower.Value);
      }
      else {
        Assert.False(ints.HasLesserValue);
      }

      if (equal.HasValue) {
        Assert.True(ints.HasEqualValue);
        Assert.AreEqual(ints.EqualValue, equal.Value);
      }
      else {
        Assert.False(ints.HasEqualValue);
      }

      if (higher.HasValue) {
        Assert.True(ints.HasGreaterValue);
        Assert.AreEqual(ints.GreaterValue, higher.Value);
      }
      else {
        Assert.False(ints.HasGreaterValue);
      }
    }
  }
}