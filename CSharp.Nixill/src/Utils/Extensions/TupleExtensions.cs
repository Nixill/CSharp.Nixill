namespace Nixill.Utils.Extensions;

public static class TupleExtensions
{
  public static (T First, T Second) Double<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 2) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1]);
  }

  public static (T First, T Second, T Third) Triple<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 3) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1], array[2]);
  }

  public static (T First, T Second, T Third, T Fourth) Quadruple<T>(this IEnumerable<T> values)
  {
    T[] array = values.ToArray();
    if (array.Length != 4) throw new InvalidOperationException("Sequence does not contain exactly two elements.");
    return (array[0], array[1], array[2], array[3]);
  }

  public static (A A, B B, C C) Flatten<A, B, C>(this ((A A, B B) AB, C C) input) => (input.AB.A, input.AB.B, input.C);
  public static (A A, B B, C C) Flatten<A, B, C>(this (A A, (B B, C C) BC) input) => (input.A, input.BC.B, input.BC.C);

  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B) AB, C C, D D) input)
    => (input.AB.A, input.AB.B, input.C, input.D);
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B, C C) ABC, D D) input)
    => (input.ABC.A, input.ABC.B, input.ABC.C, input.D);
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this ((A A, B B) AB, (C C, D D) CD) input)
    => (input.AB.A, input.AB.B, input.CD.C, input.CD.D);
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, (B B, C C) BC, D D) input)
    => (input.A, input.BC.B, input.BC.C, input.D);
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, (B B, C C, D D) BCD) input)
    => (input.A, input.BCD.B, input.BCD.C, input.BCD.D);
  public static (A A, B B, C C, D D) Flatten<A, B, C, D>(this (A A, B B, (C C, D D) CD) input)
    => (input.A, input.B, input.CD.C, input.CD.D);
}