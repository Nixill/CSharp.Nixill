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
  public class Grid<T> : IGrid<T>
  {
    List<List<T>> BackingList = new List<List<T>>();
    int IntWidth = 0;

    /// <summary>
    /// Creates a new 0×0 grid.
    /// </summary>
    public Grid() { }

    /// <summary>
    /// Creates a new grid from an existing two-dimensional enumerable.
    ///
    /// The outer list is taken as rows, and the inner list as columns
    /// within the row.
    /// </summary>
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
    /// Creates a new grid of a specified size.
    ///
    /// All cells of the grid will be iniated to the default value for T.
    /// </summary>
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

    public T this[IntVector2 iv2]
    {
      get => BackingList[iv2.Y][iv2.X];
      set => BackingList[iv2.Y][iv2.X] = value;
    }

    [Obsolete("Use IntVector2 instead.")]
    public T this[GridReference gr]
    {
      get => BackingList[gr.Row][gr.Column];
      set
      {
        BackingList[gr.Row][gr.Column] = value;
      }
    }

    [Obsolete("Use GridRef.RC(r, c) instead.")]
    public T this[int r, int c]
    {
      get => BackingList[r][c];
      set
      {
        BackingList[r][c] = value;
      }
    }

    [Obsolete("Use GridRef.FromString(str) instead.")]
    public T this[string gr]
    {
      get => this[(GridReference)gr];
      set => this[(GridReference)gr] = value;
    }

    public int Height => BackingList.Count;
    public int Width => IntWidth;
    public int Size => Height * IntWidth;

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

    [Obsolete("Will be removed because it violates nullability contracts. Use AddColumn(default(T)) instead.")]
    public void AddColumn()
    {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList)
      {
        innerList.Add(default(T)!);
      }
    }

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

    public void AddColumn(T columnItem) => AddColumn(Enumerable.Repeat(columnItem, Height));
    public void AddColumn(Func<T> columnItemFunc) => AddColumn(Sequence.Repeat(columnItemFunc, Height));
    public void AddColumn(Func<int, T> columnItemFunc) => AddColumn(Enumerable.Range(0, Height).Select(columnItemFunc));

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

    public void AddRow(T rowItem) => AddRow(Enumerable.Repeat(rowItem, Width));
    public void AddRow(Func<T> rowItemFunc) => AddRow(Sequence.Repeat(rowItemFunc, Width));
    public void AddRow(Func<int, T> rowItemFunc) => AddRow(Enumerable.Range(0, Width).Select(rowItemFunc));

    public void Clear()
    {
      BackingList.Clear();
      IntWidth = 0;
    }

    public bool Contains(T item)
    {
      foreach (List<T> innerList in BackingList)
      {
        if (innerList.Contains(item)) return true;
      }
      return false;
    }

    public IList<T> GetColumn(int index)
    {
      return ColumnEnumerable(index).ToList();
    }

    public IEnumerable<(T Item, IntVector2 Reference)> Flatten()
      => this.SelectMany((r, y) => r.Select((i, x) => (i, new IntVector2(x, y))));

    public IEnumerator<IEnumerable<T>> GetColumnEnumerator() => Columns.GetEnumerator();
    public IEnumerator<IEnumerable<T>> GetEnumerator() => Rows.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IList<T> GetRow(int index)
    {
      return RowEnumerable(index).ToList();
    }

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

    [Obsolete("Will be removed because it violates nullability contracts. Use InsertColumn(before, default(T)) instead.")]
    public void InsertColumn(int before)
    {
      IntWidth += 1;
      foreach (List<T> innerList in BackingList)
      {
        innerList.Insert(before, default(T)!);
      }
    }

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

    public void InsertColumn(int before, T columnItem) => InsertColumn(before, Enumerable.Repeat(columnItem, Height));
    public void InsertColumn(int before, Func<T> columnItemFunc) => InsertColumn(before, Sequence.Repeat(columnItemFunc, Height));
    public void InsertColumn(int before, Func<int, T> columnItemFunc) => InsertColumn(before, Enumerable.Range(0, Height).Select(columnItemFunc));

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

    public void InsertRow(int before, IEnumerable<T> row)
    {
      List<T> rowList = row.ToList();
      if (Width != rowList.Count) throw new ArgumentException("Row width must match grid width exactly.");
      BackingList.Insert(before, rowList);
    }

    public void InsertRow(int before, T rowItem) => InsertRow(before, Enumerable.Repeat(rowItem, Width));
    public void InsertRow(int before, Func<T> rowItemFunc) => InsertRow(before, Sequence.Repeat(rowItemFunc, Width));
    public void InsertRow(int before, Func<int, T> rowItemFunc) => InsertRow(before, Enumerable.Range(0, Width).Select(rowItemFunc));

    public bool IsWithinGrid(IntVector2 reference) => reference.Y >= 0 && reference.Y < Height && reference.X >= 0 && reference.X < Width;

    public void RemoveColumnAt(int col)
    {
      if (col < 0 || col >= Width) throw new ArgumentOutOfRangeException("Can only remove existing columns.");
      IntWidth -= 1;
      foreach (var list in BackingList)
      {
        list.RemoveAt(col);
      }
    }

    public void RemoveRowAt(int row)
    {
      if (row < 0 || row >= Height) throw new ArgumentOutOfRangeException("Can only remove existing rows.");
      BackingList.RemoveAt(row);
    }

    /// <summary>
    /// Returns an enumeraor over each row of a grid as strings.
    /// </summary>
    /// <param name="input">The grid to output.</param>
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
    /// Converts a grid to a csv string.
    /// </summary>
    /// <param name="input">The grid to convert.</param>
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
    /// Converts a grid to a csv string and writes it to a file.
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

  public static class Grid
  {
    /// <summary>
    /// Reads a CSV file into a Grid of strings.
    /// </summary>
    /// <param name="path">The path of the file to read.</param>
    public static Grid<string> DeserializeFromFile(string path)
    {
      return Deserialize(FileUtils.FileCharEnumerator(path));
    }

    /// <summary>
    /// Reads a CSV stream into a Grid of strings.
    /// </summary>
    /// <param name="reader">The StreamReader to read from.</param>
    public static Grid<string> Deserialize(StreamReader reader)
    {
      return Deserialize(FileUtils.StreamCharEnumerator(reader));
    }

    /// <summary>
    /// Reads a char enumerator and converts the streamed chars into a
    /// Grid of strings.
    /// </summary>
    /// <param name="input">The input stream to read.</param>
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
