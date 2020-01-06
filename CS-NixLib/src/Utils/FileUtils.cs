using System.IO;
using System.Collections.Generic;

namespace Nixill.Utils {
  public class FileUtils {
    /// <summary>
    /// An iterator for characters within a file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    public static IEnumerable<char> FileCharEnumerator(string path) {
      StreamReader reader = new StreamReader(path);
      int lastChar = -1;
      while ((lastChar = reader.Read()) >= 0) {
        yield return (char)lastChar;
      }
    }
  }
}