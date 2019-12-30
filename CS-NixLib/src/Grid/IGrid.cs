using System;

namespace Nixill.Grid {
  public interface IGrid<T> {
    int Height { get; }
    int Width { get; }
    int Size { get; }

    void AddRow();
    void AddColumn();
    void Clear();
    void Contains(T item);
    GridReference IndexOf(T item);
    void InsertRow(int before);
    void InsertColumn(int before);
  }
}