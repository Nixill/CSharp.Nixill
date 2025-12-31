using System.Text.RegularExpressions;
using System;
using System.Numerics;

namespace Nixill.Utils;

/// <summary>
///   Provides utilities for working with numbers, including conversion.
/// </summary>
public static class NumberUtils
{
  /// <summary>
  ///   Returns true iff the source number has all of the target bits.
  ///   <para/>
  ///   Specifically, returns <c>src &amp; trg == trg</c>.
  ///   <para/>
  ///   Special case: If <c>trg</c> is 0, returns <c>src == 0</c>.
  /// </summary>
  /// <param name="src">The source number to check.</param>
  /// <param name="trg">The target number to check against.</param>
  /// <returns>Whether or not all target bits are present.</returns>
  public static bool HasAllBits(int src, int trg)
  {
    if (trg == 0) return src == 0;
    else return (src & trg) == trg;
  }

  /// <summary>
  ///   Returns true iff the source number has any of the target bits.
  ///   <para/>
  ///   Specifically, returns <c>src &amp; trg != 0</c>.
  ///   <para/>
  ///   Special case: If <c>trg</c> is 0, returns <c>src != 0</c>.
  /// </summary>
  /// <param name="src">The source number to check.</param>
  /// <param name="trg">The target number to check against.</param>
  /// <returns>Whether or not any target bits are present.</returns>
  public static bool HasAnyBits(int src, int trg)
  {
    if (trg == 0) return src != 0;
    else return (src & trg) != 0;
  }

  /// <summary>
  ///   Returns the non-negative modulus of division of <c>n</c> by <c>d</c>.
  /// </summary>
  /// <param name="n">The numerator, or dividend.</param>
  /// <param name="d">The denominator, or divisor.</param>
  /// <returns>The non-negative modulus.</returns>
  public static T NNMod<T>(T n, T d) where T : IModulusOperators<T, T, T>, IAdditionOperators<T, T, T>,
    IComparable<T>, IAdditiveIdentity<T, T>
    => (n %= d).CompareTo(T.AdditiveIdentity) < 0 ? n + d : n;

  /// <summary>
  ///   Returns the greatest common denominator (or greatest common
  ///   factor) of two numbers.
  /// </summary>
  /// <param name="a">One number.</param>
  /// <param name="b">The other number.</param>
  /// <returns>The greatest common denominator.</returns>
  public static int GCD(int a, int b) => (int)GCD((long)a, (long)b);

  /// <summary>
  ///   Returns the greatest common denominator (or greatest common
  ///   factor) of two numbers.
  /// </summary>
  /// <param name="a">One number.</param>
  /// <param name="b">The other number.</param>
  /// <returns>The greatest common denominator.</returns>
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
  /// <summary>
  ///   Returns the least common multiple of two numbers.
  /// </summary>
  /// <param name="a">One number.</param>
  /// <param name="b">The other number.</param>
  /// <returns>The least common multiple.</returns>
  public static long LCM(long a, long b) => a / GCD(a, b) * b;

  /// <summary>
  ///   Calculates the linear interpolation of a value.
  /// </summary>
  /// <param name="x">
  ///   The <c>x</c> value for which <c>y</c> should be calculated.
  /// </param>
  /// <param name="yAtX0">
  ///   The <c>y</c> value for the line at <c>x = 0</c>.
  /// </param>
  /// <param name="yAtX1">
  ///   The <c>y</c> value for the line at <c>x = 1</c>.
  /// </param>
  /// <returns>
  ///   The <c>y</c> value for the given <c>x</c> value.
  /// </returns>
  public static double Lerp(double x, double yAtX0, double yAtX1)
    => x * (yAtX1 - yAtX0) + yAtX1;

  /// <summary>
  ///   Calculates the inverse linear interpolation of a value.
  /// </summary>
  /// <param name="y">
  ///   The <c>y</c> value for which <c>x</c> should be calculated.
  /// </param>
  /// <param name="yAtX0">
  ///   The <c>y</c> value for the line at <c>x = 0</c>.
  /// </param>
  /// <param name="yAtX1">
  ///   The <c>y</c> value for the line at <c>x = 1</c>.
  /// </param>
  /// <returns>
  ///   The <c>x</c> value for the given <c>y</c> value.
  /// </returns>
  public static double InvLerp(double y, double yAtX0, double yAtX1)
    => (y - yAtX0) / (yAtX1 - yAtX0);

  /// <summary>
  ///   Calculates the inverse linear interpolation of a value, from a
  ///   presumably sorted array.
  /// </summary>
  /// <param name="args">
  ///   An array which has at least three values, which are respectively:
  ///   <list type="number">
  ///     <item>The <c>y</c> value for the line at <c>x = 0</c>.</item>
  ///     <item>
  ///       The <c>y</c> value for which <c>x</c> should be calculated.
  ///     </item>
  ///     <item>The <c>y</c> value for the line at <c>x = 1</c>.</item>
  ///   </list>
  ///   Any values beyond the third are ignored.
  /// </param>
  /// <returns>
  ///   The <c>x</c> value for the given <c>y</c> value.
  /// </returns>
  public static double InvLerp(double[] args)
    => (args[1] - args[0]) / (args[2] - args[0]);

  /// <summary>
  ///   Calculates the inverse linear interpolation for three values, and
  ///   then the linear interpolation for that result with two more.
  /// </summary>
  /// <param name="y1">
  ///   The <c>y</c> value on the first line for which <c>y</c> on the
  ///   second line should be calculated at the same <c>x</c>.
  /// </param>
  /// <param name="y1AtX0">
  ///   The <c>y</c> value for the first line at <c>x = 0</c>.
  /// </param>
  /// <param name="y1AtX1">
  ///   The <c>y</c> value for the first line at <c>x = 1</c>.
  /// </param>
  /// <param name="y2AtX0">
  ///   The <c>y</c> value for the second line at <c>x = 0</c>.
  /// </param>
  /// <param name="y2AtX1">
  ///   The <c>y</c> value for the second line at <c>x = 1</c>.
  /// </param>
  /// <returns>
  ///   The <c>y</c> value on the second line at the same <c>x</c> as the
  ///   given <c>y</c> on the first line.
  /// </returns>
  public static double ReLerp(double y1, double y1AtX0, double y1AtX1, double y2AtX0, double y2AtX1)
    => Lerp(InvLerp(y1, y1AtX0, y1AtX1), y2AtX0, y2AtX1);
}
