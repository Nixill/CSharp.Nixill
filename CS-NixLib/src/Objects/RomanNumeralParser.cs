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

    public long ToLong(string roman) {
      bool neg = false;
      roman = roman.ToUpper();

      if (roman.StartsWith('-')) {
        neg = true;
        roman = roman.Substring(1);
      }

      string[] romanParts = roman.Split("_");
      long ret = 0;

      foreach (string input in romanParts) {
        ret *= 1000;
        ret += GetRomanPart(input, 'M');
      }

      if (neg) return -ret;
      else return ret;
    }

    private int GetRomanPart(string input, char which) {
      if (input == "O" || input == "") return 0;

      string[] pieces = input.Split(which);
      int lastPiece = 0;
      int subPieces = 0;

      foreach (string piece in pieces) {
        subPieces += lastPiece;

        if (piece.Length > 0) {
          lastPiece = which switch
          {
            'M' => GetRomanPart(piece, 'D'),
            'D' => GetRomanPart(piece, 'C'),
            'C' => GetRomanPart(piece, 'L'),
            'L' => GetRomanPart(piece, 'X'),
            'X' => GetRomanPart(piece, 'V'),
            'V' => piece.Length,
            _ => throw new RomanParsingException("Roman numerals can only consist of MDCLXVI_, with a possible - at the beginning, or O for 0.")
          };
        }
      }

      return lastPiece - subPieces + (pieces.Length - 1) * which switch
      {
        'M' => 1000,
        'D' => 500,
        'C' => 100,
        'L' => 50,
        'X' => 10,
        'V' => 5,
        _ => throw new RomanParsingException("Roman numerals can only consist of MDCLXVI_, with a possible - at the beginning, or O for 0.")
      };
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