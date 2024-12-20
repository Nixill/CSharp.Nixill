using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nixill.Utils.Extensions;

namespace Nixill.Collections;

/// <summary>
///   A dictionary whose keys are enumerable sequences, stored as a tree
///   of those sequences.
/// </summary>
/// <typeparam name="K">
///   The type of elements in the sequences that form keys in the dictionary.
///   <para/>
///   Note that this is specifically the <em>elements of the
///   sequence</em>. For example, you would write <c>char</c> here if you
///   want to have a dictionary of <c>string</c>s, or <c>int</c> if you
///   want a dictionary of <c>int[]</c>s.
/// </typeparam>
/// <typeparam name="V">
///   The type of values in the dictionary.
/// </typeparam>
public class RecursiveDictionary<K, V> : IRecursiveDictionary<K, V> where K : notnull
{
  /// <summary>
  ///   Creates a new, empty <see cref="RecursiveDictionary{K, V}"/>.
  /// </summary>
  public RecursiveDictionary() : this([], EqualityComparer<K>.Default) { }

  /// <summary>
  ///   Creates a new <see cref="RecursiveDictionary{K, V}"/> with the
  ///   given entries.
  /// </summary>
  /// <param name="entries">The initial entries.</param>
  public RecursiveDictionary(IEnumerable<KeyValuePair<IEnumerable<K>, V>> entries) : this(entries, EqualityComparer<K>.Default) { }

  /// <summary>
  ///   Creates a new, empty <see cref="RecursiveDictionary{K, V}"/> with
  ///   the given key element comparer.
  /// </summary>
  /// <param name="comparer">The comparer.</param>
  public RecursiveDictionary(IEqualityComparer<K> comparer) : this([], comparer) { }

  /// <summary>
  ///   Creates a new <see cref="RecursiveDictionary{K, V}"/> with the
  ///   given entries and key element comparer.
  /// </summary>
  /// <param name="entries">The initial entries.</param>
  /// <param name="comparer">The comparer.</param>
  public RecursiveDictionary(IEnumerable<KeyValuePair<IEnumerable<K>, V>> entries, IEqualityComparer<K> comparer)
  {
    Children = new(comparer);
    Comparer = comparer;
    foreach (var entry in entries) Add(entry);
  }

  /// <summary>
  ///   Get: Whether or not this <see cref="RecursiveDictionary{K, V}"/>
  ///   has a value for the empty-sequence key.
  /// </summary>
  public bool HasEmptyKeyValue { get; private set; } = false;

  V? DirectValue = default(V);
  Dictionary<K, RecursiveDictionary<K, V>> Children;

  /// <summary>
  ///   Get: The comparer that checks for equality between key elements.
  /// </summary>
  public IEqualityComparer<K> Comparer { get; }

  /// <summary>
  ///   Get: The number of items contained by this
  ///   <see cref="RecursiveDictionary{K, V}"/>, including the empty-key
  ///   value (if applicable) and all children (if any).
  /// </summary>
  public int Count { get; private set; } = 0;

  /// <summary>
  ///   Get or set: The value associated with a specified key.
  /// </summary>
  /// <param name="key">The key.</param>
  /// <exception cref="ArgumentNullException">
  ///   <c>key</c> is <c>null</c>.
  /// </exception>
  /// <exception cref="KeyNotFoundException">
  ///   The provided key was not found in this dictionary.
  /// </exception>
  public V this[IEnumerable<K> key]
  {
    get
    {
      if (key == null) throw new ArgumentNullException("key");

      IEnumerator<K> enumerator = key.GetEnumerator();
      if (RecursivelyTryGet(enumerator, out V? value))
      {
        return value;
      }
      else
      {
        throw new KeyNotFoundException();
      }
    }
    set
    {
      if (key == null) throw new ArgumentNullException("key");

      IEnumerator<K> enumerator = key.GetEnumerator();
      RecursivelySet(enumerator, value);
    }
  }

  /// <summary>
  ///   Get: A collection of the keys in this dictionary.
  /// </summary>
  public ICollection<IEnumerable<K>> Keys => KeysEnumerable().ToList();

  /// <summary>
  ///   Get: A collection of the values in this dictionary.
  /// </summary>
  public ICollection<V> Values => ValuesEnumerable().ToList();

  /// <summary>
  ///   Get: Whether or not this dictionary is read-only (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => false;

  /// <summary>
  ///   Get: A collection of the first-element prefixes in this dictionary.
  /// </summary>
  public ICollection<K> Prefixes => Children.Keys;

  /// <summary>
  ///   Adds the given key and value to the dictionary if the key is not
  ///   already in use.
  /// </summary>
  /// <param name="key">The key to add.</param>
  /// <param name="value">The value to add.</param>
  /// <exception cref="ArgumentNullException">
  ///   <c>key</c> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   <c>key</c> is already present in the dictionary.
  /// </exception>
  public void Add(IEnumerable<K> key, V value)
  {
    if (key == null) throw new ArgumentNullException("key");
    if (!RecursivelyAdd(key.GetEnumerator(), value)) throw new ArgumentException($"The dictionary already contains the key \"{key}\".");
  }

  /// <summary>
  ///   Adds the given key-value pair to the collection even if this
  ///   overwrites another value.
  /// </summary>
  /// <param name="item">The item to add.</param>
  public void Add(KeyValuePair<IEnumerable<K>, V> item)
    => RecursivelyAdd(item.Key.GetEnumerator(), item.Value, true);

  /// <summary>
  ///   Clears the collection.
  /// </summary>
  public void Clear()
  {
    HasEmptyKeyValue = false;
    DirectValue = default(V);
    Children.Clear();
    Count = 0;
  }

  /// <summary>
  ///   Whether the collection contains the given value.
  /// </summary>
  /// <param name="item">The key-value pair to find.</param>
  /// <returns>
  ///   <c>true</c> iff the dictionary contains the given key, and the
  ///   associated value matches the given value.
  /// </returns>
  public bool Contains(KeyValuePair<IEnumerable<K>, V> item)
  {
    if (item.Key == null) return false;
    return RecursivelyTryGet(item.Key.GetEnumerator(), out var value) && object.Equals(item.Value, value);
  }

  /// <summary>
  ///   Whether the collection contains the given key.
  /// </summary>
  /// <param name="key">The key to find.</param>
  /// <returns>
  ///   <c>true</c> iff the dictionary contains the given key.
  /// </returns>
  public bool ContainsKey(IEnumerable<K> key)
  {
    if (key == null) return false;
    return RecursivelyTryGet(key.GetEnumerator(), out var _);
  }

  /// <summary>
  ///   Whether the dictionary contains any keys with the given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>
  ///   <c>true</c> iff at least one key in the dictionary starts with
  ///   this prefix.
  /// </returns>
  public bool ContainsPrefix(IEnumerable<K> key)
  {
    if (key == null) return false;
    return RecursivelyContainsPrefix(key.GetEnumerator());
  }

  /// <inheritdoc/>
  public void CopyTo(KeyValuePair<IEnumerable<K>, V>[] array, int arrayIndex)
  {
    foreach (var kvp in this) array[arrayIndex++] = kvp;
  }

  /// <summary>
  ///   Returns the collection as an <see cref="IEnumerable{T}"/>.
  /// </summary>
  /// <returns>The enumerable.</returns>
  public IEnumerable<KeyValuePair<IEnumerable<K>, V>> Enumerate()
  {
    if (HasEmptyKeyValue) yield return new(Enumerable.Empty<K>(), DirectValue!);

    foreach (var child in Children)
    {
      foreach (var kvp in child.Value.Enumerate())
      {
        yield return new(kvp.Key.Prepend(child.Key), kvp.Value);
      }
    }
  }

  /// <summary>
  ///   Returns the enumerator over the collection.
  /// </summary>
  /// <returns>The enumerator.</returns>
  public IEnumerator<KeyValuePair<IEnumerable<K>, V>> GetEnumerator()
  {
    return Enumerate().GetEnumerator();
  }

  /// <summary>
  ///   Gets the sub-dictionary for a given prefix.
  /// </summary>
  /// <remarks>
  ///   The sub-dictionary is a linked view. It is read-only, but changes
  ///   to the original are reflected in the view. It becomes unlinked if
  ///   emptied.
  /// </remarks>
  /// <param name="prefix">The prefix.</param>
  /// <returns>The sub-dictionary.</returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  /// <exception cref="KeyNotFoundException">
  ///   The given prefix does not exist in the dictionary.
  /// </exception>
  public IRecursiveDictionary<K, V> GetPrefix(IEnumerable<K> prefix)
  {
    if (prefix == null) throw new ArgumentNullException("prefix");
    if (!RecursivelyTryGetPrefix(prefix.GetEnumerator(), out var dict)) throw new KeyNotFoundException();
    return dict;
  }

  /// <summary>
  ///   Removes the given key from the dictionary.
  /// </summary>
  /// <param name="key">The key to remove.</param>
  /// <returns>
  ///   <c>true</c> if an entry was actually removed from the dictionary
  ///   (where <c>false</c> probably indicates that the entry did not
  ///   exist in the first place).
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>key</c> is <c>null</c>.
  /// </exception>
  public bool Remove(IEnumerable<K> key)
  {
    if (key == null) throw new ArgumentNullException("key");

    if (RecursivelyRemove(key.GetEnumerator(), v => true))
    {
      Count -= 1;
      return true;
    }

    return false;
  }

  /// <summary>
  ///   Removes the given key-value pair from the dictionary.
  /// </summary>
  /// <param name="item">The item to remove.</param>
  /// <returns>
  ///   <c>true</c> if an entry was actually removed from the dictionary
  ///   (where <c>false</c> probably indicates that the entry did not
  ///   exist or match in the first place).
  /// </returns>
  public bool Remove(KeyValuePair<IEnumerable<K>, V> item)
  {
    if (RecursivelyRemove(item.Key.GetEnumerator(), v => object.Equals(v, item.Value)))
    {
      Count -= 1;
      return true;
    }

    return false;
  }

  public int RemovePrefix(IEnumerable<K> prefix)
  {
    if (prefix == null) throw new ArgumentNullException("prefix");

    int removed = RecursivelyRemovePrefix(prefix.GetEnumerator());
    Count -= removed;
    return removed;
  }

  /// <summary>
  ///   Tries to get the sub-dictionary for a given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <param name="result">
  ///   If this method returns <c>true</c>, this is the sub-dictionary for
  ///   the given prefix. Modifications to this dictionary are reflected
  ///   in the original. Otherwise, this is <c>null</c>.
  /// </param>
  /// <returns>
  ///   Whether or not the dictionary contains the given prefix.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public bool TryGetPrefix(IEnumerable<K> key, [MaybeNullWhen(false)] out IRecursiveDictionary<K, V> dict)
  {
    if (key == null) throw new ArgumentNullException("key");

    return RecursivelyTryGetPrefix(key.GetEnumerator(), out dict);
  }

  /// <summary>
  ///   Attempts to get the value associated with the given key.
  /// </summary>
  /// <param name="key">The key to get.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this is the value associated
  ///   with the given key. Otherwise, it's <c>default(V)</c>.
  /// </param>
  /// <returns>Whether or not the value was found.</returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>key</c> is <c>null</c>.
  /// </exception>
  public bool TryGetValue(IEnumerable<K> key, [MaybeNullWhen(false)] out V value)
  {
    if (key == null) throw new ArgumentNullException("key");

    return RecursivelyTryGet(key.GetEnumerator(), out value);
  }

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator()
  {
    throw new NotImplementedException();
  }

  IEnumerable<IEnumerable<K>> KeysEnumerable()
  {
    if (HasEmptyKeyValue) yield return [];

    foreach (var child in Children)
    {
      foreach (var key in child.Value.Keys)
      {
        yield return key.Prepend(child.Key);
      }
    }
  }

  IEnumerable<V> ValuesEnumerable()
  {
    if (HasEmptyKeyValue) yield return DirectValue!;

    foreach (var child in Children)
    {
      foreach (var value in child.Value.Values)
      {
        yield return value;
      }
    }
  }

  bool RecursivelyTryGet(IEnumerator<K> enumerator, [MaybeNullWhen(false)] out V value)
  {
    if (!enumerator.MoveNext())
    {
      value = DirectValue;
      return HasEmptyKeyValue;
    }

    K key = enumerator.Current;
    if (!Children.TryGetValue(key, out var child))
    {
      value = default(V);
      return false;
    }

    return child.RecursivelyTryGet(enumerator, out value);
  }

  // return value indicates item added
  bool RecursivelySet(IEnumerator<K> enumerator, V value)
  {
    if (!enumerator.MoveNext())
    {
      bool itemAdded = !HasEmptyKeyValue;
      HasEmptyKeyValue = true;
      DirectValue = value;
      Count += 1;
      return itemAdded;
    }

    var child = Children.GetOrSet(enumerator.Current, () => new());

    if (child.RecursivelySet(enumerator, value))
    {
      Count += 1;
      return true;
    }
    else return false;
  }

  // return value indicates item added
  bool RecursivelyAdd(IEnumerator<K> enumerator, V value, bool overwrite = false)
  {
    if (!enumerator.MoveNext())
    {
      bool newValue = !HasEmptyKeyValue;
      if (newValue || overwrite)
      {
        HasEmptyKeyValue = true;
        DirectValue = value;
      }
      return newValue;
    }

    var child = Children.GetOrSet(enumerator.Current, () => new());

    if (child.RecursivelyAdd(enumerator, value))
    {
      Count += 1;
      return true;
    }
    else return false;
  }

  bool RecursivelyRemove(IEnumerator<K> enumerator, Predicate<V> condition)
  {
    if (!enumerator.MoveNext())
    {
      if (!HasEmptyKeyValue) return false;
      if (!condition(DirectValue!)) return false;

      HasEmptyKeyValue = false;
      DirectValue = default(V);
      Count -= 1;
      return true;
    }

    K key = enumerator.Current;

    if (!Children.TryGetValue(key, out var child)) return false;

    if (child.RecursivelyRemove(enumerator, condition))
    {
      Count -= 1;
      if (child.Count == 0) Children.Remove(key);
      return true;
    }

    return false;
  }

  bool RecursivelyContainsPrefix(IEnumerator<K> enumerator)
  {
    if (!enumerator.MoveNext())
    {
      return true;
    }

    K key = enumerator.Current;

    if (!Children.TryGetValue(key, out var child))
    {
      return false;
    }

    return child.RecursivelyContainsPrefix(enumerator);
  }

  bool RecursivelyTryGetPrefix(IEnumerator<K> enumerator, [MaybeNullWhen(false)] out IRecursiveDictionary<K, V> output)
  {
    if (!enumerator.MoveNext())
    {
      output = new RecursiveDictionary<K, V>(this);
      return true;
    }

    K key = enumerator.Current;

    if (!Children.TryGetValue(key, out var child))
    {
      output = null;
      return false;
    }

    return child.RecursivelyTryGetPrefix(enumerator, out output);
  }

  int RecursivelyRemovePrefix(IEnumerator<K> enumerator)
  {
    int removed;

    if (!enumerator.MoveNext())
    {
      removed = Count;
      Clear();
      return removed;
    }

    K key = enumerator.Current;

    if (!Children.TryGetValue(key, out var child))
    {
      return 0;
    }

    removed = child.RecursivelyRemovePrefix(enumerator);
    if (child.Count == 0) Children.Remove(key);
    Count -= removed;
    return removed;
  }
}
