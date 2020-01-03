using System.Collections.Generic;
using System.Text;

namespace Nixill.Grid.CSV {
  public class CSVParser {
    public static Grid<string> FileToGrid(File input, bool emptyStrings = true) {
      // get iterator of strings
    }

    public static Grid<string> StringToGrid(string input, bool emptyStrings = true) {
      // iterate through lines of string
    }

    public static Grid<string> EnumerableToGrid(IEnumerable<char> input, bool emptyStrings = true) {
      IList<IList<string>> backingList = new List<IList<string>>();
      List<string> innerList = new List<string>();
      StringBuilder val = new StringBuilder();
      bool isEmptyLine = true;
      bool inQuotes = false;
      bool lastIsCR = false;
      bool lastIsDQ = false;

      foreach (char chr in input) {
        if (chr == '"') {
          if (inQuotes) {
            inQuotes = false;
            lastIsDQ = true;
          }
          else {
            if (lastIsDQ) {
              val.Append('"');
              lastIsDQ = false;
            }
            inQuotes = true;
          }
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (inQuotes) {
          val.Append(chr);
          lastIsDQ = false;
          lastIsCR = false;
        }
        else if (chr == ',') {
          innerList.Add(val.ToString());
          val.Clear();
          lastIsDQ = false;
          lastIsCR = false;
          isEmptyLine = false;
        }
        else if (chr == '\r') {
          innerList.Add(val.ToString());
          backingList.Add(innerList);
          val.Clear();
          innerList = new List<string>();
          lastIsCR = true;
          isEmptyLine = true;
        }
        else if (chr == '\n') {
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
          val.Append(chr);
        }
      }

      if (!isEmptyLine) {
        innerList.Add(val.ToString());
        backingList.Add(innerList);
      }

      return new Grid<string>(backingList);
    }
  }
}