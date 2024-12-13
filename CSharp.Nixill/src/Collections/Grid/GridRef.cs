using System.Text.RegularExpressions;
using Nixill.Objects;
using Nixill.Utils;

namespace Nixill.Collections;

public static class GridRef
{
  static Regex A1Form = new Regex("^([A-Za-z]+)(\\d+)$");
  static Regex R1C1Form = new Regex("^[Rr](\\d+)[Cc](\\d+)$");

  public static IntVector2 XY(int x, int y) => new IntVector2(x: x, y: y);
  public static IntVector2 RC(int r, int c) => new IntVector2(y: r, x: c);

  public static IntVector2 FromString(string str)
  {
    Match regMatch = A1Form.Match(str);
    int row;
    int column;
    if (regMatch.Success)
    {
      column = ColumnNameToNumber(regMatch.Groups[1].Value);
      row = int.Parse(regMatch.Groups[2].Value) - 1;
    }
    else
    {
      regMatch = R1C1Form.Match(str);
      if (regMatch.Success)
      {
        row = int.Parse(regMatch.Groups[1].Value) - 1;
        column = int.Parse(regMatch.Groups[2].Value) - 1;
      }
      else throw new ArgumentException("GridReferences from strings must be in A1 format or R1C1 format.");
    }

    if (column < 0) throw new ArgumentOutOfRangeException("col", "Cannot be negative.");
    if (row < 0) throw new ArgumentOutOfRangeException("row", "Cannot be negative.");

    return new IntVector2(y: row, x: column);
  }

  public static (int X, int Y) ToXY(this IntVector2 input) => (input.X, input.Y);
  public static (int Row, int Column) ToRC(this IntVector2 input) => (input.Y, input.X);

  public static IntVector2 Transposed(this IntVector2 input) => new IntVector2(x: input.Y, y: input.X);

  public static int ColumnNameToNumber(string name)
  {
    return NumberConverter.Parse<int>(name, 26, Digits.Alpha, bijective: true) - 1;
  }

  public static string ColumnNumberToName(int num)
  {
    return NumberConverter.Format(num + 1, 26);
  }

  public static string ToA1String(this IntVector2 input)
  {
    return ColumnNumberToName(input.X) + (input.Y + 1);
  }

  public static string ToR1C1String(this IntVector2 input)
  {
    return "R" + (input.Y + 1) + "C" + (input.X + 1);
  }

  public static int Compare(IntVector2 left, IntVector2 right)
    => Sequence.FirstNonZero(
      left.Y - right.Y,
      left.X - right.X
    );
}