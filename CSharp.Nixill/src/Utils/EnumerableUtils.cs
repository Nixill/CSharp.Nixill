using System;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;

namespace Nixill.Utils;

public static class EnumerableUtils
{
  public static IEnumerable<TResult> Product<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> func) =>
    left.Join(right, x => 1, y => 1, func);

  public static IEnumerable<(TLeft, TRight)> Product<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
    left.Join(right, x => 1, y => 1, (left, right) => (left, right));

  public static IEnumerable<T> Repeat<T>(Func<T> func, int count) =>
    Enumerable.Repeat(0, count).Select(x => func());

  public static IEnumerable<T> RepeatInfinite<T>(Func<T> func)
  {
    while (true)
    {
      yield return func();
    }
  }

  public static IEnumerable<T> RepeatInfinite<T>(T item)
  {
    while (true)
    {
      yield return item;
    }
  }

  public static void Do<T>(this IEnumerable<T> list, Action<T> act)
  {
    foreach (T item in list)
    {
      act(item);
    }
  }

  public static void Do<T>(this IEnumerable<T> list, Action<T, int> act)
  {
    int count = 0;
    foreach (T item in list)
    {
      act(item, count++);
    }
  }

  public static IEnumerable<T> Of<T>(params T[] items)
  {
    foreach (T item in items) yield return item;
  }

  public static IEnumerable<T> Of<T>(T item)
  {
    yield return item;
  }

  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list) where T : IComparable<T>
  {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;

    foreach (T item in list)
    {
      if (!assigned || baseline.CompareTo(item) < 0)
      {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(item) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list, IComparer<T> comp)
  {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;
    Comparison<T> cFunc = comp.Compare;

    foreach (T item in list)
    {
      if (!assigned || cFunc(baseline, item) < 0)
      {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, item) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MaxManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator) where TResult : IComparable<TResult>
  {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;

    foreach (TSource item in list)
    {
      TResult mutation = mutator(item);
      if (!assigned || baseline.CompareTo(mutation) < 0)
      {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(mutation) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MaxManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator, IComparer<TResult> comp)
  {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;
    Comparison<TResult> cFunc = comp.Compare;

    foreach (TSource item in list)
    {
      TResult mutation = mutator(item);
      if (!assigned || cFunc(baseline, mutation) < 0)
      {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, mutation) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list) where T : IComparable<T>
  {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;

    foreach (T item in list)
    {
      if (!assigned || baseline.CompareTo(item) > 0)
      {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(item) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list, IComparer<T> comp)
  {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;
    Comparison<T> cFunc = comp.Compare;

    foreach (T item in list)
    {
      if (!assigned || cFunc(baseline, item) > 0)
      {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, item) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MinManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator) where TResult : IComparable<TResult>
  {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;

    foreach (TSource item in list)
    {
      TResult mutation = mutator(item);
      if (!assigned || baseline.CompareTo(mutation) > 0)
      {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(mutation) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MinManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator, IComparer<TResult> comp)
  {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;
    Comparison<TResult> cFunc = comp.Compare;

    foreach (TSource item in list)
    {
      TResult mutation = mutator(item);
      if (!assigned || cFunc(baseline, mutation) > 0)
      {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, mutation) == 0)
      {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<IEnumerable<TSource>> ChunkWhile<TSource>(this IEnumerable<TSource> items,
      Func<TSource, bool> predicate)
  {
    List<TSource> list = null;

    foreach (TSource item in items)
    {
      if (predicate(item))
      {
        if (list == null) list = new();
        list.Add(item);
      }
      else
      {
        if (list == null) yield return Enumerable.Empty<TSource>();
        else
        {
          yield return list;
          list = null;
        }
      }
    }

    if (list != null) yield return list;
  }

  public static string FormString(this IEnumerable<char> chars) => new string(chars.ToArray());

  public static IEnumerable<TSource> WhereOrderedBy<TSource, TKey>(this IEnumerable<TSource> sequence,
  Func<TSource, TKey> mutator, IComparer<TKey> comparer, bool desc = false, bool distinctly = false)
  {
    bool assigned = false;
    TKey last = default(TKey);

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

  public static T Pop<T>(this IList<T> list)
  {
    T value = list[0];
    list.RemoveAt(0);
    return value;
  }
}