using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections.Grid {
  public class Grid<T> : IGrid<T> {
    List<List<T>> BackingList = new List<List<T>>();
    int IntWidth = 0;

    /// <summary>
    /// Creates a new 0×0 grid.
    /// </summary>
    public Grid() { }

    /// <summary>
    /// Creates a new grid from an existing list of lists.
    ///
    /// The outer list is taken as rows, and the inner list as columns
    /// within the row.
    /// </summary>
    public Grid(IEnumerable<IEnumerable<T>> list) {
      int widest = 0;
      foreach (IEnumerable<T> innerList in list) {
        var newList = new List<T>(innerList);
        if (newList.Count > widest) widest = newList.Count;
        BackingList.Add(new List<T>(innerList));
      }

      foreach (IList<T> innerList in BackingList) {
        for (int i = innerList.Count; i < widest; i++) {
          innerList.Add(default(T));
        }
      }

      IntWidth = widest;
    }

    /// <summary>
    /// Creates a new grid of a specified size.
    ///
    /// All cells of the grid will be iniated to the default value for T.
    /// </summary>
    public Grid(int width, int height) {
      IntWidth = width;
      foreach (int r in Enumerable.Range(0, height)) {
        List<T> innerList = new List<T>();
        foreach (int c in Enumerable.Range(0, width)) {
          innerList.Add(default(T));
        }
        BackingList.Add(innerList);
      }
    }

    public T this[GridReference gr] {
      get => BackingList[gr.Row][gr.Column];
      set {
        BackingList[gr.Row][gr.Column] = value;
      }
    }

    public T this[int r, int c] {
      get => BackingList[r][c];
      set {
        BackingList[r][c] = value;
      }
    }

    public T this[string gr] {
      get => this[(GridReference)gr];
      set => this[(GridReference)gr] = value;
    }

    public int Height => BackingList.Count;

    public int Width => IntWidth;

    public int Size => Height * IntWidth;

    public void AddColumn() {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList) {
        innerList.Add(default(T));
      }
    }

    public void AddColumn<U>(IList<U> column) where U : T {
      if (Height == 0 && Width == 0) {
        IntWidth += 1;
        foreach (U item in column) {
          List<T> innerList = new List<T>();
          innerList.Add(item);
          BackingList.Add(innerList);
        }
      }
      else {
        if (column.Count != Height) throw new ArgumentException("Column height must match grid height exactly, or grid must be empty.");
        IntWidth += 1;
        for (int i = 0; i < Height; i++) {
          BackingList[i].Add(column[i]);
        }
      }
    }

    public void AddColumn(T columnItem) => AddColumn(Enumerable.Repeat(columnItem, Height).ToList());
    public void AddColumn(Func<T> columnItemFunc) => AddColumn(Enumerable.Range(0, Height).Select(x => columnItemFunc()).ToList());
    public void AddColumn(Func<int, T> columnItemFunc) => AddColumn(Enumerable.Range(0, Height).Select(columnItemFunc).ToList());

    public void AddRow() {
      List<T> innerList = new List<T>();
      while (innerList.Count < IntWidth) {
        innerList.Add(default(T));
      }
      BackingList.Add(innerList);
    }

    public void AddRow<U>(IList<U> row) where U : T {
      if (Height == 0 && Width == 0) {
        IntWidth = row.Count;
      }
      else {
        if (row.Count != Width) throw new ArgumentException("Row width must match grid width exactly, or grid must be empty.");
      }
      BackingList.Add(new List<T>((IList<T>)row));
    }

    public void AddRow(T rowItem) => AddRow(Enumerable.Repeat(rowItem, Width).ToList());
    public void AddRow(Func<T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(x => rowItemFunc()).ToList());
    public void AddRow(Func<int, T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc).ToList());

    public void Clear() {
      BackingList.Clear();
      IntWidth = 0;
    }

    public bool Contains(T item) {
      foreach (List<T> innerList in BackingList) {
        if (innerList.Contains(item)) return true;
      }
      return false;
    }

    public IList<T> GetColumn(int index) {
      return new GridLine<T>(this, true, index);
    }

    public IEnumerator<IEnumerable<T>> GetColumnEnumerator() {
      for (int i = 0; i < Width; i++) {
        yield return GetColumn(i);
      }
    }

    public IEnumerator<IEnumerable<T>> GetEnumerator() {
      for (int i = 0; i < Height; i++) {
        yield return GetRow(i);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IList<T> GetRow(int index) {
      return new GridLine<T>(this, false, index);
    }

    public GridReference IndexOf(T item) {
      for (int r = 0; r < Height; r++) {
        List<T> innerList = BackingList[r];
        int index = innerList.IndexOf(item);
        if (index != 1) return new GridReference(index, r);
      }
      return null;
    }

    public GridReference IndexOfTransposed(T item) {
      GridReference lowIndex = null;
      for (int r = 0; r < Height; r++) {
        List<T> innerList = BackingList[r];
        int index = innerList.IndexOf(item);
        if (index == 0) return new GridReference(0, r);
        if (index > 0 && (lowIndex == null || index < lowIndex.Column)) lowIndex = new GridReference(index, r);
      }
      return lowIndex;
    }

    public void InsertColumn(int before) {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList) {
        innerList.Insert(before, default(T));
      }
    }

    public void InsertColumn<U>(int before, IList<U> column) where U : T {
      IntWidth += 1;
      if (Height != column.Count) throw new ArgumentException("Column height must match grid height exactly.");
      for (int i = 0; i < Height; i++) {
        BackingList[i].Insert(before, column[i]);
      }
    }

    public void InsertColumn(int before, T columnItem) => InsertColumn(before, Enumerable.Repeat(columnItem, Height).ToList());
    public void InsertColumn(int before, Func<T> columnItemFunc) => InsertColumn(before, Enumerable.Range(0, Height).Select(x => columnItemFunc()).ToList());
    public void InsertColumn(int before, Func<int, T> columnItemFunc) => InsertColumn(before, Enumerable.Range(0, Height).Select(columnItemFunc).ToList());

    public void InsertRow(int before) {
      List<T> innerList = new List<T>();
      for (int i = 0; i < IntWidth; i++) {
        innerList.Add(default(T));
      }
      BackingList.Insert(before, innerList);
    }

    public void InsertRow<U>(int before, IList<U> row) where U : T {
      if (Width != row.Count) throw new ArgumentException("Row width must match grid width exactly.");
      List<T> innerList = new List<T>((ICollection<T>)row);
      BackingList.Insert(before, innerList);
    }

    public void InsertRow(int before, T rowItem) => InsertRow(before, Enumerable.Repeat(rowItem, Width).ToList());
    public void InsertRow(int before, Func<T> rowItemFunc) => InsertRow(before, Enumerable.Range(0, Width).Select(x => rowItemFunc()).ToList());
    public void InsertRow(int before, Func<int, T> rowItemFunc) => InsertRow(before, Enumerable.Range(0, Width).Select(rowItemFunc).ToList());

    public void RemoveColumnAt(int col) {
      if (col < 0 || col >= Width) throw new ArgumentOutOfRangeException("Can only remove existing columns.");
      IntWidth -= 1;
      foreach (var list in BackingList) {
        list.RemoveAt(col);
      }
    }

    public void RemoveRowAt(int row) {
      if (row < 0 || row >= Height) throw new ArgumentOutOfRangeException("Can only remove existing rows.");
      BackingList.RemoveAt(row);
    }
  }

  /// <summary>
  /// One line - either a whole row or column - of a grid.
  ///
  /// The constructed GridLine is a window to a static point. GridLines to
  /// invalid rows or columns (higher than the range) can be constructed,
  /// but attempts to retrieve data from them will cause exceptions. A
  /// GridLine won't move with the actual line it initially references,
  /// i.e. a GridLine for column 2 will always point to whatever happens
  /// to be the column in that position. However, the size of the line
  /// will change with the size of that axis of the grid.
  ///
  /// A GridLine's structure cannot be modified from the GridLine class,
  /// but will reflect any changes made from the Grid class. Individual
  /// elements are likewise synced but can be changed from either class.
  /// </summary>
  public class GridLine<T> : IList<T> {
    public IGrid<T> ParentGrid { get; internal set; }
    public bool IsColumn { get; internal set; }
    public int Index { get; internal set; }

    public T this[int index] {
      get {
        if (!IsColumn) return ParentGrid[Index, index];
        else return ParentGrid[index, Index];
      }
      set {
        if (!IsColumn) ParentGrid[Index, index] = value;
        else ParentGrid[index, Index] = value;
      }
    }

    public int Count => IsColumn ? ParentGrid.Height : ParentGrid.Width;

    public bool IsReadOnly => false;

    public GridLine(IGrid<T> parent, bool isColumn, int index) {
      ParentGrid = parent;
      IsColumn = isColumn;
      Index = index;
    }

    public void Add(T item) {
      throw new System.NotSupportedException();
    }

    public void Clear() {
      throw new System.NotSupportedException();
    }

    public bool Contains(T item) {
      foreach (T itm in this) {
        if (object.Equals(itm, item)) return true;
      }
      return false;
    }

    public void CopyTo(T[] array, int arrayIndex) {
      ((List<T>)this).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator() {
      int len = Count;
      for (int i = 0; i < len; i++) {
        yield return this[i];
      }
    }

    public int IndexOf(T item) {
      int len = Count;
      for (int i = 0; i < len; i++) {
        if (object.Equals(item, this[i])) return i;
      }
      return -1;
    }

    public void Insert(int index, T item) {
      throw new System.NotSupportedException();
    }

    public bool Remove(T item) {
      throw new System.NotSupportedException();
    }

    public void RemoveAt(int index) {
      throw new System.NotSupportedException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      int len = Count;
      for (int i = 0; i < len; i++) {
        yield return this[i];
      }
    }

    public static explicit operator List<T>(GridLine<T> input) => new List<T>(input);
  }
}