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
      List<List<string>> backingList = new List<List<string>>();
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

        }
      }
    }
  }
}