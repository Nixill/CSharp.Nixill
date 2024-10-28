using System;
using System.Linq;

namespace Nixill.Utils
{
  /// <summary>
  /// Methods with which to ease making comparisons in other classes.
  /// </summary>
  public static class CompareUtils
  {
    /// <summary>
    /// Returns the first non-zero parameter.
    /// 
    /// If all parameters are zero, or no parameters are supplied, zero is returned.
    /// </summary>
    public static int FirstNonZero(params int[] ints)
    {
      foreach (int i in ints)
      {
        if (i != 0)
        {
          return i;
        }
      }
      return 0;
    }

    public static bool ContainsAny(this string str, params string[] values)
      => values.Any(s => str.Contains(s));

    public static bool ContainsAny(this string str, StringComparison comparison, params string[] values)
      => values.Any(s => str.Contains(s, comparison));
  }
}