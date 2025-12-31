using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Utils.Trials;

public static class TrialExtensions
{
  public static IEnumerable<ITrialResult<TIn, TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  public static IEnumerable<ITrialResult<TIn, TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, int, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  public static SuccessesAndFailures<TIn, TOut> TrySelectSplit<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
  {
    ITrialResult<TIn, TOut>[] results = [.. elements.TrySelect(mutator)];
    return new SuccessesAndFailures<TIn, TOut>(
      results.Where(r => r.Success).Cast<SuccessfulTrialResult<TIn, TOut>>(),
      results.Where(r => !r.Success).Cast<FailedTrialResult<TIn, TOut>>()
    );
  }

  public static SuccessesAndFailures<TIn, TOut> TrySelectSplit<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, int, TOut> mutator)
  {
    ITrialResult<TIn, TOut>[] results = [.. elements.TrySelect(mutator)];
    return new SuccessesAndFailures<TIn, TOut>(
      results.Where(r => r.Success).Cast<SuccessfulTrialResult<TIn, TOut>>(),
      results.Where(r => !r.Success).Cast<FailedTrialResult<TIn, TOut>>()
    );
  }

  static ITrialResult<TIn, TOut> GetTrialResult<TIn, TOut>(TIn element, int index, Func<TIn, TOut> mutator)
  {
    try
    {
      return new SuccessfulTrialResult<TIn, TOut>(index, element, mutator(element));
    }
    catch (Exception ex)
    {
      return new FailedTrialResult<TIn, TOut>(index, element, ex);
    }
  }

  static ITrialResult<TIn, TOut> GetTrialResult<TIn, TOut>(TIn element, int index, Func<TIn, int, TOut> mutator)
  {
    try
    {
      return new SuccessfulTrialResult<TIn, TOut>(index, element, mutator(element, index));
    }
    catch (Exception ex)
    {
      return new FailedTrialResult<TIn, TOut>(index, element, ex);
    }
  }
}

public interface ITrialResult<TIn, TOut>
{
  public bool Success { get; }
  public int Index { get; }
  public TIn Original { get; }
  public TOut Result { get; }
  public Exception Exception { get; }

  public bool TryGet([MaybeNullWhen(false)] out TOut result)
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

public readonly struct SuccessfulTrialResult<TIn, TOut>(int index, TIn original, TOut result) : ITrialResult<TIn, TOut>
{
  public bool Success => true;
  public TIn Original => original;
  public TOut Result => result;
  Exception ITrialResult<TIn, TOut>.Exception => throw new TrialSucceededException();
  public int Index => index;

  public override string? ToString() => Result?.ToString();
}

public readonly struct FailedTrialResult<TIn, TOut>(int index, TIn original, Exception exception) : ITrialResult<TIn, TOut>
{
  public bool Success => false;
  public TIn Original => original;
  TOut ITrialResult<TIn, TOut>.Result => throw new TrialFailedException(exception);
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

public record struct SuccessesAndFailures<TIn, TOut>(IEnumerable<SuccessfulTrialResult<TIn, TOut>> Successes,
  IEnumerable<FailedTrialResult<TIn, TOut>> Failures)
  : IEnumerable<ITrialResult<TIn, TOut>>
{
  IEnumerable<ITrialResult<TIn, TOut>> Enumerate()
  {
    foreach (var t in Successes) yield return t;
    foreach (var t in Failures) yield return t;
  }

  public IEnumerator<ITrialResult<TIn, TOut>> GetEnumerator() => Enumerate().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
