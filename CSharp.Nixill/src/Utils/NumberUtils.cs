using System.Text.RegularExpressions;
using System;

namespace Nixill.Utils {
  /// <summary>
  /// Provides utilities for working with numbers, including conversion.
  /// </summary>
  public class NumberUtils {
    /// <summary>
    /// Converts a string in an arbitrary base to an int.
    /// </summary>
    /// <param name="str">The string which should be converted.</param>
    /// <param name="bs">The base from which to convert the number - 2 to
    /// 36.</param>
    public static int StringToInt(string str, int bs) {
      if (bs < 2 || bs > 36) throw new ArgumentOutOfRangeException("StringToInt only accepts bases 2 to 36.");

      int ret = 0;
      int sgn = 1;
      if (str.StartsWith("-")) {
        str = str.Substring(1);
        sgn = -1;
      }

      foreach (char chr in str) {
        ret *= bs;
        int add = CharToInt(chr);
        if (add >= bs) throw new ArgumentOutOfRangeException($"{chr} is not a valid base {bs} digit.");
        ret += add;
      }

      return sgn * ret;
    }

    /// <summary>
    /// Converts a character, which is a digit in any base of greater
    /// value than that digit, to its base 10 int value.
    ///
    /// This method doesn't take a <c>base</c> parameter because the base
    /// doesn't alter the value of the digit, e.g. <c>c</c> has value 12,
    /// whether expressed in base 13 or base 33.
    /// </summary>
    /// <param name="chr">The character to convert.</param>
    public static int CharToInt(char chr) {
      int i = (int)chr;
      // Characters preceding '0'
      if (i < 48) throw new ArgumentException("CharToInt only accepts alphanumeric characters.");
      i -= 48;
      // Characters '0' through '9'
      if (i < 10) return i;
      // Characters preceding 'A'
      else if (i < 17) throw new ArgumentException("CharToInt only accepts alphanumeric characters.");
      i -= 7;
      // Characters 'A' through 'Z'
      if (i < 36) return i;
      // Characters preceding 'a'
      else if (i < 42) throw new ArgumentException("CharToInt only accepts alphanumeric characters.");
      i -= 32;
      // Characters 'a' through 'z'
      if (i < 36) return i;
      // Characters after 'z'
      throw new ArgumentException("CharToInt only accepts alphanumeric characters.");
    }

    /// <summary>
    /// Converts an int to a string in an arbitrary base.
    /// </summary>
    /// <param name="input">The integer which should be converted.</param>
    /// <param name="bs">The base to which to convert the number - 2 to
    /// 36.</param>
    public static string IntToString(int input, int bs) {
      if (bs < 2 || bs > 36) throw new ArgumentOutOfRangeException("IntToString only accepts bases 2 to 36.");

      string ret = "";
      bool add1 = false;
      string neg = "";

      while (input != 0) {
        // • If add1 is true, add 1 to input and make add1 false
        if (add1) {
          input += 1;
          add1 = false;
        }

        // • If input is less than zero, set it to -(input+1) and make add1 and negative true
        if (input < 0) {
          input = -(input + 1);
          add1 = true;
          neg = "-";
        }

        // • The integer digit should be input % bs.
        int digit = input % bs;

        // • If add1 is true, add 1 to digit and make add1 false.
        if (add1) {
          digit += 1;
          add1 = false;
        }

        // • If digit is bs, make digit 0 and make add1 true.
        if (digit == bs) {
          digit = 0;
          add1 = true;
        }

        // • Convert digit to a letter, and put that at the beginning of ret
        ret = IntToChar(digit) + ret;

        // • Also subtract digit from input, then divide input by base.
        input -= digit;
        input /= bs;
      }

      // • If the loop never executed(ret is an empty string), return the string 0.
      if (ret == "") return "0";

      // • If this statement is reached, return ret.
      return neg + ret;
    }

    /// <summary>
    /// Converts an integer to a character representing a digit of the
    /// given value in any higher base.
    ///
    /// This method doesn't take a <c>base</c> parameter because the base
    /// doesn't alter the value of the digit, e.g. value 12 is <c>c</c>,
    /// whether expressed in base 13 or base 33.
    /// </summary>
    /// <param name="chr">The character to convert.</param>
    public static char IntToChar(int i) {
      if (i < 0 || i > 35) throw new ArgumentOutOfRangeException("Only digits 0 to 35 can be converted to chars.");

      // digits 0 through 9
      if (i < 10) return (char)(i + 48);
      // letters A through Z
      else return (char)(i + 55);
    }

    /// <summary>
    /// Returns true iff the source number has all of the target bits.
    ///
    /// Specifically, returns <c>src &amp; trg == trg</c>.
    ///
    /// Special case: If <c>trg</c> is 0, returns <c>src == 0</c>.
    /// </summary>
    public static bool HasAllBits(int src, int trg) {
      if (trg == 0) return src == 0;
      else return (src & trg) == trg;
    }

    /// <summary>
    /// Returns true iff the source number has any of the target bits.
    ///
    /// Specifically, returns <c>src &amp; trg != 0</c>.
    ///
    /// Special case: If <c>trg</c> is 0, returns <c>src != 0</c>.
    /// </summary>
    public static bool HasAnyBits(int src, int trg) {
      if (trg == 0) return src != 0;
      else return (src & trg) != 0;
    }

    /// <string>
    /// Converts a number given in Leading Zero format to an int.
    ///
    /// Leading Zero format is a sequence in which numbers which start
    /// with zero are distinct from numbers which do not. For example,
    /// base 10 would count as follows: <c>0, 1, 2, ..., 9, 00, 01, ...,
    /// 09, 10, 11, ..., 99, 000, 001, ...</c>
    ///
    /// With Leading Zero format, base 1 is a valid base (<c>0, 00, 000,
    /// ...</c>). The highest valid base is 36.
    /// </string>
    public static int LeadingZeroStringToInt(string input, int bs) {
      if (bs < 1 || bs > 36) throw new ArgumentOutOfRangeException("LeadingZeroStringToInt only accepts bases 1 to 36.");

      if (bs == 1) {
        if (!Regex.IsMatch(input, "-?0+")) throw new ArgumentException("Not a valid base 1 input");
        if (input.StartsWith('-')) return -(input.Length - 2);
        else return input.Length - 1;
      }

      int root = 0;
      int neg = 1;

      if (input.StartsWith('-')) {
        neg = -1;
        input = input.Substring(1);
      }

      for (int i = 1; i < input.Length; i++) {
        root *= bs;
        root += bs;
      }

      while (input.StartsWith('0')) {
        input = input.Substring(1);
      }

      return neg * (root + StringToInt(input, bs));
    }

    /// <summary>
    /// Converts an integer to a textual number in Leading Zero format.
    ///
    /// See
    /// <a cref="Numbers.LeadingZeroStringToInt(string, int)">LeadingZeroStringToInt()</a>
    /// for more details on what Leading Zero format is.
    /// </summary>
    public static string IntToLeadingZeroString(int input, int bs) {
      long root = 0;
      long lroot = 0;
      int len = 0;
      string neg = "";
      uint abs;

      if (bs == 1) {
        if (input < 0) return "-" + new String('0', -input + 1);
        else return new String('0', input + 1);
      }

      if (input < 0) {
        abs = (uint)(-input);
        neg = "-";
      }
      else abs = (uint)input;

      do {
        lroot = root;
        len += 1;

        root *= bs;
        root += bs;
      } while (root > lroot && root <= abs);

      abs -= (uint)lroot;

      string ret = IntToString((int)abs, bs);
      len -= ret.Length;
      ret = neg + new string('0', len) + ret;

      return ret;
    }
  }
}