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
        foreach (U item in column) {

        }
      }
      if (column.Count != Height) throw new ArgumentException("column height must match table height exactly");
    }

    public void AddRow() {
      List<T> innerList = new List<T>();
      while (innerList.Count < IntWidth) {
        innerList.Add(default(T));
      }
    }

    public void AddRow<U>(IList<U> row) where U : T {
      throw new System.NotImplementedException();
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

    public IList<T> GetColumn() {
      throw new System.NotImplementedException();
    }

    public IEnumerator<U> GetColumnEnumerator<U>() where U : IList<T> {
      throw new System.NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator() {
      foreach (List<T> innerList in BackingList) {
        foreach (T item in innerList) {
          yield return item;
        }
      }
    }

    public IEnumerator<U> GetEnumerator<U>() where U : IList<T> {
      throw new System.NotImplementedException();
    }

    public IList<T> GetRow() {
      throw new System.NotImplementedException();
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
      throw new System.NotImplementedException();
    }

    public void InsertRow(int before) {
      List<T> innerList = new List<T>();
      for (int i = 0; i < IntWidth; i++) {
        innerList.Add(default(T));
      }
      BackingList.Insert(before, innerList);
    }

    public void InsertRow<U>(int before, IList<U> row) where U : T {
      throw new System.NotImplementedException();
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