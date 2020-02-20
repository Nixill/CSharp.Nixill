using System.IO;
using System;
using NUnit.Framework;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;
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

      RomanNumeralParser rnpNone = new RomanNumeralParser(RomanNumeralRules.NONE);

      Assert.AreEqual("I", rnpNone.ToRoman(1));
      Assert.AreEqual("LXVIIII", rnpNone.ToRoman(69));
      Assert.AreEqual("MCCCCXX", rnpNone.ToRoman(1420));
      Assert.AreEqual("XXIIII_DCI", rnpNone.ToRoman(24601));
      Assert.AreEqual("MMMMCC", rnpNone.ToRoman(4200));
      Assert.AreEqual("VIII__DCCLXXXIIII", rnpNone.ToRoman(8000784));
      Assert.AreEqual("-CCXXXVII", rnpNone.ToRoman(-237));
      Assert.AreEqual("-VIIII_CCXXIII_CCCLXXII_XXXVI_DCCCLIIII_DCCLXXV_DCCCVIII", rnpNone.ToRoman(Int64.MinValue));
    }
  }
}