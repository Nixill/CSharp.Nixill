using Nixill.Objects;
using Nixill.Utils;

namespace Nixill.Utils.Legacy;

[Obsolete]
public static class LZSParser
{
  static readonly Digits digits = new Digits(" 0123456789abcdefghijklmnopqrstuvwxy");

  [Obsolete]
  public static int LeadingZeroStringToInt(string input, int bs)
  {
    long output = NumberConverter.Parse<long>(input, bs, bijective: true, digits: digits);
    if (output > 0) return (int)(output - 1);
    else if (output < 0) return (int)(output + 1);
    else return 0; // yes, that means there are three ways to get zero, but that's how the old parser worked too
  }

  [Obsolete]
  public static string IntToLeadingZeroString(int input, int bs)
  {
    long longInput = input;
    if (input == 0) return "0";
    else if (input > 0) longInput += 1;
    else if (input < 0) longInput -= 1;
    return NumberConverter.Format(longInput, bs, bijective: true, digits: digits);
  }
}