using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Utils.Trials;

public static class TrialExtensions
{
  public static IEnumerable<ITrialResult<TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  public static IEnumerable<ITrialResult<TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, int, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  public static SuccessesAndFailures<TOut> TrySelectSplit<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
  {
    ITrialResult<TOut>[] results = [.. elements.TrySelect(mutator)];
    return new SuccessesAndFailures<TOut>(
      results.Where(r => r.Success).Cast<SuccessfulTrialResult<TOut>>(),
      results.Where(r => !r.Success).Cast<FailedTrialResult<TOut>>()
    );
  }

  public static SuccessesAndFailures<TOut> TrySelectSplit<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, int, TOut> mutator)
  {
    ITrialResult<TOut>[] results = [.. elements.TrySelect(mutator)];
    return new SuccessesAndFailures<TOut>(
      results.Where(r => r.Success).Cast<SuccessfulTrialResult<TOut>>(),
      results.Where(r => !r.Success).Cast<FailedTrialResult<TOut>>()
    );
  }

  static ITrialResult<TOut> GetTrialResult<TIn, TOut>(TIn element, int index, Func<TIn, TOut> mutator)
  {
    try
    {
      return new SuccessfulTrialResult<TOut>(index, mutator(element));
    }
    catch (Exception ex)
    {
      return new FailedTrialResult<TOut>(index, ex);
    }
  }

  static ITrialResult<TOut> GetTrialResult<TIn, TOut>(TIn element, int index, Func<TIn, int, TOut> mutator)
  {
    try
    {
      return new SuccessfulTrialResult<TOut>(index, mutator(element, index));
    }
    catch (Exception ex)
    {
      return new FailedTrialResult<TOut>(index, ex);
    }
  }
}

public interface ITrialResult<T>
{
  public bool Success { get; }
  public int Index { get; }
  public T Result { get; }
  public Exception Exception { get; }

  public bool TryGet([MaybeNullWhen(false)] out T result)
  {
    if (Success)
    {
      result = Result;
      return true;
    }
    else
    {
      result = default;
      return false;
    }
  }

  public string? ToString();
}

public readonly struct SuccessfulTrialResult<T>(int index, T result) : ITrialResult<T>
{
  public bool Success => true;
  public T Result => result;
  Exception ITrialResult<T>.Exception => throw new TrialSucceededException();
  public int Index => index;

  public override string? ToString() => Result?.ToString();
}

public readonly struct FailedTrialResult<T>(int index, Exception exception) : ITrialResult<T>
{
  public bool Success => false;
  T ITrialResult<T>.Result => throw new TrialFailedException(exception);
  public Exception Exception => exception;
  public int Index => index;

  public override string? ToString() => Exception.GetType().Name;
}

public class TrialFailedException : Exception
{
  public TrialFailedException(Exception innerException) : base("The trial did not succeed.", innerException) { }
}

public class TrialSucceededException : Exception
{
  public TrialSucceededException() : base("The trial did not fail.") { }
}

public record struct SuccessesAndFailures<T>(IEnumerable<SuccessfulTrialResult<T>> Successes,
  IEnumerable<FailedTrialResult<T>> Failures)
  : IEnumerable<ITrialResult<T>>
{
  IEnumerable<ITrialResult<T>> Enumerate()
  {
    foreach (var t in Successes) yield return t;
    foreach (var t in Failures) yield return t;
  }

  public IEnumerator<ITrialResult<T>> GetEnumerator() => Enumerate().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
