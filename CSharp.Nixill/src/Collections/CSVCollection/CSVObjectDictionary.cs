using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Nixill.Serialization;
using Nixill.Utils;

namespace Nixill.Collections;

public class CSVObjectDictionary<K, V> : IDictionary<K, V>
{
  readonly List<string> _Columns = [];
  readonly Dictionary<K, V> _Contents = [];

  public IEnumerable<string> Columns => _Columns.AsReadOnly();
  public IEnumerable<KeyValuePair<K, V>> Contents => _Contents.AsReadOnly();

  public CSVObjectDictionary() { }

  public CSVObjectDictionary(IEnumerable<string> columns, IEnumerable<KeyValuePair<K, V>> contents)
  {
    _Columns = [.. columns];
    _Contents = new(contents);
  }

  public CSVObjectDictionary(CSVObjectDictionary<K, V> cloneOf)
  {
    _Columns = [.. cloneOf._Columns];
    _Contents = new(cloneOf);
  }

  public static CSVObjectDictionary<K, V> ParseObjectsFromFile(string path, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
    => ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  public static CSVObjectDictionary<K, V> ParseObjectsFromStream(StreamReader reader, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
    => ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  public static CSVObjectDictionary<K, V> ParseObjects(IEnumerable<char> input, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
  {
    IEnumerable<IList<string>> rows = CSVParser.EnumerableToRows(input);
    bool isHeaderRow = true;
    List<string> columns = [];
    Dictionary<K, V> objects = [];

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
        var kvp = deserializer(properties);
        objects.Add(kvp.Key, kvp.Value);
      }
    }

    return new CSVObjectDictionary<K, V>(columns, objects);
  }

  public string NewRow(K key, V value, Func<KeyValuePair<K, V>, IDictionary<string, string>> serializer)
  {
    _Contents.Add(key, value);
    return KeyValueToRow(key, value, serializer);
  }

  public string KeyValueToRow(K key, V value, Func<KeyValuePair<K, V>, IDictionary<string, string>> serializer)
  {
    var properties = serializer(new KeyValuePair<K, V>(key, value));
    var columnValues = _Columns.Select(c =>
    {
      if (properties.TryGetValue(c, out string v)) return v;
      return null;
    });
    return columnValues.Select(CSVParser.CSVEscape).SJoin(",");
  }

  public IEnumerable<string> RowsAsCSV(Func<KeyValuePair<K, V>, IDictionary<string, string>> serializer)
  {
    yield return _Columns.Select(CSVParser.CSVEscape).SJoin(",");
    foreach (string str in _Contents.Select(i => KeyValueToRow(i.Key, i.Value, serializer)))
      yield return str;
  }

  public string FormatCSV(Func<KeyValuePair<K, V>, IDictionary<string, string>> serializer)
    => RowsAsCSV(serializer).SJoin("\n");

  public void FormatCSVToFile(string path, Func<KeyValuePair<K, V>, IDictionary<string, string>> serializer)
    => File.WriteAllText(path, FormatCSV(serializer));

  #region IDictionary<K, V>: Contents
  public ICollection<K> Keys => ((IDictionary<K, V>)_Contents).Keys;
  public ICollection<V> Values => ((IDictionary<K, V>)_Contents).Values;
  public int Count => ((ICollection<KeyValuePair<K, V>>)_Contents).Count;
  public bool IsReadOnly => ((ICollection<KeyValuePair<K, V>>)_Contents).IsReadOnly;
  public V this[K key] { get => ((IDictionary<K, V>)_Contents)[key]; set => ((IDictionary<K, V>)_Contents)[key] = value; }

  public void Add(K key, V value)
  {
    ((IDictionary<K, V>)_Contents).Add(key, value);
  }

  public bool ContainsKey(K key)
  {
    return ((IDictionary<K, V>)_Contents).ContainsKey(key);
  }

  public bool Remove(K key)
  {
    return ((IDictionary<K, V>)_Contents).Remove(key);
  }

  public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
  {
    return ((IDictionary<K, V>)_Contents).TryGetValue(key, out value);
  }

  public void Add(KeyValuePair<K, V> item)
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).Add(item);
  }

  public void Clear()
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).Clear();
  }

  public bool Contains(KeyValuePair<K, V> item)
  {
    return ((ICollection<KeyValuePair<K, V>>)_Contents).Contains(item);
  }

  public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<K, V> item)
  {
    return ((ICollection<KeyValuePair<K, V>>)_Contents).Remove(item);
  }

  public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<K, V>>)_Contents).GetEnumerator();
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

public static class CSVObjectDictionary
{
  public static CSVObjectDictionary<K, V> ParseObjectsFromFile<K, V>(string path, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
      => CSVObjectDictionary<K, V>.ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  public static CSVObjectDictionary<K, V> ParseObjectsFromStream<K, V>(StreamReader reader, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
      => CSVObjectDictionary<K, V>.ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  public static CSVObjectDictionary<K, V> ParseObjects<K, V>(IEnumerable<char> input, Func<IDictionary<string, string>,
    KeyValuePair<K, V>> deserializer)
      => CSVObjectDictionary<K, V>.ParseObjects(input, deserializer);
}