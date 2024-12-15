using System.Collections.ObjectModel;
using Nixill.Utils.Extensions;

namespace Nixill.Objects;

/// <summary>
///   Represents a set of digits, in an arbitrary set of bases, for a
///   conversion between a string and a number.
/// </summary>
public class Digits
{
  Dictionary<char, int> CharToIntMap;
  Dictionary<int, char> IntToCharMap;

  /// <summary>
  ///   Read-only: The negative sign for this set of digits.
  /// </summary>
  public readonly char NegativeSign;

  /// <summary>
  ///   Read-only: The decimal point for this set of digits.
  /// </summary>
  public readonly char DecimalPoint;

  /// <summary>
  ///   Constructs a set of digits.
  /// </summary>
  /// <param name="digits">
  ///   The digits, in order from the digit representing 0 upwards.
  /// </param>
  /// <param name="caseSensitive">
  ///   Whether or not this set of digits is case-sensitive.
  /// </param>
  /// <param name="negative">
  ///   The negative sign for this set of digits.
  /// </param>
  /// <param name="decPoint">
  ///   The decimal point for this set of digits.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   The same character is used for multiple digits.
  ///   <para/>
  ///   Or, the negative symbol repeats one of the digit characters.
  ///   <para/>
  ///   Or, the decimal point repeats one of the digit characters.
  ///   <para/>
  ///   Or, the negative symbol and decimal point repeat each other, and
  ///   are not the character <c>\0</c>.
  /// </exception>
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

  /// <summary>
  ///   Parses a character to a number based on its position within this
  ///   set of digits.
  /// </summary>
  /// <remarks>
  ///   The base is not a parameter because it is irrelevant to this
  ///   method. For example, in <see cref="Base36">standard digits</see>,
  ///   the digit <c>b</c> still has the value of 11, and that is true of
  ///   any base of at least 12.
  /// </remarks>
  /// <param name="c">The character.</param>
  /// <returns>The numeric value.</returns>
  /// <exception cref="FormatException">
  ///   The character is not present in this set of digits.
  /// </exception>
  public int Parse(char c)
  {
    if (CharToIntMap.TryGetValue(c, out int i))
      return i;
    throw new FormatException($"The character {c} is not mapped.");
  }

  /// <summary>
  ///   Formats a number to a character based on its position within this
  ///   set of digits.
  /// </summary>
  /// <remarks>
  ///   The base is not a parameter because it is irrelevant to this
  ///   method. For example, in <see cref="Base36">standard digits</see>,
  ///   the numeric value 11 is mapped to the digit <c>b</c> in any base
  ///   of at least 12.
  /// </remarks>
  /// <param name="i">The numeric value.</param>
  /// <returns>The character.</returns>
  /// <exception cref="FormatException">
  ///   There are not enough digits to reach the specified numeric value.
  ///   <para/>
  ///   Or, the specified numeric value is negative.
  /// </exception>
  public char Format(int i)
  {
    if (IntToCharMap.TryGetValue(i, out char c))
      return c;
    throw new FormatException($"The integer {i} is not mapped.");
  }

  /// <summary>
  ///   Read-only: "Standard digits", used widely in numbering systems up
  ///   to base 36.
  /// </summary>
  /// <remarks>
  ///   <c>0123456789abcdefghijklmnopqrstuvwxyz</c>
  /// </remarks>
  public static readonly Digits Base36 = new("0123456789abcdefghijklmnopqrstuvwxyz");

  /// <summary>
  ///   Read-only: Base-64 encoding digits.
  /// </summary>
  /// <remarks>
  ///   <c>ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/</c>
  /// </remarks>
  public static readonly Digits Base64 = new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/", true);

  /// <summary>
  ///   Read-only: Alphabetic characters used for bijective numeration.
  /// </summary>
  /// <remarks>
  ///   <c>0ABCDEFGHIJKLMNOPQRSTUVWXYZ</c>
  /// </remarks>
  public static readonly Digits Alpha = new("0ABCDEFGHIJKLMNOPQRSTUVWXYZ");

  /// <summary>
  ///   Get: The highest supported base of the set of digits, which is
  ///   simply equal to the number of distinct digits represented.
  /// </summary>
  public int HighestSupportedBase => CharToIntMap.Count;
}
