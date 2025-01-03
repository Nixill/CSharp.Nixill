namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods dealing with sets.
/// </summary>
public static class SetExtensions
{
  /// <summary>
  ///   Returns all the combinations of <c>limit</c> of the elements in
  ///   the sequence, with items kept in their original sequence order.
  /// </summary>
  /// <remarks>
  ///   Distinctness of items, and therefore combinations, is not checked.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If less than or equal to 0, it is interpreted as "all but this many".
  ///   <para/>
  ///   If greater than the number of elements, the original list is
  ///   returned as the only item of the outer enumerable.
  /// </param>
  /// <returns>The combinations.</returns>
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

  /// <summary>
  ///   Returns all the distinct combinations of <c>limit</c> of the
  ///   elements in the sequence, with items kept in their original
  ///   sequence order.
  /// </summary>
  /// <remarks>
  ///   Elements are checked for equality using their default equality
  ///   comparer.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If less than or equal to 0, it is interpreted as "all but this many".
  ///   <para/>
  ///   If greater than the number of elements, the original list is
  ///   returned as the only item of the outer enumerable.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, int limit)
    => elems.CombinationsDistinct(limit, EqualityComparer<T>.Default);

  /// <summary>
  ///   Returns all the distinct combinations of <c>limit</c> of the
  ///   elements in the sequence, with items kept in their original
  ///   sequence order.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of items to select for each combination.
  ///   <para/>
  ///   If less than or equal to 0, it is interpreted as "all but this many".
  ///   <para/>
  ///   If greater than the number of elements, the original list is
  ///   returned as the only item of the outer enumerable.
  /// </param>
  /// <param name="comparer">
  ///   The comparer checking equality between elements.
  /// </param>
  /// <returns>The combinations.</returns>
  public static IEnumerable<IEnumerable<T>> CombinationsDistinct<T>(this IEnumerable<T> elems, int limit,
    IEqualityComparer<T> comparer)
  {
    T[] elemArray = elems.ToArray();

    if (limit <= 0) limit += elemArray.Length;
    if (limit < 0) return Enumerable.Empty<IEnumerable<T>>();
    if (limit >= elemArray.Length) return [elems];

    return CombsDistinct(elemArray, limit, comparer);
  }

  static IEnumerable<IEnumerable<T>> CombsDistinct<T>(T[] elemList, int keep, IEqualityComparer<T> comparer)
  {
    HashSet<T> given = new HashSet<T>(comparer);

    int skips = elemList.Length - keep;
    if (skips == 0)
    {
      yield return elemList;
      yield break;
    }

    foreach ((T elem, int i) in elemList.Take(skips + 1).WithIndex())
    {
      if (given.Contains(elem)) continue;
      given.Add(elem);

      if (keep == 1) yield return [elem];
      else
      {
        foreach (IEnumerable<T> comb in CombsDistinct(elemList[(i + 1)..], keep - 1, comparer))
        {
          yield return comb.Prepend(elem);
        }
      }
    }
  }

  /// <summary>
  ///   Returns items that are in the first sequence, except those with a
  ///   matching key in the second sequence.
  /// </summary>
  /// <remarks>
  ///   This method differs from <see cref="Enumerable.ExceptBy{TSource,
  ///     TKey}(IEnumerable{TSource}, IEnumerable{TKey}, Func{TSource,
  ///     TKey})"/> in that this method applies the mutator to both
  ///   sequences, whereas that method applies it only to the first.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <typeparam name="K">
  ///   The type of keys being compared, which must be naturally equatable.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <param name="keySelector">The key selector.</param>
  /// <returns>The limited sequence.</returns>
  public static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, K> keySelector)
    => first.ExceptBy(second.Select(keySelector), keySelector);

  /// <summary>
  ///   Returns items that are in the first sequence, except those with a
  ///   matching key in the second sequence.
  /// </summary>
  /// <remarks>
  ///   This method differs from <see cref="Enumerable.ExceptBy{TSource,
  ///     TKey}(IEnumerable{TSource}, IEnumerable{TKey}, Func{TSource,
  ///     TKey}, IEqualityComparer{TKey}?)"/> in that this method applies
  ///   the mutator to both sequences, whereas that method applies it only
  ///   to the first.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <typeparam name="K">
  ///   The type of keys being compared.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <param name="keySelector">The key selector.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The limited sequence.</returns>
  public static IEnumerable<T> ExceptDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.ExceptBy(second.Select(keySelector), keySelector, comparer);

  /// <summary>
  ///   Returns items that are in the first sequence but not the second,
  ///   treating each occurrence of the item as a distinct item.
  /// </summary>
  /// <remarks>
  ///   For example, <c>"ARBITRARY"</c> except-instances
  ///   <c>"REMARKABLE"</c> is <c>"ITRY"</c>; note that one of the three
  ///   <c>R</c>'s stayed.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <returns>The limited sequence.</returns>
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


  /// <summary>
  ///   Returns items that are in the first sequence, as long as they have
  ///   a matching key in the second sequence.
  /// </summary>
  /// <remarks>
  ///   This method differs from <see
  ///     cref="Enumerable.IntersectBy{TSource,
  ///     TKey}(IEnumerable{TSource}, IEnumerable{TKey}, Func{TSource,
  ///     TKey})"/> in that this method applies the mutator to both
  ///   sequences, whereas that method applies it only to the first.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <typeparam name="K">
  ///   The type of keys being compared, which must be naturally equatable.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <param name="keySelector">The key selector.</param>
  /// <returns>The limited sequence.</returns>
  public static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
    Func<T, K> keySelector)
    => first.IntersectBy(second.Select(keySelector), keySelector);


  /// <summary>
  ///   Returns items that are in the first sequence, as long as they have
  ///   a matching key in the second sequence.
  /// </summary>
  /// <remarks>
  ///   This method differs from <see
  ///     cref="Enumerable.IntersectBy{TSource,
  ///     TKey}(IEnumerable{TSource}, IEnumerable{TKey}, Func{TSource,
  ///     TKey}, IEqualityComparer{TKey}?)"/> in that this method applies
  ///   the mutator to both sequences, whereas that method applies it only
  ///   to the first.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <typeparam name="K">
  ///   The type of keys being compared.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <param name="keySelector">The key selector.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The limited sequence.</returns>
  public static IEnumerable<T> IntersectDualBy<T, K>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, K> keySelector, IEqualityComparer<K> comparer)
    => first.IntersectBy(second.Select(keySelector), keySelector, comparer);

  /// <summary>
  ///   Returns items from the first sequence that are also in the second,
  ///   treating each occurrence of the item as a distinct item.
  /// </summary>
  /// <remarks>
  ///   For example, <c>"ARBITRARY"</c> intersect-instances
  ///   <c>"REMARKABLE"</c> is <c>"ARBRA"</c>; note that two of the three
  ///   <c>R</c>'s stayed.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <returns>The limited sequence.</returns>
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

  /// <summary>
  ///   Returns all the permutations of <c>limit</c> elements from the
  ///   sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of elements to retrieve.
  ///   <para/>
  ///   If this is zero or negative, it is interpreted as "all but this many".
  /// </param>
  /// <returns>The permutations.</returns>
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

  /// <summary>
  ///   Returns all the distinct permutations of <c>limit</c> elements
  ///   from the sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="elems">The sequence.</param>
  /// <param name="limit">
  ///   The number of elements to retrieve.
  ///   <para/>
  ///   If this is zero or negative, it is interpreted as "all but this many".
  /// </param>
  /// <returns>The distinct permutations.</returns>
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

  /// <summary>
  ///   Returns the Cartesian product of two sequences, transformed by a
  ///   function into a third type.
  /// </summary>
  /// <typeparam name="TLeft">
  ///   The type of elements in the first sequence.
  /// </typeparam>
  /// <typeparam name="TRight">
  ///   The type of elements in the second sequence.
  /// </typeparam>
  /// <typeparam name="TResult">
  ///   The type of results of the transformation function.
  /// </typeparam>
  /// <param name="left">The first sequence.</param>
  /// <param name="right">The second sequence.</param>
  /// <param name="func">
  ///   The transformation function that takes both products and returns
  ///   a result.
  /// </param>
  /// <returns>The Cartesian product.</returns>
  public static IEnumerable<TResult> Product<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> func) =>
    left.Join(right, x => 1, y => 1, func);

  /// <summary>
  ///   Returns the Cartesian product of two sequences as tuples of an
  ///   element from each sequence.
  /// </summary>
  /// <typeparam name="TLeft">
  ///   The type of elements in the first sequence.
  /// </typeparam>
  /// <typeparam name="TRight">
  ///   The type of elements in the second sequence.
  /// </typeparam>
  /// <param name="left">The first sequence.</param>
  /// <param name="right">The second sequence.</param>
  /// <returns>The Cartesian product.</returns>
  public static IEnumerable<(TLeft, TRight)> Product<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
    left.Join(right, x => 1, y => 1, (left, right) => (left, right));

  /// <summary>
  ///   For each element in a sequence, returns that element paired with
  ///   the remainder of the sequence after it.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="input">The sequence.</param>
  /// <returns>The elements and remainders.</returns>
  public static IEnumerable<(T, IEnumerable<T>)> Remainders<T>(this IEnumerable<T> input)
  {
    T[] array = input.ToArray();

    foreach (int index in array.Indices())
      yield return (array[index], array[(index + 1)..]);
  }

  /// <summary>
  ///   Returns all elements in one sequence or the other, but not both.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <returns>
  ///   Elements that are in exactly one of the sequences.
  /// </returns>
  public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second)
  {
    var set = first.ToHashSet();
    set.SymmetricExceptWith(second);
    return set;
  }

  /// <summary>
  ///   Returns all elements in one sequence or the other, but not both.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequences.
  /// </typeparam>
  /// <param name="first">The first sequence.</param>
  /// <param name="second">The second sequence.</param>
  /// <param name="comparer">Equality comparer to check items.</param>
  /// <returns>
  ///   Elements that are in exactly one of the sequences.
  /// </returns>
  public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> first, IEnumerable<T> second,
    IEqualityComparer<T> comparer)
  {
    var set = first.ToHashSet(comparer);
    set.SymmetricExceptWith(second);
    return set;
  }
}