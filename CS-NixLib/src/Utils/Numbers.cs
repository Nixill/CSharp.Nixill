using System;

namespace Nixill.Utils {
  /// <summary>
  /// Provides utilities for working with numbers, including conversion.
  /// </summary>
  public class Numbers {
    /// <summary>
    /// Converts a string in an arbitrary base to an int.
    /// </summary>
    /// <param name="str">The string which should be converted.</param>
    /// <param name="bs">The base from which to convert the number - 2 to
    /// 36.</param>
    public static int StringToInt(string str, int bs) {
      if (bs < 2 || bs > 36) throw new ArgumentOutOfRangeException("intFromString only accepts bases 2 to 36.");

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
      if (i < 48) throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      i -= 48;
      // Characters '0' through '9'
      if (i < 10) return i;
      // Characters preceding 'A'
      else if (i < 17) throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      i -= 7;
      // Characters 'A' through 'Z'
      if (i < 36) return i;
      // Characters preceding 'a'
      else if (i < 42) throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      i -= 32;
      // Characters 'a' through 'z'
      if (i < 36) return i;
      // Characters after 'z'
      throw new ArgumentException("intFromString only accepts alphanumeric characters.");
    }

    /// <summary>
    /// Converts an int to a string in an arbitrary base.
    /// </summary>
    /// <param name="input">The integer which should be converted.</param>
    /// <param name="bs">The base to which to convert the number - 2 to
    /// 36.</param>
    public static string IntToString(int input, int bs) {
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
  }
}