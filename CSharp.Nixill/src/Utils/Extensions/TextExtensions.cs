using System.Text;
using Nixill.Collections;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods dealing with text.
/// </summary>
public static class TextExtensions
{
  /// <summary>
  ///   Joins elements of a sequence together into multiple
  ///   character-limited strings.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="seq">The sequence.</param>
  /// <param name="sep">The separator between items.</param>
  /// <param name="limit">Character limit per returned string.</param>
  /// <returns>The strings.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   <c>limit</c> is less than 1.
  /// </exception>
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

  /// <summary>
  ///   Escapes given characters from a string.
  /// </summary>
  /// <param name="input">The input string.</param>
  /// <param name="escapeChar">
  ///   The escape character. Escapes itself by doubling.
  /// </param>
  /// <param name="sequences">
  ///   Map of characters that should be escaped to escape sequences. Does
  ///   not include the escape character itself in any sequence.
  /// </param>
  /// <returns>The escaped string.</returns>
  public static string Escape(this IEnumerable<char> input, char escapeChar, IDictionary<char, string> sequences)
    => EscapeEnumerable(input, escapeChar, sequences).FormString();
  
  static IEnumerable<char> EscapeEnumerable(IEnumerable<char> input, char escapeChar, IDictionary<char, string> sequences)
  {
    foreach (char chr in input)
    {
      if (sequences.ContainsKey(chr))
      {
        yield return escapeChar;
        foreach (char chr2 in sequences[chr]) yield return chr2;
      }
      else if (chr == escapeChar)
      {
        yield return escapeChar;
        yield return escapeChar;
      }
      else
      {
        yield return chr;
      }
    }
  }

  /// <summary>
  ///   Creates a string from a char enumerable.
  /// </summary>
  /// <param name="chars">The char enumerable.</param>
  /// <returns>The string.</returns>
  public static string FormString(this IEnumerable<char> chars) => new string(chars.ToArray());

  /// <summary>
  ///   Joins objects to a string.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="objects">The sequence.</param>
  /// <param name="with">The separator.</param>
  /// <returns>The string.</returns>
  public static string StringJoin<T>(this IEnumerable<T> objects, string with)
    => string.Join(with, objects);
  
  /// <summary>
  ///   Parses escaped characters from a string.
  /// </summary>
  /// <param name="input">The input string.</param>
  /// <param name="escapeChar">
  ///   The escape character. Double this is treated as itself.
  /// </param>
  /// <param name="sequences">
  ///   Map of escape sequences (without the escape character itself) to
  ///   the haracters they represent.
  /// </param>
  /// <returns>The parsed string.</returns>
  public static string Unescape(this string input, char escapeChar, IDictionary<string, char> sequences)
    => UnescapeEnumerable(input, escapeChar, new RecursiveDictionary<char, char>(sequences.Select(kvp => new KeyValuePair<IEnumerable<char>, char>(kvp.Key, kvp.Value)))).FormString();

  public static string Unescape(this string input, char escapeChar, IDictionary<IEnumerable<char>, char> sequences)
    => UnescapeEnumerable(input, escapeChar, (sequences as RecursiveDictionary<char, char>) ?? new RecursiveDictionary<char, char>(sequences)).FormString();
  
  static IEnumerable<char> UnescapeEnumerable(IEnumerable<char> input, char escapeChar, RecursiveDictionary<char, char> sequences)
  {
    IEnumerator<char> enm = input.GetEnumerator();

    while (enm.MoveNext())
    {
      char chr = enm.Current;

      if (chr == escapeChar)
      {
        IRecursiveDictionary<char, char> view = sequences;
        while (true)
        {
          if (view.HasEmptyKeyValue)
          {
            yield return view[""];
            break;
          }

          if (!enm.MoveNext()) yield break;

          chr = enm.Current;
          
          if (chr == escapeChar && object.ReferenceEquals(view, sequences))
          {
            yield return escapeChar;
            break;
          }

          if (!view.ContainsPrefix([chr])) break;
          
          view = view.GetPrefix([chr]);
        }
      }
      else yield return chr;
    }
  }
}