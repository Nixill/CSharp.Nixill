using System.Numerics;
using Nixill.Collections;
using Nixill.Utils;

namespace Nixill.Objects;

/// <summary>
///   A vector of two ints.
/// </summary>
public readonly struct LongVector2
{
  /// <summary>
  ///   Get or init: The X coordinate of this vector.
  /// </summary>
  public long X { get; init; }

  /// <summary>
  ///   Get or init: The Y coordinate of this vector.
  /// </summary>
  public long Y { get; init; }

  /// <summary>
  ///   Creates a new LongVector2 with the given coordinates.
  /// </summary>
  /// <param name="x">The x coordinate.</param>
  /// <param name="y">The y coordinate.</param>
  public LongVector2(long x, long y) => (X, Y) = (x, y);

  /// <summary>
  ///   Converts a pair of longs into a LongVector2.
  /// </summary>
  /// <param name="input">The pair of longs, in the order of X, Y.</param>
  public static implicit operator LongVector2((long X, long Y) input) => new(input.X, input.Y);

  /// <summary>
  ///   Converts a LongVector2 into a pair of longs, in the order of X, Y.
  /// </summary>
  /// <param name="input">The LongVector2.</param>
  public static implicit operator (long X, long Y)(LongVector2 input) => (input.X, input.Y);

  /// <summary>
  ///   Converts an IntVector2 into a LongVector2.
  /// </summary>
  /// <param name="input">The IntVector2.</param>
  public static implicit operator LongVector2(IntVector2 input) => new(input.X, input.Y);

  /// <summary>
  ///   Converts a LongVector2 into an IntVector2.
  /// </summary>
  /// <param name="input">The LongVector2.</param>
  public static explicit operator IntVector2(LongVector2 input) => new((int)input.X, (int)input.Y);

  /// <summary>
  ///   Converts a pair of ints into a LongVector2.
  /// </summary>
  /// <param name="input">The pair of ints, in the order of X, Y.</param>
  public static implicit operator LongVector2((int X, int Y) input) => new(input.X, input.Y);

  /// <summary>
  ///   Converts a LongVector2 into a pair of ints, in the order of X, Y.
  /// </summary>
  /// <param name="input">The LongVector2.</param>
  public static explicit operator (int X, int Y)(LongVector2 input) => ((int)input.X, (int)input.Y);

  /// <summary>
  ///   Adds two LongVector2s by adding each individual coordinate.
  /// </summary>
  /// <param name="left">The left LongVector2.</param>
  /// <param name="right">The right LongVector2.</param>
  /// <returns>The sum of the inputs.</returns>
  public static LongVector2 operator +(LongVector2 left, LongVector2 right) => (left.X + right.X, left.Y + right.Y);

  /// <summary>
  ///   Subtracts one LongVector2 from another by subtracting each
  ///   individual coordinate.
  /// </summary>
  /// <param name="left">The left LongVector2.</param>
  /// <param name="right">The right LongVector2.</param>
  /// <returns>The difference of the inputs.</returns>
  public static LongVector2 operator -(LongVector2 left, LongVector2 right) => (left.X - right.X, left.Y - right.Y);

  /// <summary>
  ///   Negates a LongVector2 by negating its components.
  /// </summary>
  /// <param name="input">The LongVector2.</param>
  /// <returns>The negated LongVector2.</returns>
  public static LongVector2 operator -(LongVector2 input) => (-input.X, -input.Y);

  /// <summary>
  ///   Scales a LongVector2 by multiplying its components by a number.
  /// </summary>
  /// <param name="left">The LongVector2.</param>
  /// <param name="right">The number.</param>
  /// <returns>The scaled vector.</returns>
  public static LongVector2 operator *(LongVector2 left, long right) => (left.X * right, left.Y * right);

  /// <summary>
  ///   Scales a LongVector2 by multiplying a number by its components.
  /// </summary>
  /// <param name="left">The number.</param>
  /// <param name="right">The LongVector2.</param>
  /// <returns>The scaled vector.</returns>
  public static LongVector2 operator *(long left, LongVector2 right) => (left * right.X, left * right.Y);

  /// <summary>
  ///   Descales a LongVector2 by dividing its components by a number.
  /// </summary>
  /// <param name="left">The LongVector2.</param>
  /// <param name="right">The number.</param>
  /// <returns>The descaled vector.</returns>
  public static LongVector2 operator /(LongVector2 left, long right) => (left.X / right, left.Y / right);

  /// <summary>
  ///   Determines whether two LongVector2s are equal.
  /// </summary>
  /// <param name="left">The left LongVector2.</param>
  /// <param name="right">The right LongVector2.</param>
  /// <returns><c>true</c> iff the vectors are equal.</returns>
  public static bool operator ==(LongVector2 left, LongVector2 right) => left.X == right.X && left.Y == right.Y;

  /// <summary>
  ///   Determines whether two LongVector2s are not equal.
  /// </summary>
  /// <param name="left">The left LongVector2.</param>
  /// <param name="right">The right LongVector2.</param>
  /// <returns><c>true</c> iff the vectors are not equal.</returns>
  public static bool operator !=(LongVector2 left, LongVector2 right) => !(left == right);

  /// <summary>
  ///   Determines whether another object is a LongVector2 that is equal
  ///   to this one.
  /// </summary>
  /// <param name="obj">The other object.</param>
  /// <returns>
  ///   <c>true</c> iff <c>obj</c> is a LongVector2 equal to this one.
  /// </returns>
  public override bool Equals(object? obj) => obj is LongVector2 other && this == other;

  /// <summary>
  ///   Gets the hash code of this LongVector2.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => (X, Y).GetHashCode();

  /// <summary>
  ///   Returns this LongVector2 rotated 90 degrees to the left.
  /// </summary>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateLeft() => (this.Y, -this.X);

  /// <summary>
  ///   Returns this LongVector2 rotated 180 degrees.
  /// </summary>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateAround() => -this;

  /// <summary>
  ///   Returns this LongVector2 rotated 90 degrees to the right.
  /// </summary>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateRight() => (-this.Y, this.X);

  /// <summary>
  ///   Returns this LongVector2 rotated 90 degrees to the left a number of
  ///   times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateLeft(int times) => NumberUtils.NNMod(times, 4) switch
  {
    0 => this,
    1 => RotateLeft(),
    2 => RotateAround(),
    3 => RotateRight(),
    _ => throw new Exception("How did you get this? Please file an issue on GitHub!")
  };

  /// <summary>
  ///   Returns this LongVector2 rotated 180 degrees a number of times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateAround(int times) => (times % 2 == 0) ? this : RotateAround();

  /// <summary>
  ///   Returns this LongVector2 rotated 90 degrees to the right a number
  ///   of times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated LongVector2.</returns>
  public LongVector2 RotateRight(int times) => RotateLeft(-times);

  /// <summary>
  ///   Read-only: The LongVector2 <c>(0, -1)</c>, representing up.
  /// </summary>
  public static readonly LongVector2 Up = (0, -1);

  /// <summary>
  ///   Read-only: The LongVector2 <c>(1, 0)</c>, representing right.
  /// </summary>
  public static readonly LongVector2 Right = (1, 0);

  /// <summary>
  ///   Read-only: The LongVector2 <c>(0, 1)</c>, representing down.
  /// </summary>
  public static readonly LongVector2 Down = (0, 1);

  /// <summary>
  ///   Read-only: The LongVector2 <c>(-1, 0)</c>, representing left.
  /// </summary>
  public static readonly LongVector2 Left = (-1, 0);
}
