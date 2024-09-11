using System.IO;
using System.Collections.Generic;

namespace Nixill.Utils
{
  public static class FileUtils
  {
    /// <summary>
    /// An iterator for characters within a file.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    public static IEnumerable<char> FileCharEnumerator(string path)
    {
      using StreamReader reader = new StreamReader(path);
      foreach (char c in StreamCharEnumerator(reader))
      {
        yield return c;
      }
      reader.Close();
    }

    /// <summary>
    /// An iterator for characters from a stream.
    /// </summary>
    /// <param name="reader">The stream to read.</param>
    public static IEnumerable<char> StreamCharEnumerator(StreamReader reader)
    {
      int lastChar = -1;
      while ((lastChar = reader.Read()) >= 0)
      {
        yield return (char)lastChar;
      }
    }
  }
}