using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Nixill.Serialization;
using Nixill.Utils;
using Nixill.Utils.Extensions;

namespace Nixill.Collections;

/// <summary>
///   Represents a dictionary of objects deserialized from a CSV file.
/// </summary>
/// <typeparam name="K">The type of keys in the collection.</typeparam>
/// <typeparam name="V">The type of values in the collection.</typeparam>
public class CSVObjectDictionary<K, V> : IDictionary<K, V> where K : notnull
{
  readonly List<string> _Columns = [];
  readonly Dictionary<K, V> _Contents = [];

  /// <summary>
  ///   Get: A read-only sequence of the column headers for this collection.
  /// </summary>
  public IEnumerable<string> Columns => _Columns.AsReadOnly();

  /// <summary>
  ///   Get: A read-only sequence of the contents of this collection (as
  ///   key-value pairs).
  /// </summary>
  public IEnumerable<KeyValuePair<K, V>> Contents => _Contents.AsReadOnly();

  /// <summary>
  ///   Constructs a new, empty <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  public CSVObjectDictionary() { }

  /// <summary>
  ///   Constructs a <see cref="CSVObjectDictionary{K, V}"/> with given
  ///   sequences of columns and contents.
  /// </summary>
  /// <param name="columns">The column headers.</param>
  /// <param name="contents">The objects.</param>
  public CSVObjectDictionary(IEnumerable<string> columns, IEnumerable<KeyValuePair<K, V>> contents)
  {
    _Columns = [.. columns];
    _Contents = new(contents);
  }

  /// <summary>
  ///   Constructs a <see cref="CSVObjectDictionary{K, V}"/> as a clone
  ///   (with the same columns and contents) of another.
  /// </summary>
  /// <param name="cloneOf">
  ///   The <see cref="CSVObjectDictionary{K, V}"/> to clone.
  /// </param>
  public CSVObjectDictionary(CSVObjectDictionary<K, V> cloneOf)
  {
    _Columns = [.. cloneOf._Columns];
    _Contents = new(cloneOf);
  }

  /// <summary>
  ///   Parses objects from a CSV file into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="path">The path to a CSV file.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the file.</returns>
  public static CSVObjectDictionary<K, V> ParseObjectsFromFile(string path,
    CSVKeyValuePairDeserializer<K, V> deserializer)
    => ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  /// <summary>
  ///   Parses objects from a CSV stream into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="reader">
  ///   A reader on the input stream of CSV data.
  /// </param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the stream.</returns>
  public static CSVObjectDictionary<K, V> ParseObjectsFromStream(StreamReader reader,
    CSVKeyValuePairDeserializer<K, V> deserializer)
    => ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  /// <summary>
  ///   Parses objects from a string of CSV data into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="input">A string of CSV data.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the data.</returns>
  public static CSVObjectDictionary<K, V> ParseObjects(IEnumerable<char> input,
    CSVKeyValuePairDeserializer<K, V> deserializer)
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
        IDictionary<string, string?> properties = columns.Zip(row).ToPropertyDictionary();
        var kvp = deserializer(properties);
        objects.Add(kvp.Key, kvp.Value);
      }
    }

    return new CSVObjectDictionary<K, V>(columns, objects);
  }

  /// <summary>
  ///   Adds an item to the collection.
  /// </summary>
  /// <param name="key">The key to use.</param>
  /// <param name="value">The value to add.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>
  ///   The string representation of the newly added object in this
  ///   collection's CSV data.
  /// </returns>
  public string NewRow(K key, V value, CSVKeyValuePairSerializer<K, V> serializer)
  {
    _Contents.Add(key, value);
    return KeyValueToRow(key, value, serializer);
  }

  /// <summary>
  ///   Returns the string representation of an object of this collection
  ///   in this collection's CSV data.
  /// </summary>
  /// <remarks>
  ///   The item need not necessarily be in the collection itself. The
  ///   returned string is what the representation would be if it were.
  /// </remarks>
  /// <param name="key">The key to use.</param>
  /// <param name="value">The value to serialize.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>
  ///   The string representation of the given object in this collection's
  ///   CSV data.
  /// </returns>
  public string KeyValueToRow(K key, V value, CSVKeyValuePairSerializer<K, V> serializer)
  {
    var properties = serializer(new KeyValuePair<K, V>(key, value));
    var columnValues = _Columns.Select(c =>
    {
      if (properties.TryGetValue(c, out string? v)) return v;
      return null;
    });
    return columnValues.Select(CSVParser.CSVEscape).StringJoin(",");
  }

  /// <summary>
  ///   Returns a sequence of all of the rows of this collection,
  ///   including the header, serialized to CSV data.
  /// </summary>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>
  ///   A sequence representing all the rows of this collection as
  ///   serialized CSV data.
  /// </returns>
  public IEnumerable<string> RowsAsCSV(CSVKeyValuePairSerializer<K, V> serializer)
  {
    yield return _Columns.Select(CSVParser.CSVEscape).StringJoin(",");
    foreach (string str in _Contents.Select(i => KeyValueToRow(i.Key, i.Value, serializer)))
      yield return str;
  }

  /// <summary>
  ///   Returns this entire collection as a single CSV string.
  /// </summary>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>This collection as a CSV string.</returns>
  public string FormatCSV(CSVKeyValuePairSerializer<K, V> serializer)
    => RowsAsCSV(serializer).StringJoin("\n");

  /// <summary>
  ///   Writes this entire collection as a CSV file.
  /// </summary>
  /// <param name="path">The path of the file to write.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  public void FormatCSVToFile(string path, CSVKeyValuePairSerializer<K, V> serializer)
    => File.WriteAllText(path, FormatCSV(serializer));

  #region IDictionary<K, V>: Contents
  /// <summary>
  ///   Get: An <see cref="ICollection{K}"/> containing the keys of the
  ///   contents of this <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  public ICollection<K> Keys => ((IDictionary<K, V>)_Contents).Keys;

  /// <summary>
  ///   Get: An <see cref="ICollection{V}"/> containing the values of the
  ///   contents of this <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  public ICollection<V> Values => ((IDictionary<K, V>)_Contents).Values;

  /// <summary>
  ///   Get: The number of objects in the contents of this
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  public int Count => ((ICollection<KeyValuePair<K, V>>)_Contents).Count;

  /// <summary>
  ///   Get: Whether or not this <see cref="CSVObjectDictionary{K, V}"/>
  ///   is read-only (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => ((ICollection<KeyValuePair<K, V>>)_Contents).IsReadOnly;

  /// <summary>
  ///   Get or set: The value associated with a given key within the
  ///   contents of this <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="key">The key to get or set.</param>
  public V this[K key] { get => ((IDictionary<K, V>)_Contents)[key]; set => ((IDictionary<K, V>)_Contents)[key] = value; }

  /// <summary>
  ///   Adds a key and value to this <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="key">The key to add.</param>
  /// <param name="value">The value to add.</param>
  public void Add(K key, V value)
  {
    ((IDictionary<K, V>)_Contents).Add(key, value);
  }

  /// <summary>
  ///   Returns whether or not this
  ///   <see cref="CSVObjectDictionary{K, V}"/> contains the given key.
  /// </summary>
  /// <param name="key">The key for which to check.</param>
  /// <returns>
  ///   <c>true</c> iff the key is present in this
  ///   <see cref="CSVObjectDictionary{K, V}"/>; <c>false</c> otherwise.
  /// </returns>
  public bool ContainsKey(K key)
  {
    return ((IDictionary<K, V>)_Contents).ContainsKey(key);
  }

  /// <summary>
  ///   Removes from this <see cref="CSVObjectDictionary{K, V}"/> the
  ///   entry with the given key.
  /// </summary>
  /// <param name="key">The key to remove.</param>
  /// <returns>
  ///   <c>true</c> iff the key was removed from this
  ///   <see cref="CSVObjectDictionary{K, V}"/>; <c>false</c> otherwise
  ///   (including if no such key was present to begin with).
  /// </returns>
  public bool Remove(K key)
  {
    return ((IDictionary<K, V>)_Contents).Remove(key);
  }

  /// <summary>
  ///   Gets the value associated with the specified key.
  /// </summary>
  /// <param name="key">The key whose value to get.</param>
  /// <param name="value">
  ///   When this method returns, the value associated with the specified
  ///   key, if the key was found; otherwise, <c>default(V)</c>.
  /// </param>
  /// <returns>Whether or not such an entry was found.</returns>
  public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
  {
    return ((IDictionary<K, V>)_Contents).TryGetValue(key, out value);
  }

  /// <summary>
  ///   Adds an entry to this dictionary.
  /// </summary>
  /// <param name="item">The entry to add.</param>
  public void Add(KeyValuePair<K, V> item)
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).Add(item);
  }

  /// <summary>
  ///   Clears the contents of this dictionary.
  /// </summary>
  /// <remarks>
  ///   This does not clear the column listing from this dictionary.
  /// </remarks>
  public void Clear()
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).Clear();
  }

  /// <summary>
  ///   Returns whether or not this dictionary contains a given entry.
  /// </summary>
  /// <param name="item">The entry to locate.</param>
  /// <returns><c>true</c> iff the item was found.</returns>
  public bool Contains(KeyValuePair<K, V> item)
  {
    return ((ICollection<KeyValuePair<K, V>>)_Contents).Contains(item);
  }

  /// <summary>
  ///   Copies the entries of this dictionary to an Array, starting at a
  ///   particular Array index.
  /// </summary>
  /// <param name="array">
  ///   The one-dimensional Array that is the destination of the entries
  ///   copied from this dictionary. The Array must have zero-based indexing.
  /// </param>
  /// <param name="arrayIndex">
  ///   The zero-based index in array at which copying begins.
  /// </param>
  public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
  {
    ((ICollection<KeyValuePair<K, V>>)_Contents).CopyTo(array, arrayIndex);
  }

  /// <summary>
  ///   Removes an entry from this dictionary, if it's present.
  /// </summary>
  /// <param name="item">The entry to remove.</param>
  /// <returns>
  ///   <c>true</c> iff the entry was successfully removed from the
  ///   collection; <c>false</c> otherwise (including if it was not found).
  /// </returns>
  public bool Remove(KeyValuePair<K, V> item)
  {
    return ((ICollection<KeyValuePair<K, V>>)_Contents).Remove(item);
  }

  /// <summary>
  ///   Returns an enumerator over the entries of this dictionary.
  /// </summary>
  /// <returns>The enumerator.</returns>
  public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
  {
    return ((IEnumerable<KeyValuePair<K, V>>)_Contents).GetEnumerator();
  }

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_Contents).GetEnumerator();
  }
  #endregion

  #region IList: Columns
  /// <summary>
  ///   Determines the index of a specific column header.
  /// </summary>
  /// <param name="item">The column header to locate.</param>
  /// <returns>
  ///   The index in the columns list where the column was located, or -1
  ///   if it was not found.
  /// </returns>
  public int IndexOfColumn(string item)
  {
    return ((IList<string>)_Columns).IndexOf(item);
  }

  /// <summary>
  ///   Inserts a new column into this collection's columns list.
  /// </summary>
  /// <param name="index">
  ///   The index at which to insert the new column.
  /// </param>
  /// <param name="item">The new column.</param>
  public void InsertColumn(int index, string item)
  {
    ((IList<string>)_Columns).Insert(index, item);
  }

  /// <summary>
  ///   Removes a column with a given index from this collection's
  ///   columns list.
  /// </summary>
  /// <param name="index">The index at which to remove a column.</param>
  public void RemoveColumnAt(int index)
  {
    ((IList<string>)_Columns).RemoveAt(index);
  }

  /// <summary>
  ///   Adds a new column to the end of this collection's columns list.
  /// </summary>
  /// <param name="item">The new column.</param>
  public void AddColumn(string item)
  {
    ((ICollection<string>)_Columns).Add(item);
  }

  /// <summary>
  ///   Clears the list of columns from this collection.
  /// </summary>
  public void ClearColumns()
  {
    ((ICollection<string>)_Columns).Clear();
  }

  /// <summary>
  ///   Returns whether or not this collection's columns list contains a
  ///   certain column.
  /// </summary>
  /// <param name="item">The column to find.</param>
  /// <returns>
  ///   <c>true</c> iff the column is in the columns list; <c>false</c>
  ///   otherwise.
  /// </returns>
  public bool ContainsColumn(string item)
  {
    return ((ICollection<string>)_Columns).Contains(item);
  }

  /// <summary>
  ///   Copies the columns of this collection to an Array, starting at a
  ///   particular Array index.
  /// </summary>
  /// <param name="array">
  ///   The one-dimensional Array that is the destination of the columns
  ///   copied from this collection. The Array must have zero-based indexing.
  /// </param>
  /// <param name="arrayIndex">
  ///   The zero-based index in array at which copying begins.
  /// </param>
  public void CopyColumnsTo(string[] array, int arrayIndex)
  {
    ((ICollection<string>)_Columns).CopyTo(array, arrayIndex);
  }

  /// <summary>
  ///   Removes an column from this collection's columns list, if it's present.
  /// </summary>
  /// <param name="item">The column to remove.</param>
  /// <returns>
  ///   <c>true</c> iff the column was successfully removed from the
  ///   collection; <c>false</c> otherwise (including if it was not found).
  /// </returns>
  public bool RemoveColumn(string item)
  {
    return ((ICollection<string>)_Columns).Remove(item);
  }

  /// <summary>
  ///   Returns an enumerator over the columns of this collection.
  /// </summary>
  /// <returns>The enumerator.</returns>
  public IEnumerator<string> GetColumnEnumerator()
  {
    return ((IEnumerable<string>)_Columns).GetEnumerator();
  }
  #endregion
}

public static class CSVObjectDictionary
{
  /// <summary>
  ///   Parses objects from a CSV file into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <typeparam name="K">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <param name="path">The path to a CSV file.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the file.</returns>
  public static CSVObjectDictionary<K, V> ParseObjectsFromFile<K, V>(string path,
    CSVKeyValuePairDeserializer<K, V> deserializer) where K : notnull
      => CSVObjectDictionary<K, V>.ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  /// <summary>
  ///   Parses objects from a CSV stream into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="reader">
  ///   A reader on the input stream of CSV data.
  /// </param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the stream.</returns>
  public static CSVObjectDictionary<K, V> ParseObjectsFromStream<K, V>(StreamReader reader,
    CSVKeyValuePairDeserializer<K, V> deserializer) where K : notnull
      => CSVObjectDictionary<K, V>.ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  /// <summary>
  ///   Parses objects from a string of CSV data into a
  ///   <see cref="CSVObjectDictionary{K, V}"/>.
  /// </summary>
  /// <param name="input">A string of CSV data.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the data.</returns>
  public static CSVObjectDictionary<K, V> ParseObjects<K, V>(IEnumerable<char> input,
    CSVKeyValuePairDeserializer<K, V> deserializer) where K : notnull
      => CSVObjectDictionary<K, V>.ParseObjects(input, deserializer);
}

/// <summary>
///   Deserializes a row of a CSV properties into a key-value entry
///   represented by those properties.
/// </summary>
/// <typeparam name="K">
///   The type of key for the entry being deserialized.
/// </typeparam>
/// <typeparam name="V">
///   The type of value for the entry being deserialized.
/// </typeparam>
/// <param name="properties">
///   A dictionary mapping properties to values as represented in CSV
///   data. Note that a get operation on this dictionary will always
///   succeed; <c>null</c> will be returned for keys that the dictionary
///   does not contain, as well as for keys the dictionary maps to blank
///   values (<see cref="IDictionary{K,V}.ContainsKey(K)" /> can be used
///   to distinguish between these cases).
/// </param>
/// <returns>
///   The key-value entry deserialized from these properties.
/// </returns>
public delegate KeyValuePair<K, V> CSVKeyValuePairDeserializer<K, V>(IDictionary<string, string?> properties);

/// <summary>
///   Serializes an object into a row of CSV properties.
/// </summary>
/// <typeparam name="K">
///   The type of key for the entry being serialized.
/// </typeparam>
/// <typeparam name="V">
///   The type of value for the entry being serialized.
/// </typeparam>
/// <param name="keyValuePair">The key-value entry to serialize.</param>
/// <returns>
///   A dictionary mapping properties to values as represented in CSV data.
/// </returns>
public delegate IDictionary<string, string?> CSVKeyValuePairSerializer<K, V>(KeyValuePair<K, V> keyValuePair);