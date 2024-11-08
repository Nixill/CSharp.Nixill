using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Nixill.Serialization
{
  /// <summary>
  /// This class contains static methods to convert between Grids of
  /// strings and comma separated value format files and text.
  /// </summary>
  /// <remarks>
  /// The parser and formatter follow the specifications laid out in
  /// <a href="https://tools.ietf.org/html/rfc4180">RFC 4180</a>, with the
  /// following exceptions:
  ///
  /// <list type="bullet">
  /// <item><description>
  /// When parsing, records may be separated by CRLF, CR, or LF (but not
  /// LFCR).
  /// </description></item>
  /// <item><description>
  /// When parsing, two consecutive double quotes always produces a double
  /// quote character, even when not enclosed in quotes.
  /// </description></item>
  /// <item><description>
  /// When formatting, records are separated by LF, not CRLF.
  /// </description></item>
  /// </list>
  /// </remarks>
  public static class CSVParser
  {
    /// <summary>
    /// Reads a char enumerator and converts the streamed chars into a
    /// stream of grid rows.
    /// </summary>
    /// <param name="input">The input stream to read.</param>
    public static IEnumerable<IList<string>> EnumerableToRows(IEnumerable<char> input)
    {
      List<string> innerList = new List<string>();
      StringBuilder val = new StringBuilder();

      // Whether the current line is *completely* empty, devoid of all
      // chars at all - if this is true at the end of the input, don't add
      // what would be an empty row
      bool isEmptyLine = true;

      // Whether the current value is in quotes - when true, newlines and
      // commas should be taken literally.
      bool inQuotes = false;

      // Whether the previous character was a carriage return, outside of
      // quotes - when true, a linefeed should be ignored
      bool lastIsCR = false;

      // Whether the previous character was a double quote mark - when
      // true, another double quote mark should literally insert one into
      // the record.
      bool lastIsDQ = false;

      foreach (char chr in input)
      {
        if (chr == '"')
        {
          // Quotes always change "in quotes" state, and two in a row add 
          // a " char to the value.
          if (lastIsDQ)
          {
            lastIsDQ = false;
            val.Append('"');
          }
          else
          {
            lastIsDQ = true;
          }
          inQuotes = !inQuotes;
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (inQuotes)
        {
          // Aside from quotes, all characters within quotes are treated
          // literally.
          val.Append(chr);
          lastIsDQ = false;
          lastIsCR = false;
        }
        else if (chr == ',')
        {
          // Outside quotes, commas add new values to the current row.
          innerList.Add(val.ToString());
          val.Clear();
          lastIsDQ = false;
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (chr == '\r')
        {
          // Outside quotes, \r characters create new rows.
          innerList.Add(val.ToString());
          yield return innerList;
          val.Clear();
          innerList = new List<string>();
          lastIsCR = true;
          isEmptyLine = true;
        }
        else if (chr == '\n')
        {
          // Outside quotes, \n characters create new rows - except for \n
          // characters that immediately follow \r characters, in which
          // cases the row was already created by the \r character.
          if (lastIsCR)
          {
            lastIsCR = false;
          }
          else
          {
            innerList.Add(val.ToString());
            yield return innerList;
            val.Clear();
            innerList = new List<string>();
            isEmptyLine = true;
          }
        }
        else
        {
          // Everything else is literal in a CSV.
          val.Append(chr);
          lastIsCR = false;
          lastIsDQ = false;
          isEmptyLine = false;
        }
      }

      if (!isEmptyLine)
      {
        innerList.Add(val.ToString());
        yield return innerList;
      }
    }

    /// <summary>
    /// Returns a single string, escaped to be one CSV value.
    ///
    /// If the input string contains any quotes (<c>"</c>), carriage
    /// returns (<c>\r</c>), linefeeds (<c>\n</c>), or commas (<c>,</c>),
    /// all quotes within the string are doubled and the string gets
    /// wrapped in quotes. Otherwise, the input string is returned
    /// unaltered.
    /// </summary>
    public static string CSVEscape(string input)
    {
      if (input == null) return "";

      if (input.Contains('\"') || input.Contains(',') || input.Contains('\n') || input.Contains('\r'))
      {
        input = '"' + input.Replace("\"", "\"\"") + '"';
      }
      return input;
    }
  }
}