using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nixill.Collections;

/// <summary>
///   A navigable dictionary backed by an AVL tree.
/// </summary>
/// <typeparam name="K">
///   The type of keys stored in this AVLTreeDictionary.
/// </typeparam>
/// <typeparam name="V">
///   The type of values stored in this AVLTreeDictionary.
/// </typeparam>
public class AVLTreeDictionary<K, V> : INavigableDictionary<K, V>
{
  #region Fields
  private AVLTreeSet<KeyValuePair<K, V>> BackingSet;
  private Comparison<K> KeyComparer;
  private V DefaultValue = default(V)!;
  #endregion

  #region Properties
  #region Implementing IDictionary<K, V>
  /// <summary>
  ///   Get or set: The value associated with the specified key.
  /// </summary>
  /// <param name="key">The key to get or set.</param>
  /// <exception cref="KeyNotFoundException">
  ///   On a get operation, this is thrown if the specified key is not
  ///   in the dictionary.
  /// </exception>
  public V this[K key]
  {
    get
    {
      NodeTriplet<KeyValuePair<K, V>> nodes = BackingSet.SearchAround(new KeyValuePair<K, V>(key, DefaultValue));
      if (nodes.HasEqualValue)
      {
        return nodes.EqualValue.Value;
      }
      else
      {
        throw new KeyNotFoundException("The specified key was not found in the dictionary.");
      }
    }
    set
    {
      var kvp = new KeyValuePair<K, V>(key, value);
      // As a reminder, BackingSet's equality checker only checks the keys.
      if (BackingSet.Contains(kvp))
      {
        // ... and ReplaceValue doesn't check equality.
        BackingSet.ReplaceValue(kvp, kvp);
      }
      else
      {
        BackingSet.Add(kvp);
      }
    }
  }

  /// <summary>
  ///   Get: A sequence of all the keys in the dictionary.
  /// </summary>
  /// <remarks>
  ///   The keys are returned ordered.
  /// </remarks>
  public ICollection<K> Keys => BackingSet.Select(x => x.Key).ToArray();

  /// <summary>
  ///   Get: A sequence of all the values in the dictionary.
  /// </summary>
  public ICollection<V> Values => BackingSet.Select(x => x.Value).ToArray();
  #endregion

  #region Implementing ICollection<KeyValuePair<K, V>>
  /// <summary>
  ///   Get: The number of entries in the dictionary.
  /// </summary>
  public int Count => BackingSet.Count;

  /// <summary>
  ///   Get: Whether or not this dictionary is read-only (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => false;
  #endregion
  #endregion

  #region Constructors
  /// <summary>
  ///   Constructs a new, empty <see cref="AVLTreeDictionary{K, V}"/>
  ///   with the default comparer for the type <c>K</c>.
  /// </summary>
  public AVLTreeDictionary() : this([], GetComparer()) { }

  /// <summary>
  ///   Constructs a new, empty <see cref="AVLTreeDictionary{K, V}"/>
  ///   with the specified comparer for the type <c>K</c>.
  /// </summary>
  /// <param name="comparer">
  ///   Comparer between the keys of this dictionary.
  /// </param>
  public AVLTreeDictionary(IComparer<K> comparer) : this([], comparer.Compare) { }

  /// <summary>
  ///   Constructs a new, empty <see cref="AVLTreeDictionary{K, V}"/>
  ///   with the specified comparison function for the type <c>K</c>.
  /// </summary>
  /// <param name="comparer">
  ///   Comparison function between the keys of this dictionary.
  /// </param>
  public AVLTreeDictionary(Comparison<K> comparer) : this([], comparer) { }

  /// <summary>
  ///   Constructs a <see cref="AVLTreeDictionary{K, V}"/> with the
  ///   given entries and the default comparer for the type <c>K</c>.
  /// </summary>
  /// <param name="elems">
  ///   The entries with which the dictionary should initially be populated.
  /// </param>
  public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems) : this(elems, GetComparer()) { }

  /// <summary>
  ///   Constructs a <see cref="AVLTreeDictionary{K, V}"/> with the
  ///   given entries and comparer for the type <c>K</c>.
  /// </summary>
  /// <param name="elems">
  ///   The entries with which the dictionary should initially be populated.
  /// </param>
  /// <param name="comparer">
  ///   Comparer between the keys of this dictionary.
  /// </param>
  public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems, IComparer<K> comparer) : this(elems, comparer.Compare) { }

  /// <summary>
  ///   Constructs a <see cref="AVLTreeDictionary{K, V}"/> with the
  ///   given entries and comparison function for the type <c>K</c>.
  /// </summary>
  /// <param name="elems">
  ///   The entries with which the dictionary should initially be populated.
  /// </param>
  /// <param name="comparer">
  ///   Comparison function between the keys of this dictionary.
  /// </param>
  public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems, Comparison<K> comparer)
  {
    this.KeyComparer = comparer;
    BackingSet = new AVLTreeSet<KeyValuePair<K, V>>(elems, (left, right) => KeyComparer(left.Key, right.Key));
  }
  #endregion

  #region Public Methods
  /// <summary>
  ///   Whether or not the dictionary contains any entries.
  /// </summary>
  /// <returns>
  ///   <c>true</c> iff the dictionary is empty; <c>false</c> otherwise.
  /// </returns>
  public bool IsEmpty() => BackingSet.IsEmpty();

  /// <summary>
  ///   Gets the entries "near" a given key.
  /// </summary>
  /// <remarks>
  ///   This specifically returns a triplet that contains, if they
  ///   exist, the entry with a key equal to the given value, the entry
  ///   with the highest key less than the given value, and the entry
  ///   with the lowest key greater than the given value.
  /// </remarks>
  /// <param name="from">The value to search near.</param>
  /// <returns>The triplet.</returns>
  public NodeTriplet<KeyValuePair<K, V>> EntriesAround(K from) => BackingSet.SearchAround(new KeyValuePair<K, V>(from, default(V)!));

  /// <summary>
  ///   Gets the keys "near" a given key.
  /// </summary>
  /// <remarks>
  ///   This specifically returns a triplet that contains, if they
  ///   exist, the key equal to the given value, the highest key less
  ///   than the given value, and the lowest key greater than the given
  ///   value.
  /// </remarks>
  /// <param name="from">The value to search near.</param>
  /// <returns>The triplet.</returns>
  public NodeTriplet<K> KeysAround(K from)
  {
    var entries = EntriesAround(from);
    var lesser = (entries.HasLesserValue) ? new AVLTreeSet<K>.Node<K> { Data = entries.LesserValue.Key } : null;
    var equal = (entries.HasEqualValue) ? new AVLTreeSet<K>.Node<K> { Data = entries.EqualValue.Key } : null;
    var greater = (entries.HasGreaterValue) ? new AVLTreeSet<K>.Node<K> { Data = entries.GreaterValue.Key } : null;
    return new NodeTriplet<K>((lesser, equal, greater));
  }
  #endregion

  #region Interface Implementations
  #region INavigableDictionary<K, V>
  /// <summary>
  ///   Returns whether or not an entry exists with a key less than the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns><c>true</c> iff any lower entry exists.</returns>
  public bool ContainsLower(K from) => TryGetLowerEntry(from, out var placeholder);

  /// <summary>
  ///   Attempts to retrieve the entry with the highest key less than
  ///   the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="entry">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   entry found. Otherwise, it contains default values for the types.
  /// </param>
  /// <returns><c>true</c> iff a lower entry exists.</returns>
  public bool TryGetLowerEntry(K from, out KeyValuePair<K, V> entry) =>
    BackingSet.TryGetLower(new KeyValuePair<K, V>(from, DefaultValue), out entry);

  /// <summary>
  ///   Attempts to retrieve the highest key less than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="key">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   key found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff a lower key exists.</returns>
  public bool TryGetLowerKey(K from, out K key)
  {
    bool res = BackingSet.TryGetLower(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
    key = entry.Key;
    return res;
  }

  /// <summary>
  ///   Returns the highest key less than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The lower key.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is less than or
  ///   equal to the lowest key in the dictionary.
  /// </exception>
  public K LowerKey(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Lower keys in an empty AVLTreeDictionary.");
    if (TryGetLowerKey(from, out K key))
    {
      return key;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Lower key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns the entry with the highest key less than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The lower entry.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is less than or
  ///   equal to the lowest key in the dictionary.
  /// </exception>
  public KeyValuePair<K, V> LowerEntry(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Lower keys in an empty AVLTreeDictionary.");
    if (TryGetLowerEntry(from, out KeyValuePair<K, V> entry))
    {
      return entry;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Lower key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns whether or not an entry exists with a key less than or
  ///   equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>
  ///   <c>true</c> iff any lower or equal entry exists.
  /// </returns>
  public bool ContainsFloor(K from) => TryGetFloorEntry(from, out var placeholder);

  /// <summary>
  ///   Attempts to retrieve the entry with the highest key less than or
  ///   equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="entry">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   entry found. Otherwise, it contains default values for the types.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff any lower or equal entry exists.
  /// </returns>
  public bool TryGetFloorEntry(K from, out KeyValuePair<K, V> entry) =>
    BackingSet.TryGetFloor(new KeyValuePair<K, V>(from, DefaultValue), out entry);

  /// <summary>
  ///   Attempts to retrieve the highest key less than or equal to the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="key">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   key found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff any lower or equal key exists.</returns>
  public bool TryGetFloorKey(K from, out K key)
  {
    bool res = BackingSet.TryGetFloor(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
    key = entry.Key;
    return res;
  }

  /// <summary>
  ///   Returns the highest key less than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The lower or equal key.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is less than the
  ///   lowest key in the dictionary.
  /// </exception>
  public K FloorKey(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Floor keys in an empty AVLTreeDictionary.");
    if (TryGetFloorKey(from, out K key))
    {
      return key;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Floor key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns the entry with the highest key less than or equal to the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The lower or equal entry.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is less than the
  ///   lowest key in the dictionary.
  /// </exception>
  public KeyValuePair<K, V> FloorEntry(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Floor keys in an empty AVLTreeDictionary.");
    if (TryGetFloorEntry(from, out KeyValuePair<K, V> entry))
    {
      return entry;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Floor key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns whether or not an entry exists with a key greater than
  ///   or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>
  ///   <c>true</c> iff any higher or equal entry exists.
  /// </returns>
  public bool ContainsCeiling(K from) => TryGetCeilingEntry(from, out var placeholder);

  /// <summary>
  ///   Attempts to retrieve the entry with the lowest key greater than
  ///   or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="entry">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   entry found. Otherwise, it contains default values for the types.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff any higher or equal entry exists.
  /// </returns>
  public bool TryGetCeilingEntry(K from, out KeyValuePair<K, V> entry) =>
    BackingSet.TryGetCeiling(new KeyValuePair<K, V>(from, DefaultValue), out entry);

  /// <summary>
  ///   Attempts to retrieve the lowest key greater than or equal to the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="key">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   key found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff any higher or equal key exists.</returns>
  public bool TryGetCeilingKey(K from, out K key)
  {
    bool res = BackingSet.TryGetCeiling(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
    key = entry.Key;
    return res;
  }

  /// <summary>
  ///   Returns the lowest key greater than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The higher or equal key.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is greater than the
  ///   highest key in the dictionary.
  /// </exception>
  public K CeilingKey(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Ceiling keys in an empty AVLTreeDictionary.");
    if (TryGetCeilingKey(from, out K key))
    {
      return key;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Ceiling key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns the entry with the lowest key greater than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The higher or equal entry.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is greater than the
  ///   highest key in the dictionary.
  /// </exception>
  public KeyValuePair<K, V> CeilingEntry(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Ceiling keys in an empty AVLTreeDictionary.");
    if (TryGetCeilingEntry(from, out KeyValuePair<K, V> entry))
    {
      return entry;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Ceiling key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns whether or not an entry exists with a key greater than
  ///   the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns><c>true</c> iff any higher entry exists.</returns>
  public bool ContainsHigher(K from) => TryGetHigherEntry(from, out var placeholder);

  /// <summary>
  ///   Attempts to retrieve the entry with the lowest key greater than
  ///   the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="entry">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   entry found. Otherwise, it contains default values for the types.
  /// </param>
  /// <returns><c>true</c> iff any higher entry exists.</returns>
  public bool TryGetHigherEntry(K from, out KeyValuePair<K, V> entry) =>
    BackingSet.TryGetHigher(new KeyValuePair<K, V>(from, DefaultValue), out entry);

  /// <summary>
  ///   Attempts to retrieve the lowest key greater than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <param name="key">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   key found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff any higher key exists.</returns>
  public bool TryGetHigherKey(K from, out K key)
  {
    bool res = BackingSet.TryGetHigher(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
    key = entry.Key;
    return res;
  }

  /// <summary>
  ///   Returns the lowest key greater than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The higher key.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is greater than or
  ///   equal to the highest key in the dictionary.
  /// </exception>
  public K HigherKey(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Higher keys in an empty AVLTreeDictionary.");
    if (TryGetHigherKey(from, out K key))
    {
      return key;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Higher key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns the entry with the lowest key greater than the given value.
  /// </summary>
  /// <param name="from">The value to find a key near.</param>
  /// <returns>The higher entry.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The dictionary is not empty, but <c>from</c> is greater than or
  ///   equal to the highest key in the dictionary.
  /// </exception>
  public KeyValuePair<K, V> HigherEntry(K from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have Higher keys in an empty AVLTreeDictionary.");
    if (TryGetHigherEntry(from, out KeyValuePair<K, V> entry))
    {
      return entry;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no Higher key in the AVLTreeDictionary.");
  }

  /// <summary>
  ///   Returns the entry with the lowest key in the dictionary.
  /// </summary>
  /// <returns>The lowest entry.</returns>
  public KeyValuePair<K, V> LowestEntry() => BackingSet.LowestValue();

  /// <summary>
  ///   Returns the lowest key in the dictionary.
  /// </summary>
  /// <returns>The lowest key.</returns>
  public K LowestKey() => BackingSet.LowestValue().Key;

  /// <summary>
  ///   Returns the entry with the highest key in the dictionary.
  /// </summary>
  /// <returns>The highest entry.</returns>
  public KeyValuePair<K, V> HighestEntry() => BackingSet.HighestValue();

  /// <summary>
  ///   Returns the highest key in the dictionary.
  /// </summary>
  /// <returns>The highest key.</returns>
  public K HighestKey() => BackingSet.HighestValue().Key;
  #endregion

  #region IDictionary<K, V>
  /// <summary>
  ///   Adds the given key and value to the dictionary.
  /// </summary>
  /// <param name="key">The key to add.</param>
  /// <param name="value">The value to add.</param>
  /// <exception cref="ArgumentException">
  ///   The specified <c>key</c> is already present in the dictionary.
  /// </exception>
  public void Add(K key, V value)
  {
    var kvp = new KeyValuePair<K, V>(key, value);
    // As a reminder, BackingSet's equality checker only checks the keys.
    if (BackingSet.Contains(kvp))
    {
      throw new ArgumentException("The specified key is already in use.");
    }
    else
    {
      BackingSet.Add(kvp);
    }
  }

  /// <summary>
  ///   Returns whether or not the dictionary contains the given key.
  /// </summary>
  /// <param name="key">The key to find.</param>
  /// <returns>
  ///   <c>true</c> iff the key was found in the dictionary.
  /// </returns>
  public bool ContainsKey(K key) => BackingSet.Contains(new KeyValuePair<K, V>(key, DefaultValue));

  /// <summary>
  ///   Removes the given key from the dictionary.
  /// </summary>
  /// <param name="key">The key to remove.</param>
  /// <returns>
  ///   <c>true</c> iff the key was removed from the dictionary,
  ///   otherwise <c>false</c> (which may indicate that the key was not
  ///   present to begin with).
  /// </returns>
  public bool Remove(K key) => BackingSet.Remove(new KeyValuePair<K, V>(key, DefaultValue));

  /// <summary>
  ///   Attempts to retrieve the value for the given key.
  /// </summary>
  /// <param name="key">The key to retrieve.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   key found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns>Whether or not the key was found.</returns>
  public bool TryGetValue(K key, out V value)
  {
    NodeTriplet<KeyValuePair<K, V>> nodes = BackingSet.SearchAround(new KeyValuePair<K, V>(key, DefaultValue));
    if (nodes.HasEqualValue)
    {
      value = nodes.EqualValue.Value;
      return true;
    }
    else
    {
      value = default(V)!;
      return false;
    }
  }
  #endregion

  #region ICollection<KeyValuePair<K, V>>
  /// <summary>
  ///   Adds the given entry to the dictionary.
  /// </summary>
  /// <remarks>
  ///   If another entry in the dictionary has the same key, that entry
  ///   is overwritten.
  /// </remarks>
  /// <param name="entry">The entry to add.</param>
  public void Add(KeyValuePair<K, V> entry)
  {
    // As a reminder, BackingSet's equality checker only checks the keys.
    if (BackingSet.Contains(entry))
    {
      // ... and ReplaceValue doesn't check equality.
      BackingSet.ReplaceValue(entry, entry);
    }
    else
    {
      BackingSet.Add(entry);
    }
  }

  /// <summary>
  ///   Removes all entries from this dictionary.
  /// </summary>
  public void Clear() => BackingSet.Clear();

  /// <summary>
  ///   Returns whether or not this dictionary contains an entry with
  ///   the given key and value.
  /// </summary>
  /// <param name="entry">The entry to find.</param>
  /// <returns>
  ///   <c>true</c> iff the given key is found and associated with the
  ///   given value.
  /// </returns>
  public bool Contains(KeyValuePair<K, V> entry)
  {
    var nodes = BackingSet.SearchAround(entry);
    return (nodes.HasEqualValue && nodes.EqualValue.Value!.Equals(entry.Value));
  }

  /// <summary>
  ///   Copies this dictionary's entries to a one-dimensional array.
  /// </summary>
  /// <param name="array">The destination array.</param>
  /// <param name="index">
  ///   The index within the array to start copying.
  /// </param>
  public void CopyTo(KeyValuePair<K, V>[] array, int index) => BackingSet.CopyTo(array, index);

  /// <summary>
  ///   Removes an entry from this dictionary.
  /// </summary>
  /// <param name="entry">The entry to remove.</param>
  /// <returns>
  ///   <c>true</c> iff an entry was removed from the dictionary,
  ///   <c>false</c> otherwise (which may indicate that the key was not
  ///   found or that the value did not match).
  /// </returns>
  public bool Remove(KeyValuePair<K, V> entry)
  {
    if (Contains(entry)) return BackingSet.Remove(entry);
    else return false;
  }
  #endregion

  #region IEnumerable<KeyValuePair<K, V>>
  /// <inheritdoc/>
  public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => BackingSet.GetEnumerator();
  #endregion

  #region IEnumerable
  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => BackingSet.GetEnumerator();
  #endregion
  #endregion

  #region Private Methods
  private static Comparison<K> GetComparer()
  {
    if (typeof(IComparable<K>).IsAssignableFrom(typeof(K)) || typeof(System.IComparable).IsAssignableFrom(typeof(K)))
    {
      return Comparer<K>.Default.Compare;
    }
    else
    {
      throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The type {0} cannot be compared. It must implement IComparable<T> or IComparable interface", typeof(K).FullName));
    }
  }
  #endregion
}
