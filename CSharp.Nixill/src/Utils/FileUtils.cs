using System.IO;
using System.Collections.Generic;

namespace Nixill.Utils;

/// <summary>
///   Utilities for dealing with file reading and writing.
/// </summary>
public static class FileUtils
{
  /// <summary>
  ///   An iterator for characters within a file.
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
  ///   An iterator for characters from a stream.
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

  /// <summary>
  ///   Copy the files in one directory to another.
  /// </summary>
  /// <param name="sourceDir">The source directory.</param>
  /// <param name="destinationDir">
  ///   The destination directory. This includes the name of the copy of
  ///   the source directory. For example, <c>sourceDir/test.txt</c>
  ///   is copied to <c>destinationDir/test.txt</c>, not
  ///   <c>destinationDir/sourceDir/test.txt</c>.
  /// </param>
  /// <param name="recursive">
  ///   Whether subfolders should also be copied.
  /// </param>
  /// <exception cref="DirectoryNotFoundException">
  ///   The source directory does not exist.
  /// </exception>
  public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
  {
    // Get information about the source directory
    var dir = new DirectoryInfo(sourceDir);

    // Check if the source directory exists
    if (!dir.Exists)
      throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    // Cache directories before we start copying
    DirectoryInfo[] dirs = dir.GetDirectories();

    // Create the destination directory
    Directory.CreateDirectory(destinationDir);

    // Get the files in the source directory and copy to the destination directory
    foreach (FileInfo file in dir.GetFiles())
    {
      string targetFilePath = Path.Combine(destinationDir, file.Name);
      file.CopyTo(targetFilePath);
    }

    // If recursive and copying subdirectories, recursively call this method
    if (recursive)
    {
      foreach (DirectoryInfo subDir in dirs)
      {
        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, true);
      }
    }
  }
}
