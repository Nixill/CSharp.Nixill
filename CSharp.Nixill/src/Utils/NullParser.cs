namespace Nixill.Utils;

/// <summary>
///   Provides methods that try to parse strings to either nullable values
///   or default values.
/// </summary>
public static class NullParser
{
  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its bool equivalent.
  /// </summary>
  /// <param name="input">A string containing the bool to convert.</param>
  /// <returns>
  ///   The bool value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static bool? Bool(string input) => bool.TryParse(input, out bool result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its byte equivalent.
  /// </summary>
  /// <param name="input">A string containing the byte to convert.</param>
  /// <returns>
  ///   The byte value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static byte? Byte(string input) => byte.TryParse(input, out byte result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its sbyte equivalent.
  /// </summary>
  /// <param name="input">A string containing the sbyte to convert.</param>
  /// <returns>
  ///   The sbyte value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static sbyte? SByte(string input) => sbyte.TryParse(input, out sbyte result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its char equivalent.
  /// </summary>
  /// <param name="input">A string containing the char to convert.</param>
  /// <returns>
  ///   The char value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static char? Char(string input) => char.TryParse(input, out char result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its decimal equivalent.
  /// </summary>
  /// <param name="input">A string containing the decimal to convert.</param>
  /// <returns>
  ///   The decimal value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static decimal? Decimal(string input) => decimal.TryParse(input, out decimal result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its double equivalent.
  /// </summary>
  /// <param name="input">A string containing the double to convert.</param>
  /// <returns>
  ///   The double value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static double? Double(string input) => double.TryParse(input, out double result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its float equivalent.
  /// </summary>
  /// <param name="input">A string containing the float to convert.</param>
  /// <returns>
  ///   The float value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static float? Float(string input) => float.TryParse(input, out float result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its int equivalent.
  /// </summary>
  /// <param name="input">A string containing the int to convert.</param>
  /// <returns>
  ///   The int value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static int? Int(string input) => int.TryParse(input, out int result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its uint equivalent.
  /// </summary>
  /// <param name="input">A string containing the uint to convert.</param>
  /// <returns>
  ///   The uint value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static uint? UInt(string input) => uint.TryParse(input, out uint result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its long equivalent.
  /// </summary>
  /// <param name="input">A string containing the long to convert.</param>
  /// <returns>
  ///   The long value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static long? Long(string input) => long.TryParse(input, out long result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its ulong equivalent.
  /// </summary>
  /// <param name="input">A string containing the ulong to convert.</param>
  /// <returns>
  ///   The ulong value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static ulong? ULong(string input) => ulong.TryParse(input, out ulong result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its short equivalent.
  /// </summary>
  /// <param name="input">A string containing the short to convert.</param>
  /// <returns>
  ///   The short value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static short? Short(string input) => short.TryParse(input, out short result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its ushort equivalent.
  /// </summary>
  /// <param name="input">A string containing the ushort to convert.</param>
  /// <returns>
  ///   The ushort value, if the input was parsed successfully; otherwise,
  ///   <c>null</c>.
  /// </returns>
  public static ushort? UShort(string input) => ushort.TryParse(input, out ushort result) ? result : null;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its bool equivalent.
  /// </summary>
  /// <param name="input">A string containing the bool to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The bool value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static bool Bool(string input, bool defaultValue) => bool.TryParse(input, out bool result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its byte equivalent.
  /// </summary>
  /// <param name="input">A string containing the byte to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The byte value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static byte Byte(string input, byte defaultValue) => byte.TryParse(input, out byte result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its sbyte equivalent.
  /// </summary>
  /// <param name="input">A string containing the sbyte to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The sbyte value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static sbyte SByte(string input, sbyte defaultValue) => sbyte.TryParse(input, out sbyte result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its char equivalent.
  /// </summary>
  /// <param name="input">A string containing the char to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The char value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static char Char(string input, char defaultValue) => char.TryParse(input, out char result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its decimal equivalent.
  /// </summary>
  /// <param name="input">A string containing the decimal to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The decimal value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static decimal Decimal(string input, decimal defaultValue) => decimal.TryParse(input, out decimal result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its double equivalent.
  /// </summary>
  /// <param name="input">A string containing the double to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The double value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static double Double(string input, double defaultValue) => double.TryParse(input, out double result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its float equivalent.
  /// </summary>
  /// <param name="input">A string containing the float to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The float value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static float Float(string input, float defaultValue) => float.TryParse(input, out float result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its int equivalent.
  /// </summary>
  /// <param name="input">A string containing the int to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The int value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static int Int(string input, int defaultValue) => int.TryParse(input, out int result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its uint equivalent.
  /// </summary>
  /// <param name="input">A string containing the uint to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The uint value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static uint UInt(string input, uint defaultValue) => uint.TryParse(input, out uint result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its long equivalent.
  /// </summary>
  /// <param name="input">A string containing the long to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The long value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static long Long(string input, long defaultValue) => long.TryParse(input, out long result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its ulong equivalent.
  /// </summary>
  /// <param name="input">A string containing the ulong to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The ulong value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static ulong ULong(string input, ulong defaultValue) => ulong.TryParse(input, out ulong result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its short equivalent.
  /// </summary>
  /// <param name="input">A string containing the short to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The short value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static short Short(string input, short defaultValue) => short.TryParse(input, out short result) ? result : defaultValue;

  /// <summary>
  ///   Tries to convert the specified string representation of a value to
  ///   its ushort equivalent.
  /// </summary>
  /// <param name="input">A string containing the ushort to convert.</param>
  /// <param name="defaultValue">The value to return if parsing fails.</param>
  /// <returns>
  ///   The ushort value, if the input was parsed successfully; otherwise,
  ///   <c>defaultValue</c>.
  /// </returns>
  public static ushort UShort(string input, ushort defaultValue) => ushort.TryParse(input, out ushort result) ? result : defaultValue;

}