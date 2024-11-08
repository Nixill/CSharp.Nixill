using System.Text;

namespace Nixill.Utils.Extensions;

public static class TextExtensions
{
  public static IEnumerable<string> CharLimitedJoin<T>(this IEnumerable<T> seq, string sep, int limit)
  {
    if (limit < 1) throw new ArgumentOutOfRangeException("limit must be at least 1");

    // Memory to hold onto when a line's length is exceeded.
    string memory = "";

    // Iterator over the strings to be joined.
    IEnumerator<string> iter = seq.Select(x => x?.ToString() ?? "").GetEnumerator();

    while (true)
    {
      // If the first string is too long for a whole line, output the
      // part that fits into a line.
      while (memory.Length >= limit)
      {
        yield return memory[..limit];
        memory = memory[limit..];
      }

      // Start building a new output line.
      StringBuilder build = new(memory);
      bool buildEmpty = true;
      string output = "";

      while (build.Length < limit)
      {
        // Save previous output now, in case adding next item brings it
        // over the limit
        output = build.ToString();
        if (iter.MoveNext())
        {
          if (build.Length > 0 || !buildEmpty) build.Append(sep);
          build.Append(iter.Current);
          buildEmpty = false;
        }
        else
        {
          // If end of output, return it.
          if (output != "" || !buildEmpty) yield return output;
          yield break;
        }
      }

      // Return output to start preparing for next line
      yield return output;
      memory = iter.Current;
    }
  }

  public static string FormString(this IEnumerable<char> chars) => new string(chars.ToArray());

  public static string StringJoin<T>(this IEnumerable<T> objects, string with)
    => string.Join(with, objects);
}