namespace Nixill.Utils.Extensions;

public static class SetExtensions
{
  #region Combinations
  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order. Distinctness of items, and therefore combinations, is not
  ///   checked.
  ///   <para/>
  ///   This method hangs if used on an infinite sequence. Use
  ///   <see cref="CombinationsInfinite{T}(IEnumerable{T}, Index)" />
  ///   instead.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If it's a "from end" index, it is interpreted as "all but this many".
  ///   <para/>
  ///   If the resulting count is less than or equal to 0, <c>[]</c> is
  ///   returned.
  ///   <para/>
  ///   If the resulting count is greater than or equal to the number of
  ///   items in the sequence, <c>[elems]</c> is returned.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elems, Index limit)
    => Combinations(elems, limit..limit);

  /// <summary>
  ///   Returns all combinations of elements from the sequence that have
  ///   lengths within the specified range.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order. Distinctness of items, and therefore combinations, is not
  ///   checked.
  ///   <para/>
  ///   This method hangs if used on an infinite sequence. Use
  ///   <see cref="CombinationsInfinite{T}(IEnumerable{T}, Range)" />
  ///   instead.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="range">
  ///   The numbers of items to return in each combination.
  ///   <para/>
  ///   If one of the indices is "from end", that index is interpreted as
  ///   "all but this many".
  ///   <para/>
  ///   If both resulting counts are less than or equal to zero, <c>[]</c>
  ///   is returned.
  ///   <para/>
  ///   If both resulting counts are greater than or equal to the size of
  ///   the sequence, <c>[elems]</c> is returned.
  /// </param>
  /// <returns>The combinations.</returns>
  /// <exception cref="IndexOutOfRangeException">
  ///   After computation, the range has a starting value that is larger
  ///   than the ending value.
  /// </exception>
  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elems, Range range)
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
    return Combs(new ArraySegment<T>(array), int.Max(minLength, 1), maxLength);
  }

  static IEnumerable<IEnumerable<T>> Combs<T>(ArraySegment<T> seg, int minLength, int maxLength)
  {
    // Shortcut: If the entire segment is the minimum allowable length,
    // then the whole segment is the only remaining combination that can
    // be returned.
    if (seg.Count == minLength)
    {
      yield return seg;
      yield break;
    }

    // Otherwise, iterate every element in the list that would head a
    // segment of allowable length.
    for (int i = 0; i <= (seg.Count - minLength); i++)
    {
      // If the minimum length is itself one, then that element by itself
      // is an allowable combination.
      if (minLength == 1) yield return [seg[i]];

      // Recursion is needed if larger combinations are allowable.
      if (maxLength > 1)
      {
        // For each combination of one fewer element from the remainder of
        // the list...
        foreach (var seq in Combs(seg[(i + 1)..], int.Max(minLength - 1, 1), maxLength - 1))
        {
          // ... yield that combination prepended by the current element.
          yield return [seg[i], .. seq];
        }
      }
    }
  }
  #endregion

  #region CombinationsDistinct
  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order. Distinctness of items is checked using the default equality
  ///   comparer.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If it's a "from end" index, it is interpreted as "all but this many".
  ///   <para/>
  ///   If the resulting count is less than or equal to 0, <c>[]</c> is
  ///   returned.
  ///   <para/>
  ///   If the resulting count is greater than or equal to the number of
  ///   items in the sequence, <c>[elems]</c> is returned.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, Index limit)
    => CombinationsDistinct(elems, limit..limit, EqualityComparer<T>.Default);

  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order. Distinctness of items is checked using the default equality
  ///   comparer.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  ///   The numbers of items to return in each sequence.
  ///   <para/>
  ///   If one of the indices is "from end", that index is interpreted as
  ///   "all but this many".
  ///   <para/>
  ///   If both resulting counts are less than or equal to zero, <c>[]</c>
  ///   is returned.
  ///   <para/>
  ///   If both resulting counts are greater than or equal to the size of
  ///   the sequence, <c>[elems]</c> is returned.
  /// <returns>The combinations.</returns>
  /// <exception cref="IndexOutOfRangeException">
  ///   After computation, the range has a starting value that is larger
  ///   than the ending value.
  /// </exception>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, Range range)
    => CombinationsDistinct(elems, range, EqualityComparer<T>.Default);

  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If it's a "from end" index, it is interpreted as "all but this many".
  ///   <para/>
  ///   If the resulting count is less than or equal to 0, <c>[]</c> is
  ///   returned.
  ///   <para/>
  ///   If the resulting count is greater than or equal to the number of
  ///   items in the sequence, <c>[elems]</c> is returned.
  /// </param>
  /// <param name="comparer">
  ///   The equality comparer with which to check distinctness of items.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, Index limit,
    IEqualityComparer<T> comparer)
    => CombinationsDistinct(elems, limit..limit, comparer);

  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence.
  /// </summary>
  /// <remarks>
  ///   Elements in each subsequence are kept in their original sequence
  ///   order.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="range">
  ///   The numbers of items to return in each sequence.
  ///   <para/>
  ///   If one of the indices is "from end", that index is interpreted as
  ///   "all but this many".
  ///   <para/>
  ///   If both resulting counts are less than or equal to zero, <c>[]</c>
  ///   is returned.
  ///   <para/>
  ///   If both resulting counts are greater than or equal to the size of
  ///   the sequence, <c>[elems]</c> is returned.
  /// </param>
  /// <param name="comparer">
  ///   The equality comparer with which to check distinctness of items.
  /// </param>
  /// <returns>The combinations.</returns>
  /// <exception cref="IndexOutOfRangeException">
  ///   After computation, the range has a starting value that is larger
  ///   than the ending value.
  /// </exception>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, Range range,
    IEqualityComparer<T> comparer)
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
    return CombsDistinct(array, int.Max(minLength, 1), maxLength, comparer);
  }

  static IEnumerable<IEnumerable<T>> CombsDistinct<T>(T[] seg, int minLength, int maxLength,
    IEqualityComparer<T> comparer)
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

      // Now remove all copies of the first element from the segment.
      seg = [.. seg.Where(e => !comparer.Equals(seg[0], e))];
    }

    // And lastly, if the remainder of the segment is itself exactly the
    // minimum length, then it is itself an allowable combination.
    if (seg.Length == minLength) yield return seg;
  }
  #endregion

  #region CombinationsInfinite
  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in an
  ///   infinite sequence.
  /// </summary>
  /// <remarks>
  ///   When fed an infinite sequence, this method itself produces an
  ///   infinite sequence. Partial enumeration of this sequence causes
  ///   partial enumeration of the input sequence. Distinctness of
  ///   elements, and therefore of combinations, is not checked.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   The input sequence must produce at least this many items before
  ///   any combinations are returned.
  ///   <para/>
  ///   If it's a "from end" index, it is interpreted as "all but this
  ///   many". This is re-evaluated as each item is produced by the input
  ///   sequence, causing this method to produce increasingly longer
  ///   sequences as enumeration continues.
  ///   <para/>
  ///   If the max size is 0, or the input sequence terminates before the
  ///   value of the min size is reached, <c>[]</c> is returned.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsInfinite<T>(this IEnumerable<T> elems, Index limit)
  => CombinationsInfinite(elems, limit..limit);

  /// <summary>
  ///   Returns all the combinations lements from an inifite sequence that
  ///   have lengths within the specified range.
  /// </summary>
  /// <remarks>
  ///   When fed an infinite sequence, this method itself produces an
  ///   infinite sequence. Partial enumeration of this sequence causes
  ///   partial enumeration of the input sequence. Distinctness of
  ///   elements, and therefore of combinations, is not checked.
  /// </remarks>
  /// <typeparam name="T">The type of elements in the sequence.</typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="range">
  ///   The numbers of items to select for each combination.
  ///   <para/>
  ///   The input sequence must produce at least this many items before
  ///   any combinations are returned.
  ///   <para/>
  ///   If one of the indices is "from end", that index is interpreted as
  ///   "all but this many". This is re-evaluated as each item is produced
  ///   by the input sequence, causing that bound to be increasingly
  ///   larger as enumeration continues.
  ///   <para/>
  ///   If the start index is "from end" and the end index is "from
  ///   start", then this sequence terminates once the minimum size
  ///   exceeds the maximum size.
  ///   <para/>
  ///   If the Index is 0, or the input sequence terminates before the
  ///   value of the Index is reached, <c>[]</c> is returned.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsInfinite<T>(this IEnumerable<T> elems, Range range)
  {
    // Take some shortcuts with values that will never produce valid results.
    if (!range.Start.IsFromEnd && !range.End.IsFromEnd && range.Start.Value > range.End.Value)
      throw new IndexOutOfRangeException("Minimum size cannot be greater than maximum size.");
    if (range.Start.IsFromEnd && range.End.IsFromEnd && range.Start.Value < range.End.Value)
      throw new IndexOutOfRangeException("Minimum size cannot be greater than maximum size (for \"from end\" indices " +
        "this means that Start cannot be closer to zero).");
    if (range.End.Equals(0)) yield break;

    // This stores values generated in order.
    List<T> values = [];

    // This is the enumerator over the input sequence.
    IEnumerator<T> enm = elems.GetEnumerator();

    // Sizing!
    int produced = 0;
    int minSize = range.Start.GetOffset(0);
    bool minSizePlus = range.Start.IsFromEnd;
    int maxSize = range.End.GetOffset(0);
    bool maxSizePlus = range.End.IsFromEnd;

    // Now we go through the elements in order.
    for (produced = 1; enm.MoveNext(); produced++)
    {
      if (minSizePlus) minSize++;
      if (maxSizePlus) maxSize++;

      if (minSizePlus && !maxSizePlus && minSize > maxSize) yield break;

      // When a combination of size 1 is permissible, return the current
      // element.
      if (minSize == 1) yield return [enm.Current];

      // Get all sub-combinations that are an element shorter than the
      // current range of size, and then append the current element to
      // each one of them.
      if (maxSize - 1 > 0 && minSize <= produced)
      {
        foreach (IEnumerable<T> combo in values.Combinations(int.Max(minSize - 1, 1)..(maxSize - 1)))
        {
          yield return [.. combo, enm.Current];
        }
      }

      // If the current size is increasing, then the new max size is also
      // permissible of all combinations, including those that don't
      // include the newest element. Those that do will have already been
      // returned, but the rest should be returned now.
      if (maxSizePlus && maxSize > 0)
      {
        foreach (IEnumerable<T> combo in values.Combinations(maxSize))
        {
          yield return combo;
        }
      }

      // Now throw the current element into the list of values.
      values.Add(enm.Current);
    }
  }
  #endregion
}