using System;
using System.Text.RegularExpressions;
using Nixill.Objects;
using Nixill.Utils;

namespace Nixill.Collections
{
  [Obsolete("Use IntVector2 (Nixill.Objects) or static GridRef methods instead.")]
  public class GridReference : IComparable<GridReference>
  {
    static Regex A1Form = new Regex("^([A-Za-z]+)(\\d+)$");
    static Regex R1C1Form = new Regex("^[Rr](\\d+)[Cc](\\d+)$");

    /// <summary>The column of the referenced cell.</summary>
    public int Column { get; init; }

    /// <summary>The row of the referenced cell.</summary>
    public int Row { get; init; }

    /// <summary>
    /// This GridReference, transposed (Row and column swapped).
    /// </summary>
    public GridReference Transposed { get => GridReference.RC(c: Row, r: Column); }

    private GridReference() { }

    /// <summary>
    /// Constructs a new GridReference given a string representation
    /// thereof.
    ///
    /// This constructor accepts A1 or R1C1 notations, but does not
    /// support absolute notation for A1 (with <c>$</c>) or relative
    /// notation for R1C1 (with <c>[]</c>).
    /// </summary>
    /// <param name="input">The string representation to use.</param>
    [Obsolete("Use GridRef.FromString(input) instead.")]
    public GridReference(string input)
    {
      Match regMatch = A1Form.Match(input);
      int row;
      int column;
      if (regMatch.Success)
      {
        column = ColumnNameToNumber(regMatch.Groups[1].Value);
        row = int.Parse(regMatch.Groups[2].Value) - 1;
      }
      else
      {
        regMatch = R1C1Form.Match(input);
        if (regMatch.Success)
        {
          row = int.Parse(regMatch.Groups[1].Value) - 1;
          column = int.Parse(regMatch.Groups[2].Value) - 1;
        }
        else throw new ArgumentException("GridReferences from strings must be in A1 format or R1C1 format.");
      }

      if (column < 0) throw new ArgumentOutOfRangeException("col", "Cannot be negative.");
      if (row < 0) throw new ArgumentOutOfRangeException("row", "Cannot be negative.");

      Row = row;
      Column = column;
    }

    /// <summary>
    /// Compares this GridReference to another, returning a negative
    /// integer, zero, or a positive integer if this GridReference is less
    /// than, equal to, or greater than the supplied GridReference
    /// respectively.
    ///
    /// It compares rows first, before columns; a GridReference is "less
    /// than" another GridReference lower and to the left.
    /// </summary>
    [Obsolete("Use GridRef.Compare(this, other) instead.")]
    public int CompareTo(GridReference? other)
    {
      return Sequence.FirstNonZero(
        Row - other!.Row,
        Column - other.Column
      );
    }

    /// <summary>
    /// Returns a string representation of this GridReference in A1
    /// notation.
    /// </summary>
    [Obsolete("Use the IntVector2 extension method ToA1String() instead.")]
    public string ToA1String()
    {
      return ColumnNumberToName(Column) + (Row + 1);
    }

    /// <summary>
    /// Returns a string representation of this GridReference in R1C1
    /// notation.
    /// </summary>
    [Obsolete("Use the IntVector2 extension method ToR1C1String() instead.")]
    public string ToR1C1String()
    {
      return "R" + (Row + 1) + "C" + (Column + 1);
    }

    public override string ToString() => ToA1String();

    public override bool Equals(object? obj)
    {
      if (obj is GridReference other)
      {
        return other.Row == Row && other.Column == Column;
      }
      else return false;
    }

    public override int GetHashCode()
    {
      return (Row & 0xFFFF) << 16
      + (Column & 0xFFFF);
    }

    /// <summary>
    /// Changes a column name, as seen in several popular spreadsheet
    /// software, to a number. A becomes 0, B becomes 1, etc.
    /// </summary>
    [Obsolete("Use GridRef.ColumnNameToNumber(name) instead.")]
    public static int ColumnNameToNumber(string name)
    {
      return NumberConverter.Parse<int>(name, 26, Digits.Alpha, bijective: true) - 1;
    }

    /// <summary>
    /// Changes a column number to a name as seen in several popular
    /// spreadsheet software. 0 becomes A, 1 becomes B, etc.
    /// </summary>
    [Obsolete("Use GridRef.ColumnNumberToName(num) instead.")]
    public static string ColumnNumberToName(int num)
    {
      return NumberConverter.Format(num + 1, 26);
    }

    [Obsolete("Use GridRef.XY(x, y) instead.")]
    public static GridReference XY(int x, int y) => new GridReference { Row = y, Column = x };
    [Obsolete("Use GridRef.RC(r, c) instead.")]
    public static GridReference RC(int r, int c) => new GridReference { Row = r, Column = c };

    public static explicit operator GridReference(string input) => new GridReference(input);

    public static implicit operator string(GridReference input) => input.ToString();

    public static bool operator ==(GridReference left, GridReference right) => left.Equals(right);
    public static bool operator !=(GridReference left, GridReference right) => !left.Equals(right);
    public static bool operator <(GridReference left, GridReference right) => left.CompareTo(right) < 0;
    public static bool operator >(GridReference left, GridReference right) => left.CompareTo(right) > 0;
    public static bool operator <=(GridReference left, GridReference right) => left.CompareTo(right) <= 0;
    public static bool operator >=(GridReference left, GridReference right) => left.CompareTo(right) >= 0;
  }
}