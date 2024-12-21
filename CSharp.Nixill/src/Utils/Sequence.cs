namespace Nixill.Utils;

/// <summary>
///   Utility class for forming sequences.
/// </summary>
public static class Sequence
{
  /// <summary>
  ///   Returns the first non-zero argument in the sequence.
  /// </summary>
  /// <param name="ints">The items.</param>
  /// <returns>
  ///   The first item that is non-zero, or <c>0</c> if all items are <c>0</c>.
  /// </returns>
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

  /// <summary>
  ///   Creates a sequence from a generic for loop.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="seed">The initial element.</param>
  /// <param name="predicate">
  ///   The condition that must pass to keep generating values.
  /// </param>
  /// <param name="step">
  ///   The function to increment values.
  /// </param>
  /// <returns>A sequence over all the values.</returns>
  public static IEnumerable<T> For<T>(T seed, Predicate<T> predicate, Func<T, T> step)
  {
    for (T value = seed; predicate(value); value = step(value)) yield return value;
  }

  /// <summary>
  ///   Creates a sequence from a generic, conditionless for loop.
  /// </summary>
  /// <remarks>
  ///   This method produces a sequence of infinite length. Methods that
  ///   expect to wholly enumerate a sequence before returning anything
  ///   will hang when this sequence is used as input.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="seed">The initial element.</param>
  /// <param name="step">
  ///   The function to increment values.
  /// </param>
  /// <returns>A sequence over all the values.</returns>
  public static IEnumerable<T> ForInfinite<T>(T seed, Func<T, T> step)
  {
    for (T value = seed; true; value = step(value)) yield return value;
  }

  /// <summary>
  ///   Returns the value that breaks a for loop.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="seed">The initial element.</param>
  /// <param name="endCondition">
  ///   The condition that <em>breaks</em> the loop.
  /// </param>
  /// <param name="step">
  ///   The function to increment values.
  /// </param>
  /// <returns>The first element that passes the end condition.</returns>
  public static T ForUntil<T>(T seed, Predicate<T> endCondition, Func<T, T> step)
  {
    T value = seed;
    for (; !endCondition(value); value = step(value)) { }
    return value;
  }

  /// <summary>
  ///   Returns a sequence of the given items.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="items">The elements of the sequence.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<T> Of<T>(params T[] items)
  {
    foreach (T item in items) yield return item;
  }

  /// <summary>
  ///   Returns a sequence consisting of a single item.
  /// </summary>
  /// <typeparam name="T">The type of element in the sequence.</typeparam>
  /// <param name="item">The item.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<T> Of<T>(T item)
  {
    yield return item;
  }

  /// <summary>
  ///   Returns a sequence of function return values.
  /// </summary>
  /// <remarks>
  ///   The functions are evaluated one at a time, only when the next item
  ///   in the sequence is required.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="funcs">The functions.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<T> Of<T>(params Func<T>[] funcs)
  {
    foreach (Func<T> func in funcs) yield return func();
  }

  /// <summary>
  ///   Calls a function to produce values a given number of times.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="func">The function.</param>
  /// <param name="count">The number of times to repeat it.</param>
  /// <returns>The sequence of return values.</returns>
  public static IEnumerable<T> Repeat<T>(Func<T> func, int count) =>
    Enumerable.Repeat(0, count).Select(x => func());

  /// <summary>
  ///   Infinitely calls a function to produce values.
  /// </summary>
  /// <remarks>
  ///   This method produces a sequence of infinite length. Methods that
  ///   expect to wholly enumerate a sequence before returning anything
  ///   will hang when this sequence is used as input.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="func">The function.</param>
  /// <returns>The infinitely long sequence of return values.</returns>
  public static IEnumerable<T> RepeatInfinite<T>(Func<T> func)
  {
    while (true)
    {
      yield return func();
    }
  }

  /// <summary>
  ///   Infinitely yields a single item.
  /// </summary>
  /// <remarks>
  ///   This method produces a sequence of infinite length. Methods that
  ///   expect to wholly enumerate a sequence before returning anything
  ///   will hang when this sequence is used as input.
  /// </remarks>
  /// <typeparam name="T">The type of element in the sequence.</typeparam>
  /// <param name="item">The item.</param>
  /// <returns>The infinitely long sequence of this item.</returns>
  public static IEnumerable<T> RepeatInfinite<T>(T item)
  {
    while (true)
    {
      yield return item;
    }
  }
}
