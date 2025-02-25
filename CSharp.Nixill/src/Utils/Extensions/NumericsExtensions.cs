using System.Numerics;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Extensions on sequences of classes that implement numeric or
///   comparable interfaces.
/// </summary>
public static class NumericsExtensions
{
  /// <summary>
  ///   Returns the mean average of a given set of inputs.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects in the collection. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of its own type,
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IDivisionOperators{T, int, T}">divisible</see>
  ///       by <see cref="int"/>, producing itself.
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="inputs">The inputs to average.</param>
  /// <returns>The mean average of the inputs.</returns>
  /// <exception cref="DivideByZeroException">
  ///   The input is an empty sequence.
  /// </exception>
  public static T Average<T>(this IEnumerable<T> inputs)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IDivisionOperators<T, int, T>
  => inputs.Average<T, T>();

  /// <summary>
  ///   Returns the mean average of a given set of inputs.
  /// </summary>
  /// <typeparam name="TInput">
  ///   The type of objects in the collection. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of <c>TOutput</c>, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       <c>TOutput</c> to produce another <c>TOutput</c>.
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <typeparam name="TOutput">
  ///   The type of the final answer. This type must be
  ///   <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///   <see cref="int"/>, producing itself.
  /// </typeparam>
  /// <param name="inputs">The inputs to average.</param>
  /// <returns>The mean average of the inputs.</returns>
  /// <exception cref="DivideByZeroException">
  ///   The input is an empty sequence.
  /// </exception>
  public static TOutput Average<TInput, TOutput>(this IEnumerable<TInput> inputs)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
    where TOutput : IDivisionOperators<TOutput, int, TOutput>
  {
    TInput[] inputArray = inputs.ToArray();

    if (inputArray.Length == 0) throw new DivideByZeroException("Cannot get average of a zero-length input.");

    TOutput sum = inputArray.Sum<TInput, TOutput>();
    return sum / inputArray.Length;
  }

  /// <summary>
  ///   Returns the mean average of a given set of inputs, or a default
  ///   value if there are no inputs.
  /// </summary>
  /// <typeparam name="TInput">
  ///   The type of objects in the collection. This type must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of <c>TOutput</c>, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       <c>TOutput</c> to produce another <c>TOutput</c>.
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <typeparam name="TOutput">
  ///   The type of the final answer. This type must be
  ///   <see cref="IDivisionOperators{T, T, T}">divisible</see> by
  ///   <see cref="int"/>, producing itself.
  /// </typeparam>
  /// <param name="inputs">The inputs to average.</param>
  /// <param name="defaultValue">
  ///   The value returned if the sequence is empty.
  /// </param>
  /// <returns>
  ///   The mean average of the inputs, or <c>defaultValue</c>.
  /// </returns>
  public static TOutput Average<TInput, TOutput>(this IEnumerable<TInput> inputs, TOutput defaultValue)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
    where TOutput : IDivisionOperators<TOutput, int, TOutput>
  {
    TInput[] inputArray = inputs.ToArray();

    if (inputArray.Length == 0) return defaultValue;

    TOutput sum = inputArray.Sum<TInput, TOutput>();
    return sum / inputArray.Length;
  }

  /// <summary>
  ///   Returns all equally maximal values in the sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence, which must be naturally
  ///   comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <returns>The equally maximal values.</returns>
  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list) where T : IComparable<T>
    => ManyMax(list, x => x, x => x, Comparer<T>.Default);

  /// <summary>
  ///   Returns all equally maximal values in the sequence according to a
  ///   given comparer.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The equally maximal values.</returns>
  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list, IComparer<T> comparer)
    => ManyMax(list, x => x, x => x, comparer);

  /// <summary>
  ///   Invokes a transform function on each element of a sequence.
  ///   Returns all equally maximal resulting values.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements after transformation, which must be
  ///   naturally comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The transformation function.</param>
  /// <returns>The equally maximal transformed values.</returns>
  public static IEnumerable<TOut> MaxMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, mutator, y => y, Comparer<TOut>.Default);

  /// <summary>
  ///   Invokes a transform function on each element of a sequence.
  ///   Returns all equally maximal values according to a given comparer.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements after transformation.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The transformation function.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The equally maximal transformed values.</returns>
  public static IEnumerable<TOut> MaxMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, mutator, y => y, comparer);

  /// <summary>
  ///   Returns all values in a sequence which are equally maximal
  ///   according to a specified key selector function.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of keys being compared, which must be naturally comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The key selector function.</param>
  /// <returns>
  ///   The original values that have equally maximal keys.
  /// </returns>
  public static IEnumerable<TIn> MaxManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, x => x, mutator, Comparer<TOut>.Default);

  /// <summary>
  ///   Returns all values in a sequence which are equally maximal
  ///   according to a specified key selector function and a specified
  ///   comparer.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">The type of keys being compared.</typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The key selector function.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>
  ///   The original values that have equally maximal keys.
  /// </returns>
  public static IEnumerable<TIn> MaxManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, x => x, mutator, comparer);

  /// <summary>
  ///   Returns all equally minimal values in the sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence, which must be naturally
  ///   comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <returns>The equally minimal values.</returns>
  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list) where T : IComparable<T>
    => ManyMax(list, x => x, x => x, Comparer<T>.Default.Invert());

  /// <summary>
  ///   Returns all equally minimal values in the sequence according to a
  ///   given comparer.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The equally minimal values.</returns>
  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list, IComparer<T> comparer)
    => ManyMax(list, x => x, x => x, comparer.Invert());

  /// <summary>
  ///   Invokes a transform function on each element of a sequence.
  ///   Returns all equally minimal resulting values.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements after transformation, which must be
  ///   naturally comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The transformation function.</param>
  /// <returns>The equally minimal transformed values.</returns>
  public static IEnumerable<TOut> MinMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, mutator, y => y, Comparer<TOut>.Default.Invert());

  /// <summary>
  ///   Invokes a transform function on each element of a sequence.
  ///   Returns all equally minimal values according to a given comparer.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements after transformation.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The transformation function.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>The equally minimal transformed values.</returns>
  public static IEnumerable<TOut> MinMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, mutator, y => y, comparer.Invert());

  /// <summary>
  ///   Returns all values in a sequence which are equally minimal
  ///   according to a specified key selector function.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of keys being compared, which must be naturally comparable.
  /// </typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The key selector function.</param>
  /// <returns>
  ///   The original values that have equally minimal keys.
  /// </returns>
  public static IEnumerable<TIn> MinManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, x => x, mutator, Comparer<TOut>.Default.Invert());

  /// <summary>
  ///   Returns all values in a sequence which are equally minimal
  ///   according to a specified key selector function and a specified
  ///   comparer.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements in the original sequence.
  /// </typeparam>
  /// <typeparam name="TOut">The type of keys being compared.</typeparam>
  /// <param name="list">The sequence.</param>
  /// <param name="mutator">The key selector function.</param>
  /// <param name="comparer">The comparer.</param>
  /// <returns>
  ///   The original values that have equally minimal keys.
  /// </returns>
  public static IEnumerable<TIn> MinManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, x => x, mutator, comparer.Invert());

  static IEnumerable<TOut> ManyMax<TIn, TOut, TKey>(IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    Func<TOut, TKey> keySelector, IComparer<TKey> comparison)
  {
    TKey current = default(TKey)!;
    List<TOut> output = [];
    bool assigned = false;

    foreach (TIn item in list)
    {
      TOut mutated = mutator(item);
      TKey key = keySelector(mutated);

      int result = comparison.Compare(current, key);

      if (!assigned || result < 0)
      {
        current = key;
        output = [mutated];
        assigned = true;
      }
      else if (result == 0)
      {
        output.Add(mutated);
      }
    }

    return output;
  }

  /// <summary>
  ///   Returns the sum of a sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence, which must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of its own type, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       itself.
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <param name="inputs">The inputs to sum.</param>
  /// <returns>The sum of the inputs.</returns>
  public static T Sum<T>(this IEnumerable<T> inputs)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
  => inputs.Sum<T, T>();

  /// <summary>
  ///   Returns the sum of a sequence.
  /// </summary>
  /// <typeparam name="TInput">
  ///   The type of elements in the sequence, which must:
  ///   <list type="bullet">
  ///     <item>
  ///       have an <see cref="IAdditiveIdentity{T, T}">additive
  ///       identity</see> of <c>TOutput</c>, and
  ///     </item>
  ///     <item>
  ///       be <see cref="IAdditionOperators{T, T, T}">addable</see> to
  ///       <c>TOutput</c> to produce another <c>TOutput</c>.
  ///     </item>
  ///   </list>
  /// </typeparam>
  /// <typeparam name="TOutput"></typeparam>
  /// <param name="inputs"></param>
  /// <returns></returns>
  public static TOutput Sum<TInput, TOutput>(this IEnumerable<TInput> inputs)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
  {
    TOutput output = TInput.AdditiveIdentity;
    foreach (TInput input in inputs) output = input + output;
    return output;
  }
}

file static class ComparisonInverter
{
  public static IComparer<T> Invert<T>(this IComparer<T> original)
    => Comparer<T>.Create((l, r) => -original.Compare(l, r));
}