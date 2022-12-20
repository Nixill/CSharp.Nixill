using System.Text.RegularExpressions;

namespace Nixill.Utils
{
  public static class RegexUtils
  {
    public static bool TryMatch(string input, string pattern, out Match match)
    {
      match = Regex.Match(input, pattern);
      return match.Success;
    }

    public static bool TryMatch(this Regex regex, string input, out Match match)
    {
      match = regex.Match(input);
      return match.Success;
    }

    public static bool TryGroup(this Match match, int number, out string value)
    {
      value = match.Groups[number].Value;
      return match.Groups[number].Success;
    }

    public static bool TryGroup(this Match match, string name, out string value)
    {
      value = match.Groups[name].Value;
      return match.Groups[name].Success;
    }
  }
}