using System.Numerics;
using Nixill.Collections;
using Nixill.Utils;

namespace Nixill.Objects;

/// <summary>
///   A vector of two ints.
/// </summary>
public readonly struct IntVector2
{
  /// <summary>
  ///   Get or init: The X coordinate of this vector.
  /// </summary>
  public int X { get; init; }

  /// <summary>
  ///   Get or init: The Y coordinate of this vector.
  /// </summary>
  public int Y { get; init; }

  /// <summary>
  ///   Creates a new IntVector2 with the given coordinates.
  /// </summary>
  /// <param name="x">The x coordinate.</param>
  /// <param name="y">The y coordinate.</param>
  public IntVector2(int x, int y) => (X, Y) = (x, y);

  /// <summary>
  ///   Converts a pair of ints into an IntVector2.
  /// </summary>
  /// <param name="input">The pair of ints, in the order of X, Y.</param>
  public static implicit operator IntVector2((int X, int Y) input) => new(input.X, input.Y);

  /// <summary>
  ///   Converts an IntVector2 into a pair of ints, in the order of X, Y.
  /// </summary>
  /// <param name="input">The IntVector2.</param>
  public static implicit operator (int X, int Y)(IntVector2 input) => (input.X, input.Y);

  // warning disable justification: cannot suppress by tagging the cast
  [Obsolete]
  public static implicit operator IntVector2(
#pragma warning disable CS0618 // GridReference is obsolete
    GridReference
#pragma warning restore CS0618
  input) => new(input.Column, input.Row);
  [Obsolete]
  public static implicit operator
#pragma warning disable CS0618
    GridReference
#pragma warning restore CS0618
    (IntVector2 input) => GridReference.XY(input.X, input.Y);

  /// <summary>
  ///   Adds two IntVector2 by adding each individual coordinate.
  /// </summary>
  /// <param name="left">The left IntVector2.</param>
  /// <param name="right">The right IntVector2.</param>
  /// <returns>The sum of the inputs.</returns>
  public static IntVector2 operator +(IntVector2 left, IntVector2 right) => (left.X + right.X, left.Y + right.Y);

  /// <summary>
  ///   Subtracts one IntVector2 from another by subtracting each
  ///   individual coordinate.
  /// </summary>
  /// <param name="left">The left IntVector2.</param>
  /// <param name="right">The right IntVector2.</param>
  /// <returns>The difference of the inputs.</returns>
  public static IntVector2 operator -(IntVector2 left, IntVector2 right) => (left.X - right.X, left.Y - right.Y);

  /// <summary>
  ///   Negates an IntVector2 by negating its components.
  /// </summary>
  /// <param name="input">The IntVector2.</param>
  /// <returns>The negated IntVector2.</returns>
  public static IntVector2 operator -(IntVector2 input) => (-input.X, -input.Y);

  /// <summary>
  ///   Scales an IntVector2 by multiplying its components by a number.
  /// </summary>
  /// <param name="left">The IntVector2.</param>
  /// <param name="right">The number.</param>
  /// <returns>The scaled vector.</returns>
  public static IntVector2 operator *(IntVector2 left, int right) => (left.X * right, left.Y * right);

  /// <summary>
  ///   Scales an IntVector2 by multiplying a number by its components.
  /// </summary>
  /// <param name="left">The number.</param>
  /// <param name="right">The IntVector2.</param>
  /// <returns>The scaled vector.</returns>
  public static IntVector2 operator *(int left, IntVector2 right) => (left * right.X, left * right.Y);

  /// <summary>
  ///   Descales an IntVector2 by dividing its components by a number.
  /// </summary>
  /// <param name="left">The IntVector2.</param>
  /// <param name="right">The number.</param>
  /// <returns>The descaled vector.</returns>
  public static IntVector2 operator /(IntVector2 left, int right) => (left.X / right, left.Y / right);

  /// <summary>
  ///   Determines whether two IntVector2s are equal.
  /// </summary>
  /// <param name="left">The left IntVector2.</param>
  /// <param name="right">The right IntVector2.</param>
  /// <returns><c>true</c> iff the vectors are equal.</returns>
  public static bool operator ==(IntVector2 left, IntVector2 right) => left.X == right.X && left.Y == right.Y;

  /// <summary>
  ///   Determines whether two IntVector2s are not equal.
  /// </summary>
  /// <param name="left">The left IntVector2.</param>
  /// <param name="right">The right IntVector2.</param>
  /// <returns><c>true</c> iff the vectors are not equal.</returns>
  public static bool operator !=(IntVector2 left, IntVector2 right) => !(left == right);

  /// <summary>
  ///   Determines whether another object is an IntVector2 that is equal
  ///   to this one.
  /// </summary>
  /// <param name="obj">The other object.</param>
  /// <returns>
  ///   <c>true</c> iff <c>obj</c> is an IntVector2 equal to this one.
  /// </returns>
  public override bool Equals(object? obj) => obj is IntVector2 other && this == other;

  /// <summary>
  ///   Gets the hash code of this IntVector2.
  /// </summary>
  /// <returns>The hash code.</returns>
  public override int GetHashCode() => (X, Y).GetHashCode();

  /// <summary>
  ///   Returns this IntVector2 rotated 90 degrees to the left.
  /// </summary>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateLeft() => (this.Y, -this.X);

  /// <summary>
  ///   Returns this IntVector2 rotated 180 degrees.
  /// </summary>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateAround() => -this;

  /// <summary>
  ///   Returns this IntVector2 rotated 90 degrees to the right.
  /// </summary>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateRight() => (-this.Y, this.X);

  /// <summary>
  ///   Returns this IntVector2 rotated 90 degrees to the left a number of
  ///   times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateLeft(int times) => NumberUtils.NNMod(times, 4) switch
  {
    0 => this,
    1 => RotateLeft(),
    2 => RotateAround(),
    3 => RotateRight(),
    _ => throw new Exception("How did you get this? Please file an issue on GitHub!")
  };

  /// <summary>
  ///   Returns this IntVector2 rotated 180 degrees a number of times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateAround(int times) => (times % 2 == 0) ? this : RotateAround();

  /// <summary>
  ///   Returns this IntVector2 rotated 90 degrees to the right a number
  ///   of times.
  /// </summary>
  /// <param name="times">The number of times to rotate.</param>
  /// <returns>The rotated IntVector2.</returns>
  public IntVector2 RotateRight(int times) => RotateLeft(-times);

  /// <inheritdoc/>
  public override string ToString()
  {
    return $"(X: {X}, Y: {Y})";
  }

  /// <summary>
  ///   Read-only: The IntVector2 <c>(0, -1)</c>, representing up.
  /// </summary>
  public static readonly IntVector2 Up = (0, -1);

  /// <summary>
  ///   Read-only: The IntVector2 <c>(1, 0)</c>, representing right.
  /// </summary>
  public static readonly IntVector2 Right = (1, 0);

  /// <summary>
  ///   Read-only: The IntVector2 <c>(0, 1)</c>, representing down.
  /// </summary>
  public static readonly IntVector2 Down = (0, 1);

  /// <summary>
  ///   Read-only: The IntVector2 <c>(-1, 0)</c>, representing left.
  /// </summary>
  public static readonly IntVector2 Left = (-1, 0);
}