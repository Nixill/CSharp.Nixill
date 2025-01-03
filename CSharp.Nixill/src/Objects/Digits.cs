using System.Collections;
using System.Collections.ObjectModel;
using Nixill.Utils.Extensions;

namespace Nixill.Objects;

/// <summary>
///   Represents a set of digits, in an arbitrary set of bases, for a
///   conversion between a string and a number.
/// </summary>
public class Digits : IEnumerable<(char, int)>
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
  ///   Read-only: Whether or not this set of digits is case-sensitive.
  /// </summary>
  public readonly CaseOption CaseSetting;

  /// <summary>
  ///   Constructs a set of digits.
  /// </summary>
  /// <param name="digits">
  ///   The digits, in order from the digit representing 0 upwards.
  /// </param>
  /// <param name="caseSetting">
  ///   Whether or not this set of digits is case-sensitive.
  /// </param>
  /// <param name="negative">
  ///   The negative sign for this set of digits.
  /// </param>
  /// <param name="decPoint">
  ///   The decimal point for this set of digits.
  /// </param>
  /// <param name="caseSetting">
  ///   The case setting for this digit set. If it's not
  ///   <see cref="CaseOption.CaseSensitive"/>, then it can also be
  ///   overridden on a per-format basis.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   The same character is used for multiple digits.
  ///   <para/>
  ///   Or, the case setting is not <see cref="CaseOption.CaseSensitive"/>
  ///   and the upper- and lowercase versions of the same character are
  ///   used for multiple digits.
  ///   <para/>
  ///   Or, the negative symbol repeats one of the digit characters.
  ///   <para/>
  ///   Or, the decimal point repeats one of the digit characters.
  ///   <para/>
  ///   Or, the negative symbol and decimal point repeat each other, and
  ///   are not the character <c>\0</c>.
  /// </exception>
  public Digits(IEnumerable<char> digits, CaseOption caseSetting = CaseOption.FormatAsEntered,
    char negative = '-', char decPoint = '.')
  {
    StringComparison strCompare = caseSetting == CaseOption.CaseSensitive
      ? StringComparison.InvariantCulture
      : StringComparison.InvariantCultureIgnoreCase;

    string allDigits = digits.FormString();
    string distinctDigits = (caseSetting != CaseOption.CaseSensitive
      ? digits.Select(char.ToLower) : digits).Distinct().FormString();

    if (allDigits.Equals(distinctDigits, strCompare)) throw new InvalidOperationException($"Repeated digits ({allDigits})");
    if (distinctDigits.Contains(negative, strCompare)) throw new InvalidOperationException($"Negative symbol repeats a digit");
    if (distinctDigits.Contains(decPoint, strCompare)) throw new InvalidOperationException($"Decimal point repeats a digit");
    if ((caseSetting == CaseOption.CaseSensitive
        ? (negative == decPoint)
        : (char.ToLowerInvariant(negative) == char.ToLowerInvariant(decPoint))) && negative != '\0')
      throw new InvalidOperationException($"Negative symbol and decimal point are the same");

    CharToIntMap = [];
    IntToCharMap = [];

    foreach ((char c, int i) in digits.WithIndex())
    {
      if (caseSetting == CaseOption.CaseSensitive)
      {
        CharToIntMap[c] = i;
      }
      else
      {
        CharToIntMap[char.ToLowerInvariant(c)] = CharToIntMap[char.ToUpperInvariant(c)] = i;
      }

      if (caseSetting == CaseOption.FormatUppercase)
      {
        IntToCharMap[i] = char.ToUpperInvariant(c);
      }
      else if (caseSetting == CaseOption.FormatLowercase)
      {
        IntToCharMap[i] = char.ToLowerInvariant(c);
      }
      else
      {
        IntToCharMap[i] = c;
      }
    }

    NegativeSign = negative;
    DecimalPoint = decPoint;
    CaseSetting = caseSetting;
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
  /// <param name="caseSetting">
  ///   The case setting for this call.
  ///   <para/>
  ///   This is ignored if the object's <see cref="CaseSetting"/> is
  ///   <see cref="CaseOption.CaseSensitive"/>.
  /// </param>
  /// <returns>The character.</returns>
  /// <exception cref="FormatException">
  ///   There are not enough digits to reach the specified numeric value.
  ///   <para/>
  ///   Or, the specified numeric value is negative.
  /// </exception>
  public char Format(int i, CaseOption caseSetting = CaseOption.FormatAsEntered)
  {
    if (CaseSetting == CaseOption.CaseSensitive)
      caseSetting = CaseOption.CaseSensitive;

    if (IntToCharMap.TryGetValue(i, out char c))
    {
      if (caseSetting == CaseOption.FormatUppercase) return char.ToUpperInvariant(c);
      else if (caseSetting == CaseOption.FormatLowercase) return char.ToLowerInvariant(c);
      else return c;
    }
    throw new FormatException($"The integer {i} is not mapped.");
  }

  /// <summary>
  ///   Get: The highest supported base of the set of digits, which is
  ///   simply equal to the number of distinct digits represented.
  /// </summary>
  public int HighestSupportedBase => CharToIntMap.Count;

  /// <summary>
  ///   Returns a sequence over all the characters that are part of the
  ///   digit set, and their associated integer values.
  /// </summary>
  /// <returns>The sequence.</returns>
  public IEnumerable<(char, int)> Sequence()
    => IntToCharMap
      .OrderBy(kvp => kvp.Key)
      .Select(kvp => (kvp.Value, kvp.Key));

  /// <inheritdoc/>
  public IEnumerator<(char, int)> GetEnumerator() => Sequence().GetEnumerator();

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
  public static readonly Digits Base64 = new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/",
    CaseOption.CaseSensitive);

  /// <summary>
  ///   Read-only: Alphabetic characters used for bijective numeration.
  /// </summary>
  /// <remarks>
  ///   <c>0ABCDEFGHIJKLMNOPQRSTUVWXYZ</c>
  /// </remarks>
  public static readonly Digits Alpha = new("0ABCDEFGHIJKLMNOPQRSTUVWXYZ");
}

public enum CaseOption
{
  CaseSensitive = 0,
  FormatAsEntered = 1,
  FormatUppercase = 2,
  FormatLowercase = 3
}