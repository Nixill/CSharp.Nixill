using System.Text.RegularExpressions;

namespace Nixill.Utils;

public static class TextUtils
{
  public static bool ContainsAny(this string str, params string[] values)
    => values.Any(s => str.Contains(s));

  public static bool ContainsAny(this string str, StringComparison comparison, params string[] values)
    => values.Any(s => str.Contains(s, comparison));

  public static bool TryMatch(string input, string pattern, out Match match)
  {
    match = Regex.Match(input, pattern);
    return match.Success;
  }
}