using System;

namespace Nixill.Utils {
  public class Numbers {
    public static int IntFromString(string str, int bs) {
      if (bs < 2 || bs > 36) {
        throw new ArgumentOutOfRangeException("intFromString only accepts bases 2 to 36.");
      }

      int ret = 0;
      int neg = 1;
      if (str.StartsWith("-")) {
        str = str.Substring(1);
        neg = -1;
      }

      foreach (char chr in str) {
        ret *= bs;
        int add = IntFromChar(chr);
        if (add >= bs) {
          throw new ArgumentOutOfRangeException($"{chr} is not a valid base {bs} digit.");
        }
        ret += add;
      }

      return neg * ret;
    }

    public static int IntFromChar(char chr) {
      int i = (int)chr;
      // Characters preceding '0'
      if (i < 48) {
        throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      }
      i -= 48;
      // Characters '0' through '9'
      if (i < 10) {
        return i;
      }
      // Characters preceding 'A'
      else if (i < 17) {
        throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      }
      i -= 7;
      // Characters 'A' through 'Z'
      if (i < 36) {
        return i;
      }
      // Characters preceding 'a'
      else if (i < 42) {
        throw new ArgumentException("intFromString only accepts alphanumeric characters.");
      }
      i -= 32;
      // Characters 'a' through 'z'
      if (i < 36) {
        return i;
      }
      // Characters after 'z'
      throw new ArgumentException("intFromString only accepts alphanumeric characters.");
    }

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

    public static char IntToChar(int i) {
      if (i < 0 || i > 35) {
        throw new ArgumentOutOfRangeException("Only digits 0 to 35 can be converted to chars.");
      }

      // digits 0 through 9
      if (i < 10) return (char)(i + 48);
      // letters A through Z
      else return (char)(i + 55);
    }
  }
}