namespace Nixill.Utils.Extensions;

/// <summary>
///   Extension methods dealing with tuples.
/// </summary>
public static class TupleExtensions
{
  /// <summary>
  ///   Converts a sequence with exactly two elements into a tuple of
  ///   those two elements.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="values">The sequence.</param>
  /// <returns>The tuple.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The sequence contains fewer than or more than two elements.
  /// </exception>
  public static (T First, T Second) Double<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 2) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1]);
  }

  /// <summary>
  ///   Converts a sequence with exactly three elements into a tuple of
  ///   those three elements.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="values">The sequence.</param>
  /// <returns>The tuple.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The sequence contains fewer than or more than three elements.
  /// </exception>
  public static (T First, T Second, T Third) Triple<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 3) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1], array[2]);
  }

  /// <summary>
  ///   Converts a sequence with exactly four elements into a tuple of
  ///   those four elements.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of elements in the sequence.
  /// </typeparam>
  /// <param name="values">The sequence.</param>
  /// <returns>The tuple.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The sequence contains fewer than or more than four elements.
  /// </exception>
  public static (T First, T Second, T Third, T Fourth) Quadruple<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 4) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1], array[2], array[3]);
  }

  /// <summary>
  ///   Flattens a bumpy tuple of three elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C) Flatten<A, B, C>(this ((A A, B B) AB, C C) input) => (input.AB.A, input.AB.B, input.C);

  /// <summary>
  ///   Flattens a bumpy tuple of three elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C) Flatten<A, B, C>(this (A A, (B B, C C) BC) input) => (input.A, input.BC.B, input.BC.C);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B) AB, C C, D D) input)
    => (input.AB.A, input.AB.B, input.C, input.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B, C C) ABC, D D) input)
    => (input.ABC.A, input.ABC.B, input.ABC.C, input.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B) AB, (C C, D D) CD) input)
    => (input.AB.A, input.AB.B, input.CD.C, input.CD.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, (B B, C C) BC, D D) input)
    => (input.A, input.BC.B, input.BC.C, input.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, (B B, C C, D D) BCD) input)
    => (input.A, input.BCD.B, input.BCD.C, input.BCD.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, B B, (C C, D D) CD) input)
    => (input.A, input.B, input.CD.C, input.CD.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (((A A, B B) AB, C C) ABC, D D) input)
    => (input.ABC.AB.A, input.ABC.AB.B, input.ABC.C, input.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, (B B, C C) BC) ABC, D D) input)
    => (input.ABC.A, input.ABC.BC.B, input.ABC.BC.C, input.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, ((B B, C C) BC, D D) BCD) input)
    => (input.A, input.BCD.BC.B, input.BCD.BC.C, input.BCD.D);

  /// <summary>
  ///   Flattens a bumpy tuple of four elements.
  /// </summary>
  /// <typeparam name="A">The type of the first element.</typeparam>
  /// <typeparam name="B">The type of the second element.</typeparam>
  /// <typeparam name="C">The type of the third element.</typeparam>
  /// <typeparam name="D">The type of the fourth element.</typeparam>
  /// <param name="input">The bumpy tuple.</param>
  /// <returns>The flattened tuple.</returns>
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, (B B, (C C, D D) CD) BCD) input)
    => (input.A, input.BCD.B, input.BCD.CD.C, input.BCD.CD.D);

  public static IEnumerable<(int Index, A First, B Second)> IndexFlatten<A, B>(this IEnumerable<(A A, B B)> input)
    => input.Select((itm, ind) => (ind, itm.A, itm.B));

  public static IEnumerable<(int Index, A First, B Second, C Third)> IndexFlatten<A, B, C>(
      this IEnumerable<(A A, B B, C C)> input)
    => input.Select((itm, ind) => (ind, itm.A, itm.B, itm.C));

  public static IEnumerable<(int Index, A First, B Second, C Third, D Fourth)> IndexFlatten<A, B, C, D>(
      this IEnumerable<(A A, B B, C C, D D)> input)
    => input.Select((itm, ind) => (ind, itm.A, itm.B, itm.C, itm.D));

  public static IEnumerable<(int Index, A First, B Second, C Third, D Fourth, E Fifth)> IndexFlatten<A, B, C, D, E>(
      this IEnumerable<(A A, B B, C C, D D, E E)> input)
    => input.Select((itm, ind) => (ind, itm.A, itm.B, itm.C, itm.D, itm.E));
}