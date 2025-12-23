using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Nixill.Objects;

namespace Nixill.Collections;

public class OffsetGrid<T> : IGrid<T>
{
  Grid<T> BackingGrid;

  // To be clear: Positive values here mean that GridReference (0, 0)
  // refers to a cell below and right of the top-left cell of the
  // backing grid.
  int RowOffset = 0;
  int ColumnOffset = 0;

  /// <summary>
  ///   The index of the topmost row in the grid.
  /// </summary>
  public int Top => -RowOffset;

  /// <summary>
  ///   The index of the leftmost column of the grid.
  /// </summary>
  public int Left => -ColumnOffset;

  /// <summary>
  ///   The index of the bottom-most row in the grid.
  /// </summary>
  public int Bottom => Top + Height;

  /// <summary>
  ///   The index of the rightmost column in the grid.
  /// </summary>
  public int Right => Left + Width;

  /// <summary>
  ///   Constructs a new 0×0 grid.
  /// </summary>
  public OffsetGrid()
  {
    BackingGrid = new();
  }

  /// <summary>
  ///   Constructs a new grid from an existing two-dimensional enumerable.
  /// </summary>
  /// <remarks>
  ///   The outer list is taken as rows, and the inner list as columns
  ///   within the row.
  /// </remarks>
  /// <param name="list">The enumerable to copy.</param>
  /// <param name="rowOffset">
  ///   Which row of the backing grid should be row 0 of this
  ///   OffsetGrid. Need not necessarily be a row that actually exists
  ///   within the backing grid.
  /// </param>
  /// <param name="colOffset">
  ///   Which column of the backing grid should be column 0 of this
  ///   OffsetGrid. Need not necessarily be a column that actually
  ///   exists within the backing grid.
  /// </param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   <c>list</c> is not properly rectangular (that is to say, all of
  ///   the sub-enumerables are of a consistent length).
  /// </exception>
  public OffsetGrid(IEnumerable<IEnumerable<T>> list, int rowOffset = 0, int colOffset = 0)
  {
    BackingGrid = new(list);
    RowOffset = rowOffset;
    ColumnOffset = colOffset;
  }

  /// <summary>
  ///   Constructs a new grid from an existing two-dimensional enumerable.
  /// </summary>
  /// <remarks>
  ///   The outer list is taken as rows, and the inner list as columns
  ///   within the row.
  ///   <para />
  ///   The <c>filler</c> is used if the enumerable is not strictly
  ///   rectangular.
  /// </remarks>
  /// <param name="list">The enumerable to copy.</param>
  /// <param name="filler">Filler for gaps in the enumerables.</param>
  public OffsetGrid(IEnumerable<IEnumerable<T>> list, T filler, int rowOffset = 0, int colOffset = 0)
  {
    BackingGrid = new(list, filler);
    RowOffset = rowOffset;
    ColumnOffset = colOffset;
  }

  /// <summary>
  ///   Constructs a new grid of a specified size with a given value in
  ///   every cell.
  /// </summary>
  /// <param name="width">The width of the grid.<param>
  /// <param name="height">The height of the grid.</param>
  /// <param name="filler">
  ///   The contents of every cell of the grid.
  /// </param>
  public OffsetGrid(int width, int height, T filler, int rowOffset = 0, int colOffset = 0)
  {
    BackingGrid = new(width, height, filler);
    RowOffset = rowOffset;
    ColumnOffset = colOffset;
  }

  /// <summary>
  ///   Constructs a new grid of a specified size, with a given function
  ///   generating a value for every cell.
  /// </summary>
  /// <param name="width">The width of the grid.</param>
  /// <param name="height">The height of the grid.</param>
  /// <param name="filler">
  ///   A function evaluated for every cell in the grid, which returns
  ///   the value that should be inserted into that cell.
  /// </param>
  public OffsetGrid(int width, int height, Func<T> filler, int rowOffset = 0, int colOffset = 0)
  {
    BackingGrid = new(width, height, filler);
    RowOffset = rowOffset;
    ColumnOffset = colOffset;
  }

  /// <summary>
  ///   Constructs a new grid of a specified size, with a given function
  ///   turning a cell's position into a value for every cell.
  /// </summary>
  /// <param name="width">The width of the grid.</param>
  /// <param name="height">The height of the grid.</param>
  /// <param name="filler">
  ///   A function evaluated for every cell in the grid, which receives
  ///   that cell's position and returns the value that should be
  ///   inserted into that cell.
  /// </param>
  public OffsetGrid(int width, int height, Func<IntVector2, T> filler, int rowOffset = 0, int colOffset = 0)
  {
    BackingGrid = new(width, height, filler);
    RowOffset = rowOffset;
    ColumnOffset = colOffset;
  }

  /// <inheritdoc/>
  public T this[IntVector2 gr]
  {
    get => BackingGrid[gr - (RowOffset, ColumnOffset)];
    set => BackingGrid[gr - (RowOffset, ColumnOffset)] = value;
  }

  /// <inheritdoc/>
  [Obsolete("Use IntVector2 instead.")]
  public T this[GridReference gr]
  {
    get => this[(IntVector2)gr];
    set => this[(IntVector2)gr] = value;
  }

  /// <inheritdoc/>
  [Obsolete("Use GridRef.RC(r, c) instead.")]
  public T this[int r, int c]
  {
    get => this[GridRef.RC(r, c)];
    set => this[GridRef.RC(r, c)] = value;
  }

  /// <remarks>
  ///   This is obsolete, because string references only work for
  ///   positive coordinates in a Grid.
  /// </remarks>
  /// <inheritdoc/>
  [Obsolete("A1 or R1C1 format is not recommended for OffsetGrids.")]
  public T this[string gr]
  {
    get => this[GridRef.FromString(gr)];
    set => this[GridRef.FromString(gr)] = value;
  }

  /// <inheritdoc/>
  public int Height => BackingGrid.Height;

  /// <inheritdoc/>
  public int Width => BackingGrid.Width;

  /// <inheritdoc/>
  public int Size => BackingGrid.Size;

  /// <inheritdoc/>
  public IEnumerable<IEnumerable<T>> Rows => BackingGrid.Rows;

  /// <inheritdoc/>
  public IEnumerable<IEnumerable<T>> Columns => BackingGrid.Columns;

  /// <inheritdoc/>
  [Obsolete("Will be removed because it violates nullability contracts. Use AddColumn(default(T)) instead.")]
  public void AddColumn() => BackingGrid.AddColumn();

  /// <inheritdoc/>
  public void AddColumn(IEnumerable<T> column) => BackingGrid.AddColumn(column);

  /// <inheritdoc/>
  public void AddColumn(T columnItem) => BackingGrid.AddColumn(columnItem);

  /// <inheritdoc/>
  public void AddColumn(Func<T> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);

  /// <inheritdoc/>
  public void AddColumn(Func<int, T> columnItemFunc) => BackingGrid.AddColumn(columnItemFunc);

  /// <summary>
  ///   Adds a new column with existing data to the left side of this grid.
  /// </summary>
  /// <remarks>
  ///   This is obsolete as, for a nullable reference type, it forces
  ///   null values into a non-nullable grid. If you need to do that
  ///   anyway (or don't care about nullability constraints), you can
  ///   use <see cref="AddColumnLeft(T)"/> with a null parameter.
  /// </remarks>
  [Obsolete("Will be removed because it violates nullability contracts. Use AddColumnLeft(default(T)) instead.")]
  public void AddColumnLeft()
  {
    BackingGrid.InsertColumn(0);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Adds a new column with existing data to the left side of this grid.
  /// </summary>
  /// <param name="column">
  ///   The existing data to add. For a non-empty grid, the size of this
  ///   sequence must match the grid's <see cref="Height"/>.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   The grid is not empty and the size of the given sequence does
  ///   not match the grid's <see cref="Height"/>.
  /// </exception>
  public void AddColumnLeft(IEnumerable<T> column)
  {
    BackingGrid.InsertColumn(0, column);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Adds a new column to the left side of this grid with a single value.
  /// </summary>
  /// <param name="columnItem">The item to fill every cell with.</param>
  public void AddColumnLeft(T columnItem)
  {
    BackingGrid.InsertColumn(0, columnItem);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Adds a new column to the left side of this grid with generated
  ///   values.
  /// </summary>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the column to be filled with.
  /// </param>
  public void AddColumnLeft(Func<T> columnItemFunc)
  {
    BackingGrid.InsertColumn(0, columnItemFunc);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Adds a new column to the right side of this grid with generated
  ///   values.
  /// </summary>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that receives the row number
  ///   and generates values for the column to be filled with.
  /// </param>
  public void AddColumnLeft(Func<int, T> columnItemFunc)
  {
    BackingGrid.InsertColumn(0, columnItemFunc);
    ColumnOffset += 1;
  }

  [Obsolete("Will be removed because it violates nullability contracts. Use AddRow(default(T)) instead.")]
  /// <inheritdoc/>
  public void AddRow() => BackingGrid.AddRow();

  /// <inheritdoc/>
  public void AddRow(IEnumerable<T> row) => BackingGrid.AddRow(row);

  /// <inheritdoc/>
  public void AddRow(T rowItem) => AddRow(Enumerable.Repeat(rowItem, Width).ToList());

  /// <inheritdoc/>
  public void AddRow(Func<T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(x => rowItemFunc()).ToList());

  /// <inheritdoc/>
  public void AddRow(Func<int, T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc).ToList());

  /// <summary>
  ///   Adds a new, empty row to the top of this grid.
  /// </summary>
  /// <remarks>
  ///   This is obsolete as, for a nullable reference type, it forces
  ///   null values into a non-nullable grid. If you need to do that
  ///   anyway (or don't care about nullability constraints), you can
  ///   use <see cref="AddRowTop(T)"/> with a null parameter.
  /// </remarks>
  [Obsolete("Will be removed because it violates nullability contracts. Use AddRowTop(default(T)) instead.")]
  public void AddRowTop()
  {
    BackingGrid.InsertRow(0);
    RowOffset += 1;
  }

  /// <summary>
  ///   Adds a new row with existing data to the top of the grid.
  /// </summary>
  /// <param name="row">
  ///   The existing data to add. For a non-empty grid, the size of this
  ///   sequence must match the grid's <see cref="Width"/>.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   The grid is not empty and the size of the given sequence does
  ///   not match the grid's <see cref="Width"/>.
  /// </exception>
  public void AddRowTop(IEnumerable<T> row)
  {
    BackingGrid.InsertRow(0, row);
    RowOffset += 1;
  }

  /// <summary>
  ///   Adds a new row to the top of the grid with a single value.
  /// </summary>
  /// <param name="rowItem">The item to fill every cell with.</param>
  public void AddRowTop(T rowItem)
  {
    BackingGrid.InsertRow(0, rowItem);
    RowOffset += 1;
  }

  /// <summary>
  ///   Adds a new row to the bottom of the grid with generated values.
  /// </summary>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the row to be filled with.
  /// </param>
  public void AddRowTop(Func<T> rowItemFunc)
  {
    BackingGrid.InsertRow(0, rowItemFunc);
    RowOffset += 1;
  }

  /// <summary>
  ///   Adds a new row to the bottom of the grid with generated values.
  /// </summary>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that receives the column
  ///   number and generates values for the row to be filled with.
  /// </param>
  public void AddRowTop(Func<int, T> rowItemFunc)
  {
    BackingGrid.InsertRow(0, rowItemFunc);
    RowOffset += 1;
  }

  /// <inheritdoc/>
  public void Clear()
  {
    BackingGrid.Clear();
  }

  /// <inheritdoc/>
  public bool Contains(T item) => BackingGrid.Contains(item);

  /// <inheritdoc/>
  public IEnumerable<(IntVector2 Reference, T Item)> Flatten()
    => this.SelectMany((r, y) => r.Select((i, x) => (GridRef.XY(x - ColumnOffset, y - RowOffset), i)));

  /// <inheritdoc/>
  public IList<T> GetColumn(int index) => BackingGrid.GetColumn(index + ColumnOffset);

  /// <inheritdoc/>
  public IEnumerator<IEnumerable<T>> GetColumnEnumerator() => BackingGrid.GetColumnEnumerator();

  /// <inheritdoc/>
  public IEnumerator<IEnumerable<T>> GetEnumerator() => BackingGrid.GetEnumerator();

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => BackingGrid.GetEnumerator();

  /// <inheritdoc/>
  public IList<T> GetRow(int index) => BackingGrid.GetRow(index + RowOffset);

  /// <inheritdoc/>
  public IntVector2? IndexOf(T item)
  {
    IntVector2? rfc = BackingGrid.IndexOf(item);
    if (rfc! != null!)
    {
      return new(rfc.Value.X - ColumnOffset, rfc.Value.Y - RowOffset);
    }
    return null;
  }

  /// <inheritdoc/>
  public IntVector2? IndexOfTransposed(T item)
  {
    IntVector2? rfc = BackingGrid.IndexOfTransposed(item);
    if (rfc! != null!)
    {
      return new(rfc.Value.X - ColumnOffset, rfc.Value.Y - RowOffset);
    }
    return null;
  }

  /// <inheritdoc/>
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumn(before, default(T)) instead.")]
  public void InsertColumn(int before) => BackingGrid.InsertColumn(before);

  /// <inheritdoc/>
  public void InsertColumn(int before, IEnumerable<T> column) => BackingGrid.InsertColumn(before, column);

  /// <inheritdoc/>
  public void InsertColumn(int before, T columnItem) => BackingGrid.InsertColumn(before, columnItem);

  /// <inheritdoc/>
  public void InsertColumn(int before, Func<T> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);

  /// <inheritdoc/>
  public void InsertColumn(int before, Func<int, T> columnItemFunc) => BackingGrid.InsertColumn(before, columnItemFunc);

  /// <summary>
  ///   Inserts a new, empty column before the specified column of this
  ///   grid, and shifts all columns left.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the newly added column retain the same
  ///   index as before this method is called.
  ///   <para/>
  ///   This is obsolete as, for a nullable reference type, it forces
  ///   null values into a non-nullable grid. If you need to do that
  ///   anyway (or don't care about nullability constraints), you can
  ///   use <see cref="InsertColumn(int, T)"/> with a null second parameter.
  /// </remarks>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumnShiftLeft(before, default(T)) instead.")]
  public void InsertColumnShiftLeft(int before)
  {
    BackingGrid.InsertColumn(before + ColumnOffset);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Inserts a new column with existing data before the specified
  ///   column of this grid, and shifts all columns left.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the newly added column retain the same
  ///   index as before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  /// <param name="column">
  ///   The existing data to add. For a non-empty grid, the size of this
  ///   sequence must match the grid's <see cref="Height"/>.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   The grid is not empty and the size of the given sequence does
  ///   not match the grid's <see cref="Height"/>.
  /// </exception>
  public void InsertColumnShiftLeft(int before, IEnumerable<T> column)
  {
    BackingGrid.InsertColumn(before + ColumnOffset, column);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Inserts a new column before the specified column of this grid
  ///   with a single value, and shifts all columns left.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the newly added column retain the same
  ///   index as before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  /// <param name="columnItem">The item to fill every cell with.</param>
  public void InsertColumnShiftLeft(int before, T columnItem)
  {
    BackingGrid.InsertColumn(before + ColumnOffset, columnItem);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Inserts a new column before the specified column of this grid
  ///   with generated values, and shifts all columns left.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the newly added column retain the same
  ///   index as before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the column to be filled with.
  /// </param>
  public void InsertColumnShiftLeft(int before, Func<T> columnItemFunc)
  {
    BackingGrid.InsertColumn(before + ColumnOffset, columnItemFunc);
    ColumnOffset += 1;
  }

  /// <summary>
  ///   Inserts a new column before the specified column of this grid
  ///   with generated values, and shifts all columns left.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the newly added column retain the same
  ///   index as before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that receives the row number
  ///   and generates values for the column to be filled with.
  /// </param>
  public void InsertColumnShiftLeft(int before, Func<int, T> columnItemFunc)
  {
    BackingGrid.InsertColumn(before + ColumnOffset, columnItemFunc);
    ColumnOffset += 1;
  }

  /// <inheritdoc/>
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertRow(before, default(T)) instead.")]
  public void InsertRow(int before) => BackingGrid.InsertRow(before);

  /// <inheritdoc/>
  public void InsertRow(int before, IEnumerable<T> row) => BackingGrid.InsertRow(before, row);

  /// <inheritdoc/>
  public void InsertRow(int before, T rowItem) => BackingGrid.InsertRow(before, rowItem);

  /// <inheritdoc/>
  public void InsertRow(int before, Func<T> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);

  /// <inheritdoc/>
  public void InsertRow(int before, Func<int, T> rowItemFunc) => BackingGrid.InsertRow(before, rowItemFunc);

  /// <summary>
  ///   Inserts a new, empty row before the specified row of this grid,
  ///   and shifts all rows up.
  /// </summary>
  /// <remarks>
  ///   Rows below the newly added row retain the same index as
  ///   before this method is called.
  /// <para/>
  ///   This is obsolete as, for a nullable reference type, it forces
  ///   null values into a non-nullable grid. If you need to do that
  ///   anyway (or don't care about nullability constraints), you can
  ///   use <see cref="InsertRow(int, T)"/> with a null second parameter.
  /// </remarks>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertRowShiftUp(before, default(T)) instead.")]
  public void InsertRowShiftUp(int before)
  {
    BackingGrid.InsertRow(before + RowOffset);
    RowOffset += 1;
  }

  /// <summary>
  ///   Inserts a new row with existing data before the specified row of
  ///   this grid.
  /// </summary>
  /// <remarks>
  ///   Rows below the newly added row retain the same index as
  ///   before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  /// <param name="row">
  ///   The existing data to add. For a non-empty grid, the size of this
  ///   sequence must match the grid's <see cref="Width"/>.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   The grid is not empty and the size of the given sequence does
  ///   not match the grid's <see cref="Width"/>.
  /// </exception>
  public void InsertRowShiftUp(int before, IEnumerable<T> row)
  {
    BackingGrid.InsertRow(before + RowOffset, row);
    RowOffset += 1;
  }

  /// <summary>
  ///   Inserts a new row before the specified row of this grid with a
  ///   single value.
  /// </summary>
  /// <remarks>
  ///   Rows below the newly added row retain the same index as
  ///   before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  /// <param name="rowItem">The item to fill every cell with.</param>
  public void InsertRowShiftUp(int before, T rowItem)
  {
    BackingGrid.InsertRow(before + RowOffset, rowItem);
    RowOffset += 1;
  }

  /// <summary>
  ///   Inserts a new row before the specified row of this grid with
  ///   generated values.
  /// </summary>
  /// <remarks>
  ///   Rows below the newly added row retain the same index as
  ///   before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the row to be filled with.
  /// </param>
  public void InsertRowShiftUp(int before, Func<T> rowItemFunc)
  {
    BackingGrid.InsertRow(before + RowOffset, rowItemFunc);
    RowOffset += 1;
  }

  /// <summary>
  ///   Inserts a new row before the specified row of this grid with
  ///   generated values.
  /// </summary>
  /// <remarks>
  ///   Rows below the newly added row retain the same index as
  ///   before this method is called.
  /// </remarks>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that receives the column
  ///   number and generates values for the row to be filled with.
  /// </param>
  public void InsertRowShiftUp(int before, Func<int, T> rowItemFunc)
  {
    BackingGrid.InsertRow(before + RowOffset, rowItemFunc);
    RowOffset += 1;
  }

  /// <inheritdoc/>
  public bool IsWithinGrid(IntVector2 reference) => reference.Y >= Top && reference.Y < Bottom
    && reference.X >= Left && reference.X < Right;

  /// <summary>
  ///   Removes the specified column from this grid.
  /// </summary>
  /// <param name="col">The column to remove.</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The column is not within the width of this grid.
  /// </exception>
  public void RemoveColumnAt(int col) => BackingGrid.RemoveColumnAt(col + ColumnOffset);

  /// <summary>
  ///   Removes the specified column from this grid and shifts all
  ///   columns to the right.
  /// </summary>
  /// <remarks>
  ///   Columns to the right of the removed column retain the same index
  ///   as before this method is called.
  /// </remarks>
  /// <param name="col">The column to remove.</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The column is not within the width of this grid.
  /// </exception>
  public void RemoveColumnShiftRight(int col)
  {
    BackingGrid.RemoveColumnAt(col + ColumnOffset);
    ColumnOffset -= 1;
  }

  /// <inheritdoc/>
  public void RemoveRowAt(int row) => BackingGrid.RemoveRowAt(row + RowOffset);

  /// <summary>
  ///   Removes the specified row from this grid and shifts all rows up.
  /// </summary>
  /// <remarks>
  ///   Rows below the removed row retain the same index as before this
  ///   method is called.
  /// </remarks>
  /// <param name="row">The row to remove.</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The row is not within the height of this grid.
  /// </exception>
  public void RemoveRowShiftDown(int row)
  {
    BackingGrid.RemoveRowAt(row + RowOffset);
    RowOffset -= 1;
  }
}
