using System.Text.RegularExpressions;
using Nixill.Utils;

namespace Nixill.Attributes;

/// <summary>
///   An attribute that can be applied to the values of an enum to create
///   a quick "which regex does this string match" test.
/// </summary>
/// <seealso href="https://github.com/StevenH237/CSharp.Nixill/blob/master/CSharp.Nixill/doc/CodeExamples/Attributes/RegexTest.md">
///   More detailed example
/// </seealso>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class RegexTestAttribute : Attribute
{
  /// <summary>
  ///   Read-only: The regex used on this particular instance of the attribute.
  /// </summary>
  public readonly Regex UsedRegex;

  /// <summary>
  ///   Constructs a new RegexTestAttribute with the given pattern and options.
  /// </summary>
  /// <param name="pattern">The regex pattern for this test.</param>
  /// <param name="options">The regex options for this test.</param>
  public RegexTestAttribute(string pattern, RegexOptions options = RegexOptions.None)
  {
    UsedRegex = new Regex(pattern, options);
  }

  /// <summary>
  ///   Constructs a new RegexTestAttribute with the given pattern,
  ///   options, and timeout.
  /// </summary>
  /// <param name="pattern">The regex pattern for this test.</param>
  /// <param name="regexTimeoutMilliseconds">
  ///   The timeout for matches in this test.
  /// </param>
  /// <param name="options">The regex options for this test.</param>
  public RegexTestAttribute(string pattern, double regexTimeoutMilliseconds, RegexOptions options = RegexOptions.None)
  {
    UsedRegex = new Regex(pattern, options, TimeSpan.FromMilliseconds(regexTimeoutMilliseconds));
  }

  /// <summary>
  ///   Tests a string against the regexes of a given enum.
  /// </summary>
  /// <typeparam name="T">The enum type to test against.</typeparam>
  /// <param name="input">The string to test.</param>
  /// <returns>
  ///   One of:
  ///   <list type="bullet">
  ///     <item>
  ///       <c>T</c>: The lowest-value enum constant that had a matching
  ///       regex; <c>Match</c>: The <see cref="Match"/> associated with
  ///       that match.
  ///     </item>
  ///     <item>
  ///       <c>T</c>: The enum constant associated with the value
  ///       <c>0</c>; <c>Match?</c>: <c>null</c>.
  ///     </item>
  ///   </list>
  /// </returns>
  [Obsolete("Use TestAgainst<T>(string, Match?) instead.")]
  public static (T, Match?) TestAgainst<T>(string input)
    where T : struct, Enum
  {
    foreach (var test in EnumUtils.ValuesWithAttribute<T, RegexTestAttribute>().OrderBy(t => t.Value))
    {
      Regex regex = test.Attribute.UsedRegex;
      try
      {
        Match match = regex.Match(input);
        if (match.Success) return (test.Value, match);
      }
      catch (RegexMatchTimeoutException) { /* do nothing */ }
    }

    return (default(T), null);
  }

  /// <summary>
  ///   Tests a string against the regexes of a given enum.
  /// </summary>
  /// <remarks>
  ///   This overload could be directly passed into a <see langword="switch"/>
  ///   statement.
  /// </remarks>
  /// <typeparam name="T">The enum type to test against.</typeparam>
  /// <param name="input">The string to test.</param>
  /// <param name="match">
  ///   When this method returns, this parameter contains the <see cref="Match"/>
  ///   associated with the match, if any match was found. Otherwise, it
  ///   contains <see langword="null"/>.
  /// </param>
  /// <returns>
  ///   The lowest-value enum constant that had a matching regex. If no
  ///   enum constant had a matching regex, the constant with the value 0
  ///   is returned.
  /// </returns>
  public static T TestAgainst<T>(string input, out Match? match)
    where T : struct, Enum
  {
    foreach (var test in EnumUtils.ValuesWithAttribute<T, RegexTestAttribute>().OrderBy(t => t.Value))
    {
      Regex regex = test.Attribute.UsedRegex;
      try
      {
        match = regex.Match(input);
        if (match.Success) return test.Value;
      }
      catch (RegexMatchTimeoutException) { /* do nothing */ }
    }

    match = null;
    return default(T);
  }


}