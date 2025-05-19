In an enum type, apply this attribute (with a regex pattern, and optionally some regex options, as its constructor params) to constants to the values which should be tested. The enum value corresponding to `0` should not receive an attribute; it will be returned if none of the regexes are matched.

Use the method `RegexTestAttribute.TestAgainst<T>` to perform a test on a given string. The returned enum value is the lowest-numbered value for which the test passed, and the given Match object is the resulting match.

Full code example:

```cs
public enum TestEnum
{
  [RegexTest(@"^(\d{4})-(\d\d)-(\d\d)$")] Date = 1,
  [RegexTest(@"^(\d\d):(\d\d):(\d\d)$")] Time = 2,
  [RegexTest(@"^\d+$")] Number = 3,
  Text = 0
}

public static class Program
{
  static string Identify(string input)
  {
    switch (RegexTestAttribute.TestAgainst<TestEnum>(input, out Match? mtc))
    {
      case TestEnum.Date:
        return $"A date with year {mtc.Groups[1].Value}, month {mtc.Groups[2].Value}, and day {mtc.Groups[3].Value}";
      case TestEnum.Time:
        return $"A time with hour {mtc.Groups[1].Value}, minute {mtc.Groups[2].Value}, and second {mtc.Groups[3].Value}";
      case TestEnum.Number:
        return $"The integer {input}";
      default:
        else return $"The string {input}";
    }
  }

  [Obsolete]
  static string IdentifyOld(string input)
  {
    (TestEnum which, Match? mtc) = RegexTestAttribute.TestAgainst<TestEnum>(input);

    if (which == TestEnum.Date)
      return $"A date with year {mtc.Groups[1].Value}, month {mtc.Groups[2].Value}, and day {mtc.Groups[3].Value}";
    else if (which == TestEnum.Time)
      return $"A time with hour {mtc.Groups[1].Value}, minute {mtc.Groups[2].Value}, and second {mtc.Groups[3].Value}";
    else if (which == TestEnum.Number)
      return $"The integer {input}";
    else return $"The string {input}";
  }

  static void Main(string[] args)
  {
    Console.WriteLine(Identify("2024-12-12"));
    Console.WriteLine(Identify("21:29:37"));
    Console.WriteLine(Identify("237"));
    Console.WriteLine(Identify("Hello world!"));
  }
}
```