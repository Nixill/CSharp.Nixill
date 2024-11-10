namespace Nixill.Utils.Extensions;

public static class SetExtensions
{
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
    if (limit >= elemArray.Length) return [elems];

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
      if (keep == 1) yield return [elem];
      else
      {
        foreach (IEnumerable<T> comb in Combs(elemList.Skip(i + 1), keep - 1))
        {
          yield return comb.Prepend(elem);
        }
      }
    }
  }

  public static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, K> keySelector)
    => first.ExceptBy(second.Select(keySelector), keySelector);

  public static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.ExceptBy(second.Select(keySelector), keySelector, comparer);

  public static IEnumerable<T> ExceptInstances<T>(this IEnumerable<T> first, IEnumerable<T> second)
    where T : notnull
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

  public static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
    Func<T, K> keySelector)
    => first.IntersectBy(second.Select(keySelector), keySelector);

  public static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.IntersectBy(second.Select(keySelector), keySelector, comparer);

  public static IEnumerable<T> IntersectInstances<T>(this IEnumerable<T> first, IEnumerable<T> second)
    where T : notnull
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
        yield return [elem];
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
          yield return [elem];
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

  public static IEnumerable<TResult> Product<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> func) =>
    left.Join(right, x => 1, y => 1, func);

  public static IEnumerable<(TLeft, TRight)> Product<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
    left.Join(right, x => 1, y => 1, (left, right) => (left, right));

  public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second)
  {
    var set = first.ToHashSet();
    set.SymmetricExceptWith(second);
    return set;
  }

  public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second,
    IEqualityComparer<T> comparer)
  {
    var set = first.ToHashSet(comparer);
    set.SymmetricExceptWith(second);
    return set;
  }
}