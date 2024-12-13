using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections
{
  public class OffsetGrid<T> : IGrid<T>
  {
    Grid<T> BackingGrid;

    // To be clear: Positive values here mean that GridReference (0, 0)
    // refers to a cell below and right of the top-left cell of the
    // backing grid.
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
    /// Creates a new grid from an existing list of lists.
    ///
    /// The outer list is taken as rows, and the inner list as columns
    /// within the row.
    /// </summary>
    public OffsetGrid(IEnumerable<IEnumerable<T>> list, T filler, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(list, filler);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    /// <summary>
    /// Creates a new grid of a specified size.
    ///
    /// All cells of the grid will be iniated to the default value for T.
    /// </summary>
    public OffsetGrid(int width, int height, T filler, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(width, height, filler);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    public OffsetGrid(int width, int height, Func<T> filler, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(width, height, filler);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    public OffsetGrid(int width, int height, Func<IntVector2, T> filler, int rowOffset = 0, int colOffset = 0)
    {
      BackingGrid = new(width, height, filler);
      RowOffset = rowOffset;
      ColumnOffset = colOffset;
    }

    public T this[IntVector2 gr]
    {
      get => BackingGrid[gr - (RowOffset, ColumnOffset)];
      set => BackingGrid[gr - (RowOffset, ColumnOffset)] = value;
    }

    [Obsolete("Use IntVector2 instead.")]
    public T this[GridReference gr]
    {
      get => this[(IntVector2)gr];
      set => this[(IntVector2)gr] = value;
    }

    [Obsolete("Use GridRef.RC(r, c) instead.")]
    public T this[int r, int c]
    {
      get => this[GridRef.RC(r, c)];
      set => this[GridRef.RC(r, c)] = value;
    }

    [Obsolete("Use GridRef.FromString(str) intead.")]
    public T this[string gr]
    {
      get => this[GridRef.FromString(gr)];
      set => this[GridRef.FromString(gr)] = value;
    }

    public int Height => BackingGrid.Height;
    public int Width => BackingGrid.Width;
    public int Size => BackingGrid.Size;
    public IEnumerable<IEnumerable<T>> Rows => BackingGrid.Rows;
    public IEnumerable<IEnumerable<T>> Columns => BackingGrid.Columns;

    [Obsolete("Will be removed because it violates nullability contracts. Use AddColumn(default(T)) instead.")]
    public void AddColumn() => BackingGrid.AddColumn();
    public void AddColumn(IEnumerable<T> column) => BackingGrid.AddColumn(column);
    public void AddColumn(T columnItem) => BackingGrid.AddColumn(columnItem);
    public void AddColumn(Func<T> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);
    public void AddColumn(Func<int, T> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);

    [Obsolete("Will be removed because it violates nullability contracts. Use AddColumnLeft(default(T)) instead.")]
    public void AddColumnLeft()
    {
      BackingGrid.InsertColumn(0);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(IEnumerable<T> column)
    {
      BackingGrid.InsertColumn(0, column);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(T columnItem)
    {
      BackingGrid.InsertColumn(0, columnItem);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(Func<T> columnItemFunc)
    {
      BackingGrid.InsertColumn(0, columnItemFunc);
      ColumnOffset += 1;
    }

    public void AddColumnLeft(Func<int, T> columnItemFunc)
    {
      BackingGrid.InsertColumn(0, columnItemFunc);
      ColumnOffset += 1;
    }

    [Obsolete("Will be removed because it violates nullability contracts. Use AddRow(default(T)) instead.")]
    public void AddRow() => BackingGrid.AddRow();
    public void AddRow(IEnumerable<T> row) => BackingGrid.AddRow(row);
    public void AddRow(T rowItem) => AddRow(Enumerable.Repeat(rowItem, Width).ToList());
    public void AddRow(Func<T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(x => rowItemFunc()).ToList());
    public void AddRow(Func<int, T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc).ToList());

    [Obsolete("Will be removed because it violates nullability contracts. Use AddRowTop(default(T)) instead.")]
    public void AddRowTop()
    {
      BackingGrid.InsertRow(0);
      RowOffset += 1;
    }

    public void AddRowTop(IEnumerable<T> row)
    {
      BackingGrid.InsertRow(0, row);
      RowOffset += 1;
    }

    public void AddRowTop(T rowItem)
    {
      BackingGrid.InsertRow(0, rowItem);
      RowOffset += 1;
    }

    public void AddRowTop(Func<T> rowItemFunc)
    {
      BackingGrid.InsertRow(0, rowItemFunc);
      RowOffset += 1;
    }

    public void AddRowTop(Func<int, T> rowItemFunc)
    {
      BackingGrid.InsertRow(0, rowItemFunc);
      RowOffset += 1;
    }

    public void Clear()
    {
      BackingGrid.Clear();
    }

    public bool Contains(T item) => BackingGrid.Contains(item);

    public IEnumerable<(T Item, IntVector2 Reference)> Flatten()
      => this.SelectMany((r, y) => r.Select((i, x) => (i, GridRef.XY(x - ColumnOffset, y - RowOffset))));

    public IList<T> GetColumn(int index) => BackingGrid.GetColumn(index + ColumnOffset);
    public IEnumerator<IEnumerable<T>> GetColumnEnumerator() => BackingGrid.GetColumnEnumerator();
    public IEnumerator<IEnumerable<T>> GetEnumerator() => BackingGrid.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => BackingGrid.GetEnumerator();
    public IList<T> GetRow(int index) => BackingGrid.GetRow(index + RowOffset);

    public IntVector2? IndexOf(T item)
    {
      IntVector2? rfc = BackingGrid.IndexOf(item);
      if (rfc! != null!)
      {
        return new(rfc.Value.X - ColumnOffset, rfc.Value.Y - RowOffset);
      }
      return null;
    }

    public IntVector2? IndexOfTransposed(T item)
    {
      IntVector2? rfc = BackingGrid.IndexOfTransposed(item);
      if (rfc! != null!)
      {
        return new(rfc.Value.X - ColumnOffset, rfc.Value.Y - RowOffset);
      }
      return null;
    }

    [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumn(before, default(T)) instead.")]
    public void InsertColumn(int before) => BackingGrid.InsertColumn(before);
    public void InsertColumn(int before, IEnumerable<T> column) => BackingGrid.InsertColumn(before, column);
    public void InsertColumn(int before, T columnItem) => BackingGrid.InsertColumn(before, columnItem);
    public void InsertColumn(int before, Func<T> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);
    public void InsertColumn(int before, Func<int, T> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);

    [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumnShiftLeft(before, default(T)) instead.")]
    public void InsertColumnShiftLeft(int before)
    {
      BackingGrid.InsertColumn(before + ColumnOffset);
      ColumnOffset += 1;
    }

    public void InsertColumnShiftLeft(int before, IEnumerable<T> column)
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

    [Obsolete("Will be removed because it violates nullability contracts. Use InsertRow(before, default(T)) instead.")]
    public void InsertRow(int before) => BackingGrid.InsertRow(before);
    public void InsertRow(int before, IEnumerable<T> row) => BackingGrid.InsertRow(before, row);
    public void InsertRow(int before, T rowItem) => BackingGrid.InsertRow(before, rowItem);
    public void InsertRow(int before, Func<T> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);
    public void InsertRow(int before, Func<int, T> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);

    [Obsolete("Will be removed because it violates nullability contracts. Use InsertRowShiftUp(before, default(T)) instead.")]
    public void InsertRowShiftUp(int before)
    {
      BackingGrid.InsertRow(before + RowOffset);
      RowOffset += 1;
    }

    public void InsertRowShiftUp(int before, IEnumerable<T> row)
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

    public bool IsWithinGrid(IntVector2 reference) => reference.Y >= Top && reference.Y < Bottom
      && reference.X >= Left && reference.X < Right;

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