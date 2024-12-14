using System.Text.RegularExpressions;
using Nixill.Objects;
using Nixill.Utils;

namespace Nixill.Collections;

/// <summary>
///   A collection of static and extension methods useful for converting
///   <see cref="IntVector2"/>s to and from other forms of grid references.
/// </summary>
public static class GridRef
{
  static Regex A1Form = new Regex("^([A-Za-z]+)(\\d+)$");
  static Regex R1C1Form = new Regex("^[Rr](\\d+)[Cc](\\d+)$");

  /// <summary>
  ///   Returns an <see cref="IntVector2"/> with parameters specified in
  ///   XY order.
  /// </summary>
  /// <remarks>
  ///   This is the same order as in the
  ///   <see cref="IntVector2.IntVector2(int, int)">constructor</see>, but
  ///   the method may be useful for being unambiguous.
  /// </remarks>
  /// <param name="x">The x coordinate of this reference.</param>
  /// <param name="y">The y coordinate of this reference.</param>
  /// <returns>The specified IntVector2.</returns>
  public static IntVector2 XY(int x, int y) => new IntVector2(x: x, y: y);

  /// <summary>
  ///   Returns an <see cref="IntVector2"/> with parameters specified in
  ///   row-column order.
  /// </summary>
  /// <param name="r">The row of this reference.</param>
  /// <param name="c">The column of this reference.</param>
  /// <returns>The specified IntVector2.</returns>
  public static IntVector2 RC(int r, int c) => new IntVector2(y: r, x: c);

  /// <summary>
  ///   Returns an <see cref="IntVector2"/> with its parameters specified
  ///   by a string.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
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

  /// <summary>
  ///   Converts an <see cref="IntVector2"/> into an X-Y ordered tuple.
  /// </summary>
  /// <remarks>
  ///   This is the same as the <c>(int, int)</c> cast, but the method can
  ///   be useful for being unambiguous.
  /// </remarks>
  /// <param name="input">The vector.</param>
  /// <returns>The tuple.</returns>
  public static (int X, int Y) ToXY(this IntVector2 input) => (input.X, input.Y);

  /// <summary>
  ///   Converts an <see cref="IntVector2"/> into a row-column ordered tuple.
  /// </summary>
  /// <param name="input">The vector.</param>
  /// <returns>The tuple.</returns>
  public static (int Row, int Column) ToRC(this IntVector2 input) => (input.Y, input.X);

  /// <summary>
  ///   Swaps the two axes of an <see cref="IntVector2"/>.
  /// </summary>
  /// <param name="input">The vector.</param>
  /// <returns>The vector, but with its axes swapped.</returns>
  public static IntVector2 Transposed(this IntVector2 input) => new IntVector2(x: input.Y, y: input.X);

  /// <summary>
  ///   Converts a column name, such as <c>A</c>, <c>B</c>, <c>AJ</c>, to
  ///   a 0-indexed column number, such as <c>0</c>, <c>1</c>, <c>35</c>.
  /// </summary>
  /// <param name="name">The column name.</param>
  /// <returns>The column number.</returns>
  public static int ColumnNameToNumber(string name)
  {
    return NumberConverter.Parse<int>(name, 26, Digits.Alpha, bijective: true) - 1;
  }

  /// <summary>
  ///   Converts a 0-indexed column number, such as <c>0</c>, <c>1</c>,
  ///   <c>35</c>, to a column name, such as <c>A</c>, <c>B</c>, <c>AJ</c>.
  /// </summary>
  /// <param name="num">The column number.</param>
  /// <returns>The column name.</returns>
  public static string ColumnNumberToName(int num)
  {
    return NumberConverter.Format(num + 1, 26);
  }

  /// <summary>
  ///   Converts an <see cref="IntVector2"/> to an A1-style string
  ///   representation of the coordinate it specifies.
  /// </summary>
  /// <param name="input">The vector.</param>
  /// <returns>The A1-style string.</returns>
  public static string ToA1String(this IntVector2 input)
  {
    return ColumnNumberToName(input.X) + (input.Y + 1);
  }

  /// <summary>
  ///   Converts an <see cref="IntVector2"/> to an R1C1-style string
  ///   representation of the coordinate it specifies.
  /// </summary>
  /// <param name="input">The vector.</param>
  /// <returns>The R1C1-style string.</returns>
  public static string ToR1C1String(this IntVector2 input)
  {
    return "R" + (input.Y + 1) + "C" + (input.X + 1);
  }

  /// <summary>
  ///   Compares two <see cref="IntVector2"/>s by row, comparing by column
  ///   if necessary to break a tie.
  /// </summary>
  /// <param name="left">The left vector.</param>
  /// <param name="right">The right vector.</param>
  /// <returns>
  ///   <list type="bullet">
  ///     <item>
  ///       A negative integer if <c>left</c> is a lower row than
  ///       <c>right</c>, or the same row but a lower column.
  ///     </item>
  ///     <item>
  ///       A positive integer if <c>left</c> is a higher row than
  ///       <c>right</c>, or the same row but a higher column.
  ///     </item>
  ///     <item>
  ///       Zero, if the two are equal.
  ///     </item>
  ///   </list>
  /// </returns>
  public static int Compare(IntVector2 left, IntVector2 right)
    => Sequence.FirstNonZero(
      left.Y - right.Y,
      left.X - right.X
    );
}