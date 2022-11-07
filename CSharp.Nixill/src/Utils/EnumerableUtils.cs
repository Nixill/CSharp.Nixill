using System;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;

namespace Nixill.Utils;

public static class EnumerableUtils {
  public static IEnumerable<TResult> Product<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> func) =>
    left.Join(right, x => 1, y => 1, func);

  public static IEnumerable<(TLeft, TRight)> Product<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
    left.Join(right, x => 1, y => 1, (left, right) => (left, right));

  public static IEnumerable<T> Repeat<T>(Func<T> func, int count) =>
    Enumerable.Repeat(0, count).Select(x => func());

  public static IEnumerable<T> RepeatInfinite<T>(Func<T> func) {
    while (true) {
      yield return func();
    }
  }

  public static IEnumerable<T> RepeatInfinite<T>(T item) {
    while (true) {
      yield return item;
    }
  }

  public static void Do<T>(this IEnumerable<T> list, Action<T> act) {
    foreach (T item in list) {
      act(item);
    }
  }

  public static void Do<T>(this IEnumerable<T> list, Action<T, int> act) {
    int count = 0;
    foreach (T item in list) {
      act(item, count++);
    }
  }

  public static IEnumerable<T> Of<T>(params T[] items) {
    foreach (T item in items) yield return item;
  }

  public static IEnumerable<T> Of<T>(T item) {
    yield return item;
  }

  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list) where T : IComparable<T> {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;

    foreach (T item in list) {
      if (!assigned || baseline.CompareTo(item) < 0) {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(item) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list, IComparer<T> comp) {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;
    Comparison<T> cFunc = comp.Compare;

    foreach (T item in list) {
      if (!assigned || cFunc(baseline, item) < 0) {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, item) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MaxManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator) where TResult : IComparable<TResult> {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;

    foreach (TSource item in list) {
      TResult mutation = mutator(item);
      if (!assigned || baseline.CompareTo(mutation) < 0) {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(mutation) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MaxManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator, IComparer<TResult> comp) {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;
    Comparison<TResult> cFunc = comp.Compare;

    foreach (TSource item in list) {
      TResult mutation = mutator(item);
      if (!assigned || cFunc(baseline, mutation) < 0) {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, mutation) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list) where T : IComparable<T> {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;

    foreach (T item in list) {
      if (!assigned || baseline.CompareTo(item) > 0) {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(item) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list, IComparer<T> comp) {
    T baseline = default(T);
    List<T> allItems = null;
    bool assigned = false;
    Comparison<T> cFunc = comp.Compare;

    foreach (T item in list) {
      if (!assigned || cFunc(baseline, item) > 0) {
        baseline = item;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, item) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MinManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator) where TResult : IComparable<TResult> {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;

    foreach (TSource item in list) {
      TResult mutation = mutator(item);
      if (!assigned || baseline.CompareTo(mutation) > 0) {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (baseline.CompareTo(mutation) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

  public static IEnumerable<TSource> MinManyBy<TSource, TResult>(this IEnumerable<TSource> list,
      Func<TSource, TResult> mutator, IComparer<TResult> comp) {
    TResult baseline = default(TResult);
    List<TSource> allItems = null;
    bool assigned = false;
    Comparison<TResult> cFunc = comp.Compare;

    foreach (TSource item in list) {
      TResult mutation = mutator(item);
      if (!assigned || cFunc(baseline, mutation) > 0) {
        baseline = mutation;
        allItems = new() { item };
        assigned = true;
      }
      else if (cFunc(baseline, mutation) == 0) {
        allItems.Add(item);
      }
    }

    return allItems;
  }

}