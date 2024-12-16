using System.Text.RegularExpressions;

namespace Nixill.Utils.Extensions
{
  /// <summary>
  ///   Extension methods that operate on <see cref="Regex"/>es and
  ///   <see cref="Match"/>es.
  /// </summary>
  public static class RegexExtensions
  {
    /// <summary>
    ///   Attempts to match the input with the given regex.
    /// </summary>
    /// <remarks>
    ///   Unlike most <c>Try</c> methods, the <c>match</c> parameter of
    ///   this method will reliably be non-null regardless of the return
    ///   value of the method.
    /// </remarks>
    /// <param name="regex">The regex to match.</param>
    /// <param name="input">The input to match against.</param>
    /// <param name="match">
    ///   Once this method returns, this parameter is the result of the
    ///   match test.
    /// </param>
    /// <returns><c>match.Success</c>.</returns>
    public static bool TryMatch(this Regex regex, string input, out Match match)
    {
      match = regex.Match(input);
      return match.Success;
    }

    /// <summary>
    ///   Attempts to get the value of a group within a match.
    /// </summary>
    /// <param name="match">The match to check.</param>
    /// <param name="number">Which group number to check.</param>
    /// <param name="value">
    ///   Once this method returns, this parameter is the contents of the
    ///   specified group.
    /// </param>
    /// <returns><c>match.Groups[number].Success</c>.</returns>
    public static bool TryGroup(this Match match, int number, out string value)
    {
      value = match.Groups[number].Value;
      return match.Groups[number].Success;
    }

    /// <summary>
    ///   Attempts to get the value of a group within a match.
    /// </summary>
    /// <param name="match">The match to check.</param>
    /// <param name="name">Which group name to check.</param>
    /// <param name="value">
    ///   Once this method returns, this parameter is the contents of the
    ///   specified group.
    /// </param>
    /// <returns><c>match.Groups[name].Success</c>.</returns>
    public static bool TryGroup(this Match match, string name, out string value)
    {
      value = match.Groups[name].Value;
      return match.Groups[name].Success;
    }
  }
}