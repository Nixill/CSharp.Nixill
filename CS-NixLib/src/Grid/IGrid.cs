using System.Collections;
using System.Collections.Generic;

namespace Nixill.Grid {
  /// <summary>
  /// Represents a two-dimensional ordered collection of objects.
  /// </summary>
  /// <typeparam name="T">The type of elements on the grid.</typeparam>
  public interface IGrid<T> {
    /// <summary>The height - number of rows - of the IGrid.</summary>
    int Height { get; }

    /// <summary>The width - number of columns - of the IGrid.</summary>
    int Width { get; }

    /// <summary>The size - width times height - of the IGrid.</summary>
    int Size { get; }

    /// <summary>The value of a single cell in the grid.</summary>
    /// <param name="r">The row of the cell for which to get a
    /// value.</param>
    /// <param name="c">The column of the cell for which to get a
    /// value.</param>
    T this[int r, int c] { get; set; }

    /// <summary>The value of a single cell in the grid.</summary>
    /// <param name="gr">The cell for which to get a value.</param>
    T this[GridReference gr] { get; set; }

    /// <summary>Adds an empty column to the right of the grid.</summary>
    void AddColumn();

    /// <summary>
    /// Adds a column to the right of the grid.
    ///
    /// The height of the grid must match the size of the list, or the
    /// grid must be empty.
    /// </summary>
    /// <param name="column">The row to add.</param>
    void AddColumn<U>(IList<U> column) where U : T;

    /// <summary>Adds an empty row to the bottom of the grid.</summary>
    void AddRow();

    /// <summary>
    /// Adds a row to the bottom of the grid.
    ///
    /// The width of the grid must match the size of the list, or the
    /// grid must be empty.
    /// </summary>
    /// <param name="row">The row to add.</param>
    void AddRow<U>(IList<U> row) where U : T;

    /// <summary>Clears all values of the grid.</summary>
    void Clear();

    /// <summary>Checks whether the grid contains a given item.</summary>
    /// <param name="item">The item to check for presence.</param>
    bool Contains(T item);

    /// <summary>
    /// Returns a single column as a subclass of IList.
    /// </summary>
    IList<T> GetColumn();

    /// <summary>
    /// Returns an enumerator through the columns of a grid.
    /// </summary>
    IEnumerator<U> GetColumnEnumerator<U>() where U : IList<T>;

    /// <summary>
    /// Returns an enumerator through the rows of a grid.
    /// </summary>
    IEnumerator<U> GetEnumerator<U>() where U : IList<T>;

    /// <summary>
    /// Returns a single column as a subclass of IList.
    /// </summary>
    IList<T> GetRow();

    /// <summary>
    /// Returns the first index of a particular item.
    ///
    /// This method checks each row, left to right, in order from top to
    /// bottom.
    /// </summary>
    /// <param name="item">The item to check for location.</param>
    GridReference IndexOf(T item);

    /// <summary>
    /// Returns the first index of a particular item.
    ///
    /// This method checks each column, top to bottom, in order from left
    /// to right.
    /// </summary>
    /// <param name="item">The item to check for location.</param>
    GridReference IndexOfTransposed(T item);

    /// <summary>
    /// Inserts an empty column in the middle of the grid.
    /// </summary>
    /// <param name="before">The existing column to the left of which the
    /// new column should be placed.</param>
    void InsertColumn(int before);

    /// <summary>
    /// Inserts a column in the middle of the grid.
    ///
    /// The height of the grid must match the size of the list.
    /// </summary>
    /// <param name="before">The existing column to the left of which the
    /// new column should be placed.</param>
    /// <param name="column">The column to add.</param>
    void InsertColumn<U>(int before, IList<U> column) where U : T;

    /// <summary>Inserts an empty row in the middle of the grid.</summary>
    /// <param name="before">The existing row above which the new row
    /// should be placed.</param>
    void InsertRow(int before);

    /// <summary>
    /// Inserts a row in the middle of the grid.
    ///
    /// The width of the grid must match the size of the list.
    /// </summary>
    /// <param name="before">The existing row above which the new row
    /// should be placed.</param>
    /// <param name="row">The row to add.</param>
    void InsertRow<U>(int before, IList<U> row) where U : T;
  }
}