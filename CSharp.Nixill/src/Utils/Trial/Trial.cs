using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Utils.Trials;

/// <summary>
///   Provides extensions that run trials on input data.
/// </summary>
public static class TrialExtensions
{
  /// <summary>
  ///   Attempts to mutate each element of a sequence according to a
  ///   mutator function, returning the results of each trial.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the input sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements in the output sequence.
  /// </typeparam>
  /// <param name="elements">The original sequence.</param>
  /// <param name="mutator">The mutator function.</param>
  /// <returns>The results of attempting to mutate each element.</returns>
  public static IEnumerable<ITrialResult<TIn, TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  /// <summary>
  ///   Attempts to mutate each element of a sequence according to a
  ///   mutator function, returning the results of each trial.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the input sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements in the output sequence.
  /// </typeparam>
  /// <param name="elements">The original sequence.</param>
  /// <param name="mutator">
  ///   The mutator function, which also receives the index of each element.
  /// </param>
  /// <returns>The results of attempting to mutate each element.</returns>
  public static IEnumerable<ITrialResult<TIn, TOut>> TrySelect<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, int, TOut> mutator)
    => elements.Select((itm, ind) => GetTrialResult(itm, ind, mutator));

  /// <summary>
  ///   Attempts to mutate each element of a sequence according to a
  ///   mutator function, returning the results of each trial, split
  ///   between successes and failures.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the input sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements in the output sequence.
  /// </typeparam>
  /// <param name="elements">The original sequence.</param>
  /// <param name="mutator">The mutator function.</param>
  /// <returns>
  ///   The results of attempting to mutate each element, split between
  ///   successes and failures.
  /// </returns>
  public static SuccessesAndFailures<TIn, TOut> TrySelectSplit<TIn, TOut>(this IEnumerable<TIn> elements,
    Func<TIn, TOut> mutator)
  {
    ITrialResult<TIn, TOut>[] results = [.. elements.TrySelect(mutator)];
    return new SuccessesAndFailures<TIn, TOut>(
      results.Where(r => r.Success).Cast<SuccessfulTrialResult<TIn, TOut>>(),
      results.Where(r => !r.Success).Cast<FailedTrialResult<TIn, TOut>>()
    );
  }

  /// <summary>
  ///   Attempts to mutate each element of a sequence according to a
  ///   mutator function, returning the results of each trial, split
  ///   between successes and failures.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the input sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements in the output sequence.
  /// </typeparam>
  /// <param name="elements">The original sequence.</param>
  /// <param name="mutator">
  ///   The mutator function, which also receives the index of each element.
  /// </param>
  /// <returns>
  ///   The results of attempting to mutate each element, split between
  ///   successes and failures.
  /// </returns>
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

/// <summary>
///   Represents the result of a trial.
/// </summary>
/// <typeparam name="TIn">The type of the original element.</typeparam>
/// <typeparam name="TOut">
///   The type of the result of the function.
/// </typeparam>
public interface ITrialResult<TIn, TOut>
{
  /// <summary>
  ///   Get: Whether or not the trial succeeded (did not throw an exception).
  /// </summary>
  public bool Success { get; }

  /// <summary>
  ///   Get: The index of this trial in the original sequence.
  /// </summary>
  public int Index { get; }

  /// <summary>
  ///   Get: The original element from the original sequence.
  /// </summary>
  public TIn Original { get; }

  /// <summary>
  ///   If the trial was successful (an exception was not thrown), gets
  ///   the result of the mutator function.
  /// </summary>
  /// <exception cref="TrialFailedException">
  ///   The trial did not succeed (it threw an exception).
  /// </exception>
  public TOut Result { get; }

  /// <summary>
  ///   If the trial failed (threw an exception), gets the exception that
  ///   was thrown.
  /// </summary>
  /// <exception cref="TrialSucceededException">
  ///   The trial succeeded (an exception was not thrown).
  /// </exception>
  public Exception Exception { get; }

  /// <summary>
  ///   Returns whether or not the trial succeeded, and if so, the result.
  /// </summary>
  /// <param name="result">
  ///   When this method returns <see langword="true"/>, this parameter
  ///   contains the result of the trial. When <see langword="false"/> is
  ///   returned, this parameter contains the default of <typeparamref name="TOut"/>.
  /// </param>
  /// <returns>
  ///   Whether or not the trial succeeded.
  /// </returns>
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

/// <summary>
///   Represents the result of a successful trial.
/// </summary>
/// <typeparam name="TIn">The type of the original element.</typeparam>
/// <typeparam name="TOut">
///   The type of the result of the function.
/// </typeparam>
/// <param name="index">The index of this trial.</param>
/// <param name="original">The original element before the trial.</param>
/// <param name="result">The result of the trial.</param>
public readonly struct SuccessfulTrialResult<TIn, TOut>(int index, TIn original, TOut result) : ITrialResult<TIn, TOut>
{
  /// <summary>
  ///   Get: Whether or not the trial was successful (always <see langword="true"/>).
  /// </summary>
  public bool Success => true;

  /// <inheritdoc/>
  public TIn Original => original;

  /// <summary>
  ///   Get: The result of the trial.
  /// </summary>
  public TOut Result => result;

  /// <summary>
  ///   Get: Throws a <see cref="TrialSucceededException"/>.
  /// </summary>
  Exception ITrialResult<TIn, TOut>.Exception => throw new TrialSucceededException();

  /// <inheritdoc/>
  public int Index => index;

  /// <summary>
  ///   Returns a string representation of this successful trial.
  /// </summary>
  /// <returns>As described above.</returns>
  public override string? ToString() => $"{Index}: {Original} => {Result}";
}

/// <summary>
///   Represents the result of a failed trial.
/// </summary>
/// <typeparam name="TIn">The type of the original element.</typeparam>
/// <typeparam name="TOut">
///   The type of the result of the function.
/// </typeparam>
/// <param name="index">The index of this trial.</param>
/// <param name="original">The original element before the trial.</param>
/// <param name="exception">The exception thrown by the mutator.</param>
public readonly struct FailedTrialResult<TIn, TOut>(int index, TIn original, Exception exception) : ITrialResult<TIn, TOut>
{
  /// <summary>
  ///   Get: Whether or not the trial succeeded (always <see langword="false">).
  /// </summary>
  public bool Success => false;

  /// <inheritdoc/>
  public TIn Original => original;

  /// <summary>
  ///   Get: Throws a <see cref="TrialFailedException"/>.
  /// </summary>
  TOut ITrialResult<TIn, TOut>.Result => throw new TrialFailedException(exception);

  /// <summary>
  ///   Get: The exception thrown during the trial.
  /// </summary>
  public Exception Exception => exception;

  /// <inheritdoc/>
  public int Index => index;

  /// <summary>
  ///   Returns a string representation of this failed trial.
  /// </summary>
  /// <returns>As described above.</returns>
  public override string? ToString() => $"{Index}: {Original} => {Exception.GetType().Name}";
}

/// <summary>
///   An exception thrown when attempting to retrieve the <see cref="ITrialResult{TIn, TOut}.Result">Result</see>
///   of a failed trial.
/// </summary>
public class TrialFailedException : Exception
{
  /// <summary>
  ///   Constructs a new TrialFailedException.
  /// </summary>
  /// <param name="innerException">
  ///   The exception thrown during the trial.
  /// </param>
  public TrialFailedException(Exception innerException) : base("The trial did not succeed.", innerException) { }
}

/// <summary>
///   An exception thrown when attempting to retrieve the <see cref="ITrialResult{TIn, TOut}.Exception">Exception</see>
///   of a successful trial.
/// </summary>
public class TrialSucceededException : Exception
{
  /// <summary>
  ///   Constructs a new TrialSucceededException.
  /// </summary>
  public TrialSucceededException() : base("The trial did not fail.") { }
}

/// <summary>
///   A representation of all the successful and failed trials in a
///   sequence.
/// </summary>
/// <typeparam name="TIn">
///   The type of elements in the input sequence.
/// </typeparam>
/// <typeparam name="TOut">
///   The type of elements in the output sequence.
/// </typeparam>
/// <param name="Successes">
///   The sequence of successful trials.
/// </param>
/// <param name="Failures">
///   The sequence of failed trials.
/// </param>
public record struct SuccessesAndFailures<TIn, TOut>(IEnumerable<SuccessfulTrialResult<TIn, TOut>> Successes,
  IEnumerable<FailedTrialResult<TIn, TOut>> Failures)
  : IEnumerable<ITrialResult<TIn, TOut>>
{
  IEnumerable<ITrialResult<TIn, TOut>> Enumerate()
  {
    foreach (var t in Successes) yield return t;
    foreach (var t in Failures) yield return t;
  }

  /// <inheritdoc/>
  public IEnumerator<ITrialResult<TIn, TOut>> GetEnumerator() => Enumerate().GetEnumerator();

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
