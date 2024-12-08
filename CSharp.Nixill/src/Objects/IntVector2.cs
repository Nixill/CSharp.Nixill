using System.Numerics;
using Nixill.Collections;
using Nixill.Utils;

public readonly struct IntVector2
{
  public int X { get; init; }
  public int Y { get; init; }

  public IntVector2(int x, int y) => (X, Y) = (x, y);

  public static implicit operator IntVector2((int X, int Y) input) => new(input.X, input.Y);
  public static implicit operator (int X, int Y)(IntVector2 input) => (input.X, input.Y);

  public static implicit operator IntVector2(GridReference input) => new(input.Column, input.Row);
  public static implicit operator GridReference(IntVector2 input) => GridReference.XY(input.X, input.Y);

  public static IntVector2 operator +(IntVector2 left, IntVector2 right) => (left.X + right.X, left.Y + right.Y);
  public static IntVector2 operator -(IntVector2 left, IntVector2 right) => (left.X - right.X, left.Y - right.Y);
  public static IntVector2 operator -(IntVector2 input) => (-input.X, -input.Y);
  public static IntVector2 operator *(IntVector2 left, int right) => (left.X * right, left.Y * right);
  public static IntVector2 operator *(int left, IntVector2 right) => (left * right.X, left * right.Y);

  public static bool operator ==(IntVector2 left, IntVector2 right) => left.X == right.X && left.Y == right.Y;
  public static bool operator !=(IntVector2 left, IntVector2 right) => !(left == right);

  public override bool Equals(object? obj) => obj is IntVector2 other && this == other;
  public override int GetHashCode() => (X, Y).GetHashCode();

  public IntVector2 RotateLeft() => (this.Y, -this.X);
  public IntVector2 RotateAround() => -this;
  public IntVector2 RotateRight() => (-this.Y, this.X);

  public IntVector2 RotateLeft(int times) => NumberUtils.NNMod(times, 4) switch
  {
    0 => this,
    1 => RotateLeft(),
    2 => RotateAround(),
    3 => RotateRight(),
    _ => throw new Exception("How did you get this? Please file an issue on GitHub!")
  };
  public IntVector2 RotateAround(int times) => (times % 2 == 0) ? this : RotateAround();
  public IntVector2 RotateRight(int times) => RotateLeft(-times);

  public static readonly IntVector2 Up = (0, -1);
  public static readonly IntVector2 Right = (1, 0);
  public static readonly IntVector2 Down = (0, 1);
  public static readonly IntVector2 Left = (-1, 0);
}