using Nixill.Collections;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Provides some shortcut methods for two-dimensional <see cref="IEnumerable{T}"/>s.
/// </summary>
public static class TwoDimensionalExtensions
{
  /// <summary>
  ///   Determines whether all elements of all sub-sequences satisfy a
  ///   condition.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences to which the condition is applied.
  /// </param>
  /// <param name="condition">The condition.</param>
  /// <returns>
  ///   <see langword="true"/> if <paramref name="input"/> is empty; or
  ///   if, for every sub-sequence in <paramref name="input"/>, either
  ///   that sub-sequence is empty or all of its elements fulfill the
  ///   condition. Otherwise, <see langword="false"/>.
  /// </returns>
  public static bool All2D<T>(this IEnumerable<IEnumerable<T>> input, Func<T, bool> condition)
    => input.All(i => i.All(condition));

  /// <summary>
  ///   Determines whether a sequence contains any sub-sequences that
  ///   contain any elements.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences which should be checked for
  ///   emptiness.
  /// </param>
  /// <returns>
  ///   <see langword="true"/> if <paramref name="input"/> contains at
  ///   least one sub-sequence that contains at least one element; <see langword="false"/>
  ///   otherwise.
  /// </returns>
  public static bool Any2D<T>(this IEnumerable<IEnumerable<T>> input) => input.Any(i => i.Any());

  /// <summary>
  ///   Determines whether a sequence contains any sub-sequences that
  ///   contain any elements that satisfy a condition.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences to which the condition is applied.
  /// </param>
  /// <param name="condition">The condition.</param>
  /// <returns>
  ///   <see langword="true"/> if <paramref name="input"/> contains at
  ///   least one sub-sequence that contains at least one element that
  ///   matches <paramref name="condition"/>; <see langword="false"/> if
  ///   <paramref name="input"/> is empty, all of its sub-sequences are
  ///   empty, or none of the elements of those sub-sequences satisfy the
  ///   condition.
  /// </returns>
  public static bool Any2D<T>(this IEnumerable<IEnumerable<T>> input, Func<T, bool> condition)
    => input.Any(i => i.Any(condition));

  /// <summary>
  ///   Determines whether a sequence contains a sub-sequence that
  ///   contains a specified element.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences in which to locate a value.
  /// </param>
  /// <param name="item">
  ///   The value to locate in the sequence.
  /// </param>
  /// 
  public static bool Contains2D<T>(this IEnumerable<IEnumerable<T>> input, T item) => input.Any(i => i.Contains(item));

  /// <summary>
  ///   Returns the number of elements in all sub-sequences of a sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences for which elements should be counted.
  /// </param>
  /// <returns>
  ///   The number of elements in all sub-sequences of the input sequence.
  /// </returns>
  public static int Count2D<T>(this IEnumerable<IEnumerable<T>> input) => input.Sum(i => i.Count());

  /// <summary>
  ///   Returns a number that represents how many elements in all
  ///   sub-sequences of a sequence satisfy a condition.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences which should be tested and
  ///   counted.
  /// </param>
  /// <param name="condition">The condition.</param>
  /// <returns>
  ///   The number of elements in the sub-sequences of the input sequence
  ///   that satisfy the given condition.
  /// </returns>
  public static int Count2D<T>(this IEnumerable<IEnumerable<T>> input, Func<T, bool> condition)
    => input.Sum(i => i.Count(condition));

  /// <summary>
  ///   Projects each element of each sub-sequence of a sequence into a
  ///   new form.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements returned by <paramref name="selector"/>.
  /// </typeparam>
  /// <param name="input">
  ///   A sequence of sequences of values upon which to invoke a transform
  ///   function.
  /// </param>
  /// <param name="selector">
  ///   A transform function to apply to each element.
  /// </param>
  /// <returns>
  ///   The sequence of sequences with projected values.
  /// </returns>
  public static IEnumerable<IEnumerable<TOut>> Select2D<TIn, TOut>(this IEnumerable<IEnumerable<TIn>> input,
    Func<TIn, TOut> selector)
    => input.Select(i => i.Select(selector));

  /// <summary>
  ///   Projects each element of each sub-sequence of a sequence into a
  ///   new form, incorporating that element's indices.
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of elements returned by <paramref name="selector"/>.
  /// </typeparam>
  /// <param name="input">
  ///   A sequence of sequences of values upon which to invoke a transform
  ///   function.
  /// </param>
  /// <param name="selector">
  ///   A transform function to apply to each element. Its parameters are:
  ///   <list type="number">
  ///     <item>The element of the original sequence.</item>
  ///     <item>The index of that element within the outer sequence.</item>
  ///     <item>The index of that element within the inner sequence.</item>
  ///   </list>
  /// </param>
  /// <returns>
  ///   The sequence of sequences with projected values.
  /// </returns>
  public static IEnumerable<IEnumerable<TOut>> Select2D<TIn, TOut>(this IEnumerable<IEnumerable<TIn>> input,
    Func<TIn, int, int, TOut> selector)
    => input.Select((itm1, ind1) => itm1.Select((itm2, ind2) => selector(itm2, ind1, ind2)));

  /// <summary>
  ///   Returns an enumerable that incorporates each element's indices
  ///   into a tuple.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences which should be indexed.
  /// </param>
  /// <returns>
  ///   A sequence of sequences of tuples that contain, in order:
  ///   <list type="number">
  ///     <item>
  ///       The index of this element within the outer enumerable.
  ///     </item>
  ///     <item>
  ///       The index of this element within the inner enumerable.
  ///     </item>
  ///     <item>The element itself.</item>
  ///   </list>
  /// </returns>
  public static IEnumerable<IEnumerable<(int FirstIndex, int SecondIndex, T Item)>> Index2D<T>(this IEnumerable<IEnumerable<T>> input)
    => input.Select((itm1, ind1) => itm1.Select((itm2, ind2) => (ind1, ind2, itm2)));

  /// <summary>
  ///   Returns a flattened enumerable that incorporates each element's
  ///   original indices into a tuple.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences which should be flattened and
  ///   indexed.
  /// </param>
  /// <returns>
  ///   A single sequence that contains all the sub-sequences' elements,
  ///   with the original indices of these elements incorporated into
  ///   tuples as follows:
  ///   <list type="number">
  ///     <item>
  ///       The index of this element within the outer enumerable.
  ///     </item>
  ///     <item>
  ///       The index of this element within the inner enumerable.
  ///     </item>
  ///     <item>The element itself.</item>
  ///   </list>
  /// </returns>
  public static IEnumerable<(int FirstIndex, int SecondIndex, T Item)> IndexAndFlatten2D<T>(this IEnumerable<IEnumerable<T>> input)
    => input.Index2D().SelectMany(i => i);

  /// <summary>
  ///   Returns a flattened enumerable containing all of the elements of
  ///   sub-sequences of the original sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences which should be flattened.
  /// </param>
  /// <returns>
  ///   A single sequence that contains all the sub-sequences' elements.
  /// </returns>
  public static IEnumerable<T> Flatten2D<T>(this IEnumerable<IEnumerable<T>> input)
    => input.SelectMany(i => i);

  /// <summary>
  ///   Returns the number of elements in all sub-sequences of a sequence.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences for which elements should be counted.
  /// </param>
  /// <returns>
  ///   The number of elements in all sub-sequences of the input sequence.
  /// </returns>
  public static long LongCount2D<T>(this IEnumerable<IEnumerable<T>> input) => input.Sum(i => i.LongCount());

  /// <summary>
  ///   Creates an array of arrays from a sequence of sequences.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences from which to create an array of
  ///   arrays.
  /// </param>
  /// <returns>
  ///   An array that contains arrays that contain the elements from the
  ///   input sequences.
  /// </returns>
  public static T[][] ToArray2D<T>(this IEnumerable<IEnumerable<T>> input)
    => input.Select(Enumerable.ToArray).ToArray();

  /// <summary>
  ///   Creates a list of lists from a sequence of sequences.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences from which to create a list of lists.
  /// </param>
  /// <returns>
  ///   An list that contains lists that contain the elements from the
  ///   input sequences.
  /// </returns>
  public static List<List<T>> ToList2D<T>(this IEnumerable<IEnumerable<T>> input)
    => input.Select(Enumerable.ToList).ToList();

  /// <summary>
  ///   Creates a grid from a sequence of sequences.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements of <paramref name="input"/>.
  /// </typeparam>
  /// <param name="input">
  ///   The input sequence of sequences from which to create a grid.
  /// </param>
  /// <param name="filler">
  ///   The element with which gaps in the grid (from unevenly sized
  ///   sub-sequences) should be filled.
  /// </param>
  /// <returns>
  ///   An grid that contains the elements from the input sequences.
  /// </returns>
  public static Grid<T> ToGrid2D<T>(this IEnumerable<IEnumerable<T>> input, T filler)
    => new Grid<T>(input, filler);
}
