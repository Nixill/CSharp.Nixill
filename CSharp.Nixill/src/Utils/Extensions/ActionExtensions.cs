namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods that perform actions.
/// </summary>
public static class ActionExtensions
{
  /// <summary>
  ///   Performs an action for every item in a sequence.
  /// </summary>
  /// <typeparam name="T">The type of items in the sequence.</typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="act">The action to perform.</param>
  public static void Do<T>(this IEnumerable<T> list, Action<T> act)
  {
    foreach (T item in list)
    {
      act(item);
    }
  }

  /// <summary>
  ///   Performs an action for every item (with its index) in a sequence.
  /// </summary>
  /// <typeparam name="T">The type of items in the sequence.</typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="act">The action to perform.</param>
  public static void Do<T>(this IEnumerable<T> list, Action<T, int> act)
  {
    int count = 0;
    foreach (T item in list)
    {
      act(item, count++);
    }
  }

  /// <summary>
  ///   Performs a series of tasks concurrently, without waiting for any
  ///   return values.
  /// </summary>
  /// <param name="tasks">The tasks to perform.</param>
  public static void NoWait(this IEnumerable<Task> tasks)
  {
    tasks.Do(x => { });
  }

  /// <summary>
  ///   Performs a series of tasks concurrently. Produces no return values
  ///   besides an awaitable Task.
  /// </summary>
  /// <param name="tasks">The tasks to perform.</param>
  /// <returns>
  ///   A single task tracking the completion of all input tasks.
  /// </returns>
  public static async Task WaitAllNoReturn(this IEnumerable<Task> tasks)
  {
    Task[] array = [.. tasks];
    foreach (Task t in array) await t;
  }

  /// <summary>
  ///   Performs a series of tasks concurrently, then returns all their
  ///   values.
  /// </summary>
  /// <remarks>
  ///   This method doesn't seem to work right at the moment.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of values returned by the tasks.
  /// </typeparam>
  /// <param name="tasks">The tasks to perform.</param>
  /// <returns></returns>
  public static async IAsyncEnumerable<T> WaitAllReturns<T>(this IEnumerable<Task<T>> tasks)
  {
    Task<T>[] array = [.. tasks];
    foreach (Task<T> t in array)
    {
      yield return await t;
    }
  }
}