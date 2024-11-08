using System.Numerics;

namespace Nixill.Utils.Extensions;

public static class NumericsExtensions
{
  public static T Average<T>(this IEnumerable<T> inputs)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>, IDivisionOperators<T, int, T>
  => inputs.Average<T, T>();

  public static TOutput Average<TInput, TOutput>(this IEnumerable<TInput> inputs)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
    where TOutput : IDivisionOperators<TOutput, int, TOutput>
  {
    TInput[] inputArray = inputs.ToArray();

    if (inputArray.Length == 0) throw new DivideByZeroException("Cannot get average of a zero-length input.");

    TOutput sum = inputArray.Sum<TInput, TOutput>();
    return sum / inputArray.Length;
  }

  public static TOutput Average<TInput, TOutput>(this IEnumerable<TInput> inputs, TOutput defaultValue)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
    where TOutput : IDivisionOperators<TOutput, int, TOutput>
  {
    TInput[] inputArray = inputs.ToArray();

    if (inputArray.Length == 0) return defaultValue;

    TOutput sum = inputArray.Sum<TInput, TOutput>();
    return sum / inputArray.Length;
  }

  public static T Sum<T>(this IEnumerable<T> inputs)
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
  => inputs.Sum<T, T>();

  public static TOutput Sum<TInput, TOutput>(this IEnumerable<TInput> inputs)
    where TInput : IAdditionOperators<TInput, TOutput, TOutput>, IAdditiveIdentity<TInput, TOutput>
  {
    TOutput output = TInput.AdditiveIdentity;
    foreach (TInput input in inputs) output = input + output;
    return output;
  }


  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list) where T : IComparable<T>
    => ManyMax(list, x => x, x => x, Comparer<T>.Default);

  public static IEnumerable<T> MaxMany<T>(this IEnumerable<T> list, IComparer<T> comparer)
    => ManyMax(list, x => x, x => x, comparer);

  public static IEnumerable<TOut> MaxMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, mutator, y => y, Comparer<TOut>.Default);

  public static IEnumerable<TOut> MaxMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, mutator, y => y, comparer);

  public static IEnumerable<TIn> MaxManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, x => x, mutator, Comparer<TOut>.Default);

  public static IEnumerable<TIn> MaxManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, x => x, mutator, comparer);

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list) where T : IComparable<T>
    => ManyMax(list, x => x, x => x, Comparer<T>.Default.Invert());

  public static IEnumerable<T> MinMany<T>(this IEnumerable<T> list, IComparer<T> comparer)
    => ManyMax(list, x => x, x => x, comparer.Invert());

  public static IEnumerable<TOut> MinMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, mutator, y => y, Comparer<TOut>.Default.Invert());

  public static IEnumerable<TOut> MinMany<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, mutator, y => y, comparer.Invert());

  public static IEnumerable<TIn> MinManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator)
    where TOut : IComparable<TOut>
    => ManyMax(list, x => x, mutator, Comparer<TOut>.Default.Invert());

  public static IEnumerable<TIn> MinManyBy<TIn, TOut>(this IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    IComparer<TOut> comparer)
    => ManyMax(list, x => x, mutator, comparer.Invert());

  static IEnumerable<TOut> ManyMax<TIn, TOut, TKey>(IEnumerable<TIn> list, Func<TIn, TOut> mutator,
    Func<TOut, TKey> keySelector, IComparer<TKey> comparison)
  {
    TKey current = default(TKey);
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
}

file static class ComparisonInverter
{
  public static IComparer<T> Invert<T>(this IComparer<T> original)
    => Comparer<T>.Create((l, r) => -original.Compare(l, r));
}