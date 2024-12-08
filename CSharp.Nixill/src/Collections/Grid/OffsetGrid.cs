using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections
{
  public class OffsetGrid<T> : IGrid<T>
  {
    Grid<T> BackingGrid;
    int RowOffset = 0;
    int ColumnOffset = 0;

    /// <value>The topmost row of the grid.</value>
    public int Top => -RowOffset;

    /// <value>The leftmost column of the grid.</value>
    public int Left => -ColumnOffset;

    /// <value>The row below the bottom of the grid.</value>
    public int Bottom => Top + Height;

    /// <value>The column beyond the right edge of the grid.</value>
    public int Right => Left + Width;

    /// <summary>
    /// Creates a new 0×0 grid.
    /// </summary>
    public OffsetGrid()
    {
      BackingGrid = new();
    }

    /// <summary>
    /// Creates a new grid from an existing list of lists.
    ///
    /// The outer list is taken as rows, and the inner list as columns
    /// within the row.
    /// </summary>
    public OffsetGrid(IEnumerable<IEnumerable<T>> list, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(list);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    /// <summary>
    /// Creates a new grid of a specified size.
    ///
    /// All cells of the grid will be iniated to the default value for T.
    /// </summary>
    public OffsetGrid(int width, int height, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(width, height);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    public T? this[GridReference gr]
    {
      get => this[gr.Row, gr.Column];
      set => this[gr.Row, gr.Column] = value;
    }

    public T? this[int r, int c]
    {
      get => BackingGrid[r + RowOffset, c + ColumnOffset];
      set => BackingGrid[r + RowOffset, c + ColumnOffset] = value;
    }

    public T? this[string gr]
    {
      get => this[(GridReference)gr];
      set => this[(GridReference)gr] = value;
    }

    public int Height => BackingGrid.Height;
    public int Width => BackingGrid.Width;
    public int Size => BackingGrid.Size;
    public IEnumerable<IEnumerable<T?>> Rows => BackingGrid.Rows;
    public IEnumerable<IEnumerable<T?>> Columns => BackingGrid.Columns;

    public void AddColumn() => BackingGrid.AddColumn();
    public void AddColumn<U>(IEnumerable<U?> column) where U : T => BackingGrid.AddColumn(column);
    public void AddColumn(T? columnItem) => BackingGrid.AddColumn(columnItem);
    public void AddColumn(Func<T?> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);
    public void AddColumn(Func<int, T?> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);

    public void AddColumnLeft()
    {
      BackingGrid.InsertColumn(0);
      ColumnOffset += 1;
    }

    public void AddColumnLeft<U>(IEnumerable<U?> column) where U : T
    {
      BackingGrid.InsertColumn(0, column);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(T? columnItem)
    {
      BackingGrid.InsertColumn(0, columnItem);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(Func<T?> columnItemFunc)
    {
      BackingGrid.InsertColumn(0, columnItemFunc);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(Func<int, T?> columnItemFunc)
    {
      BackingGrid.InsertColumn(0, columnItemFunc);
      ColumnOffset += 1;
    }

    public void AddRow() => BackingGrid.AddRow();
    public void AddRow<U>(IEnumerable<U?> row) where U : T => BackingGrid.AddRow(row);
    public void AddRow(T? rowItem) => AddRow(Enumerable.Repeat(rowItem, Width).ToList());
    public void AddRow(Func<T?> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(x => rowItemFunc()).ToList());
    public void AddRow(Func<int, T?> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc).ToList());

    public void AddRowTop()
    {
      BackingGrid.InsertRow(0);
      RowOffset += 1;
    }

    public void AddRowTop<U>(IEnumerable<U?> row) where U : T
    {
      BackingGrid.InsertRow(0, row);
      RowOffset += 1;
    }

    public void AddRowTop(T? rowItem)
    {
      BackingGrid.InsertRow(0, rowItem);
      RowOffset += 1;
    }

    public void AddRowTop(Func<T?> rowItemFunc)
    {
      BackingGrid.InsertRow(0, rowItemFunc);
      RowOffset += 1;
    }

    public void AddRowTop(Func<int, T?> rowItemFunc)
    {
      BackingGrid.InsertRow(0, rowItemFunc);
      RowOffset += 1;
    }

    public void Clear()
    {
      BackingGrid.Clear();
    }

    public bool Contains(T? item) => BackingGrid.Contains(item);

    public IEnumerable<(T? Item, GridReference Reference)> Flatten()
      => this.SelectMany((l, r) => l.Select((i, c) => (i, GridReference.XY(c - ColumnOffset, r - RowOffset))));

    public IList<T?> GetColumn(int index) => BackingGrid.GetColumn(index + ColumnOffset);
    public IEnumerator<IEnumerable<T?>> GetColumnEnumerator() => BackingGrid.GetColumnEnumerator();
    public IEnumerator<IEnumerable<T?>> GetEnumerator() => BackingGrid.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => BackingGrid.GetEnumerator();
    public IList<T?> GetRow(int index) => BackingGrid.GetRow(index + RowOffset);
    public GridReference? IndexOf(T? item)
    {
      GridReference? rfc = BackingGrid.IndexOf(item);
      if (rfc! != null!)
      {
        return GridReference.XY(rfc.Column - ColumnOffset, rfc.Row - RowOffset);
      }
      return null;
    }

    public GridReference? IndexOfTransposed(T? item)
    {
      GridReference? rfc = BackingGrid.IndexOfTransposed(item);
      if (rfc! != null!)
      {
        return GridReference.XY(rfc.Column - ColumnOffset, rfc.Row - RowOffset);
      }
      return null;
    }

    public void InsertColumn(int before) => BackingGrid.InsertColumn(before);
    public void InsertColumn<U>(int before, IEnumerable<U?> column) where U : T => BackingGrid.InsertColumn(before, column);
    public void InsertColumn(int before, T? columnItem) => BackingGrid.InsertColumn(before, columnItem);
    public void InsertColumn(int before, Func<T?> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);
    public void InsertColumn(int before, Func<int, T?> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);

    public void InsertColumnShiftLeft(int before)
    {
      BackingGrid.InsertColumn(before + ColumnOffset);
      ColumnOffset += 1;
    }

    public void InsertColumnShiftLeft<U>(int before, IEnumerable<U> column) where U : T
    {
      BackingGrid.InsertColumn(before + ColumnOffset, column);
      ColumnOffset += 1;
    }

    public void InsertColumnShiftLeft(int before, T columnItem)
    {
      BackingGrid.InsertColumn(before + ColumnOffset, columnItem);
      ColumnOffset += 1;
    }

    public void InsertColumnShiftLeft(int before, Func<T> columnItemFunc)
    {
      BackingGrid.InsertColumn(before + ColumnOffset, columnItemFunc);
      ColumnOffset += 1;
    }

    public void InsertColumnShiftLeft(int before, Func<int, T> columnItemFunc)
    {
      BackingGrid.InsertColumn(before + ColumnOffset, columnItemFunc);
      ColumnOffset += 1;
    }

    public void InsertRow(int before) => BackingGrid.InsertRow(before);
    public void InsertRow<U>(int before, IEnumerable<U?> row) where U : T => BackingGrid.InsertRow(before, row);
    public void InsertRow(int before, T? rowItem) => BackingGrid.InsertRow(before, rowItem);
    public void InsertRow(int before, Func<T?> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);
    public void InsertRow(int before, Func<int, T?> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);

    public void InsertRowShiftUp(int before)
    {
      BackingGrid.InsertRow(before + RowOffset);
      RowOffset += 1;
    }

    public void InsertRowShiftUp<U>(int before, IEnumerable<U> row) where U : T
    {
      BackingGrid.InsertRow(before + RowOffset, row);
      RowOffset += 1;
    }

    public void InsertRowShiftUp(int before, T rowItem)
    {
      BackingGrid.InsertRow(before + RowOffset, rowItem);
      RowOffset += 1;
    }

    public void InsertRowShiftUp(int before, Func<T> rowItemFunc)
    {
      BackingGrid.InsertRow(before + RowOffset, rowItemFunc);
      RowOffset += 1;
    }

    public void InsertRowShiftUp(int before, Func<int, T> rowItemFunc)
    {
      BackingGrid.InsertRow(before + RowOffset, rowItemFunc);
      RowOffset += 1;
    }

    public bool IsWithinGrid(GridReference reference) => reference.Row >= -RowOffset && reference.Row < Height - RowOffset
      && reference.Column >= -ColumnOffset && reference.Column <= Width - ColumnOffset;

    public void RemoveColumnAt(int col) => BackingGrid.RemoveColumnAt(col + ColumnOffset);

    public void RemoveColumnShiftRight(int col)
    {
      BackingGrid.RemoveColumnAt(col + ColumnOffset);
      ColumnOffset -= 1;
    }

    public void RemoveRowAt(int row) => BackingGrid.RemoveRowAt(row + RowOffset);

    public void RemoveRowShiftDown(int row)
    {
      BackingGrid.RemoveRowAt(row + RowOffset);
      RowOffset -= 1;
    }
  }
}