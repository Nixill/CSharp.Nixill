using System.Numerics;
using Nixill.Collections;
using Nixill.Utils;

public readonly struct LongVector2
{
  public long X { get; init; }
  public long Y { get; init; }

  public LongVector2(long x, long y) => (X, Y) = (x, y);

  public static implicit operator LongVector2((long X, long Y) input) => new(input.X, input.Y);
  public static implicit operator (long X, long Y)(LongVector2 input) => (input.X, input.Y);

  public static implicit operator LongVector2(IntVector2 input) => new(input.X, input.Y);
  public static explicit operator IntVector2(LongVector2 input) => new((int)input.X, (int)input.Y);

  public static implicit operator LongVector2((int X, int Y) input) => new(input.X, input.Y);
  public static explicit operator (int X, int Y)(LongVector2 input) => ((int)input.X, (int)input.Y);

  public static LongVector2 operator +(LongVector2 left, LongVector2 right) => (left.X + right.X, left.Y + right.Y);
  public static LongVector2 operator -(LongVector2 left, LongVector2 right) => (left.X - right.X, left.Y - right.Y);
  public static LongVector2 operator -(LongVector2 input) => (-input.X, -input.Y);
  public static LongVector2 operator *(LongVector2 left, long right) => (left.X * right, left.Y * right);
  public static LongVector2 operator *(long left, LongVector2 right) => (left * right.X, left * right.Y);

  public static bool operator ==(LongVector2 left, LongVector2 right) => left.X == right.X && left.Y == right.Y;
  public static bool operator !=(LongVector2 left, LongVector2 right) => !(left == right);

  public override bool Equals(object? obj) => obj is LongVector2 other && this == other;
  public override int GetHashCode() => (X, Y).GetHashCode();

  public LongVector2 RotateLeft() => (this.Y, -this.X);
  public LongVector2 RotateAround() => -this;
  public LongVector2 RotateRight() => (-this.Y, this.X);

  public LongVector2 RotateLeft(int times) => NumberUtils.NNMod(times, 4) switch
  {
    0 => this,
    1 => RotateLeft(),
    2 => RotateAround(),
    3 => RotateRight(),
    _ => throw new Exception("How did you get this? Please file an issue on GitHub!")
  };
  public LongVector2 RotateAround(int times) => (times % 2 == 0) ? this : RotateAround();
  public LongVector2 RotateRight(int times) => RotateLeft(-times);

  public static readonly LongVector2 Up = (0, -1);
  public static readonly LongVector2 Right = (1, 0);
  public static readonly LongVector2 Down = (0, 1);
  public static readonly LongVector2 Left = (-1, 0);
}