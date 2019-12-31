using System.Text.RegularExpressions;
using Nixill.Utils;

namespace Nixill.Grid {
  public class GridReference {
    static Regex A1Form = new Regex("^([A-Za-z]+)(\\d+)$");
    static Regex R1C1Form = new Regex("^R(\\d+)C(\\d+)$");

    static Cipher ColNameToNum = new Cipher("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "0123456789ABCDEFGHIJKLMNOP");
    static Cipher ColNumToName = ColNameToNum.Reverse;

    /// <summary>The column of the referenced cell.</summary>
    public int Column { get; }

    /// <summary>The row of the referenced cell.</summary>
    public int Row { get; }

    /// <summary>
    /// Constructs a new GridReference given its individual coordinates.
    /// </summary>
    /// <param name="column">The column to use.</param>
    /// <param name="row">The row to use.</param>
    public GridReference(int col, int row) {
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
    public GridReference(string input) {
      Match regMatch = A1Form.Match(input);
      int row;
      int column;
      if (regMatch.Success) {

      }
    }

    public static ColumnNameToNumber(string name) {
      name = ColNameToNum.Apply(name);
      return Numbers.
    }
  }
}