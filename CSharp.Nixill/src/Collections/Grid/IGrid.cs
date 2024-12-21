using System;
using System.Collections.Generic;

namespace Nixill.Collections;

/// <summary>
///   Represents a two-dimensional ordered collection of objects.
/// </summary>
/// <typeparam name="T">The type of elements on the grid.</typeparam>
public interface IGrid<T> : IEnumerable<IEnumerable<T>>
{
  /// <summary>
  ///   Get: The height of the grid (the number of rows it contains).
  /// </summary>
  int Height { get; }

  /// <summary>
  ///   Get: The width of the grid (the number of columns each row
  ///   contains).
  /// </summary>
  int Width { get; }

  /// <summary>
  ///   Get: The size of the grid, which is its height times its width.
  /// </summary>
  int Size { get; }

  /// <summary>
  ///   Get: A collection of the rows, from top to bottom, each of which
  ///   is itself a collection of the values in that row from left to right.
  /// </summary>
  IEnumerable<IEnumerable<T>> Rows { get; }

  /// <summary>
  ///   Get: A collection of the columns, from left to right, each of
  ///   which is itself a collection of the values in that column from
  ///   top to bottom.
  /// </summary>
  IEnumerable<IEnumerable<T>> Columns { get; }

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
  T this[int r, int c] { get; set; }

  /// <summary>
  ///   Get or set: The value at a specified cell.
  /// </summary>
  /// <remarks>
  ///   This is obsolete, as the <see cref="GridReference"/> type itself
  ///   is obsolete. Use <see cref="this[IntVector2]"/> instead.
  /// </remarks>
  /// <param name="gr">The cell to get or set.</param>
  [Obsolete("Use IntVector2 instead.")]
  T this[GridReference gr] { get; set; }

  /// <summary>
  ///   Get or set: The value at a specified cell.
  /// </summary>
  /// <param name="iv2">The cell to get or set.</param>
  T this[IntVector2 iv2] { get; set; }

  /// <summary>
  ///   Get or set: The value at a specified cell.
  /// </summary>
  /// <remarks>
  ///   This is obsolete, because a string reference to the cell is too
  ///   niche to keep on the main Grid object itself. You can use
  ///   <see cref="this[IntVector2]">this</see>[<see cref="GridRef.FromString(string)"/>]
  ///   if you still need the functionality.
  /// </remarks>
  /// <param name="gr"></param>
  [Obsolete("Use GridRef.FromString(str) instead.")]
  T this[string gr] { get; set; }

  /// <summary>
  ///   Adds a new, empty column to the right side of this grid.
  /// </summary>
  /// <remarks>
  ///   This is obsolete as, for a nullable reference type, it forces
  ///   null values into a non-nullable grid. If you need to do that
  ///   anyway (or don't care about nullability constraints), you can
  ///   use <see cref="AddColumn(T)"/> with a null parameter.
  /// </remarks>
  [Obsolete("Will be removed because it violates nullability contracts. Use AddColumn(default(T)) directly instead.")]
  void AddColumn();

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
  void AddColumn(IEnumerable<T> column);

  /// <summary>
  ///   Adds a new column to the right side of this grid with a single value.
  /// </summary>
  /// <param name="columnItem">The item to fill every cell with.</param>
  void AddColumn(T columnItem);

  /// <summary>
  ///   Adds a new column to the right side of this grid with generated
  ///   values.
  /// </summary>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the column to be filled with.
  /// </param>
  void AddColumn(Func<T> columnItemFunc);

  /// <summary>
  ///   Adds a new column to the right side of this grid with generated
  ///   values.
  /// </summary>
  /// <param name="columnItemFunc">
  ///   The function, called once per cell, that receives the row number
  ///   and generates values for the column to be filled with.
  /// </param>
  void AddColumn(Func<int, T> columnItemFunc);

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
  void AddRow();

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
  void AddRow(IEnumerable<T> row);

  /// <summary>
  ///   Adds a new row to the bottom of the grid with a single value.
  /// </summary>
  /// <param name="rowItem">The item to fill every cell with.</param>
  void AddRow(T rowItem);

  /// <summary>
  ///   Adds a new row to the bottom of the grid with generated values.
  /// </summary>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that generates values for
  ///   the row to be filled with.
  /// </param>
  void AddRow(Func<T> rowItemFunc);

  /// <summary>
  ///   Adds a new row to the bottom of the grid with generated values.
  /// </summary>
  /// <param name="rowItemFunc">
  ///   The function, called once per cell, that receives the column
  ///   number and generates values for the row to be filled with.
  /// </param>
  void AddRow(Func<int, T> rowItemFunc);

  /// <summary>Clears the grid, resetting its size to zero.</summary>
  void Clear();

  /// <summary>
  ///   Returns whether or not any cell of this grid contains the given
  ///   value.
  /// </summary>
  /// <param name="item">The value to look for.</param>
  /// <returns><c>true</c> iff any cell contains the value.</returns>
  bool Contains(T item);

  /// <summary>
  ///   Returns all cells, with their reference, in a linear sequence,
  ///   rows first.
  /// </summary>
  /// <returns>The sequence.</returns>
  IEnumerable<(T Item, IntVector2 Reference)> Flatten();

  /// <summary>
  ///   Returns a copy of the specified column as an <see cref="IList{T}"/>.
  /// </summary>
  /// <param name="index">Which column to get.</param>
  /// <returns>The requested column.</returns>
  IList<T> GetColumn(int which);

  /// <summary>
  ///   Returns a columns-first <see cref="IEnumerator{T}"/> over the
  ///   sequence.
  /// </summary>
  /// <returns>The enumerator.</returns>
  IEnumerator<IEnumerable<T>> GetColumnEnumerator();

  /// <summary>
  ///   Returns a copy of the specified row as an <see cref="IList{T}"/>.
  /// </summary>
  /// <param name="index">Which row to get.</param>
  /// <returns>The requested row.</returns>
  IList<T> GetRow(int which);

  /// <summary>
  ///   Returns the reference of the first cell where the specified
  ///   value can be found, enumerating rows-first.
  /// </summary>
  /// <param name="item">The item to find.</param>
  /// <returns>
  ///   The reference to the first cell that contains the item, or
  ///   <c>null</c> if it's not found.
  /// </returns>
  IntVector2? IndexOf(T item);

  /// <summary>
  ///   Returns the reference of the first cell where the specified
  ///   value can be found, enumerating columns-first.
  /// </summary>
  /// <param name="item">The item to find.</param>
  /// <returns>
  ///   The reference to the first cell that contains the item, or
  ///   <c>null</c> if it's not found.
  /// </returns>
  IntVector2? IndexOfTransposed(T item);

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
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumn(before, default(T)) directly instead.")]
  void InsertColumn(int before);

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
  void InsertColumn(int before, IEnumerable<T> column);

  /// <summary>
  ///   Inserts a new column before the specified column of this grid
  ///   with a single value.
  /// </summary>
  /// <param name="before">
  ///   The column before which to insert the new one.
  /// </param>
  /// <param name="columnItem">The item to fill every cell with.</param>
  void InsertColumn(int before, T columnItem);

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
  void InsertColumn(int before, Func<T> columnItemFunc);

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
  void InsertColumn(int before, Func<int, T> columnItemFunc);

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
  ///   use <see cref="AddRow(T)"/> with a null parameter.
  /// </remarks>
  [Obsolete("Will be removed because it violates nullability contracts. Use InsertRow(before, default(T)) directly instead.")]
  void InsertRow(int before);

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
  void InsertRow(int before, IEnumerable<T> row);

  /// <summary>
  ///   Inserts a new row before the specified row of this grid with a
  ///   single value.
  /// </summary>
  /// <param name="before">
  ///   The row before which to insert the new one.
  /// </param>
  /// <param name="rowItem">The item to fill every cell with.</param>
  void InsertRow(int before, T rowItem);

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
  void InsertRow(int before, Func<T> rowItemFunc);

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
  void InsertRow(int before, Func<int, T> rowItemFunc);

  /// <summary>
  ///   Returns whether or not the specified reference falls within this
  ///   grid.
  /// </summary>
  /// <param name="reference">The reference to check.</param>
  /// <returns>
  ///   <c>true</c> iff the specified reference is a cell in this grid.
  /// </returns>
  bool IsWithinGrid(IntVector2 reference);
}
