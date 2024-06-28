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
    List<TSource> allItems = new();
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
    List<TSource> allItems = new();
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

  static void MakeListAdd<T>(ref List<T> list, T item)
  {
    if (list == null) list = new();
    list.Add(item);
  }

  public static IEnumerable<IEnumerable<TSource>> ChunkWhile<TSource>(this IEnumerable<TSource> items,
      Func<TSource, bool> predicate, bool appendFails = false, bool prependFails = false, bool skipEmpty = false)
      => items.ChunkWhile((_, i) => predicate(i), default(TSource), appendFails, prependFails, skipEmpty);

  public static IEnumerable<IEnumerable<TSource>> ChunkWhile<TSource>(this IEnumerable<TSource> items,
      Func<TSource, TSource, bool> predicate, TSource firstComparison = default(TSource), bool appendFails = false,
      bool prependFails = false, bool skipEmpty = false)
  {
    List<TSource> list = null;

    TSource priorItem = firstComparison;

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

  public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> elems, int limit = 0)
  {
    List<T> elemList = elems.ToList();
    int count = elemList.Count;
    if (limit < 1) limit += count;

    // Handles both the final level of recursion *and* empty lists
    if (count < 2)
    {
      yield return elemList;
      yield break;
    }

    foreach (int i in Enumerable.Range(0, count))
    {
      T elem = elemList.Pop();

      if (limit == 1)
      {
        yield return EnumerableUtils.Of(elem);
      }
      else
      {
        foreach (IEnumerable<T> sublist in elemList.Permutations(limit - 1))
        {
          yield return sublist.Prepend(elem);
        }
      }

      elemList.Add(elem);
    }
  }

  public static IEnumerable<IEnumerable<T>> PermutationsDistinct<T>(this IEnumerable<T> elems, int limit = 0)
  {
    List<T> elemList = elems.ToList();
    int count = elemList.Count;
    if (limit < 1) limit += count;

    // Handles both the final level of recursion *and* empty lists
    if (count < 2)
    {
      yield return elemList;
      yield break;
    }

    HashSet<T> yielded = new();

    foreach (int i in Enumerable.Range(0, count))
    {
      T elem = elemList.Pop();

      if (!yielded.Contains(elem))
      {
        yielded.Add(elem);

        if (limit == 1)
        {
          yield return EnumerableUtils.Of(elem);
        }
        else
        {
          foreach (IEnumerable<T> sublist in elemList.PermutationsDistinct(limit - 1))
          {
            yield return sublist.Prepend(elem);
          }
        }
      }

      elemList.Add(elem);
    }
  }

  // "limit" is non-optional here because 0 would just reutrn basically
  // the original list.
  //
  // Also, this immediately enumerates everything because otherwise it
  // would carry a risk of multiple enumeration.
  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elems, int limit)
  {
    T[] elemArray = elems.ToArray();

    if (limit <= 0) limit += elemArray.Length;
    if (limit < 0) return Enumerable.Empty<IEnumerable<T>>();
    if (limit >= elemArray.Length) return EnumerableUtils.Of(elems);

    return Combs(elemArray, limit);
  }

  static IEnumerable<IEnumerable<T>> Combs<T>(IEnumerable<T> elemList, int keep)
  {
    int skips = elemList.Count() - keep;
    if (skips == 0)
    {
      yield return elemList;
      yield break;
    }

    foreach ((T elem, int i) in elemList.Take(skips + 1).WithIndex())
    {
      if (keep == 1) yield return EnumerableUtils.Of(elem);
      else
      {
        foreach (IEnumerable<T> comb in Combs(elemList.Skip(i + 1), keep - 1))
        {
          yield return comb.Prepend(elem);
        }
      }
    }
  }

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

  public static T AggregateFromFirst<T>(this IEnumerable<T> elems, Func<T, T, T> aggregation)
  {
    bool assigned = false;
    T aggregate = default(T);

    foreach (T item in elems)
    {
      if (!assigned)
      {
        aggregate = item;
        assigned = true;
      }
      else
      {
        aggregate = aggregation(aggregate, item);
      }
    }

    if (!assigned) throw new InvalidOperationException("Sequence contains no elements.");
    return aggregate;
  }

  public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> original)
  {
    return original.Select((x, i) => (x, i));
  }

  static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, K> keySelector)
    => first.ExceptBy(second.Select(keySelector), keySelector);

  static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.ExceptBy(second.Select(keySelector), keySelector, comparer);

  static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, K> keySelector)
    => first.IntersectBy(second.Select(keySelector), keySelector);

  static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.IntersectBy(second.Select(keySelector), keySelector, comparer);

  static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second)
  {
    var set = first.ToHashSet();
    set.SymmetricExceptWith(second);
    return set;
  }

  static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
  {
    var set = first.ToHashSet(comparer);
    set.SymmetricExceptWith(second);
    return set;
  }

  public static IEnumerable<T> ExceptInstances<T>(this IEnumerable<T> first, IEnumerable<T> second)
  {
    Dictionary<T, int> counts = second.GroupBy(t => t).Select(g => new KeyValuePair<T, int>(g.Key, g.Count())).ToDictionary();

    foreach (T item in first)
    {
      if (counts.ContainsKey(item) && counts[item] > 0)
      {
        counts[item]--;
      }
      else
      {
        yield return item;
      }
    }
  }

  public static IEnumerable<T> IntersectInstances<T>(this IEnumerable<T> first, IEnumerable<T> second)
  {
    Dictionary<T, int> counts = second.GroupBy(t => t).Select(g => new KeyValuePair<T, int>(g.Key, g.Count())).ToDictionary();

    foreach (T item in first)
    {
      if (counts.ContainsKey(item) && counts[item] > 0)
      {
        counts[item]--;
        yield return item;
      }
    }
  }

  public static string SJoin<T>(this IEnumerable<T> objects, string with)
    => string.Join(with, objects);

  public static IEnumerable<(T, T)> Pairs<T>(this IEnumerable<T> sequence)
  {
    bool first = true;
    T last = default(T);

    foreach (T item in sequence)
    {
      if (!first) yield return (last, item);
      last = item;
      first = false;
    }
  }
}

[Flags]
public enum ChunkWhileOptions
{
  None = 0,
  DiscardFailures = 0,
  AppendFailures = 1,
  PrependFailures = 2
}