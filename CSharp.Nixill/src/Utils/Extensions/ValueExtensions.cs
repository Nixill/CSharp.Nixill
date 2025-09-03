namespace Nixill.Utils.ValueExtensions;

/// <summary>
///   Provides extension methods for working with values.
/// </summary>
public static class ValueExtensions
{
  /// <summary>
  ///   Assigns a value to a variable, inline, then returns it.
  /// </summary>
  /// <typeparam name="T">The type of value.</typeparam>
  /// <param name="value">The value.</param>
  /// <param name="variable">The variable.</param>
  /// <returns><c>value</c>.</returns>
  public static T AssignTo<T>(this T value, out T variable)
  {
    variable = value;
    return value;
  }

  /// <summary>
  ///   Returns a different value if the incoming value is the default for
  ///   its type.
  /// </summary>
  /// <typeparam name="T">The type of value.</typeparam>
  /// <param name="value">The incoming value.</param>
  /// <param name="defaultValue">
  ///   The value to return if the incoming value is its default.
  /// </param>
  /// <returns>Either the incoming value or the specified value.</returns>
  public static T ValueIfDefault<T>(this T value, T defaultValue)
    => object.ReferenceEquals(value, default(T)) ? defaultValue : value;

  /// <summary>
  ///   Generates and returns a different value if the incoming value is
  ///   the default for its type.
  /// </summary>
  /// <typeparam name="T">The type of value.</typeparam>
  /// <param name="value">The incoming value.</param>
  /// <param name="defaultValue">
  ///   The function to run if the incoming value is its default.
  /// </param>
  /// <returns>
  ///   Either the incoming value or the result of the function.
  /// </returns>
  public static T ValueIfDefault<T>(this T value, Func<T> defaultValue)
    => object.ReferenceEquals(value, default(T)) ? defaultValue() : value;

  /// <summary>
  ///   Returns a different string if the incoming value is null or an
  ///   empty string.
  /// </summary>
  /// <param name="value">The incoming string.</param>
  /// <param name="defaultValue">
  ///   The value to return if the incoming string is null or empty.
  /// </param>
  /// <returns>Either the incoming string or the specified string.</returns>
  public static string ValueIfEmptyString(this string? value, string defaultValue)
    => (value == null || value == "") ? defaultValue : value;

  /// <summary>
  ///   Generates and returns a different string if the incoming value is
  ///   null or an empty string.
  /// </summary>
  /// <param name="value">The incoming string.</param>
  /// <param name="defaultValue">
  ///   The function to run if the incoming string is null or empty.
  /// </param>
  /// <returns>
  ///   Either the incoming string or the result of the specified function.
  /// </returns>
  public static string ValueIfEmptyString(this string? value, Func<string> defaultValue)
    => (value == null || value == "") ? defaultValue() : value;
}