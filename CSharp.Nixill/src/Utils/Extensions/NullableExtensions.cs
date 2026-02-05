namespace Nixill.Utils.Extensions;

public static class NullableExtensions
{
  public static T? ElementAtOrNull<T>(this IEnumerable<T> sequence, int index) where T : struct
  {
    try
    {
      return sequence.ElementAt(index);
    }
    catch (ArgumentOutOfRangeException)
    {
      return null;
    }
  }

  public static T? ElementAtOrNull<T>(this IEnumerable<T> sequence, Index index) where T : struct
  {
    try
    {
      return sequence.ElementAt(index);
    }
    catch (ArgumentOutOfRangeException)
    {
      return null;
    }
  }

  public static T? FirstOrNull<T>(this IEnumerable<T> sequence) where T : struct
  {
    try
    {
      return sequence.First();
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  public static T? FirstOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> condition) where T : struct
  {
    try
    {
      return sequence.First(condition);
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  public static T? LastOrNull<T>(this IEnumerable<T> sequence) where T : struct
  {
    try
    {
      return sequence.Last();
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  public static T? LastOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> condition) where T : struct
  {
    try
    {
      return sequence.Last(condition);
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  public static T? SingleOrNull<T>(this IEnumerable<T> sequence) where T : struct
  {
    try
    {
      return sequence.Single();
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  public static T? SingleOrNull<T>(this IEnumerable<T> sequence, Func<T, bool> condition) where T : struct
  {
    try
    {
      return sequence.Single(condition);
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }
}