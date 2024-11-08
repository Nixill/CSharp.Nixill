namespace Nixill.Utils;

public static class Sequence
{
  public static IEnumerable<T> For<T>(T seed, Predicate<T> predicate, Func<T, T> step)
  {
    for (T value = seed; predicate(value); value = step(value)) yield return value;
  }

  public static IEnumerable<T> ForInfinite<T>(T seed, Func<T, T> step)
  {
    for (T value = seed; true; value = step(value)) yield return value;
  }

  public static T ForUntil<T>(T seed, Predicate<T> endCondition, Func<T, T> step)
  {
    T value = seed;
    for (; !endCondition(value); value = step(value)) { }
    return value;
  }

  public static IEnumerable<T> Of<T>(params T[] items)
  {
    foreach (T item in items) yield return item;
  }

  public static IEnumerable<T> Of<T>(T item)
  {
    yield return item;
  }

  public static IEnumerable<T> Of<T>(params Func<T>[] funcs)
  {
    foreach (Func<T> func in funcs) yield return func();
  }

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

  public static int FirstNonZero(params int[] ints)
  {
    foreach (int i in ints)
    {
      if (i != 0)
      {
        return i;
      }
    }
    return 0;
  }
}
