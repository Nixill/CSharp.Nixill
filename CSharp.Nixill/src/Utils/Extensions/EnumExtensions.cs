using System.Text.RegularExpressions;

namespace Nixill.Utils.Extensions;

/// <summary>
///   A class that contains extension methods for enum values.
/// </summary>
public static class EnumExtensions
{
  static Regex Number = new(@"^-?\d+$");

  /// <summary>
  ///   Gets the enum constants that comprise a composite value.
  /// </summary>
  /// <typeparam name="TEnum">
  ///   The type of the enum.
  /// </typeparam>
  /// <param name="value">
  ///   The composite value.
  /// </param>
  /// <param name="throwException">
  ///   Whether or not to throw an exception if the composite value cannot
  ///   be comprised of constants.
  /// </param>
  /// <returns>The flags.</returns>
  /// <exception cref="InvalidCastException">
  ///   The composite value cannot be comprised of defined constants, and
  ///   <c>throwException</c> is true.
  /// </exception>
  public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value, bool throwException = false)
    where TEnum : struct, Enum
  {
    string str = value.ToString();

    if (Number.IsMatch(str))
      if (throwException) throw new InvalidCastException($"Not a valid {typeof(TEnum).Name} value: {value}");
      else return [];
    else return str.Split(", ").Select(Enum.Parse<TEnum>);
  }
}