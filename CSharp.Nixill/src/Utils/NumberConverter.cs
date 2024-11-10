using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;
using Nixill.Collections;
using Nixill.Objects;
using Nixill.Utils.Extensions;

namespace Nixill.Utils;

#region NumberConverter
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

  #region ├ Parsing / DigitArray
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

  public static T Parse<T>(IEnumerable<int> whole, int numberBase, bool isNegative = false, bool bijective = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
    => Parse<T>(new DigitArray(whole, [], isNegative), numberBase, bijective);

  public static T Parse<T>(IEnumerable<int> whole, IEnumerable<int> frac, int numberBase, bool isNegative = false)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IMultiplyOperators<T, T, T>,
      IMultiplicativeIdentity<T, T>, IComparable<T>, ISubtractionOperators<T, T, T>, IDivisionOperators<T, T, T>
    // IEqualityOperators<T, T, bool>
    => Parse<T>(new DigitArray(whole, frac, isNegative), numberBase, false);

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
public readonly struct DigitArray
{
  public readonly ReadOnlyCollection<int> WholePart { get; init; }
  public readonly ReadOnlyCollection<int> DecimalPart { get; init; } = Array.Empty<int>().AsReadOnly();
  public readonly bool IsNegative { get; init; } = false;

  public DigitArray(IEnumerable<int> wholePart, bool negative = false)
  {
    WholePart = wholePart.ToArray().AsReadOnly();
    IsNegative = negative;
  }

  public DigitArray(IEnumerable<int> wholePart, IEnumerable<int> decimalPart, bool negative = false)
  {
    WholePart = wholePart.ToArray().AsReadOnly();
    DecimalPart = decimalPart.ToArray().AsReadOnly();
    IsNegative = negative;
  }
}
#endregion

#region NumberConversionException
public class NumberParsingException : Exception
{
  public int WhichDigit { get; init; }

  public NumberParsingException(int which) : base()
  {
    WhichDigit = which;
  }

  public NumberParsingException(string message, int which) : base(message)
  {
    WhichDigit = which;
  }

  public NumberParsingException(string message, Exception innerException, int which) : base(message, innerException)
  {
    WhichDigit = which;
  }
}

public class NumberFormattingException : Exception
{
  public NumberFormattingException() : base() { }
  public NumberFormattingException(string message) : base(message) { }
  public NumberFormattingException(string message, Exception innerException) : base(message, innerException) { }
}
#endregion