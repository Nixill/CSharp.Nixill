using System.Linq;
using System;
using NUnit.Framework;
using Nixill.Objects;
using System.Collections.Generic;

namespace Nixill.Test {
  public class RomanTests {
    [Test]
    public void NumToRomanTest() {
      RomanNumeralParser rnpCommon = new RomanNumeralParser(RomanNumeralRules.COMMON);

      Assert.AreEqual("I", rnpCommon.ToRoman(1));
      Assert.AreEqual("LXIX", rnpCommon.ToRoman(69));
      Assert.AreEqual("MCDXX", rnpCommon.ToRoman(1420));
      Assert.AreEqual("XXIV_DCI", rnpCommon.ToRoman(24601));
      Assert.AreEqual("IV_CC", rnpCommon.ToRoman(4200));
      Assert.AreEqual("VIII__DCCLXXXIV", rnpCommon.ToRoman(8000784));
      Assert.AreEqual("-CCXXXVII", rnpCommon.ToRoman(-237));
      Assert.AreEqual("-IX_CCXXIII_CCCLXXII_XXXVI_DCCCLIV_DCCLXXV_DCCCVIII", rnpCommon.ToRoman(Int64.MinValue));
      Assert.AreEqual("DCCXXXI", rnpCommon.ToRoman(731));
      Assert.AreEqual("CDL_CCXCV", rnpCommon.ToRoman(450295));

      RomanNumeralParser rnpNone = new RomanNumeralParser(RomanNumeralRules.NONE);

      Assert.AreEqual("I", rnpNone.ToRoman(1));
      Assert.AreEqual("LXVIIII", rnpNone.ToRoman(69));
      Assert.AreEqual("MCCCCXX", rnpNone.ToRoman(1420));
      Assert.AreEqual("XXIIII_DCI", rnpNone.ToRoman(24601));
      Assert.AreEqual("MMMMCC", rnpNone.ToRoman(4200));
      Assert.AreEqual("VIII__DCCLXXXIIII", rnpNone.ToRoman(8000784));
      Assert.AreEqual("-CCXXXVII", rnpNone.ToRoman(-237));
      Assert.AreEqual("-VIIII_CCXXIII_CCCLXXII_XXXVI_DCCCLIIII_DCCLXXV_DCCCVIII", rnpNone.ToRoman(Int64.MinValue));
      Assert.AreEqual("DCCXXXI", rnpNone.ToRoman(731));
      Assert.AreEqual("-CCCCL_CCLXXXXV", rnpNone.ToRoman(-450295));

      RomanNumeralParser rnpMinimal = new RomanNumeralParser(new RomanNumeralRules(new Dictionary<int, string> {
        [4] = "IV",
        [9] = "IX",
        [40] = "XL",
        [45] = "VL",
        [49] = "IL",
        [90] = "XC",
        [95] = "VC",
        [99] = "IC",
        [400] = "CD",
        [450] = "LD",
        [490] = "XD",
        [495] = "VD",
        [499] = "ID",
        [900] = "CM",
        [950] = "LM",
        [990] = "XM",
        [995] = "VM",
        [999] = "IM",
      }));

      Assert.AreEqual("I", rnpNone.ToRoman(1));
      Assert.AreEqual("LXVIIII", rnpNone.ToRoman(69));
      Assert.AreEqual("MCCCCXX", rnpNone.ToRoman(1420));
      Assert.AreEqual("XXIIII_DCI", rnpNone.ToRoman(24601));
      Assert.AreEqual("MMMMCC", rnpNone.ToRoman(4200));
      Assert.AreEqual("VIII__DCCLXXXIIII", rnpNone.ToRoman(8000784));
      Assert.AreEqual("-CCXXXVII", rnpNone.ToRoman(-237));
      Assert.AreEqual("-VIIII_CCXXIII_CCCLXXII_XXXVI_DCCCLIIII_DCCLXXV_DCCCVIII", rnpNone.ToRoman(Int64.MinValue));
      Assert.AreEqual("DCCXXXI", rnpNone.ToRoman(731));
      Assert.AreEqual("-CCCCL_CCLXXXXV", rnpNone.ToRoman(-450295));
    }

    [Test]
    public void RomanToNumTest() {
      Assert.AreEqual(-237, RomanNumeralParser.ToLong("-CCXXXVII"));
      Assert.AreEqual(-450295, RomanNumeralParser.ToLong("-CCCCL_CCLXXXXV"));
      Assert.AreEqual(-450295, RomanNumeralParser.ToLong("-LD_CCVC"));
      Assert.AreEqual(1, RomanNumeralParser.ToLong("I"));
      Assert.AreEqual(1420, RomanNumeralParser.ToLong("MCCCCXX"));
      Assert.AreEqual(1420, RomanNumeralParser.ToLong("MCDXX"));
      Assert.AreEqual(24601, RomanNumeralParser.ToLong("XXIIII_DCI"));
      Assert.AreEqual(24601, RomanNumeralParser.ToLong("XXIV_DCI"));
      Assert.AreEqual(4200, RomanNumeralParser.ToLong("IV_CC"));
      Assert.AreEqual(4200, RomanNumeralParser.ToLong("MMMMCC"));
      Assert.AreEqual(450295, RomanNumeralParser.ToLong("CDL_CCXCV"));
      Assert.AreEqual(69, RomanNumeralParser.ToLong("LXIX"));
      Assert.AreEqual(69, RomanNumeralParser.ToLong("LXVIIII"));
      Assert.AreEqual(731, RomanNumeralParser.ToLong("DCCXXXI"));
      Assert.AreEqual(8000784, RomanNumeralParser.ToLong("VIII__DCCLXXXIIII"));
      Assert.AreEqual(8000784, RomanNumeralParser.ToLong("VIII__DCCLXXXIV"));
      Assert.AreEqual(Int64.MinValue, RomanNumeralParser.ToLong("-IX_CCXXIII_CCCLXXII_XXXVI_DCCCLIV_DCCLXXV_DCCCVIII"));
      Assert.AreEqual(Int64.MinValue, RomanNumeralParser.ToLong("-VIIII_CCXXIII_CCCLXXII_XXXVI_DCCCLIIII_DCCLXXV_DCCCVIII"));
    }

    [Test]
    public void RomanAndNumTest() {
      RomanNumeralParser rnpMinimal = new RomanNumeralParser(new RomanNumeralRules(new Dictionary<int, string> {
        [4] = "IV",
        [9] = "IX",
        [40] = "XL",
        [45] = "VL",
        [49] = "IL",
        [90] = "XC",
        [95] = "VC",
        [99] = "IC",
        [400] = "CD",
        [450] = "LD",
        [490] = "XD",
        [495] = "VD",
        [499] = "ID",
        [900] = "CM",
        [950] = "LM",
        [990] = "XM",
        [995] = "VM",
        [999] = "IM",
      }));
      RomanNumeralParser rnpNone = new RomanNumeralParser(RomanNumeralRules.NONE);
      RomanNumeralParser rnpCommon = new RomanNumeralParser(RomanNumeralRules.COMMON);

      RomanNumeralParser[] rnps = { rnpNone, rnpCommon, rnpMinimal };

      foreach (RomanNumeralParser rnp in rnps) {
        foreach (int i in Enumerable.Range(0, 2000)) {
          string roman = rnp.ToRoman(i);
          long i2 = RomanNumeralParser.ToLong(roman);
          Assert.AreEqual(i, i2);
        }
      }
    }
  }
}