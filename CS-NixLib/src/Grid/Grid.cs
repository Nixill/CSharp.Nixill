using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Grid {
  public class Grid<T> : IGrid<T> {
    List<List<T>> BackingList = new List<List<T>>();
    int IntWidth = 0;

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

    public void AddRow() {
      List<T> innerList = new List<T>();
      while (innerList.Count < IntWidth) {
        innerList.Add(default(T));
      }
    }

    public void AddRow<U>(IList<U> row) where U : T {
      if (Height == 0 && Width == 0) {
        IntWidth = row.Count;
      }
      else {
        if (row.Count != Width) throw new ArgumentException("Row width must match grid width exactly, or grid must be empty.");
      }
    }

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

    public IEnumerator<IList<T>> GetColumnEnumerator<U>() {
      for (int i = 0; i < Width; i++) {
        yield return GetColumn(i);
      }
    }

    public IEnumerator<IList<T>> GetEnumerator() {
      for (int i = 0; i < Height; i++) {
        yield return GetRow(i);
      }
    }

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
      if (Height != column.Count) throw new ArgumentException("Column height must match grid height exactly.");
      for (int i = 0; i < Height; i++) {
        BackingList[i].Insert(before, column[i]);
      }
    }

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
  }

  public class GridLine<T> : IList<T> {
    public IGrid<T> ParentGrid { get; internal set; }
    public bool IsColumn { get; internal set; }
    public int Index { get; internal set; }

    public T this[int index] {
      get {
        if (IsColumn) return ParentGrid[index, Index];
        else return ParentGrid[Index, index];
      }
      set {
        if (IsColumn) ParentGrid[index, Index] = value;
        else ParentGrid[Index, index] = value;
      }
    }

    public int Count => IsColumn ? ParentGrid.Height : ParentGrid.Width;

    public bool IsReadOnly => false;

    public GridLine(IGrid<T> parent, bool isColumn, int index) {
      if (index < 0) throw new ArgumentOutOfRangeException("index", "Must be at least 0.");

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