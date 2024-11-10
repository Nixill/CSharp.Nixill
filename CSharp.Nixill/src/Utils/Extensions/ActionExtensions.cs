namespace Nixill.Utils.Extensions;

public static class ActionExtensions
{
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

  public static void NoWait(this IEnumerable<Task> tasks)
  {
    tasks.Do(x => { });
  }

  public static async Task WaitAllNoReturn(this IEnumerable<Task> tasks)
  {
    Task[] array = [.. tasks];
    foreach (Task t in array) await t;
  }

  public static async IAsyncEnumerable<T> WaitAllReturns<T>(this IEnumerable<Task<T>> tasks)
  {
    Task<T>[] array = [.. tasks];
    foreach (Task<T> t in array)
    {
      yield return await t;
    }
  }
}