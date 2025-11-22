using Nixill.Collections;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods good for sequences.
/// </summary>
public static class SequenceExtensions
{
  /// <summary>
  ///   Returns the element at the given index in the list, or a
  ///   predetermined element if the list is not long enough to reach the
  ///   given index.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="index">The index at which to find an item.</param>
  /// <param name="or">The fallback item.</param>
  /// <returns>
  ///   The item at the given position in the sequence, or the fallback item.
  /// </returns>
  public static T ElementAtOr<T>(this IEnumerable<T> items, Index index, T or)
    => items.ElementAtOr(index, () => or);

  /// <summary>
  ///   Returns the element at the given index in the list, or a
  ///   calculated element if the list is not long enough to reach the
  ///   given index.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="index">The index at which to find an item.</param>
  /// <param name="or">The function to generate the fallback item.</param>
  /// <returns>
  ///   The item at the given position in the sequence, or the fallback item.
  /// </returns>
  public static T ElementAtOr<T>(this IEnumerable<T> items, Index index, Func<T> or)
    => (!index.IsFromEnd) ? items.ElementAtOr(index.Value, or)
      : items.ElementAtOrNegative(index.Value, or);

  /// <summary>
  ///   Returns the element at the given index in the list, or a
  ///   predetermined element if the list is not long enough to reach the
  ///   given index.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="atIndex">The index at which to find an item.</param>
  /// <param name="or">The fallback item.</param>
  /// <returns>
  ///   The item at the given position in the sequence, or the fallback item.
  /// </returns>
  public static T ElementAtOr<T>(this IEnumerable<T> items, int atIndex, T or)
    => items.ElementAtOr(atIndex, () => or);

  /// <summary>
  ///   Returns the element at the given index in the list, or a
  ///   calculated element if the list is not long enough to reach the
  ///   given index.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="atIndex">The index at which to find an item.</param>
  /// <param name="or">The function to generate the fallback item.</param>
  /// <returns>
  ///   The item at the given position in the sequence, or the fallback item.
  /// </returns>
  public static T ElementAtOr<T>(this IEnumerable<T> items, int atIndex, Func<T> or)
  {
    foreach ((int index, T item) in items.Index())
    {
      if (index == atIndex) return item;
    }
    return or();
  }

  static T ElementAtOrNegative<T>(this IEnumerable<T> items, int atNegativeIndex, Func<T> or)
  {
    Buffer<T> buffer = new(atNegativeIndex, items);
    if (buffer.Count != buffer.BufferSize) return or();
    else return buffer.First();
  }

  /// <summary>
  ///   Returns a slice of a sequence with a given range.
  /// </summary>
  /// <remarks>
  ///   If the <c>end</c> comes at or after the <c>start</c>, an empty
  ///   sequence is returned.
  /// </remarks>
  /// <typeparam name="TSource">
  ///   The teype of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="range">The range from which to return items.</param>
  /// <returns></returns>
  public static IEnumerable<TSource> ElementsAt<TSource>(this IEnumerable<TSource> items, Range range)
    => (!range.Start.IsFromEnd)
      ? ((!range.End.IsFromEnd) ? ElementsAtPP(items, range) : ElementsAtPN(items, range))
      : (ElementsAtNX(items, range));

  static IEnumerable<T> ElementsAtPP<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    if (end <= start) yield break;

    foreach ((int index, T item) in items.Index())
    {
      if (index >= end) yield break;
      if (index >= start) yield return item;
    }
  }

  static IEnumerable<T> ElementsAtPN<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    Buffer<(T, int)> buffer = new(end);

    foreach ((int index, T item) in items.Index())
    {
      (bool bumped, (T bumpedItem, int bumpedIndex)) = buffer.Add((item, index));
      if (bumped && bumpedIndex >= start) yield return bumpedItem;
    }
  }

  static IEnumerable<T> ElementsAtNX<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;

    Buffer<(T, int)> buffer = new(start);
    int count = 0;

    if (range.End.IsFromEnd)
    {
      foreach ((int index, T item) in items.Index())
      {
        buffer.Add((item, index));
        count = index;
      }
    }
    else
    {
      foreach ((int index, T item) in items.Index())
      {
        (bool bumped, (T bumpedItem, int bumpedIndex)) = buffer.Add((item, index));
        count = index;
        if (bumpedIndex >= range.End.Value) yield break;
      }
    }

    int end = range.End.GetOffset(count);

    foreach ((T item, int index) in buffer)
    {
      if (index >= end) yield break;
      yield return item;
    }
  }

  /// <summary>
  ///   Returns elements at the given indices within the sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="indices">
  ///   The indices; this is also the order of returns.
  /// </param>
  /// <returns>The elements at those indices.</returns>
  public static IEnumerable<T> ElementsAt<T>(this IEnumerable<T> items, params Index[] indices)
  {
    List<Index> indexes = [.. indices];
    List<int> toStore = indices.Where(i => !i.IsFromEnd).Select(i => i.Value).Distinct().Order().ToList();
    Dictionary<int, T> elementsAt = [];
    Buffer<T> buffer = new Buffer<T>(indices.Where(i => i.IsFromEnd).Select(i => i.Value).DefaultIfEmpty(0).Max());

    foreach ((int index, T item) in items.Index())
    {
      buffer.Add(item);

      if (index == toStore[0])
      {
        elementsAt[index] = item;
        toStore.Pop();
      }

      while (!indexes[0].IsFromEnd && indexes[0].Value <= index)
      {
        yield return elementsAt[indexes.Pop().Value];
        if (indexes.Count == 0) yield break;
      }
    }

    List<int> toStoreNegative = indices
      .Where(i => i.IsFromEnd)
      .Select(i => i.GetOffset(buffer.BufferSize))
      .Distinct()
      .Order()
      .ToList();
    Dictionary<int, T> elementsAtNegative = [];

    foreach ((int index, T item) in buffer.Index())
    {
      if (index + 1 == toStoreNegative[0])
      {
        elementsAtNegative[index + 1] = item;
        toStoreNegative.Pop();
      }
    }

    while (indexes.Count > 0)
    {
      Index i = indexes.Pop();
      if (i.IsFromEnd) yield return elementsAtNegative[i.GetOffset(buffer.BufferSize)];
      else yield return elementsAt[i.Value];
    }
  }

  /// <summary>
  ///   Returns the sequence except with one element skipped.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="index">The index to skip.</param>
  /// <returns>The sequence, less the skipped item.</returns>
  public static IEnumerable<T> ExceptElementAt<T>(this IEnumerable<T> items, Index index)
  {
    if (index.IsFromEnd) return ExceptElementAtFromEnd(items, index.Value);
    else return ExceptElementAt(items, index.Value);
  }

  /// <summary>
  ///   Returns the sequence except with one element skipped.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="index">The index to skip.</param>
  /// <returns>The sequence, less the skipped item.</returns>
  public static IEnumerable<T> ExceptElementAt<T>(this IEnumerable<T> items, int index)
  {
    foreach ((int itemIndex, T item) in items.Index())
    {
      if (itemIndex != index) yield return item;
    }
  }

  /// <summary>
  ///   Returns the sequence except with one element skipped.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="index">
  ///   The index to skip, counting from the end.
  /// </param>
  /// <returns>The sequence, less the skipped item.</returns>
  public static IEnumerable<T> ExceptElementAtFromEnd<T>(this IEnumerable<T> items, int index)
  {
    Buffer<T> buffer = new Buffer<T>(index);

    foreach (T item in items)
    {
      (bool bumped, T bumpedItem) = buffer.Add(item);
      if (bumped) yield return bumpedItem;
    }

    foreach (T item in buffer.Skip(1))
    {
      yield return item;
    }
  }

  /// <summary>
  ///   Returns the sequence with a ranged slice removed.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="range">The range to remove.</param>
  /// <returns>The sequence, less the specified range.</returns>
  public static IEnumerable<T> ExceptElementsAt<T>(this IEnumerable<T> items, Range range)
    => (!range.Start.IsFromEnd)
      ? ((!range.End.IsFromEnd) ? ExceptElementsAtPP(items, range) : ExceptElementsAtPN(items, range))
      : ((!range.End.IsFromEnd) ? ExceptElementsAtNP(items, range) : ExceptElementsAtNN(items, range));

  static IEnumerable<T> ExceptElementsAtPP<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    foreach ((int index, T item) in items.Index())
    {
      if (index < start || index >= end) yield return item;
    }
  }

  static IEnumerable<T> ExceptElementsAtPN<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    Buffer<(T, int)> buffer = new(end);

    foreach ((int index, T item) in items.Index())
    {
      (bool bumped, (T bumpedItem, int bumpedIndex)) = buffer.Add((item, index));
      if (bumped && bumpedIndex < start) yield return bumpedItem;
    }

    foreach ((T item, int index) in buffer)
    {
      yield return item;
    }
  }

  static IEnumerable<T> ExceptElementsAtNP<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    Buffer<(T, int)> buffer = new(start);

    foreach ((int index, T item) in items.Index())
    {
      (bool bumped, (T bumpedItem, int bumpedIndex)) = buffer.Add((item, index));
      if (bumped)
      {
        yield return bumpedItem;
      }
    }

    foreach ((T item, int index) in buffer)
    {
      if (index >= end) yield return item;
    }
  }

  static IEnumerable<T> ExceptElementsAtNN<T>(IEnumerable<T> items, Range range)
  {
    int start = range.Start.Value;
    int end = range.End.Value;

    Buffer<T> buffer = new(start);

    foreach (T item in items)
    {
      (bool bumped, T bumpedItem) = buffer.Add(item);
      if (bumped) yield return bumpedItem;
    }

    foreach (T item in buffer.Skip(start - end)) yield return item;
  }

  /// <summary>
  ///   Returns the sequence with multiple items removed.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="indices">The indices at which to skip.</param>
  /// <returns>The sequence, less the specified elements.</returns>
  public static IEnumerable<T> ExceptElementsAt<T>(this IEnumerable<T> items, params Index[] indices)
  {
    List<Index> positiveIndices = indices.Where(i => !i.IsFromEnd).OrderBy(i => i.Value).Distinct().ToList();
    List<Index> negativeIndices = indices.Where(i => i.IsFromEnd).OrderByDescending(i => i.Value).Distinct().ToList();

    Buffer<(T, int)> buffer = new(negativeIndices.Select(i => i.Value).FirstOrDefault());

    foreach ((int index, T item) in items.Index())
    {
      (bool bumped, (T bumpedItem, int bumpedIndex)) = buffer.Add((item, index));
      if (bumped)
      {
        if (positiveIndices.Count != 0 && bumpedIndex == positiveIndices[0].Value)
        {
          positiveIndices.RemoveAt(0);
        }
        else
        {
          yield return bumpedItem;
        }
      }
    }

    foreach ((int index, (T item, int wrongIndex)) in buffer.Index())
    {
      if (negativeIndices.Count != 0 && index == negativeIndices[0].GetOffset(buffer.BufferSize))
      {
        negativeIndices.Pop();
      }
      else
      {
        yield return item;
      }
    }
  }

  /// <summary>
  ///   Returns the element in the middle of the sequence.
  /// </summary>
  /// <remarks>
  ///   If the sequence contains an even number of elements, the first of
  ///   the two middle elements is returned.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <param name="singleEnumeration">
  ///   Whether or not the single enumeration method should be used.
  ///   <para/>
  ///   If <c>true</c>, the sequence will only be enumerated once. The
  ///   single enumeration method is slower and uses more memory. If
  ///   <c>false</c> or unspecified, the sequence will be enumerated one
  ///   and a half times to retrieve the middle element, but no major
  ///   memory use is required.
  /// </param>
  /// <returns></returns>
  public static T Middle<T>(this IEnumerable<T> sequence, bool singleEnumeration = false)
  {
    if (singleEnumeration) return MiddleSingle(sequence);
    else return MiddleDouble(sequence);
  }

  static T MiddleDouble<T>(IEnumerable<T> sequence)
  {
    int count = sequence.Count();
    return sequence.ElementAt(count / 2);
  }

  static T MiddleSingle<T>(IEnumerable<T> sequence)
  {
    List<T> buffer = [];

    foreach (T[] pair in sequence.Chunk(2))
    {
      buffer.Add(pair[0]);
      if (pair.Length == 1) return buffer[0];
      buffer.RemoveAt(0);
      buffer.Add(pair[1]);
    }
    return buffer[0];
  }

  /// <summary>
  ///   Returns every consecutive pair of elements in the sequence.
  /// </summary>
  /// <remarks>
  ///   For example, the pairs of the sequence <c>[1, 2, 3, 4, 5]<c/> are
  ///   <c>(1, 2)</c>, <c>(2, 3)</c>, <c>(3, 4)</c>, and <c>(4, 5)</c>.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <returns>The pairs.</returns>
  public static IEnumerable<(T, T)> Pairs<T>(this IEnumerable<T> sequence)
  {
    bool first = true;
    T last = default(T)!;

    foreach (T item in sequence)
    {
      if (!first) yield return (last, item);
      last = item;
      first = false;
    }
  }

  /// <summary>
  ///   Repeats a sequence a number of times.
  /// </summary>
  /// <remarks>
  ///   <c>seq</c> is enumerated from scratch each time it is repeated.
  ///   Exercise caution when using with sequences where multiple
  ///   enumeration changes the results (you may wish to `ToArray()` first).
  /// </remarks>
  /// <typeparam name="T">The type of items in the sequence.</typeparam>
  /// <param name="seq">The sequence.</param>
  /// <param name="count">The number of times to repeat it.</param>
  /// <returns>The repeated sequence.</returns>
  public static IEnumerable<T> Repeat<T>(this IEnumerable<T> seq, int count)
  {
    foreach (int i in Enumerable.Range(0, count))
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  /// <summary>
  ///   Repeats a sequence infinitely.
  /// </summary>
  /// <remarks>
  ///   <c>seq</c> is enumerated from scratch each time it is repeated.
  ///   Exercise caution when using with sequences where multiple
  ///   enumeration changes the results (you may wish to `ToArray()` first).
  ///   <para/>
  ///   When the input <c>seq</c> is empty, this method will hang.
  ///   <para/>
  ///   This method produces a sequence of infinite length. Methods that
  ///   expect to wholly enumerate a sequence before returning anything
  ///   will hang when this sequence is used as input.
  /// </remarks>
  /// <typeparam name="T">The type of items in the sequence.</typeparam>
  /// <param name="seq">The sequence.</param>
  /// <returns>The infinitely repeated sequence.</returns>
  public static IEnumerable<T> RepeatInfinite<T>(this IEnumerable<T> seq)
  {
    while (true)
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  /// <summary>
  ///   Aggregates all the values in a sequence, returning the aggregate
  ///   value at every step in the sequence.
  /// </summary>
  /// <typeparam name="TIn">The type of items in the sequence.</typeparam>
  /// <typeparam name="TOut">The type of the aggregate of the items.</typeparam>
  /// <param name="input">The input sequence.</param>
  /// <param name="seed">
  ///   The initial aggregate value (not included in the output).
  /// </param>
  /// <param name="aggregator">
  ///   The function that adds the current value to the aggregate.
  /// </param>
  /// <returns>A sequence of all the aggregate values.</returns>
  /// <remarks>
  ///   <c>aggregator</c> is never run if the input sequence is empty.
  /// </remarks>
  public static IEnumerable<TOut> SelectAggregate<TIn, TOut>(this IEnumerable<TIn> input, TOut seed,
    Func<TOut, TIn, TOut> aggregator)
  {
    TOut agg = seed;
    foreach (TIn item in input)
      yield return agg = aggregator(agg, item);
  }

  /// <summary>
  ///   Aggregates all the values in a sequence, returning the aggregate
  ///   value at every step in the sequence.
  /// </summary>
  /// <typeparam name="TIn">The type of items in the sequence.</typeparam>
  /// <typeparam name="TOut">The type of the aggregate of the items.</typeparam>
  /// <param name="input">The input sequence.</param>
  /// <param name="seeder">
  ///   The function that turns the first item into an aggregate.
  /// </param>
  /// <param name="aggregator">
  ///   The function that adds the current value to the aggregate.
  /// </param>
  /// <returns>A sequence of all the aggregate values.</returns>
  /// <remarks>
  ///   <c>seeder</c> is never run if the input sequence is empty.
  ///   <c>aggregator</c> is never run if the input sequence has fewer
  ///   than two items.
  /// </remarks>
  public static IEnumerable<TOut> SelectAggregate<TIn, TOut>(this IEnumerable<TIn> input, Func<TIn, TOut> seeder,
    Func<TOut, TIn, TOut> aggregator)
  {
    IEnumerator<TIn> enm = input.GetEnumerator();

    if (!enm.MoveNext()) yield break;

    TOut agg = seeder(enm.Current);
    yield return agg;

    while (enm.MoveNext())
      yield return agg = aggregator(agg, enm.Current);
  }

  /// <summary>
  ///   Aggregates all the values in a sequence, returning the aggregate
  ///   value at every step in the sequence, using the first item in the
  ///   sequence as the seed.
  /// </summary>
  /// <typeparam name="T">The type of items in the sequence.</typeparam>
  /// <param name="input">The input sequence.</param>
  /// <param name="aggregator">
  ///   The function that adds the current value to the aggregate.
  /// </param>
  /// <returns>A sequence of all the aggregate values.</returns>
  /// <remarks>
  ///   The first item is always returned unchanged. <c>aggregator</c> is
  ///   never run if the input sequence has fewer than two items.
  /// </remarks>
  public static IEnumerable<T> SelectAggregate<T>(this IEnumerable<T> input, Func<T, T, T> aggregator)
  {
    IEnumerator<T> enm = input.GetEnumerator();

    if (!enm.MoveNext()) yield break;

    T agg = enm.Current;
    yield return agg;

    while (enm.MoveNext())
      yield return agg = aggregator(agg, enm.Current);
  }

  /// <summary>
  ///   Applies a transformation function to the elements of a sequence,
  ///   ignoring and discarding errors.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The original type of elements in the sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The transformed type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The sequence.</param>
  /// <param name="selector">The transformation function.</param>
  /// <returns>The transformed items.</returns>
  public static IEnumerable<TOut> SelectUnerrored<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
  {
    foreach (TIn item in items)
    {
      if (TryRun(item, selector, out var selected)) yield return selected;
    }
  }

  /// <summary>
  ///   Selects mutated elements for which a condition passes, such as if
  ///   a value can be parsed or a key exists in a dictionary, while
  ///   skipping elements for which the condition doesn't pass.
  /// </summary>
  /// <typeparam name="TIn">The input type.</typeparam>
  /// <typeparam name="TOut">The output type.</typeparam>
  /// <param name="input">The input sequence.</param>
  /// <param name="outPredicate">
  ///   The function that attempts to transform items.
  /// </param>
  /// <returns>The sequence of successful transformations.</returns>
  public static IEnumerable<TOut> TrySelect<TIn, TOut>(this IEnumerable<TIn> input, Func<TIn, (bool Passed, TOut Mutated)> conditionalSelector)
  {
    foreach (TIn item in input)
    {
      (bool passed, TOut mutated) = conditionalSelector(item);
      if (passed) yield return mutated;
    }
  }

  /// <summary>
  ///   Splits a sequence into subsequences when the positions between two
  ///   consecutive elements meet a certain condition.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="sequence">
  ///   The sequence.
  /// </param>
  /// <param name="check">
  ///   The function to determine whether the sequence should be split
  ///   between the given elements.
  /// </param>
  /// <returns>The split sequence.</returns>
  public static IEnumerable<IEnumerable<T>> SplitBetween<T>(this IEnumerable<T> sequence, Func<T, T, bool> check)
  {
    var enumerator = sequence.GetEnumerator();
    List<T> chunk = [];

    if (!enumerator.MoveNext()) yield break;

    chunk = [enumerator.Current];
    T last = enumerator.Current;

    while (enumerator.MoveNext())
    {
      if (check(last, enumerator.Current))
      {
        yield return chunk;
        chunk = [enumerator.Current];
      }
      else
      {
        chunk.Add(enumerator.Current);
      }
    }

    yield return chunk;
  }

  /// <summary>
  ///   Splits a sequence into subsequences on elements that meet a
  ///   certain condition (removing those elements from the sequence).
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="sequence">
  ///   The sequence.
  /// </param>
  /// <param name="check">
  ///   The function to determine whether the sequence should be split
  ///   across the given element.
  /// </param>
  /// <returns>The split sequence.</returns>
  public static IEnumerable<IEnumerable<T>> SplitOn<T>(this IEnumerable<T> sequence, Func<T, bool> check)
  {
    List<T> chunk = [];

    foreach (T item in sequence)
    {
      if (check(item))
      {
        if (chunk.Count > 0) yield return chunk;
        chunk = [];
      }
      else
      {
        chunk.Add(item);
      }
    }

    if (chunk.Count > 0) yield return chunk;
  }

  static bool TryRun<TIn, TOut>(TIn item, Func<TIn, TOut> selector, out TOut selected)
  {
    try
    {
      selected = selector(item);
      return true;
    }
    catch (Exception)
    {
      selected = default(TOut)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns elements from a sequence that are already in order by a
  ///   specific key, discarding the ones that break that order.
  /// </summary>
  /// <typeparam name="TSource">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <typeparam name="TKey">
  ///   The type of keys being compared.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <param name="mutator">The selector of keys for comparison.</param>
  /// <param name="comparer">The comparer of keys.</param>
  /// <param name="desc">
  ///   If <c>true</c>, elements' keys are expected to be in descending
  ///   order. Otherwise, elements are expected to be in ascending order.
  /// </param>
  /// <param name="distinctly">
  ///   If <c>true</c>, elements' keys must change for each returned
  ///   value. Otherwise, keys can be unchanged between returns, so long
  ///   as they don't change in the wrong direction.
  /// </param>
  /// <returns>A sequence of ordered elements.</returns>
  public static IEnumerable<TSource> WhereOrderedBy<TSource, TKey>(this IEnumerable<TSource> sequence,
  Func<TSource, TKey> mutator, IComparer<TKey> comparer, bool desc = false, bool distinctly = false)
  {
    bool assigned = false;
    TKey last = default(TKey)!;

    Func<int, bool> expected;

    if (desc)
      if (distinctly) expected = (i) => i < 0;
      else expected = (i) => i <= 0;
    else
      if (distinctly) expected = (i) => i > 0;
    else expected = (i) => i >= 0;

    foreach (TSource item in sequence)
    {
      TKey key = mutator(item);

      if ((!assigned) || expected(comparer.Compare(key, last)))
      {
        last = key;
        assigned = true;
        yield return item;
      }
    }
  }

  /// <summary>
  ///   Returns elements from a sequence that are already in order by a
  ///   specific key, discarding the ones that break that order.
  /// </summary>
  /// <typeparam name="TSource">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <typeparam name="TKey">
  ///   The type of keys being compared, which must be naturally
  ///   comparable.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <param name="mutator">The selector of keys for comparison.</param>
  /// <param name="desc">
  ///   If <c>true</c>, elements' keys are expected to be in descending
  ///   order. Otherwise, elements are expected to be in ascending order.
  /// </param>
  /// <param name="distinctly">
  ///   If <c>true</c>, elements' keys must change for each returned
  ///   value. Otherwise, keys can be unchanged between returns, so long
  ///   as they don't change in the wrong direction.
  /// </param>
  /// <returns>A sequence of ordered elements.</returns>
  public static IEnumerable<TSource> WhereOrderedBy<TSource, TKey>(this IEnumerable<TSource> sequence,
    Func<TSource, TKey> mutator, bool desc = false, bool distinctly = false)
      => sequence.WhereOrderedBy(mutator, Comparer<TKey>.Default, desc, distinctly);

  /// <summary>
  ///   Returns elements from a sequence that are already in order,
  ///   discarding the ones that break that order.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <param name="comparer">The comparer of keys.</param>
  /// <param name="desc">
  ///   If <c>true</c>, elements' keys are expected to be in descending
  ///   order. Otherwise, elements are expected to be in ascending order.
  /// </param>
  /// <param name="distinctly">
  ///   If <c>true</c>, elements' keys must change for each returned
  ///   value. Otherwise, keys can be unchanged between returns, so long
  ///   as they don't change in the wrong direction.
  /// </param>
  /// <returns>A sequence of ordered elements.</returns>
  public static IEnumerable<T> WhereOrdered<T>(this IEnumerable<T> sequence, IComparer<T> comparer, bool desc = false,
    bool distinctly = false) => sequence.WhereOrderedBy(x => x, comparer, desc, distinctly);

  /// <summary>
  ///   Returns elements from a sequence that are already in order,
  ///   discarding the ones that break that order.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence, which must be naturally
  ///   comparable.
  /// </typeparam>
  /// <param name="sequence">The sequence.</param>
  /// <param name="desc">
  ///   If <c>true</c>, elements' keys are expected to be in descending
  ///   order. Otherwise, elements are expected to be in ascending order.
  /// </param>
  /// <param name="distinctly">
  ///   If <c>true</c>, elements' keys must change for each returned
  ///   value. Otherwise, keys can be unchanged between returns, so long
  ///   as they don't change in the wrong direction.
  /// </param>
  /// <returns>A sequence of ordered elements.</returns>
  public static IEnumerable<T> WhereOrdered<T>(this IEnumerable<T> sequence, bool desc = false, bool distinctly = false)
    => sequence.WhereOrderedBy(x => x, Comparer<T>.Default, desc, distinctly);

  /// <summary>
  ///   Returns each element of a sequence in a tuple with its index in
  ///   the sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="original">The original sequence.</param>
  /// <returns>The sequence of tuples.</returns>
  [Obsolete("Use Index() instead. Note switched tuple element order!")]
  public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> original)
  {
    return original.Select((x, i) => (x, i));
  }
}