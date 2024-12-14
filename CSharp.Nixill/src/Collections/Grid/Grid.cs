using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Nixill.Serialization;
using System.Text;
using System.IO;
using Nixill.Utils;

namespace Nixill.Collections
{
  /// <summary>
  ///   Represents a two-dimensional grid of objects.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects contained by this Grid.
  /// </typeparam>
  public class Grid<T> : IGrid<T>
  {
    List<List<T>> BackingList = new List<List<T>>();
    int IntWidth = 0;

    /// <summary>
    ///   Constructs a new 0×0 grid.
    /// </summary>
    public Grid() { }

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
    public Grid(IEnumerable<IEnumerable<T>> list, T filler)
    {
      int widest = 0;

      foreach (IEnumerable<T> innerList in list)
      {
        var newList = new List<T>(innerList);
        if (newList.Count > widest) widest = newList.Count;
        BackingList.Add(new List<T>(innerList));
      }

      foreach (IList<T> innerList in BackingList)
      {
        for (int i = innerList.Count; i < widest; i++)
        {
          innerList.Add(filler);
        }
      }

      IntWidth = widest;
    }

    /// <summary>
    ///   Constructs a new grid from an existing two-dimensional enumerable.
    /// </summary>
    /// <remarks>
    ///   The outer list is taken as rows, and the inner list as columns
    ///   within the row.
    /// </remarks>
    /// <param name="list">The enumerable to copy.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <c>list</c> is not properly rectangular (that is to say, all of
    ///   the sub-enumerables are of a consistent length).
    /// </exception>
    public Grid(IEnumerable<IEnumerable<T>> list)
    {
      IntWidth = 0;
      bool first = true;

      foreach (IEnumerable<T> innerList in list)
      {
        List<T> newList = [.. innerList];

        if (first)
        {
          IntWidth = newList.Count;
          first = false;
        }
        else
        {
          if (IntWidth != newList.Count) throw new ArgumentOutOfRangeException("All sub-enumerables must be the same size.");
        }

        BackingList.Add(newList);
      }
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
    public Grid(int width, int height, T filler)
    {
      IntWidth = width;
      foreach (int r in Enumerable.Range(0, height))
      {
        List<T> innerList = new List<T>();
        foreach (int c in Enumerable.Range(0, width))
        {
          innerList.Add(filler);
        }
        BackingList.Add(innerList);
      }
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
    public Grid(int width, int height, Func<T> filler)
    {
      IntWidth = width;
      foreach (int r in Enumerable.Range(0, height))
      {
        List<T> innerList = [];
        foreach (int c in Enumerable.Range(0, width))
        {
          innerList.Add(filler());
        }
        BackingList.Add(innerList);
      }
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
    public Grid(int width, int height, Func<IntVector2, T> filler)
    {
      IntWidth = width;
      foreach (int r in Enumerable.Range(0, height))
      {
        List<T> innerList = [];
        foreach (int c in Enumerable.Range(0, width))
        {
          innerList.Add(filler(GridRef.RC(r, c)));
        }
      }
    }

    /// <summary>
    ///   Get or set: The value at a specified cell.
    /// </summary>
    /// <param name="iv2">The cell to get or set.</param>
    public T this[IntVector2 iv2]
    {
      get => BackingList[iv2.Y][iv2.X];
      set => BackingList[iv2.Y][iv2.X] = value;
    }

    /// <summary>
    ///   Get or set: The value at a specified cell.
    /// </summary>
    /// <remarks>
    ///   This is obsolete, as the <see cref="GridReference"/> type itself
    ///   is obsolete. Use <see cref="this[IntVector2]"/> instead.
    /// </remarks>
    /// <param name="gr">The cell to get or set.</param>
    [Obsolete("Use IntVector2 instead.")]
    public T this[GridReference gr]
    {
      get => BackingList[gr.Row][gr.Column];
      set
      {
        BackingList[gr.Row][gr.Column] = value;
      }
    }

    /// <summary>
    ///   Get or set: The value at a specified cell.
    /// </summary>
    /// <remarks>
    ///   This is obsolete, because two ints is ambiguous as a cell
    ///   reference (they could be row/column or X/Y). This indexer uses
    ///   the order of row/column, but you should use
    ///   <see cref="this[IntVector2]">this</see>[<see cref="GridRef.RC(int, int)"/>]
    ///   as an unambiguous call (or use the
    ///   <see cref="GridRef.XY(int, int)"/> method if you actually meant
    ///   that!).
    /// </remarks>
    /// <param name="r">The row of the cell to get or set.</param>
    /// <param name="c">The column of the cell to get or set.</param>
    [Obsolete("Use GridRef.RC(r, c) instead.")]
    public T this[int r, int c]
    {
      get => BackingList[r][c];
      set
      {
        BackingList[r][c] = value;
      }
    }

    /// <summary>
    ///   Get or set: The value at a specified cell.
    /// </summary>
    /// <remarks>
    ///   This is obsolete, because a string reference to the cell is too
    ///   niche to keep on the main Grid object itself. You can use
    ///   <see cref="this[IntVector2]">this</see>[<see cref="GridRef.FromString(string)"/>]
    ///   if you still need the functionality.
    /// </remarks>
    /// <param name="gr">The reference to get.</param>
    [Obsolete("Use GridRef.FromString(str) instead.")]
    public T this[string gr]
    {
      get => this[(GridReference)gr];
      set => this[(GridReference)gr] = value;
    }

    /// <summary>
    ///   Get: The height of the grid (the number of rows it contains).
    /// </summary>
    public int Height => BackingList.Count;

    /// <summary>
    ///   Get: The width of the grid (the number of columns each row
    ///   contains).
    /// </summary>
    public int Width => IntWidth;

    /// <summary>
    ///   Get: The size of the grid, which is its height times its width.
    /// </summary>
    public int Size => Height * IntWidth;

    /// <summary>
    ///   Get: A collection of the rows, from top to bottom, each of which
    ///   is itself a collection of the values in that row from left to right.
    /// </summary>
    public IEnumerable<IEnumerable<T>> Rows
    {
      get
      {
        for (int i = 0; i < Height; i++)
        {
          yield return RowEnumerable(i);
        }
      }
    }

    IEnumerable<T> RowEnumerable(int index)
    {
      for (int i = 0; i < Width; i++)
      {
        yield return this[GridRef.RC(index, i)];
      }
    }

    /// <summary>
    ///   Get: A collection of the columns, from left to right, each of
    ///   which is itself a collection of the values in that column from
    ///   top to bottom.
    /// </summary>
    public IEnumerable<IEnumerable<T>> Columns
    {
      get
      {
        for (int i = 0; i < Width; i++)
        {
          yield return ColumnEnumerable(i);
        }
      }
    }

    IEnumerable<T> ColumnEnumerable(int index)
    {
      for (int i = 0; i < Height; i++)
      {
        yield return this[GridRef.RC(i, index)];
      }
    }

    /// <summary>
    ///   Adds a new, empty column to the right side of this grid.
    /// </summary>
    /// <remarks>
    ///   This is obsolete as, for a nullable reference type, it forces
    ///   null values into a non-nullable grid. If you need to do that
    ///   anyway (or don't care about nullability constraints), you can
    ///   use <see cref="AddColumn(T)"/> with a null parameter.
    /// </remarks>
    [Obsolete("Will be removed because it violates nullability constraints. Use AddColumn(default(T)) instead.")]
    public void AddColumn()
    {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList)
      {
        innerList.Add(default(T)!);
      }
    }

    /// <summary>
    ///   Adds a new column with existing data to the right side of this grid.
    /// </summary>
    /// <param name="column">
    ///   The existing data to add. For a non-empty grid, the size of this
    ///   sequence must match the grid's <see cref="Height"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The grid is not empty and the size of the given sequence does
    ///   not match the grid's <see cref="Height"/>.
    /// </exception>
    public void AddColumn(IEnumerable<T> column)
    {
      // For columns, this immediate conversion ensures single enumeration.
      List<T> colList = column.ToList();

      if (Height == 0 && Width == 0)
      {
        IntWidth += 1;
        foreach (T item in column)
        {
          BackingList.Add([item]);
        }
      }
      else
      {
        if (colList.Count != Height) throw new ArgumentException("Column height must match grid height exactly, or grid must be empty.");
        IntWidth += 1;
        for (int i = 0; i < Height; i++)
        {
          BackingList[i].Add(colList[i]);
        }
      }
    }

    /// <summary>
    ///   Adds a new column to the right side of this grid with a single value.
    /// </summary>
    /// <param name="columnItem">The item to fill every cell with.</param>
    public void AddColumn(T columnItem) => AddColumn(Enumerable.Repeat(columnItem, Height));

    /// <summary>
    ///   Adds a new column to the right side of this grid with generated
    ///   values.
    /// </summary>
    /// <param name="columnItemFunc">
    ///   The function, called once per cell, that generates values for
    ///   the column to be filled with.
    /// </param>
    public void AddColumn(Func<T> columnItemFunc) => AddColumn(Sequence.Repeat(columnItemFunc, Height));

    /// <summary>
    ///   Adds a new column to the right side of this grid with generated
    ///   values.
    /// </summary>
    /// <param name="columnItemFunc">
    ///   The function, called once per cell, that receives the row number
    ///   and generates values for the column to be filled with.
    /// </param>
    public void AddColumn(Func<int, T> columnItemFunc) => AddColumn(Enumerable.Range(0, Height).Select(columnItemFunc));

    /// <summary>
    ///   Adds a new, empty row to the bottom of this grid.
    /// </summary>
    /// <remarks>
    ///   This is obsolete as, for a nullable reference type, it forces
    ///   null values into a non-nullable grid. If you need to do that
    ///   anyway (or don't care about nullability constraints), you can
    ///   use <see cref="AddRow(T)"/> with a null parameter.
    /// </remarks>
    [Obsolete("Will be removed because it violates nullability contracts. Use AddRow(default(T)) directly instead.")]
    public void AddRow()
    {
      List<T> innerList = new List<T>();
      while (innerList.Count < IntWidth)
      {
        innerList.Add(default(T)!);
      }
      BackingList.Add(innerList);
    }

    /// <summary>
    ///   Adds a new row with existing data to the bottom of the grid.
    /// </summary>
    /// <param name="row">
    ///   The existing data to add. For a non-empty grid, the size of this
    ///   sequence must match the grid's <see cref="Width"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The grid is not empty and the size of the given sequence does
    ///   not match the grid's <see cref="Width"/>.
    /// </exception>
    public void AddRow(IEnumerable<T> row)
    {
      // For rows, this immediate conversion both ensures single
      // enumeration *and* creates the actual list that will be placed
      // into the grid.
      List<T> rowList = row.ToList();

      if (Height == 0 && Width == 0)
      {
        IntWidth = rowList.Count;
      }
      else
      {
        if (rowList.Count != Width) throw new ArgumentException("Row width must match grid width exactly, or grid must be empty.");
      }

      BackingList.Add(rowList);
    }

    /// <summary>
    ///   Adds a new row to the bottom of the grid with a single value.
    /// </summary>
    /// <param name="rowItem">The item to fill every cell with.</param>
    public void AddRow(T rowItem) => AddRow(Enumerable.Repeat(rowItem, Width));

    /// <summary>
    ///   Adds a new row to the bottom of the grid with generated values.
    /// </summary>
    /// <param name="rowItemFunc">
    ///   The function, called once per cell, that generates values for
    ///   the row to be filled with.
    /// </param>
    public void AddRow(Func<T> rowItemFunc) => AddRow(Sequence.Repeat(rowItemFunc, Width));

    /// <summary>
    ///   Adds a new row to the bottom of the grid with generated values.
    /// </summary>
    /// <param name="rowItemFunc">
    ///   The function, called once per cell, that receives the column
    ///   number and generates values for the row to be filled with.
    /// </param>
    public void AddRow(Func<int, T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc));

    /// <summary>
    ///   Clears the grid, resetting its size to zero.
    /// </summary>
    public void Clear()
    {
      BackingList.Clear();
      IntWidth = 0;
    }

    /// <summary>
    ///   Returns whether or not any cell of this grid contains the given
    ///   value.
    /// </summary>
    /// <param name="item">The value to look for.</param>
    /// <returns><c>true</c> iff any cell contains the value.</returns>
    public bool Contains(T item)
    {
      foreach (List<T> innerList in BackingList)
      {
        if (innerList.Contains(item)) return true;
      }
      return false;
    }

    /// <summary>
    ///   Returns all cells, with their reference, in a linear sequence,
    ///   rows first.
    /// </summary>
    /// <returns>The sequence.</returns>
    public IEnumerable<(T Item, IntVector2 Reference)> Flatten()
      => this.SelectMany((r, y) => r.Select((i, x) => (i, new IntVector2(x, y))));

    /// <summary>
    ///   Returns a copy of the specified column as an <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="index">Which column to get.</param>
    /// <returns>The requested column.</returns>
    public IList<T> GetColumn(int index)
    {
      return ColumnEnumerable(index).ToList();
    }

    /// <summary>
    ///   Returns a columns-first <see cref="IEnumerator{T}"/> over the
    ///   sequence.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<IEnumerable<T>> GetColumnEnumerator() => Columns.GetEnumerator();

    /// <summary>
    ///   Returns a rows-first <see cref="IEnumerator{T}"/> over the sequence.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<IEnumerable<T>> GetEnumerator() => Rows.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///   Returns a copy of the specified row as an <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="index">Which row to get.</param>
    /// <returns>The requested row.</returns>
    public IList<T> GetRow(int index)
    {
      return RowEnumerable(index).ToList();
    }

    /// <summary>
    ///   Returns the reference of the first cell where the specified
    ///   value can be found, enumerating rows-first.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>
    ///   The reference to the first cell that contains the item, or
    ///   <c>null</c> if it's not found.
    /// </returns>
    public IntVector2? IndexOf(T item)
    {
      for (int r = 0; r < Height; r++)
      {
        List<T> innerList = BackingList[r];
        int index = innerList.IndexOf(item);
        if (index != 1) return (index, r);
      }
      return null;
    }

    /// <summary>
    ///   Returns the reference of the first cell where the specified
    ///   value can be found, enumerating columns-first.
    /// </summary>
    /// <param name="item">The item to find.</param>
    /// <returns>
    ///   The reference to the first cell that contains the item, or
    ///   <c>null</c> if it's not found.
    /// </returns>
    public IntVector2? IndexOfTransposed(T item)
    {
      IntVector2? lowIndex = null;
      for (int r = 0; r < Height; r++)
      {
        List<T> innerList = BackingList[r];
        int index = innerList.IndexOf(item);
        if (index == 0) return new(0, r);
        if (index > 0 && (lowIndex! == null! || index < lowIndex.Value.X)) lowIndex = new(index, r);
      }
      return lowIndex;
    }

    /// <summary>
    ///   Inserts a new, empty column before the specified column of this grid.
    /// </summary>
    /// <remarks>
    ///   This is obsolete as, for a nullable reference type, it forces
    ///   null values into a non-nullable grid. If you need to do that
    ///   anyway (or don't care about nullability constraints), you can
    ///   use <see cref="InsertColumn(int, T)"/> with a null second parameter.
    /// </remarks>
    /// <param name="before">
    ///   The column before which to insert the new one.
    /// </param>
    [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumn(before, default(T)) instead.")]
    public void InsertColumn(int before)
    {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList)
      {
        innerList.Insert(before, default(T)!);
      }
    }

    /// <summary>
    ///   Inserts a new column with existing data before the specified
    ///   column of this grid.
    /// </summary>
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
    public void InsertColumn(int before, IEnumerable<T> column)
    {
      IntWidth += 1;
      List<T> colList = column.ToList();
      if (Height != colList.Count) throw new ArgumentException("Column height must match grid height exactly.");
      for (int i = 0; i < Height; i++)
      {
        BackingList[i].Insert(before, colList[i]);
      }
    }

    /// <summary>
    ///   Inserts a new column before the specified column of this grid
    ///   with a single value.
    /// </summary>
    /// <param name="before">
    ///   The column before which to insert the new one.
    /// </param>
    /// <param name="columnItem">The item to fill every cell with.</param>
    public void InsertColumn(int before, T columnItem) => InsertColumn(before, Enumerable.Repeat(columnItem, Height));

    /// <summary>
    ///   Inserts a new column before the specified column of this grid
    ///   with generated values.
    /// </summary>
    /// <param name="before">
    ///   The column before which to insert the new one.
    /// </param>
    /// <param name="columnItemFunc">
    ///   The function, called once per cell, that generates values for
    ///   the column to be filled with.
    /// </param>
    public void InsertColumn(int before, Func<T> columnItemFunc) => InsertColumn(before, Sequence.Repeat(columnItemFunc, Height));

    /// <summary>
    ///   Inserts a new column before the specified column of this grid
    ///   with generated values.
    /// </summary>
    /// <param name="before">
    ///   The column before which to insert the new one.
    /// </param>
    /// <param name="columnItemFunc">
    ///   The function, called once per cell, that receives the row number
    ///   and generates values for the column to be filled with.
    /// </param>
    public void InsertColumn(int before, Func<int, T> columnItemFunc) => InsertColumn(before, Enumerable.Range(0, Height).Select(columnItemFunc));

    /// <summary>
    ///   Inserts a new, empty row before the specified row of this grid.
    /// </summary>
    /// <param name="before">
    ///   The row before which to insert the new one.
    /// </param>
    /// <remarks>
    ///   This is obsolete as, for a nullable reference type, it forces
    ///   null values into a non-nullable grid. If you need to do that
    ///   anyway (or don't care about nullability constraints), you can
    ///   use <see cref="InsertRow(int, T)"/> with a null second parameter.
    /// </remarks>
    [Obsolete("Will be removed because it violates nullability contracts. Use InsertRow(before, default(T)) instead.")]
    public void InsertRow(int before)
    {
      List<T> innerList = new List<T>();
      for (int i = 0; i < IntWidth; i++)
      {
        innerList.Add(default(T)!);
      }
      BackingList.Insert(before, innerList);
    }

    /// <summary>
    ///   Inserts a new row with existing data before the specified row of
    ///   this grid.
    /// </summary>
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
    public void InsertRow(int before, IEnumerable<T> row)
    {
      List<T> rowList = row.ToList();
      if (Width != rowList.Count) throw new ArgumentException("Row width must match grid width exactly.");
      BackingList.Insert(before, rowList);
    }

    /// <summary>
    ///   Inserts a new row before the specified row of this grid with a
    ///   single value.
    /// </summary>
    /// <param name="before">
    ///   The row before which to insert the new one.
    /// </param>
    /// <param name="rowItem">The item to fill every cell with.</param>
    public void InsertRow(int before, T rowItem) => InsertRow(before, Enumerable.Repeat(rowItem, Width));

    /// <summary>
    ///   Inserts a new row before the specified row of this grid with
    ///   generated values.
    /// </summary>
    /// <param name="before">
    ///   The row before which to insert the new one.
    /// </param>
    /// <param name="rowItemFunc">
    ///   The function, called once per cell, that generates values for
    ///   the row to be filled with.
    /// </param>
    public void InsertRow(int before, Func<T> rowItemFunc) => InsertRow(before, Sequence.Repeat(rowItemFunc, Width));

    /// <summary>
    ///   Inserts a new row before the specified row of this grid with
    ///   generated values.
    /// </summary>
    /// <param name="before">
    ///   The row before which to insert the new one.
    /// </param>
    /// <param name="rowItemFunc">
    ///   The function, called once per cell, that receives the column
    ///   number and generates values for the row to be filled with.
    /// </param>
    public void InsertRow(int before, Func<int, T> rowItemFunc) => InsertRow(before, Enumerable.Range(0, Width).Select(rowItemFunc));

    /// <summary>
    ///   Returns whether or not the specified reference falls within this
    ///   grid.
    /// </summary>
    /// <param name="reference">The reference to check.</param>
    /// <returns>
    ///   <c>true</c> iff the specified reference is a cell in this grid.
    /// </returns>
    public bool IsWithinGrid(IntVector2 reference) => reference.Y >= 0 && reference.Y < Height && reference.X >= 0 && reference.X < Width;

    /// <summary>
    ///   Removes the specified column from this grid.
    /// </summary>
    /// <param name="col">The column to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The column is not within the width of this grid.
    /// </exception>
    public void RemoveColumnAt(int col)
    {
      if (col < 0 || col >= Width) throw new ArgumentOutOfRangeException("Can only remove existing columns.");
      IntWidth -= 1;
      foreach (var list in BackingList)
      {
        list.RemoveAt(col);
      }
    }

    /// <summary>
    ///   Removes the specified row from this grid.
    /// </summary>
    /// <param name="row">The row to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   The row is not within the height of this grid.
    /// </exception>
    public void RemoveRowAt(int row)
    {
      if (row < 0 || row >= Height) throw new ArgumentOutOfRangeException("Can only remove existing rows.");
      BackingList.RemoveAt(row);
    }

    /// <summary>
    ///   Returns an enumerable over each row of a grid as CSV-escaped
    ///   strings.
    /// </summary>
    /// <param name="input">The grid to output.</param>
    /// <returns>The enumerable.</returns>
    public IEnumerable<string> GridToStringEnumerable()
    {
      foreach (IEnumerable<T?> line in this)
      {
        StringBuilder ret = new StringBuilder();
        foreach (T? obj in line)
        {
          ret.Append("," + CSVParser.CSVEscape(obj?.ToString() ?? ""));
        }
        if (ret.Length > 0) ret.Remove(0, 1);
        yield return ret.ToString();
      }
    }

    /// <summary>
    ///   Converts a grid to a CSV string.
    /// </summary>
    /// <param name="input">The grid to convert.</param>
    /// <returns>The grid as a CSV string.</returns>
    public string Serialize()
    {
      StringBuilder ret = new StringBuilder();
      foreach (string line in GridToStringEnumerable())
      {
        ret.Append('\n' + line);
      }
      if (ret.Length > 0) ret.Remove(0, 1);
      return ret.ToString();
    }

    /// <summary>
    ///   Converts a grid to a csv string and writes it to a file.
    /// </summary>
    /// <param name="input">The grid to output.</param>
    /// <param name="file">The file to write to.</param>
    public void SerializeToFile(string file)
    {
      using (StreamWriter writer = new StreamWriter(file))
      {
        foreach (string line in GridToStringEnumerable())
        {
          writer.WriteLine(line);
        }
      }
    }
  }

  /// <summary>
  ///   Static methods for parsing a grid.
  /// </summary>
  public static class Grid
  {
    /// <summary>
    ///   Reads a CSV file into a Grid of strings.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>The read grid.</returns>
    public static Grid<string> DeserializeFromFile(string path)
    {
      return Deserialize(FileUtils.FileCharEnumerator(path));
    }

    /// <summary>
    ///   Reads a CSV stream into a Grid of strings.
    /// </summary>
    /// <param name="reader">The StreamReader to read from.</param>
    /// <returns>The read grid.</returns>
    public static Grid<string> Deserialize(StreamReader reader)
    {
      return Deserialize(FileUtils.StreamCharEnumerator(reader));
    }

    /// <summary>
    ///   Reads a char enumerator and converts the streamed chars into a
    ///   Grid of strings.
    /// </summary>
    /// <param name="input">The input stream to read.</param>
    /// <returns>The read grid.</returns>
    public static Grid<string> Deserialize(IEnumerable<char> input)
    // => new Grid<string>(CSVParser.EnumerableToRows(input));
    {
      List<IList<string>> backingList = new List<IList<string>>();

      foreach (IList<string> innerList in CSVParser.EnumerableToRows(input))
      {
        backingList.Add(innerList);
      }

      return new Grid<string>(backingList);
    }
  }
}
