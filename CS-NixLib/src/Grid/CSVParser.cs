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

    public static Grid<string> EnumeratorToGrid(IEnumerable<string> input, bool emptyStrings = true) {
      List<List<string>> backingList = new List<List<string>>();
      List<string> innerList = new List<string>();
      StringBuilder val = new StringBuilder();
      bool isEmptyLine = true;
      bool inQuotes = false;
      bool lastIsCR = false;
      bool lastIsDQ = false;

      foreach (string str in input) {
        foreach (char chr in str) {
          if (chr == '"') {

          }
        }
      }
    }
  }
}