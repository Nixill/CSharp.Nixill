using System.Collections.ObjectModel;
using Nixill.Utils.Extensions;

namespace Nixill.Objects;

public class Digits
{
  Dictionary<char, int> CharToIntMap;
  Dictionary<int, char> IntToCharMap;

  public readonly char NegativeSign;
  public readonly char DecimalPoint;

  public Digits(IEnumerable<char> digits, bool caseSensitive = false, char negative = '-', char decPoint = '.')
  {
    if (!caseSensitive) digits = digits.Select(char.ToUpperInvariant);

    string allDigits = digits.FormString();
    string distinctDigits = digits.Distinct().FormString();

    if (allDigits != distinctDigits) throw new InvalidOperationException($"Repeated digits ({allDigits})");
    if (distinctDigits.Contains(negative)) throw new InvalidOperationException($"Negative symbol repeats a digit");
    if (distinctDigits.Contains(decPoint)) throw new InvalidOperationException($"Decimal point repeats a digit");
    if (negative == decPoint && negative != '\0') throw new InvalidOperationException($"Negative symbol and decimal point are the same");

    CharToIntMap = digits.WithIndex().ToDictionary();
    IntToCharMap = digits.WithIndex().Select(t => (t.Index, t.Item)).ToDictionary();

    if (!caseSensitive)
      foreach ((char c, int i) in digits.WithIndex())
      {
        if (char.IsLetter(c))
        {
          char lower = char.ToLowerInvariant(c);
          CharToIntMap[lower] = i;
        }
      }

    NegativeSign = negative;
    DecimalPoint = decPoint;
  }

  public int Parse(char c)
  {
    if (CharToIntMap.TryGetValue(c, out int i))
      return i;
    throw new FormatException($"The character {c} is not mapped.");
  }

  public char Format(int i)
  {
    if (IntToCharMap.TryGetValue(i, out char c))
      return c;
    throw new FormatException($"The integer {i} is not mapped.");
  }

  public static readonly Digits Base36 = new("0123456789abcdefghijklmnopqrstuvwxyz");
  public static readonly Digits Base64 = new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/", true);
  public static readonly Digits Alpha = new("0ABCDEFGHIJKLMNOPQRSTUVWXYZ");

  public int HighestSupportedBase => CharToIntMap.Count;
}
