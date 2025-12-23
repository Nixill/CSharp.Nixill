namespace Nixill.Utils.Extensions;

internal static class SetExtensions2
{
  #region CombinationsDistinct
  private static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, Range range,
    IEqualityComparer<T> comparer, T? lookFor, int lookForAmount)
  {
    T[] array = [.. elems];

    // Compute minimum length of combinations
    int minLength = range.Start.Value;
    if (range.Start.IsFromEnd) minLength = array.Length - minLength;
    minLength = int.Clamp(minLength, 0, array.Length);

    // Compute maximum length of combinations
    int maxLength = range.End.Value;
    if (range.End.IsFromEnd) maxLength = array.Length - maxLength;
    maxLength = int.Clamp(maxLength, 0, array.Length);

    // Minimum length ≤ maximum length
    if (minLength > maxLength) throw new IndexOutOfRangeException("Minimum length must not be larger than maximum length.");

    // Shortcuts: No combinations exist with length 0, and only one
    // combination exists with length matching the array.
    if (maxLength == 0) return [];
    if (minLength == array.Length) return [array];

    // Otherwise, the long way, with a recursive function.
    return CombsDistinct(array, int.Max(minLength, 1), maxLength, comparer, lookFor, lookForAmount);
  }

  static IEnumerable<IEnumerable<T>> CombsDistinct<T>(T[] seg, int minLength, int maxLength,
    IEqualityComparer<T> comparer, T? lookFor = default, int lookForAmount = 0)
  {
    // We'll loop by removing all copies of the first element from the
    // array until it's too small to continue.
    while (seg.Length > minLength)
    {
      // If the minimum length is itself one, then the first element by
      // itself is an allowable combination.
      if (minLength == 1) yield return [seg[0]];

      // Recursion is needed if larger combinations are allowable.
      if (maxLength > 1 && seg.Length > 1)
      {
        // For each combination of one fewer element from the remainder of
        // the list...
        foreach (var seq in CombsDistinct(seg[1..], int.Max(minLength - 1, 1), maxLength - 1, comparer))
        {
          // .. yield that combination prepended by the current element.
          yield return [seg[0], .. seq];
        }
      }

      // Now we'd remove all copies of the first element from the segment.
      // However, if that means the element we need to look for is now
      // gone, we'll just stop the method instead.
      seg = [.. seg.Where(e => !comparer.Equals(seg[0], e))];
    }

    // And lastly, if the remainder of the segment is itself exactly the
    // minimum length, then it is itself an allowable combination.
    if (seg.Length == minLength) yield return seg;
  }
  #endregion
}