using Nixill.Utils;
using System;

namespace Nixill.Tables {
  public class RelativeCellReference {
    public int Row { get; protected set; }
    public int Column { get; protected set; }
    public bool StaticRow { get; protected set; }
    public bool StaticColumn { get; protected set; }

    public RelativeCellReference(int r, int c) : this(r, false, c, false) {
      // just use the overloaded constructor
    }

    public RelativeCellReference(int r, bool staticR, int c, bool staticC) {
      Row = r;
      StaticRow = staticR;
      Column = c;
      StaticColumn = staticC;
      if ((StaticRow && r < 0) || (StaticColumn && c < 0)) {
        throw new ArgumentOutOfRangeException("CellReferences cannot have static components less than 0.");
      }
    }

    public static RelativeCellReference operator +(RelativeCellReference refIn) => refIn;
    public static RelativeCellReference operator -(RelativeCellReference refIn) {
      if (refIn.StaticRow || refIn.StaticColumn) {
        // while it would technically be valid to switch 0 to 0, we won't allow that just to avoid confusion
        throw new InvalidOperationException("CellReferences cannot have static components less than 0.");
      }

      // that error also guarantees both are false here, so we can just use falses
      return new RelativeCellReference(-refIn.Row, false, -refIn.Column, false);
    }
  }

  public class CellReferenceUtils {
    public static int ColumnFromA1(string col) {
      string base26 = "";
      string base26add = "";

      if (col == "") {
        base26 = "0";
        base26add = "0";
      }
      else {
        // Convert the string, character-by-character, to a base-26 number.
        // The "add" variable accounts for all columns that have fewer letters.
        for (int i = 0; i < col.Length; i++) {
          char chr = col[i];
          chr = CharToBase26(chr);
          base26 += chr;

          base26add = "1" + base26add;
        }
      }

      return Numbers.StringToInt(base26, 26) + Numbers.StringToInt(base26add, 26) - 1;
    }

    private static char CharToBase26(char input) {
      return Numbers.IntToChar(Numbers.CharToInt(input) - 10);
    }

    public static string ColumnToA1(int col) {
      int digits = 1;
      for (; col >= Math.Pow(26, digits); digits++) {
        col -= (int)Math.Pow(26, digits);
      }

      String text = Numbers.IntToString(col, 26);

      while (text.Length < digits) {
        text = "0" + text;
      }

      String textOut = "";

      for (int i = 0; i < text.Length; i++) {
        textOut += CharFromBase26(text[i]);
      }

      return textOut;
    }

    static char CharFromBase26(char input) {
      return Numbers.IntToChar(Numbers.CharToInt(input) + 10);
    }
  }
}