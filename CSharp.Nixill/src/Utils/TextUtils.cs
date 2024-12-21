using System.Text.RegularExpressions;

namespace Nixill.Utils;

/// <summary>
///   Utility methods for dealing with text.
/// </summary>
public static class TextUtils
{
  /// <summary>
  ///   Whether or not the string contains any of the values.
  /// </summary>
  /// <param name="str">The string.</param>
  /// <param name="values">The values to look for.</param>
  /// <returns>
  ///   <c>true</c> iff the string contains any of those values.
  /// </returns>
  public static bool ContainsAny(this string str, params string[] values)
    => values.Any(s => str.Contains(s));

  /// <summary>
  ///   Whether or not the string contains any of the values.
  /// </summary>
  /// <param name="str">The string.</param>
  /// <param name="comparison">
  ///   The comparison to use for the strings.
  /// </param>
  /// <param name="values">The values to look for.</param>
  /// <returns>
  ///   <c>true</c> iff the string contains any of those values.
  /// </returns>
  public static bool ContainsAny(this string str, StringComparison comparison, params string[] values)
    => values.Any(s => str.Contains(s, comparison));

  /// <summary>
  ///   Attempts to match the input with the given regex pattern.
  /// </summary>
  /// <remarks>
  ///   Unlike most <c>Try</c> methods, the <c>match</c> parameter of this
  ///   method will reliably be non-null regardless of the return value of
  ///   the method.
  /// </remarks>
  /// <param name="input">The input to match against.</param>
  /// <param name="pattern">The regex pattern to match.</param>
  /// <param name="match">
  ///   Once this method returns, this parameter is the result of the
  ///   match test.
  /// </param>
  /// <returns><c>match.Success</c>.</returns>
  public static bool TryMatch(string input, string pattern, out Match match)
  {
    match = Regex.Match(input, pattern);
    return match.Success;
  }
}