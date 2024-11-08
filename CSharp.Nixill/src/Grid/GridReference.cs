using System;
using System.Text.RegularExpressions;
using Nixill.Objects;
using Nixill.Utils;

namespace Nixill.Collections
{
  public class GridReference : IComparable<GridReference>
  {
    static Regex A1Form = new Regex("^([A-Za-z]+)(\\d+)$");
    static Regex R1C1Form = new Regex("^[Rr](\\d+)[Cc](\\d+)$");

    /// <summary>The column of the referenced cell.</summary>
    public int Column { get; }

    /// <summary>The row of the referenced cell.</summary>
    public int Row { get; }

    /// <summary>
    /// This GridReference, transposed (Row and column swapped).
    /// </summary>
    public GridReference Transposed { get => new GridReference(Row, Column); }

    /// <summary>
    /// Constructs a new GridReference given its individual coordinates.
    /// </summary>
    /// <param name="column">The column to use.</param>
    /// <param name="row">The row to use.</param>
    public GridReference(int col, int row)
    {
      Column = col;
      Row = row;
    }

    /// <summary>
    /// Constructs a new GridReference given a string representation
    /// thereof.
    ///
    /// This constructor accepts A1 or R1C1 notations, but does not
    /// support absolute notation for A1 (with <c>$</c>) or relative
    /// notation for R1C1 (with <c>[]</c>).
    /// </summary>
    /// <param name="input">The string representation to use.</param>
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
    public int CompareTo(GridReference other)
    {
      return Sequence.FirstNonZero(
        Row - other.Row,
        Column - other.Column
      );
    }

    /// <summary>
    /// Returns a string representation of this GridReference in A1
    /// notation.
    /// </summary>
    public string ToA1String()
    {
      return ColumnNumberToName(Column) + (Row + 1);
    }

    /// <summary>
    /// Returns a string representation of this GridReference in R1C1
    /// notation.
    /// </summary>
    public string ToR1C1String()
    {
      return "R" + (Row + 1) + "C" + (Column + 1);
    }

    public override string ToString() => ToA1String();

    public override bool Equals(object obj)
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
    public static int ColumnNameToNumber(string name)
    {
      return NumberConverter.Parse<int>(name, 26, Digits.Alpha, bijective: true) - 1;
    }

    /// <summary>
    /// Changes a column number to a name as seen in several popular
    /// spreadsheet software. 0 becomes A, 1 becomes B, etc.
    /// </summary>
    public static string ColumnNumberToName(int num)
    {
      return NumberConverter.Format(num + 1, 26);
    }

    public static explicit operator GridReference(string input) => new GridReference(input);
    public static explicit operator GridReference(Tuple<int, int> input) => new GridReference(input.Item1, input.Item2);

    public static implicit operator string(GridReference input) => input.ToString();
    public static implicit operator Tuple<int, int>(GridReference input) => new Tuple<int, int>(input.Column, input.Row);

    public static bool operator ==(GridReference left, GridReference right) => left.Equals(right);
    public static bool operator !=(GridReference left, GridReference right) => !left.Equals(right);
    public static bool operator <(GridReference left, GridReference right) => left.CompareTo(right) < 0;
    public static bool operator >(GridReference left, GridReference right) => left.CompareTo(right) > 0;
    public static bool operator <=(GridReference left, GridReference right) => left.CompareTo(right) <= 0;
    public static bool operator >=(GridReference left, GridReference right) => left.CompareTo(right) >= 0;
  }
}