using System.Text.RegularExpressions;
using System;
using System.Numerics;

namespace Nixill.Utils;

/// <summary>
/// Provides utilities for working with numbers, including conversion.
/// </summary>
public static class NumberUtils
{
  /// <summary>
  /// Returns true iff the source number has all of the target bits.
  ///
  /// Specifically, returns <c>src &amp; trg == trg</c>.
  ///
  /// Special case: If <c>trg</c> is 0, returns <c>src == 0</c>.
  /// </summary>
  public static bool HasAllBits(int src, int trg)
  {
    if (trg == 0) return src == 0;
    else return (src & trg) == trg;
  }

  /// <summary>
  /// Returns true iff the source number has any of the target bits.
  ///
  /// Specifically, returns <c>src &amp; trg != 0</c>.
  ///
  /// Special case: If <c>trg</c> is 0, returns <c>src != 0</c>.
  /// </summary>
  public static bool HasAnyBits(int src, int trg)
  {
    if (trg == 0) return src != 0;
    else return (src & trg) != 0;
  }

  /// <summary>
  /// Returns the non-negative modulus of division of n by d.
  /// </summary>
  public static T NNMod<T>(T n, T d) where T : IModulusOperators<T, T, T>, IAdditionOperators<T, T, T>,
    IComparable<T>, IAdditiveIdentity<T, T>
    => (n %= d).CompareTo(T.AdditiveIdentity) < 0 ? n + d : n;

  public static int GCD(int a, int b) => (int)GCD((long)a, (long)b);
  public static long GCD(long a, long b)
  {
    // Turn negative numbers positive; since any negative integer can be expressed as -1 times a positive integer, the GCD of two negative numbers (or a negative and a positive) will be the same as GCD(|a|, |b|) anyway
    if (a < 0) a = -a;
    if (b < 0) b = -b;
    // Also, a should be the larger number.
    if (b > a) (a, b) = (b, a);
    while (b != 0)
      (a, b) = (b, a % b);
    return a;
  }

  // No int override for LCM because some combinations of ints may
  // produce long results anyway.
  public static long LCM(long a, long b) => a / GCD(a, b) * b;
}
