using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using Nixill.Collections;
using Nixill.Objects;
using Nixill.Utils.Extensions;

namespace Nixill.Utils;

#region NumberConverter
/// <summary>
///   A class with methods to convert between arbitrary numeric data types
///   and string representations of numbers.
/// </summary>
public static class NumberConverter
{
  static Dictionary<Type, INumberCache> CachedNumbers = new();

  static NumberCache<T> GetNumberCache<T>() where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>,
    IMultiplicativeIdentity<T, T>, IComparable<T>
  {
    Type t = typeof(T);
    if (!CachedNumbers.ContainsKey(t)) CachedNumbers[t] = new NumberCache<T>();
    return (NumberCache<T>)CachedNumbers[t];
  }

  #region ├ Parsing
  /// <summary>
  ///   Converts a string to an array of single digits in the same base.
  /// </summary>
  /// <remarks>
  ///   The base doesn't need to be known to use this method, as for any
  ///   given <see cref="Digits"/>, the same character will always become
  ///   the same value for any base of at least that value plus one. For
  ///   example, using <see cref="Digits.Base36"/>, <c>B</c> in base 12 is
  ///   11, <c>B</c> in base 16 is 11, and <c>B</c> in base 36 is 11.
  /// </remarks>
  /// <param name="str">
  ///   The string to convert.
  /// </param>
  /// <param name="digits">
  ///   The digit set to use.
  ///   <para/>
  ///   Defaults to <see cref="Digits.Base36"/>.
  /// </param>
  /// <returns>
  ///   The <see cref="DigitArray"/> represented by this string.
  /// </returns>
  public static DigitArray ParseToDigitArray(string str, Digits? digits = null)
  {
    digits ??= Digits.Base36; // default

    List<char> chars = str.ToList();

    // First: Is the number negative?
    bool isNegative = false;

    if (str.StartsWith(digits.NegativeSign))
    {
      isNegative = true;
      str = str[1..];
    }

    List<int> wholePart = [];
    List<int> fracPart = [];

    // Next the whole part of the number.
    while (chars.Count > 0)
    {
      char chr = chars.Pop();

      if (chr == digits.DecimalPoint)
        break;

      wholePart.Add(digits.Parse(chr));
    }

    // Then the decimal part of the number.
    while (chars.Count > 0)
    {
      char chr = chars.Pop();

      fracPart.Add(digits.Parse(chr));
    }

    // And return.
    return new DigitArray(wholePart, fracPart, isNegative);
  }

  /// <summary>
  ///   Converts an array of digits to a number in a specific base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="whole">
  ///   The whole part of the number, as digits given in order of most to
  ///   least significant.
  /// </param>
  /// <param name="numberBase">
  ///   The numeric base to which this number should be interpreted.
  /// </param>
  /// <param name="isNegative">
  ///   Whether or not this number is negative.
  /// </param>
  /// <param name="bijective">
  ///   Whether or not this number is
  ///   <see href="https://en.wikipedia.org/wiki/Bijective_numeration">bijective</see>.
  /// </param>
  /// <returns>
  ///   The number parsed.
  /// </returns>
  public static T Parse<T>(IEnumerable<int> whole, int numberBase, bool isNegative = false, bool bijective = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
    => Parse<T>(new DigitArray(whole, [], isNegative), numberBase, bijective);

  /// <summary>
  ///   Converts a pair of arrays of digits to a number in a specific base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="whole">
  ///   The whole part of the number, as digits given in order of most to
  ///   least significant.
  /// </param>
  /// <param name="frac">
  ///   The fractional part of the number, as digits given in order of
  ///   most to least significant.
  /// </param>
  /// <param name="numberBase">
  ///   The numeric base from which this number should be parsed.
  /// </param>
  /// <param name="isNegative">
  ///   Whether or not this number is negative.
  /// </param>
  /// <returns>
  ///   The number parsed.
  /// </returns>
  public static T Parse<T>(IEnumerable<int> whole, IEnumerable<int> frac, int numberBase, bool isNegative = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
    => Parse<T>(new DigitArray(whole, frac, isNegative), numberBase, false);

  /// <summary>
  ///   Converts a <see cref="DigitArray">digit array</see> to a number in
  ///   a specific base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="arr">The digit array.</param>
  /// <param name="numberBase">
  ///   The numeric base from which this number should be parsed.
  /// </param>
  /// <param name="bijective">
  ///   Whether or not this number is
  ///   <see href="https://en.wikipedia.org/wiki/Bijective_numeration">bijective</see>.
  /// </param>
  /// <returns>
  ///   The number parsed.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The base is less than 1.
  ///   <para/>
  ///   <em>Or</em>, the number is non-bijective and the base is 1.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The number is being parsed as bijective, but has a decimal part.
  /// </exception>
  /// <exception cref="NumberParsingException">
  ///   A digit in the array had a value greater than the base.
  ///   <para/>
  ///   Or, a digit in the array had a value equal to the base, and the
  ///   number is non-bijective.
  /// </exception>
  public static T Parse<T>(DigitArray arr, int numberBase, bool bijective = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
  {
    // Check for a valid base first
    if (bijective)
    {
      if (numberBase < 1)
        throw new ArgumentOutOfRangeException($"Bijective parsing accepts only positive bases.");
    }
    else if (numberBase < 2)
      throw new ArgumentOutOfRangeException($"Non-bijective parsing accepts only bases at least 2.");

    if (bijective && arr.DecimalPart.Any())
      throw new InvalidOperationException("Bijective numbers cannot have fractional parts.");

    // Start building the number
    NumberCache<T> cache = GetNumberCache<T>();

    T ret = T.AdditiveIdentity;
    T bs = cache.GetParseValue<T>(numberBase);

    int chars = 0;
    if (arr.IsNegative) chars += 1;

    // Whole number part:
    List<int> wholePart = arr.WholePart.ToList();
    while (wholePart.Count > 0)
    {
      int add = wholePart.Pop();
      ret *= bs;
      if (add > numberBase || add < 0 || (add == numberBase && !bijective) || (add == 0 && bijective))
        throw new NumberParsingException(
          $"{add} is not a valid {(bijective ? "bijective" : "")} base {numberBase} value.",
          chars
        );
      ret += cache.GetParseValue<T>(add);
      chars += 1;
    }

    chars += 1; // for the decimal point

    // Decimal part:
    List<int> decimalPart = arr.DecimalPart.ToList();
    T decimalPlace = T.MultiplicativeIdentity;
    T lastDecimalPlace = T.MultiplicativeIdentity;
    while (decimalPart.Count > 0)
    {
      decimalPlace /= bs;
      if (decimalPlace.CompareTo(T.AdditiveIdentity) == 0 || decimalPlace.CompareTo(lastDecimalPlace) == 0) break;
      lastDecimalPlace = decimalPlace;
      int add = decimalPart.Pop();
      if (add >= numberBase || add < 0)
        throw new NumberParsingException($"{add} is not a valid base {numberBase} value.", chars);
      ret += cache.GetParseValue<T>(add) * decimalPlace;
      chars += 1;
    }

    if (arr.IsNegative) return T.AdditiveIdentity - ret;
    else return ret;
  }

  /// <summary>
  ///   Converts a string representation of a number to a number in a
  ///   specific base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="str">The string representation of the number.</param>
  /// <param name="numberBase">
  ///   The numeric base from which this number should be parsed.
  /// </param>
  /// <param name="digits">
  ///   The digit set to use.
  ///   <para/>
  ///   Defaults to <see cref="Digits.Base36"/>.
  /// </param>
  /// <param name="bijective">
  ///   Whether or not this number is
  ///   <see href="https://en.wikipedia.org/wiki/Bijective_numeration">bijective</see>.
  /// </param>
  /// <returns>
  ///   The number parsed.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The base is less than 1.
  ///   <para/>
  ///   Or, the number is non-bijective and the base is 1.
  ///   <para/>
  ///   Or, the base is greater than the highest supported base of the
  ///   digit set.
  ///   <para/>
  ///   Or, the number is bijective and the base is equal to the highest
  ///   supported base of the digit set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The number is being parsed as bijective, but has a decimal part.
  /// </exception>
  /// <exception cref="NumberParsingException">
  ///   A digit in the array had a value greater than the base.
  ///   <para/>
  ///   Or, a digit in the array had a value equal to the base, and the
  ///   number is non-bijective.
  /// </exception>
  public static T Parse<T>(string str, int numberBase, Digits? digits = null, bool bijective = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
  {
    digits ??= Digits.Base36;

    if (bijective)
    {
      if (numberBase < 1 || numberBase >= digits.HighestSupportedBase)
        throw new ArgumentOutOfRangeException($"Bijective parsing accepts bases 1 to {digits.HighestSupportedBase - 1}");
    }
    else if (numberBase < 2 || numberBase > digits.HighestSupportedBase)
      throw new ArgumentOutOfRangeException($"Non-bijective parse only accepts bases 2 to {digits.HighestSupportedBase}.");

    try
    {
      return Parse<T>(ParseToDigitArray(str, digits), numberBase, bijective);
    }
    catch (NumberParsingException ex)
    {
      throw new NumberParsingException($"{str[ex.WhichDigit]}{str.Substring(str.IndexOf(' '))}", ex, ex.WhichDigit);
    }
  }
  #endregion

  #region └ Formatting
  /// <summary>
  ///   Converts a number to a <see cref="DigitArray">digit array</see>
  ///   via a specific base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IModulusOperators{T, T, T}">modulable</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="value">The number to convert.</param>
  /// <param name="numberBase">
  ///   The numeric base to which this number should be formatted.
  /// </param>
  /// <param name="bijective">
  ///   Whether or not this number is
  ///   <see href="https://en.wikipedia.org/wiki/Bijective_numeration">bijective</see>.
  /// </param>
  /// <param name="maxDecimals">
  ///   The maximum number of decimal places to which the number should be
  ///   formatted.
  /// </param>
  /// <returns>The digit array.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The base is less than 1.
  ///   <para/>
  ///   Or, the number is non-bijective and the base is 1.
  /// </exception>
  public static DigitArray FormatToDigitArray<T>(T value, int numberBase, bool bijective = false, int maxDecimals = 10)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplicativeIdentity<T, T>, IComparable<T>,
      IModulusOperators<T, T, T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>,
      IMultiplyOperators<T, T, T>
  {
    // Check for a valid base first
    if (bijective)
    {
      if (numberBase < 1)
        throw new ArgumentOutOfRangeException($"Bijective parsing accepts only positive bases.");
    }
    else if (numberBase < 2)
      throw new ArgumentOutOfRangeException($"Non-bijective parsing accepts only bases at least 2.");

    T one = T.MultiplicativeIdentity;
    T zero = T.AdditiveIdentity;

    T decimalPart = value % one;
    T wholePart = value - decimalPart;

    NumberCache<T> cache = GetNumberCache<T>();
    T bs = cache.GetParseValue<T>(numberBase);

    bool isNegative = value.CompareTo(zero) < 0;

    // Get the whole part first
    List<int> wholeInts = [];

    while (wholePart.CompareTo(zero) != 0)
    {
      T lastDigit = wholePart % bs;
      if (lastDigit.CompareTo(zero) < 0) lastDigit = zero - lastDigit;
      int add = cache.GetFormatValue(lastDigit);
      if (bijective && add == 0)
      {
        add = numberBase;
        if (!isNegative) wholePart -= bs;
        else wholePart += bs;
      }
      wholeInts.Insert(0, add);
      wholePart /= bs;
    }

    // And now the decimal part
    List<int> decimalInts = [];
    int decimalsGiven = 0;
    int zeroesGiven = 0;

    if (!bijective)
    {
      while (decimalPart.CompareTo(zero) != 0 && decimalsGiven < maxDecimals)
      {
        decimalPart *= bs;
        T nextDigit = decimalPart;
        decimalPart %= one;
        nextDigit -= decimalPart;

        if (nextDigit.CompareTo(zero) == 0)
        {
          zeroesGiven += 1;
        }
        else
        {
          foreach (int i in Enumerable.Range(1, zeroesGiven)) decimalInts.Add(0);
          zeroesGiven = 0;
          decimalInts.Add(cache.GetFormatValue(nextDigit));
        }

        decimalsGiven += 1;
      }
    }

    return new DigitArray(wholeInts, decimalInts, isNegative);
  }

  /// <summary>
  ///   Formats a digit array to a string represenation of the number.
  /// </summary>
  /// <param name="arr">
  ///   The digit array to format.
  /// </param>
  /// <param name="digits">
  ///   The digit set to use.
  /// </param>
  /// <returns>
  ///   The string representation of the number.
  /// </returns>
  /// <exception cref="NumberFormattingException">
  ///   The number is negative, but the digit set does not have a negative
  ///   sign.
  ///   <para/>
  ///   Or, the number has a decimal part, but the digit set does not have
  ///   a decimal point.
  /// </exception>
  /// <exception cref="FormatException">
  ///   There are not enough digits to reach the specified numeric value.
  ///   <para/>
  ///   Or, the specified numeric value is negative.
  /// </exception>
  public static string Format(DigitArray arr, Digits? digits = null)
  {
    digits ??= Digits.Base36;

    StringBuilder builder = new();

    if (arr.IsNegative)
    {
      if (digits.NegativeSign != '\0') builder.Append(digits.NegativeSign);
      else throw new NumberFormattingException("Cannot make negative numbers with the given set of digits");
    }

    foreach (int i in arr.WholePart)
    {
      builder.Append(digits.Format(i));
    }

    bool pointed = false;
    foreach (int i in arr.DecimalPart)
    {
      if (!pointed)
      {
        if (digits.DecimalPoint != '\0') builder.Append(digits.DecimalPoint);
        else throw new NumberFormattingException("Cannot make decimals with the given set of digits");
        pointed = true;
      }
      builder.Append(digits.Format(i));
    }

    return builder.ToString();
  }

  /// <summary>
  ///   Returns the string representation of a number in a given base.
  /// </summary>
  /// <typeparam name="T">
  ///   The type to which this should be converted. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="ISubtractionOperators{T, T, T}">subtractible</see>
  ///       from itself,
  ///     </item>
  ///     <item>
  ///       have a <see cref="IMultiplicativeIdentity{T, T}">multiplicative
  ///       identity</see> of itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IMultiplyOperators{T, T, T}">multipliable</see>
  ///       with itself,
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IModulusOperators{T, T, T}">modulable</see> by
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IComparable{T}">comparable</see> with itself
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="value">The number to convert.</param>
  /// <param name="numberBase">
  ///   The numeric base to which this number should be formatted.
  /// </param>
  /// <param name="digits">
  ///   The digit set to use.
  /// </param>
  /// <param name="bijective">
  ///   Whether or not this number is
  ///   <see href="https://en.wikipedia.org/wiki/Bijective_numeration">bijective</see>.
  /// </param>
  /// <param name="maxDecimals">
  ///   The maximum number of decimal places to which the number should be
  ///   formatted.
  /// </param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The base is less than 1.
  ///   <para/>
  ///   Or, the number is non-bijective and the base is 1.
  ///   <para/>
  ///   Or, the base is greater than the highest supported base of the
  ///   digit set.
  ///   <para/>
  ///   Or, the number is bijective and the base is equal to the highest
  ///   supported base of the digit set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The number is being parsed as bijective, but has a decimal part.
  /// </exception>
  /// <exception cref="NumberParsingException">
  ///   A digit in the array had a value greater than the base.
  ///   <para/>
  ///   Or, a digit in the array had a value equal to the base, and the
  ///   number is non-bijective.
  /// </exception>
  public static string Format<T>(T value, int numberBase, Digits? digits = null, bool bijective = false,
    int maxDecimals = 10) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplicativeIdentity<T, T>,
      IComparable<T>, IModulusOperators<T, T, T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>,
      IMultiplyOperators<T, T, T>
  {
    digits ??= Digits.Base36;

    if (bijective)
    {
      if (numberBase < 1 || numberBase >= digits.HighestSupportedBase)
        throw new ArgumentOutOfRangeException($"Bijective parsing accepts bases 1 to {digits.HighestSupportedBase - 1}");
    }
    else if (numberBase < 2 || numberBase > digits.HighestSupportedBase)
      throw new ArgumentOutOfRangeException($"Non-bijective parse only accepts bases 2 to {digits.HighestSupportedBase}.");

    return Format(FormatToDigitArray(value, numberBase, bijective, maxDecimals), digits);
  }
  #endregion
}
#endregion

#region INumberCache
internal interface INumberCache
{
  public T GetParseValue<T>(int input);
  public int GetFormatValue<T>(T input);
}
#endregion

#region NumberCache<T>
internal class NumberCache<T> : INumberCache where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>,
  IMultiplicativeIdentity<T, T>, IComparable<T>
{
  readonly Dictionary<int, T> ParseValues = new Dictionary<int, T>
  {
    [0] = T.AdditiveIdentity,
    [1] = T.MultiplicativeIdentity
  };
  readonly Dictionary<T, int> FormatValues = new Dictionary<T, int>
  {
    [T.AdditiveIdentity] = 0,
    [T.MultiplicativeIdentity] = 1
  };

  int HighestNumber = 1;
  T HighestValue = T.MultiplicativeIdentity;

  IEnumerable<(int, T)> Counter()
  {
    T count = T.MultiplicativeIdentity;
    T one = T.MultiplicativeIdentity;
    int index = 1;

    while (true)
    {
      index++;
      count += one;
      ParseValues[index] = count;
      FormatValues[count] = index;
      HighestNumber = index;
      HighestValue = count;
      yield return (index, count);
    }
  }

  IEnumerator<(int, T)> CountEnumerator;

  public NumberCache()
  {
    CountEnumerator = Counter().GetEnumerator();
  }

  public int GetFormatValue<T1>(T1 input)
  {
    if (typeof(T) != typeof(T1)) throw new InvalidOperationException($"This NumberCache does not store {typeof(T1)}s.");
    while (((T)(object)input!).CompareTo(HighestValue) > 0) CountEnumerator.MoveNext();

    return FormatValues[(T)(object)input];
  }

  public T1 GetParseValue<T1>(int input)
  {
    if (typeof(T) != typeof(T1)) throw new InvalidOperationException($"This NumberCache does not store {typeof(T1)}s.");
    while (input > HighestNumber) CountEnumerator.MoveNext();

    return (T1)(object)ParseValues[input];
  }
}
#endregion

#region DigitArray
/// <summary>
///   A representation of an array of "digits" of a single number in an
///   arbitrary base.
/// </summary>
public readonly struct DigitArray
{
  /// <summary>
  ///   Read-only: The whole part of the number. For example, 25 in base 5
  ///   is <c>[1, 0, 0]</c>.
  /// </summary>
  public readonly ReadOnlyCollection<int> WholePart { get; init; }

  /// <summary>
  ///   Read-only: The decimal part of the number. For example, 0.375 in
  ///   base 2 is <c>[0, 1, 1]</c>.
  /// </summary>
  public readonly ReadOnlyCollection<int> DecimalPart { get; init; } = Array.Empty<int>().AsReadOnly();

  /// <summary>
  ///   Read-only: Whether or not the represented number is negative.
  /// </summary>
  public readonly bool IsNegative { get; init; } = false;

  /// <summary>
  ///   Constructs a digit array with the given whole part.
  /// </summary>
  /// <param name="wholePart">The whole part.</param>
  /// <param name="negative">
  ///   Whether or not the number is negative.
  /// </param>
  public DigitArray(IEnumerable<int> wholePart, bool negative = false)
  {
    WholePart = wholePart.ToArray().AsReadOnly();
    IsNegative = negative;
  }

  /// <summary>
  ///   Constructs a digit array with the given whole and decimal parts.
  /// </summary>
  /// <param name="wholePart">The whole part.</param>
  /// <param name="decimalPart">The decimal part.</param>
  /// <param name="negative">
  ///   Whether or not the number is negative.
  /// </param>
  public DigitArray(IEnumerable<int> wholePart, IEnumerable<int> decimalPart, bool negative = false)
  {
    WholePart = wholePart.ToArray().AsReadOnly();
    DecimalPart = decimalPart.ToArray().AsReadOnly();
    IsNegative = negative;
  }
}
#endregion

#region NumberConversionException
/// <summary>
///   Represents an exception thrown during
///   <see cref="NumberConverter.Parse{T}(DigitArray, int, bool)">number
///   parsing</see>.
/// </summary>
public class NumberParsingException : Exception
{
  /// <summary>
  ///   Get or init: Which digit couldn't be parsed?
  /// </summary>
  public int WhichDigit { get; init; }

  /// <summary>
  ///   Constructs a new NumberParsingException.
  /// </summary>
  /// <param name="which">Which digit couldn't be parsed?</param>
  public NumberParsingException(int which) : base()
  {
    WhichDigit = which;
  }

  /// <summary>
  ///   Constructs a new NumberParsingException with a given message.
  /// </summary>
  /// <param name="message">Message to include.</param>
  /// <param name="which">Which digit couldn't be parsed?</param>
  public NumberParsingException(string message, int which) : base(message)
  {
    WhichDigit = which;
  }

  /// <summary>
  ///   Constructs a new NumberParsingException with a given message and
  ///   inner exception.
  /// </summary>
  /// <param name="message">Message to include.</param>
  /// <param name="innerException">
  ///   Inner exception that caused this exception.
  /// </param>
  /// <param name="which">Which digit couldn't be parsed?</param>
  public NumberParsingException(string message, Exception innerException, int which) : base(message, innerException)
  {
    WhichDigit = which;
  }
}

/// <summary>
///   Represents an exception thrown during
///   <see cref="NumberConverter.Format(DigitArray, Digits?)">number
///   formatting</see>.
/// </summary>
public class NumberFormattingException : Exception
{
  /// <summary>
  ///   Constructs a new NumberFormattingException.
  /// </summary>
  public NumberFormattingException() : base() { }

  /// <summary>
  ///   Constructs a new NumberFormattingException with a given message.
  /// </summary>
  /// <param name="message">Message to include.</param>
  public NumberFormattingException(string message) : base(message) { }

  /// <summary>
  ///   Constructs a new NumberFormattingException with a given message
  ///   and inner exception.
  /// </summary>
  /// <param name="message">Message to include.</param>
  /// <param name="innerException">
  ///   Inner exception that caused this exception.
  /// </param>
  public NumberFormattingException(string message, Exception innerException) : base(message, innerException) { }
}
#endregion