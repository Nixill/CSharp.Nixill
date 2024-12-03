namespace Nixill.Utils.Extensions;

public static class SequenceExtensions
{
  public static IEnumerable<IEnumerable<TSource?>> ChunkWhile<TSource>(this IEnumerable<TSource?> items,
    Func<TSource?, bool> predicate, bool appendFails = false, bool prependFails = false, bool skipEmpty = false)
    => items.ChunkWhile((_, i) => predicate(i), default(TSource), appendFails, prependFails, skipEmpty);

  public static IEnumerable<IEnumerable<TSource>> ChunkWhile<TSource>(this IEnumerable<TSource> items,
      Func<TSource, TSource, bool> predicate, TSource? firstComparison = default(TSource), bool appendFails = false,
      bool prependFails = false, bool skipEmpty = false)
  {
    List<TSource>? list = null;

    TSource priorItem = firstComparison!;

    foreach (TSource item in items)
    {
      if (predicate(priorItem, item))
      {
        MakeListAdd(ref list, item);
      }
      else
      {
        if (appendFails) MakeListAdd(ref list, item);

        if (list != null)
        {
          yield return list;
          list = null;
        }
        else if (!skipEmpty) yield return Enumerable.Empty<TSource>();

        if (prependFails) MakeListAdd(ref list, item);
      }

      priorItem = item;
    }

    if (list != null) yield return list;
  }

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

  public static IEnumerable<T> ExceptElementAt<T>(this IEnumerable<T> items, Index index)
  {
    if (index.IsFromEnd) return ExceptElementAtFromEnd(items, index.Value);
    else return ExceptElementAt(items, index.Value);
  }

  public static IEnumerable<T> ExceptElementAt<T>(this IEnumerable<T> items, int index)
  {
    foreach ((T item, int itemIndex) in items.WithIndex())
    {
      if (itemIndex != index) yield return item;
    }
  }

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

  static void MakeListAdd<T>(ref List<T>? list, T item)
  {
    if (list == null) list = new();
    list.Add(item);
  }

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

  public static IEnumerable<TOut> SelectUnerrored<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TOut> selector)
  {
    foreach (TIn item in items)
    {
      try
      {
        yield return selector(item);
      }
      finally { }
    }
  }

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

  public static IEnumerable<TSource> WhereOrderedBy<TSource, TKey>(this IEnumerable<TSource> sequence,
    Func<TSource, TKey> mutator, bool desc = false, bool distinctly = false)
      => sequence.WhereOrderedBy(mutator, Comparer<TKey>.Default, desc, distinctly);

  public static IEnumerable<T> WhereOrdered<T>(this IEnumerable<T> sequence, IComparer<T> comparer, bool desc = false,
    bool distinctly = false) => sequence.WhereOrderedBy(x => x, comparer, desc, distinctly);

  public static IEnumerable<T> WhereOrdered<T>(this IEnumerable<T> sequence, bool desc = false, bool distinctly = false)
    => sequence.WhereOrderedBy(x => x, Comparer<T>.Default, desc, distinctly);

  public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> original)
  {
    return original.Select((x, i) => (x, i));
  }
}