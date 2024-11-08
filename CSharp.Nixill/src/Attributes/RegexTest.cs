using System.Text.RegularExpressions;
using Nixill.Utils;

namespace Nixill.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class RegexTestAttribute : Attribute
{
  public readonly Regex UsedRegex;

  public RegexTestAttribute(string pattern, RegexOptions options = RegexOptions.None)
  {
    UsedRegex = new Regex(pattern, options);
  }

  public static (T, Match) TestAgainst<T>(string input)
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