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
///   Represents a collection of objects deserialized from a CSV file.
/// </summary>
/// <typeparam name="T">The type of object.</typeparam>
public class CSVObjectCollection<T> : IList<T>
{
  readonly List<string> _Columns = [];
  readonly List<T> _Contents = [];

  /// <summary>
  ///   Get: A read-only sequence of the column headers for this collection.
  /// </summary>
  public IEnumerable<string> Columns => _Columns.AsReadOnly();

  /// <summary>
  ///   Get: A read-only sequence of the contents of this collection.
  /// </summary>
  public IEnumerable<T> Contents => _Contents.AsReadOnly();

  /// <summary>
  ///   Constructs a new, empty <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  public CSVObjectCollection() { }

  /// <summary>
  ///   Constructs a <see cref="CSVObjectCollection{T}"/> with given
  ///   sequences of columns and contents.
  /// </summary>
  /// <param name="columns">The column headers.</param>
  /// <param name="contents">The objects.</param>
  public CSVObjectCollection(IEnumerable<string> columns, IEnumerable<T> contents)
  {
    _Columns = [.. columns];
    _Contents = [.. contents];
  }

  /// <summary>
  ///   Constructs a <see cref="CSVObjectCollection{T}"/> as a clone (with
  ///   the same columns and contents) of another.
  /// </summary>
  /// <param name="cloneOf">
  ///   The <see cref="CSVObjectCollection{T}"/> to clone.
  /// </param>
  public CSVObjectCollection(CSVObjectCollection<T> cloneOf)
  {
    _Columns = [.. cloneOf._Columns];
    _Contents = [.. cloneOf];
  }

  /// <summary>
  ///   Parses objects from a CSV file into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <param name="path">The path to a CSV file.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the file.</returns>
  public static CSVObjectCollection<T> ParseObjectsFromFile(string path, CSVObjectDeserializer<T> deserializer)
    => ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  /// <summary>
  ///   Parses objects from a CSV stream into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <param name="reader">
  ///   A reader on the input stream of CSV data.
  /// </param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the stream.</returns>
  public static CSVObjectCollection<T> ParseObjectsFromStream(StreamReader reader, CSVObjectDeserializer<T> deserializer)
    => ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  /// <summary>
  ///   Parses objects from a string of CSV data into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <param name="input">A string of CSV data.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the data.</returns>
  public static CSVObjectCollection<T> ParseObjects(IEnumerable<char> input, CSVObjectDeserializer<T> deserializer)
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
        IDictionary<string, string?> properties = columns.Zip(row).ToPropertyDictionary();
        objects.Add(deserializer(properties));
      }
    }

    return new CSVObjectCollection<T>(columns, objects);
  }

  /// <summary>
  ///   Adds an item to the collection.
  /// </summary>
  /// <param name="item">The item to add.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>
  ///   The string representation of the newly added object in this
  ///   collection's CSV data.
  /// </returns>
  public string NewRow(T item, CSVObjectSerializer<T> serializer)
  {
    _Contents.Add(item);
    return ItemToRow(item, serializer);
  }

  /// <summary>
  ///   Returns the string representation of an object of this collection
  ///   in this collection's CSV data.
  /// </summary>
  /// <remarks>
  ///   The item need not necessarily be in the collection itself. The
  ///   returned string is what the representation would be if it were.
  /// </remarks>
  /// <param name="item">The item to serialize.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>
  ///   The string representation of the given object in this collection's
  ///   CSV data.
  /// </returns>
  public string ItemToRow(T item, CSVObjectSerializer<T> serializer)
  {
    var properties = serializer(item);
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
  public IEnumerable<string> RowsAsCSV(CSVObjectSerializer<T> serializer)
  {
    yield return _Columns.Select(CSVParser.CSVEscape).StringJoin(",");
    foreach (string str in _Contents.Select(i => ItemToRow(i, serializer)))
      yield return str;
  }

  /// <summary>
  ///   Returns this entire collection as a single CSV string.
  /// </summary>
  /// <param name="serializer">The serializer for the collection.</param>
  /// <returns>This collection as a CSV string.</returns>
  public string FormatCSV(CSVObjectSerializer<T> serializer)
    => RowsAsCSV(serializer).StringJoin("\n");

  /// <summary>
  ///   Writes this entire collection as a CSV file.
  /// </summary>
  /// <param name="path">The path of the file to write.</param>
  /// <param name="serializer">The serializer for the collection.</param>
  public void FormatCSVToFile(string path, CSVObjectSerializer<T> serializer)
    => File.WriteAllText(path, FormatCSV(serializer));

  #region IList: Contents
  /// <summary>
  ///   Get: The number of objects in this collection.
  /// </summary>
  public int Count => _Contents.Count;

  /// <summary>
  ///   Get: Whether or not this collection is read-only (false).
  /// </summary>
  bool ICollection<T>.IsReadOnly => ((ICollection<T>)_Contents).IsReadOnly;

  /// <summary>
  ///   Get or set: An item at a specific index of this collection.
  /// </summary>
  /// <param name="index">
  ///   The index at which to get or set an item.
  /// </param>
  public T this[int index] { get => _Contents[index]; set => _Contents[index] = value; }

  /// <summary>
  ///   Determines the index of a specific object in this collection.
  /// </summary>
  /// <param name="item">The item to locate.</param>
  /// <returns>The index if found in the list; otherwise -1.</returns>
  public int IndexOf(T item)
  {
    return ((IList<T>)_Contents).IndexOf(item);
  }

  /// <summary>
  ///   Inserts an item into this collection.
  /// </summary>
  /// <param name="index">The index at which to insert the item.</param>
  /// <param name="item">The item to insert.</param>
  public void Insert(int index, T item)
  {
    ((IList<T>)_Contents).Insert(index, item);
  }

  /// <summary>
  ///   Removes the item at a specific index from this collection.
  /// </summary>
  /// <param name="index">The index at which to remove the item.</param>
  public void RemoveAt(int index)
  {
    ((IList<T>)_Contents).RemoveAt(index);
  }

  /// <summary>
  ///   Adds an item to this collection.
  /// </summary>
  /// <param name="item">The item to add.</param>
  public void Add(T item)
  {
    ((ICollection<T>)_Contents).Add(item);
  }

  /// <summary>
  ///   Clears the contents of this collection.
  /// </summary>
  /// <remarks>
  ///   This does not clear the column headers from this collection.
  /// </remarks>
  public void Clear()
  {
    ((ICollection<T>)_Contents).Clear();
  }

  /// <summary>
  ///   Returns whether or not this collection contains a given object.
  /// </summary>
  /// <param name="item">The object to locate.</param>
  /// <returns><c>true</c> iff the item was found.</returns>
  public bool Contains(T item)
  {
    return ((ICollection<T>)_Contents).Contains(item);
  }

  /// <summary>
  ///   Copies the elements of this collection to an Array, starting at a
  ///   particular Array index.
  /// </summary>
  /// <param name="array">
  ///   The one-dimensional Array that is the destination of the elements
  ///   copied from this collection. The Array must have zero-based indexing.
  /// </param>
  /// <param name="arrayIndex">
  ///   The zero-based index in array at which copying begins.
  /// </param>
  public void CopyTo(T[] array, int arrayIndex)
  {
    ((ICollection<T>)_Contents).CopyTo(array, arrayIndex);
  }

  /// <summary>
  ///   Removes an item from this collection, if it's present.
  /// </summary>
  /// <param name="item">The item to remove.</param>
  /// <returns>
  ///   <c>true</c> iff the item was successfully removed from the
  ///   collection; <c>false</c> otherwise (including if it was not found).
  /// </returns>
  public bool Remove(T item)
  {
    return ((ICollection<T>)_Contents).Remove(item);
  }

  /// <summary>
  ///   Returns an enumerator over the contents of this collection.
  /// </summary>
  /// <returns>The enumerator.</returns>
  public IEnumerator<T> GetEnumerator()
  {
    return ((IEnumerable<T>)_Contents).GetEnumerator();
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

/// <summary>
///   Deserializes a row of a CSV properties into an object represented by
///   those properties.
/// </summary>
/// <typeparam name="T">The type of object being deserialized.</typeparam>
/// <param name="properties">
///   A dictionary mapping properties to values as represented in CSV
///   data. Note that a get operation on this dictionary will always
///   succeed; <c>null</c> will be returned for keys that the dictionary
///   does not contain, as well as for keys the dictionary maps to blank
///   values (<see cref="IDictionary{K,V}.ContainsKey(K)" /> can be used
///   to distinguish between these cases).
/// </param>
/// <returns>
///   The object deserialized from these properties.
/// </returns>
public delegate T CSVObjectDeserializer<T>(IDictionary<string, string?> properties);

/// <summary>
///   Serializes an object into a row of CSV properties.
/// </summary>
/// <typeparam name="T">The type of object being serialized.</typeparam>
/// <param name="obj">The object to serialize.</param>
/// <returns>
///   A dictionary mapping properties to values as represented in CSV data.
/// </returns>
public delegate IDictionary<string, string?> CSVObjectSerializer<T>(T obj);

/// <summary>
///   Contains static methods to parse a <see cref="CSVObjectCollection{T}"/>.
/// </summary>
public static class CSVObjectCollection
{
  /// <summary>
  ///   Parses objects from a CSV file into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects in the collection.
  /// </typeparam>
  /// <param name="path">The path to a CSV file.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the file.</returns>
  public static CSVObjectCollection<T> ParseObjectsFromFile<T>(string path, CSVObjectDeserializer<T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(FileUtils.FileCharEnumerator(path), deserializer);

  /// <summary>
  ///   Parses objects from a CSV stream into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects in the collection.
  /// </typeparam>
  /// <param name="reader">
  ///   A reader on the input stream of CSV data.
  /// </param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the stream.</returns>
  public static CSVObjectCollection<T> ParseObjectsFromStream<T>(StreamReader reader,
    CSVObjectDeserializer<T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(FileUtils.StreamCharEnumerator(reader), deserializer);

  /// <summary>
  ///   Parses objects from a string of CSV data into a
  ///   <see cref="CSVObjectCollection{T}"/>.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects in the collection.
  /// </typeparam>
  /// <param name="input">A string of CSV data.</param>
  /// <param name="deserializer">
  ///   The deserializer for this collection.
  /// </param>
  /// <returns>The collection of the objects in the data.</returns>
  public static CSVObjectCollection<T> ParseObjects<T>(IEnumerable<char> input, CSVObjectDeserializer<T> deserializer)
    => CSVObjectCollection<T>.ParseObjects(input, deserializer);
}

internal class PropertyDictionary(Dictionary<string, string> backing) : IDictionary<string, string?>
{
  readonly Dictionary<string, string?> Backing = backing!;

  public string? this[string key]
  {
    get => Backing.TryGetValue(key, out string? value) ? value != "" ? value : null! : null!;
    set => throw new InvalidOperationException("This dictionary is read-only.");
  }

  public ICollection<string> Keys => Backing.Keys;

  public ICollection<string?> Values => Backing.Values;

  public int Count => Backing.Count;

  public bool IsReadOnly => true;

  public void Add(string key, string? value)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public void Add(KeyValuePair<string, string?> item)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public void Clear()
    => throw new InvalidOperationException("This dictionary is read-only.");

  public bool Contains(KeyValuePair<string, string?> item)
    => Backing.Contains(item);

  public bool ContainsKey(string key)
    => Backing.ContainsKey(key);

  public void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
    => ((IDictionary<string, string?>)Backing).CopyTo(array, arrayIndex);

  public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    => Backing.GetEnumerator();

  public bool Remove(string key)
    => throw new InvalidOperationException("This dictionary is read-only.");

  public bool Remove(KeyValuePair<string, string?> item)
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