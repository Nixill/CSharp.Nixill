using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

namespace Nixill.Objects {
  public class RomanNumeralParser {
    private RomanNumeralRules RuleSet;

    public RomanNumeralParser(RomanNumeralRules rules) {
      RuleSet = rules;
    }

    public string ToRoman(long input) {
      string ret = "";

      if (input >= 1000 || input <= -1000) {
        String high = ToRoman(input / 1000);
        if (Regex.IsMatch(high, @"^I+$")) ret = new string('M', high.Length);
        else ret = high + "_";
      }
      else {
        if (input < 0) {
          ret = "-";
        }
      }

      input %= 1000;

      if (input < 0) input = -input;

      while (input > 0) {
        Tuple<int, string> rule = RuleSet.RuleFor((int)input);
        ret += rule.Item2;
        input -= rule.Item1;
      }

      return ret;
    }

    public static long ToLong(string input) {
      bool neg = false;
      long ret = 0;
      int[] vals = { 0, 0, 0, 0, 0, 0, 0 };

      input = input.ToUpper();

      foreach (char c in input) {
        if (c == '-') neg = true;
        else if (c == '_') {
          foreach (int val in vals) {
            ret += val;
          }
          ret *= 1000;
          vals = new int[] { 0, 0, 0, 0, 0, 0, 0 };
        }
        else if (c == 'M') SetVals(vals, 0, 1000);
        else if (c == 'D') SetVals(vals, 1, 500);
        else if (c == 'C') SetVals(vals, 2, 100);
        else if (c == 'L') SetVals(vals, 3, 50);
        else if (c == 'X') SetVals(vals, 4, 10);
        else if (c == 'V') SetVals(vals, 5, 5);
        else if (c == 'I') vals[6] += 1;
        else if (c == 'O') { }
        else throw new RomanParsingException("Only characters -=MDCLXVIO are valid in Roman numerals.");
      }

      foreach (int val in vals) {
        ret += val;
      }

      if (neg) ret = -ret;

      return ret;
    }

    static void SetVals(int[] vals, int pos, int val) {
      vals[pos] += val;
      for (int i = pos + 1; i < 7; i++) {
        vals[pos] -= vals[i];
        vals[i] = 0;
      }
    }
  }

  public class RomanNumeralRules {
    public static readonly RomanNumeralRules NONE = new RomanNumeralRules(new Dictionary<int, string>());
    public static readonly RomanNumeralRules COMMON = new RomanNumeralRules(new Dictionary<int, string> {
      [4] = "IV",
      [9] = "IX",
      [40] = "XL",
      [90] = "XC",
      [400] = "CD",
      [900] = "CM"
    });

    private Dictionary<int, string> Rules;

    private static readonly Dictionary<int, string> _Defaults = new Dictionary<int, string> {
      [0] = "",
      [1] = "I",
      [2] = "II",
      [3] = "III",
      [4] = "IIII",
      [5] = "V",
      [10] = "X",
      [20] = "XX",
      [30] = "XXX",
      [40] = "XXXX",
      [50] = "L",
      [100] = "C",
      [200] = "CC",
      [300] = "CCC",
      [400] = "CCCC",
      [500] = "D"
    };

    private static readonly int[] _Divisors = new int[] { 1, 5, 10, 50, 100, 500 };

    public RomanNumeralRules(IDictionary<int, string> rules) {
      Rules = new Dictionary<int, string>(rules);
    }

    public Tuple<int, string> RuleFor(int val) {
      if (val > 999 || val < 0) throw new ArgumentOutOfRangeException("RuleFor", "RuleFor only accepts values 0 to 999.");

      foreach (int i in _Divisors) {
        int delta = val % i;
        val -= delta;

        if (Rules.ContainsKey(val)) return new Tuple<int, string>(val, Rules[val]);
        else if (_Defaults.ContainsKey(val)) return new Tuple<int, string>(val, _Defaults[val]);
      }

      // this should never be reached but the compiler doesn't know that
      return new Tuple<int, string>(0, "");
    }
  }

  public class RomanParsingException : ArgumentException {
    public RomanParsingException(string message) : base(message) { }
  }
}