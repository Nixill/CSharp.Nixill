using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Nixill.Grid.CSV {
  public class CSVParser {
    public static Grid<string> FileToGrid(string path, bool emptyStrings = true) {
      return EnumerableToGrid(FileCharEnumerator(path), emptyStrings);
    }

    public static IEnumerable<char> FileCharEnumerator(string path) {
      StreamReader reader = new StreamReader(path);
      int lastChar = -1;
      while ((lastChar = reader.Read()) >= 0) {
        yield return (char)lastChar;
      }
    }

    public static Grid<string> StringToGrid(string input, bool emptyStrings = true) {
      return EnumerableToGrid(input, emptyStrings);
    }

    public static Grid<string> EnumerableToGrid(IEnumerable<char> input, bool emptyStrings = true) {
      IList<IList<string>> backingList = new List<IList<string>>();
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

      foreach (char chr in input) {
        if (chr == '"') {
          // Quotes always change "in quotes" state, and two in a row add 
          // a " char to the value.
          if (lastIsDQ) {
            lastIsDQ = false;
            val.Append('"');
          }
          else {
            lastIsDQ = true;
          }
          inQuotes = !inQuotes;
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (inQuotes) {
          // Aside from quotes, all characters within quotes are treated
          // literally.
          val.Append(chr);
          lastIsDQ = false;
          lastIsCR = false;
        }
        else if (chr == ',') {
          // Outside quotes, commas add new values to the current row.
          innerList.Add(val.ToString());
          val.Clear();
          lastIsDQ = false;
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (chr == '\r') {
          // Outside quotes, \r characters create new rows.
          innerList.Add(val.ToString());
          backingList.Add(innerList);
          val.Clear();
          innerList = new List<string>();
          lastIsCR = true;
          isEmptyLine = true;
        }
        else if (chr == '\n') {
          // Outside quotes, \n characters create new rows - except for \n
          // characters that immediately follow \r characters, in which
          // cases the row was already created by the \r character.
          if (lastIsCR) {
            lastIsCR = false;
          }
          else {
            innerList.Add(val.ToString());
            backingList.Add(innerList);
            val.Clear();
            innerList = new List<string>();
            isEmptyLine = true;
          }
        }
        else {
          // Everything else is literal in a CSV.
          val.Append(chr);
          lastIsCR = false;
          lastIsDQ = false;
          isEmptyLine = false;
        }
      }

      if (!isEmptyLine) {
        innerList.Add(val.ToString());
        backingList.Add(innerList);
      }

      return new Grid<string>(backingList);
    }
  }

  public IEnumerable<string> GridToStringEnumerable<T>(IGrid<T> input) {

  }
}