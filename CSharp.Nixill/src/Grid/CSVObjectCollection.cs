using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Nixill.Utils;

namespace Nixill.Collections.Grid.CSV;

public class CSVObjectCollection<T> : IList<T>
{
  readonly List<string> _Columns = [];
  readonly List<T> _Contents = [];

  public IEnumerable<string> Columns => _Columns.AsReadOnly();
  public IEnumerable<T> Contents => _Contents.AsReadOnly();

  public CSVObjectCollection() { }

  public CSVObjectCollection(IEnumerable<string> columns, IEnumerable<T> contents)
  {
    _Columns = [.. columns];
    _Contents = [.. contents];
  }

  public CSVObjectCollection(CSVObjectCollection<T> cloneOf)
  {
    _Columns = [.. cloneOf._Columns];
    _Contents = [.. cloneOf];
  }

  public static CSVObjectCollection<T> ParseObjectsFromFile(string path, Func<IDictionary<string, string>, T> deserializer)
    => ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  public static CSVObjectCollection<T> ParseObjectsFromStream(StreamReader reader,
    Func<IDictionary<string, string>, T> deserializer)
    => ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  public static CSVObjectCollection<T> ParseObjects(IEnumerable<char> input,
    Func<IDictionary<string, string>, T> deserializer)
  {
    IEnumerable<IList<string>> rows = CSVParser.EnumerableToRows(input);
    bool isHeaderRow = true;
    List<string> columns = [];
    List<T> objects = [];

    foreach (var row in rows)
    {
      if (isHeaderRow)
      {
        columns = [.. row];
        isHeaderRow = false;
      }
      else
      {
        IDictionary<string, string> properties = columns.Zip(row).ToPropertyDictionary();
        objects.Add(deserializer(properties));
      }
    }

    return new CSVObjectCollection<T>(columns, objects);
  }

  public string NewRow(T item, Func<T, IDictionary<string, string>> serializer)
  {
    _Contents.Add(item);
    return ItemToRow(item, serializer);
  }

  public string ItemToRow(T item, Func<T, IDictionary<string, string>> serializer)
  {
    var properties = serializer(item);
    var columnValues = _Columns.Select(c =>
    {
      if (properties.TryGetValue(c, out string v)) return v;
      return null;
    });
    return columnValues.Select(CSVParser.CSVEscape).SJoin(",");
  }

  public IEnumerable<string> RowsAsCSV(Func<T, IDictionary<string, string>> serializer)
  {
    yield return _Columns.Select(CSVParser.CSVEscape).SJoin(",");
    foreach (string str in _Contents.Select(i => ItemToRow(i, serializer)))
      yield return str;
  }

  public string FormatCSV(Func<T, IDictionary<string, string>> serializer)
    => RowsAsCSV(serializer).SJoin("\n");

  public void FormatCSVToFile(string path, Func<T, IDictionary<string, string>> serializer)
    => File.WriteAllText(path, FormatCSV(serializer));

  #region IList: Contents
  public int Count => _Contents.Count;

  bool ICollection<T>.IsReadOnly => ((ICollection<T>)_Contents).IsReadOnly;

  public T this[int index] { get => _Contents[index]; set => _Contents[index] = value; }

  public int IndexOf(T item)
  {
    return ((IList<T>)_Contents).IndexOf(item);
  }

  public void Insert(int index, T item)
  {
    ((IList<T>)_Contents).Insert(index, item);
  }

  public void RemoveAt(int index)
  {
    ((IList<T>)_Contents).RemoveAt(index);
  }

  public void Add(T item)
  {
    ((ICollection<T>)_Contents).Add(item);
  }

  public void Clear()
  {
    ((ICollection<T>)_Contents).Clear();
  }

  public bool Contains(T item)
  {
    return ((ICollection<T>)_Contents).Contains(item);
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    ((ICollection<T>)_Contents).CopyTo(array, arrayIndex);
  }

  public bool Remove(T item)
  {
    return ((ICollection<T>)_Contents).Remove(item);
  }

  public IEnumerator<T> GetEnumerator()
  {
    return ((IEnumerable<T>)_Contents).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_Contents).GetEnumerator();
  }
  #endregion

  #region IList: Columns
  public int IndexOfColumn(string item)
  {
    return ((IList<string>)_Columns).IndexOf(item);
  }

  public void InsertColumn(int index, string item)
  {
    ((IList<string>)_Columns).Insert(index, item);
  }

  public void RemoveColumnAt(int index)
  {
    ((IList<string>)_Columns).RemoveAt(index);
  }

  public void AddColumn(string item)
  {
    ((ICollection<string>)_Columns).Add(item);
  }

  public void ClearColumns()
  {
    ((ICollection<string>)_Columns).Clear();
  }

  public bool ContainsColumn(string item)
  {
    return ((ICollection<string>)_Columns).Contains(item);
  }

  public void CopyColumnsTo(string[] array, int arrayIndex)
  {
    ((ICollection<string>)_Columns).CopyTo(array, arrayIndex);
  }

  public bool RemoveColumn(string item)
  {
    return ((ICollection<string>)_Columns).Remove(item);
  }

  public IEnumerator<string> GetColumnEnumerator()
  {
    return ((IEnumerable<string>)_Columns).GetEnumerator();
  }
  #endregion
}

public static class CSVObjectCollection
{
  public static CSVObjectCollection<T> ParseObjectsFromFile<T>(string path, Func<IDictionary<string, string>, T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  public static CSVObjectCollection<T> ParseObjectsFromStream<T>(StreamReader reader,
    Func<IDictionary<string, string>, T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  public static CSVObjectCollection<T> ParseObjects<T>(IEnumerable<char> input,
    Func<IDictionary<string, string>, T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(input, deserializer);
}

internal class PropertyDictionary(Dictionary<string, string> backing) : IDictionary<string, string>
{
  readonly Dictionary<string, string> Backing = backing;

  public string this[string key]
  {
    get => Backing.TryGetValue(key, out string value) ? value != "" ? value : null : null;
    set => throw new InvalidOperationException("This dictionary is read-only.");
  }

  public ICollection<string> Keys => Backing.Keys;

  public ICollection<string> Values => Backing.Values;

  public int Count => Backing.Count;

  public bool IsReadOnly => true;

  public void Add(string key, string value)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public void Add(KeyValuePair<string, string> item)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public void Clear()
    => throw new InvalidOperationException("This dictionary is read-only.");

  public bool Contains(KeyValuePair<string, string> item)
    => Backing.Contains(item);

  public bool ContainsKey(string key)
    => Backing.ContainsKey(key);

  public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    => ((IDictionary<string, string>)Backing).CopyTo(array, arrayIndex);

  public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    => Backing.GetEnumerator();

  public bool Remove(string key)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public bool Remove(KeyValuePair<string, string> item)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
  {
    bool success = Backing.TryGetValue(key, out value);

    if (value == "")
    {
      value = null;
      return false;
    }

    return success;
  }

  IEnumerator IEnumerable.GetEnumerator() => Backing.GetEnumerator();
}

internal static class PropertyDictionaryExtensions
{
  public static PropertyDictionary ToPropertyDictionary(this IEnumerable<(string, string)> dict)
    => new PropertyDictionary(dict.ToDictionary());
}