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
  public static (T, Match?) TestAgainst<T>(string input)
    where T : struct, Enum
  {
    foreach (var test in EnumUtils.ValuesWithAttribute<T, RegexTestAttribute>().OrderBy(t => t.Value))
    {
      Regex regex = test.Attribute.UsedRegex;
      Match match = regex.Match(input);
      if (match.Success) return (test.Value, match);
    }

    return (default(T), null);
  }
}