using System.Text.RegularExpressions;

namespace Nixill.Utils {
  public static class RegexUtils {
    public static bool TryMatch(string input, string pattern, out Match match) {
      match = Regex.Match(input, pattern);
      return match.Success;
    }

    public static bool TryMatch(this Regex regex, string input, out Match match) {
      match = regex.Match(input);
      return match.Success;
    }

    public static bool TryGroup(this Match match, int group, out string value) {
      bool ret = match.Groups[group].Success;

      if (ret) value = match.Groups[group].Value;
      else value = null;

      return ret;
    }
  }
}