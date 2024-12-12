using System.Collections;

namespace Nixill.Collections;

internal class TransposedGrid<T> : IGrid<T>
{
  internal IGrid<T> BackingGrid;

  public TransposedGrid(IGrid<T> grid)
  {
    BackingGrid = grid;
  }

  public T? this[GridReference gr] { get => BackingGrid[gr.Transposed]; set => BackingGrid[gr.Transposed] = value; }
  public T? this[string gr] { get => BackingGrid[((GridReference)gr).Transposed]; set => BackingGrid[((GridReference)gr).Transposed] = value; }
  public T? this[int r, int c] { get => BackingGrid[c, r]; set => BackingGrid[c, r] = value; }

  public int Height => BackingGrid.Width;
  public int Width => BackingGrid.Height;
  public int Size => BackingGrid.Size;

  public IEnumerable<IEnumerable<T?>> Rows => BackingGrid.Columns;
  public IEnumerable<IEnumerable<T?>> Columns => BackingGrid.Rows;

  public void AddColumn() => BackingGrid.AddRow();
  public void AddColumn<U>(IEnumerable<U?> column) where U : T => BackingGrid.AddRow(column);
  public void AddColumn(T? columnItem) => BackingGrid.AddRow(columnItem);
  public void AddColumn(Func<T?> columnItemFunc) => BackingGrid.AddRow(columnItemFunc);
  public void AddColumn(Func<int, T?> columnItemFunc) => BackingGrid.AddRow(columnItemFunc);

  public void AddRow() => BackingGrid.AddColumn();
  public void AddRow<U>(IEnumerable<U?> row) where U : T => BackingGrid.AddColumn(row);
  public void AddRow(T? rowItem) => BackingGrid.AddColumn(rowItem);
  public void AddRow(Func<T?> rowItemFunc) => BackingGrid.AddColumn(rowItemFunc);
  public void AddRow(Func<int, T?> rowItemFunc) => BackingGrid.AddColumn(rowItemFunc);

  public void Clear() => BackingGrid.Clear();
  public bool Contains(T item) => BackingGrid.Contains(item);

  public IEnumerable<(T? Item, GridReference Reference)> Flatten()
    => BackingGrid
      .Flatten()
      .OrderBy(t => t.Reference.Column)
      .ThenBy(t => t.Reference.Row)
      .Select(t => (t.Item, t.Reference.Transposed));

  public IList<T?> GetColumn(int which) => BackingGrid.GetRow(which);

  public IEnumerator<IEnumerable<T?>> GetColumnEnumerator() => BackingGrid.GetEnumerator();
  public IEnumerator<IEnumerable<T?>> GetEnumerator() => BackingGrid.GetColumnEnumerator();

  public IList<T?> GetRow(int which) => BackingGrid.GetColumn(which);

  public GridReference? IndexOf(T? item) => BackingGrid.IndexOfTransposed(item)?.Transposed;
  public GridReference? IndexOfTransposed(T? item) => BackingGrid.IndexOf(item)?.Transposed;

  public void InsertColumn(int before) => BackingGrid.InsertRow(before);
  public void InsertColumn<U>(int before, IEnumerable<U?> column) where U : T => BackingGrid.InsertRow(before, column);
  public void InsertColumn(int before, T? columnItem) => BackingGrid.InsertRow(before, columnItem);
  public void InsertColumn(int before, Func<T?> columnItemFunc) => BackingGrid.InsertRow(before, columnItemFunc);
  public void InsertColumn(int before, Func<int, T?> columnItemFunc) => BackingGrid.InsertRow(before, columnItemFunc);

  public void InsertRow(int before) => BackingGrid.InsertColumn(before);
  public void InsertRow<U>(int before, IEnumerable<U?> row) where U : T => BackingGrid.InsertColumn(before, row);
  public void InsertRow(int before, T? rowItem) => BackingGrid.InsertColumn(before, rowItem);
  public void InsertRow(int before, Func<T?> rowItemFunc) => BackingGrid.InsertColumn(before, rowItemFunc);
  public void InsertRow(int before, Func<int, T?> rowItemFunc) => BackingGrid.InsertColumn(before, rowItemFunc);

  public bool IsWithinGrid(GridReference reference) => BackingGrid.IsWithinGrid(reference.Transposed);

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}