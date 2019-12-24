namespace Nixill.Utils {
  /// <summary>
  /// Methods with which to ease making comparisons in other classes.
  /// </summary>
  public class CompareUtils {
    /// <summary>
    /// Returns the first non-zero parameter.
    /// 
    /// If all parameters are zero, or no parameters are supplied, zero is returned.
    /// </summary>
    public static int FirstNonZero(params int[] ints) {
      foreach (int i in ints) {
        if (ints[i] != 0) {
          return ints[i];
        }
      }
      return 0;
    }
  }
}