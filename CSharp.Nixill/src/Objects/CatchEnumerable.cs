using System.Collections;

namespace Nixill.Objects;

/// <summary>
///   A wrapper around an <see cref="IEnumerable{T}"/> that absorbs
///   exceptions thrown by the enumerable during enumeration.
/// </summary>
/// <remarks>
///   This class is not thread safe; in particular, the <see cref="Success"/>,
///   <see cref="Threw"/>, and <see cref="Exception"/> properties assume
///   single-threaded access to this instance. However, having multiple
///   separate instances of CatchEnumerable on the same input sequence,
///   each accessed by only a single thread, should be as thread-safe as
///   the original sequence itself.
/// </remarks>
/// <typeparam name="T">
///   The type of objects in the enumerable sequence.
/// </typeparam>
public class CatchEnumerable<T> : IEnumerable<T>
{
  /// <summary>
  ///   Whether or not the enumeration of the sequence completed
  ///   successfully.
  /// </summary>
  public bool Success { get; private set; } = false;

  /// <summary>
  ///   Whether or not the enumeration of the sequence failed with an
  ///   exception.
  /// </summary>
  public bool Threw { get; private set; } = false;

  /// <summary>
  ///   The exception thrown during the enumeration of the sequence.
  /// </summary>
  public Exception? Exception { get; private set; } = null;

  readonly IEnumerable<T> Input;

  /// <summary>
  ///   Constructs a new CatchEnumerable wrapping a given input sequence.
  /// </summary>
  /// <param name="input">The input sequence.</param>
  public CatchEnumerable(IEnumerable<T> input)
  {
    Input = input;
  }

  /// <summary>
  ///   Gets the sequence of items from the source.
  /// </summary>
  /// <remarks>
  ///   At the start of enumeration, <see cref="Success"/> and
  ///   <see cref="Threw"/> are both set to <c>false</c>. If enumeration
  ///   throws an exception, <see cref="Threw"/> is set to <c>true</c> and
  ///   <see cref="Exception"/> is set to not null. This also ends the
  ///   sequence. Otherwise, at the end of enumeration, <see cref="Success"/>
  ///   is set to <c>true</c>.
  /// </remarks>
  /// <returns>The sequence of items.</returns>
  public IEnumerable<T> GetSequence()
  {
    Success = false;
    Threw = false;
    Exception = null;
    IEnumerator<T> enumerator = Input.GetEnumerator();
    while (TryGetNext(enumerator)) yield return enumerator.Current;
    if (!Threw) Success = true;
  }

  bool TryGetNext(IEnumerator<T> enumerator)
  {
    try
    {
      return enumerator.MoveNext();
    }
    catch (Exception e)
    {
      Threw = true;
      Exception = e;
      return false;
    }
  }

  public IEnumerator<T> GetEnumerator() => GetSequence().GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetSequence().GetEnumerator();
}

/// <summary>
///   A wrapper around an <see cref="IEnumerator{T}"/> that absorbs
///   exceptions thrown by the enumerator during enumeration.
/// </summary>
/// <remarks>
///   This class is approximately as thread-safe as the original
///   <see cref="IEnumerator{T}"/> used as its input.
/// </remarks>
/// <typeparam name="T">
///   The type of objects in the enumerable sequence.
/// </typeparam>
public class CatchEnumerator<T> : IEnumerator<T>
{
  IEnumerator<T> Wrapped;

  /// <summary>
  ///   Whether or not an exception was thrown during enumeration.
  /// </summary>
  public bool Threw { get; private set; } = false;

  /// <summary>
  ///   The exception that was thrown during enumeration.
  /// </summary>
  public Exception? Exception { get; private set; } = null;

  /// <summary>
  ///   Whether or not the enumeration of the sequence completed
  ///   successfully.
  /// </summary>
  public bool Success { get; private set; } = false;

  /// <summary>
  ///   Constructs a new CatchEnumerator wrapping the given enumerator.
  /// </summary>
  /// <param name="input">The enumerator to wrap.</param>
  public CatchEnumerator(IEnumerator<T> input)
  {
    Wrapped = input;
  }

  /// <inheritdoc/>
  public T Current => Wrapped.Current;

  /// <inheritdoc/>
  object IEnumerator.Current => ((IEnumerator)Wrapped).Current;

  /// <inheritdoc/>
  public void Dispose()
  {
    Wrapped.Dispose();
  }

  /// <summary>
  ///   Attempts to advance the enumerator to the next element of the
  ///   collection.
  /// </summary>
  /// <returns>
  ///   <c>true</c> if the enumerator was successfully advanced to the
  ///   next element; <c>false</c> if the enumerator has passed the end of
  ///   the collection or threw an exception.
  /// </returns>
  public bool MoveNext()
  {
    try
    {
      bool result = Wrapped.MoveNext();
      if (!result) Success = true;
      return result;
    }
    catch (Exception e)
    {
      Threw = true;
      Exception = e;
      return false;
    }
  }

  /// <inheritdoc/>
  public void Reset()
  {
    Wrapped.Reset();
    Threw = false;
    Success = false;
    Exception = null;
  }
}

public static class CatchEnumerableExtensions
{
  public static CatchEnumerable<T> TryCatch<T>(this IEnumerable<T> sequence)
    => new CatchEnumerable<T>(sequence);

  public static CatchEnumerator<T> GetCatchEnumerator<T>(this IEnumerable<T> sequence)
    => new CatchEnumerator<T>(sequence.GetEnumerator());
}