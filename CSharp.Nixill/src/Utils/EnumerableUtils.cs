using System;
using System.Collections.Generic;
using Nixill.Collections;
using System.Linq;

namespace Nixill.Utils;

public static class EnumerableUtils {
  public static IEnumerable<TResult> Product<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TRight, TResult> func) =>
    left.SelectMany(l => right.Select(r => func(l, r)));

  public static IEnumerable<(TLeft, TRight)> Product<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
    left.SelectMany(l => right.Select(r => (l, r)));

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

  public static IEnumerable<T> Of<T>(T item) {
    yield return item;
  }
}