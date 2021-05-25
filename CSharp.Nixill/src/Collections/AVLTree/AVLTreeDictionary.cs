using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nixill.Collections {
  /// <summary>
  ///   This is an implementation of <see cref="IDictionary<T>" /> backed
  ///   by an AVL tree.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Much of the code from this class comes from Costin S and was
  ///     licensed under the MIT License. The original code is available
  ///     <see href="https://code.google.com/archive/p/self-balancing-avl-tree/">here</see>.
  ///   </para>
  ///   <para>
  ///     This class doesn't include the sections of code under the
  ///     <c>TREE_WITH_PARENT_POINTERS</c> or
  ///     <c>TREE_WITH_CONCAT_AND_SPLIT_OPERATIONS</c> defines.
  ///   </para>
  /// </remarks>
  /// <typeparam name="K">
  ///   The type of the keys stored in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  ///   The type of the values stored in the dictionary.
  /// </typeparam>
  public class AVLTreeDictionary<K, V> : INavigableDictionary<K, V> {
    #region Fields
    private AVLTreeSet<KeyValuePair<K, V>> BackingSet;
    private Comparison<K> KeyComparer;
    private V DefaultValue = default(V);
    #endregion

    #region Properties
    #region Implementing IDictionary<K, V>
    public V this[K key] {
      get {
        AVLTreeSet<KeyValuePair<K, V>>.NodeTriplet nodes = BackingSet.SearchAround(new KeyValuePair<K, V>(key, DefaultValue));
        if (nodes.HasEqualValue) {
          return nodes.EqualValue.Value;
        }
        else {
          throw new KeyNotFoundException("The specified key was not found in the dictionary.");
        }
      }
      set {
        var kvp = new KeyValuePair<K, V>(key, value);
        // As a reminder, BackingSet's equality checker only checks the keys.
        if (BackingSet.Contains(kvp)) {
          // ... and ReplaceValue doesn't check equality.
          BackingSet.ReplaceValue(kvp, kvp);
        }
        else {
          BackingSet.Add(kvp);
        }
      }
    }

    public ICollection<K> Keys => BackingSet.Select(x => x.Key).ToArray();
    public ICollection<V> Values => BackingSet.Select(x => x.Value).ToArray();
    #endregion

    #region Implementing ICollection<KeyValuePair<K, V>>
    public int Count => BackingSet.Count;
    public bool IsReadOnly => false;
    #endregion
    #endregion

    #region Constructors
    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using the type's
    ///   default <see cref="Comparer&lt;T&gt;"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The type isn't naturally comparable.
    /// </exception>
    public AVLTreeDictionary() : this(null, GetComparer()) { }

    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using a specified
    ///   <see cref="IComparer&lt;T&gt;"/>.
    /// </summary>
    /// <param name="comparer">
    ///   The comparer that compares the elements of the set.
    /// </param>
    public AVLTreeDictionary(IComparer<K> comparer) : this(null, comparer.Compare) { }

    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using a specified
    ///   comparison function.
    /// </summary>
    /// <param name="comparer">
    ///   The function that compares the elements of the set.
    /// </param>
    public AVLTreeDictionary(Comparison<K> comparer) : this(null, comparer) { }

    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using the type's
    ///   default <see cref="Comparer&lt;T&gt;"/> and a pre-existing set
    ///   of elements.
    /// </summary>
    /// <param name="elems">
    ///   The elements with which to pre-populate the set.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   The type isn't naturally comparable.
    /// </exception>
    public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems) : this(elems, GetComparer()) { }

    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using a specified
    ///   <see cref="IComparer&lt;T&gt;"/> and a pre-existing set of elements.
    /// </summary>
    /// <param name="elems">
    ///   The elements with which to pre-populate the set.
    /// </param>
    /// <param name="comparer">
    ///   The comparer that compares the elements of the set.
    /// </param>
    public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems, IComparer<K> comparer) : this(elems, comparer.Compare) { }

    /// <summary>
    ///   Initializes a new instance of the
    ///   <see cref="AVLTreeSet&lt;T&gt;"/> class, using a specified
    ///   comparison function and a pre-existing set of elements.
    /// </summary>
    /// <param name="elems">
    ///   The elements with which to pre-populate the set.
    /// </param>
    /// <param name="comparer">
    ///   The function that compares the elements of the set.
    /// </param>
    public AVLTreeDictionary(IEnumerable<KeyValuePair<K, V>> elems, Comparison<K> comparer) {
      this.KeyComparer = comparer;
      BackingSet = new AVLTreeSet<KeyValuePair<K, V>>((left, right) => KeyComparer(left.Key, right.Key));

      if (elems != null) {
        foreach (var elem in elems) {
          this.Add(elem);
        }
      }
    }
    #endregion

    #region Public Methods
    public bool IsEmpty() => BackingSet.IsEmpty();
    #endregion

    #region Interface Implementations
    #region INavigableDictionary<K, V>
    public bool ContainsLower(K from) => TryGetLowerEntry(from, out var placeholder);

    public bool TryGetLowerEntry(K from, out KeyValuePair<K, V> entry) =>
      BackingSet.TryGetLower(new KeyValuePair<K, V>(from, DefaultValue), out entry);

    public bool TryGetLowerKey(K from, out K key) {
      bool res = BackingSet.TryGetLower(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
      key = entry.Key;
      return res;
    }

    public K LowerKey(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Lower keys in an empty AVLTreeDictionary.");
      if (TryGetLowerKey(from, out K key)) {
        return key;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Lower key in the AVLTreeDictionary.");
    }

    public KeyValuePair<K, V> LowerEntry(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Lower keys in an empty AVLTreeDictionary.");
      if (TryGetLowerEntry(from, out KeyValuePair<K, V> entry)) {
        return entry;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Lower key in the AVLTreeDictionary.");
    }

    public bool ContainsFloor(K from) => TryGetFloorEntry(from, out var placeholder);

    public bool TryGetFloorEntry(K from, out KeyValuePair<K, V> entry) =>
      BackingSet.TryGetFloor(new KeyValuePair<K, V>(from, DefaultValue), out entry);

    public bool TryGetFloorKey(K from, out K key) {
      bool res = BackingSet.TryGetFloor(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
      key = entry.Key;
      return res;
    }

    public K FloorKey(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Floor keys in an empty AVLTreeDictionary.");
      if (TryGetFloorKey(from, out K key)) {
        return key;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Floor key in the AVLTreeDictionary.");
    }

    public KeyValuePair<K, V> FloorEntry(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Floor keys in an empty AVLTreeDictionary.");
      if (TryGetFloorEntry(from, out KeyValuePair<K, V> entry)) {
        return entry;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Floor key in the AVLTreeDictionary.");
    }

    public bool ContainsCeiling(K from) => TryGetCeilingEntry(from, out var placeholder);

    public bool TryGetCeilingEntry(K from, out KeyValuePair<K, V> entry) =>
      BackingSet.TryGetCeiling(new KeyValuePair<K, V>(from, DefaultValue), out entry);

    public bool TryGetCeilingKey(K from, out K key) {
      bool res = BackingSet.TryGetCeiling(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
      key = entry.Key;
      return res;
    }

    public K CeilingKey(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Ceiling keys in an empty AVLTreeDictionary.");
      if (TryGetCeilingKey(from, out K key)) {
        return key;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Ceiling key in the AVLTreeDictionary.");
    }

    public KeyValuePair<K, V> CeilingEntry(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Ceiling keys in an empty AVLTreeDictionary.");
      if (TryGetCeilingEntry(from, out KeyValuePair<K, V> entry)) {
        return entry;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Ceiling key in the AVLTreeDictionary.");
    }

    public bool ContainsHigher(K from) => TryGetHigherEntry(from, out var placeholder);

    public bool TryGetHigherEntry(K from, out KeyValuePair<K, V> entry) =>
      BackingSet.TryGetHigher(new KeyValuePair<K, V>(from, DefaultValue), out entry);

    public bool TryGetHigherKey(K from, out K key) {
      bool res = BackingSet.TryGetHigher(new KeyValuePair<K, V>(from, DefaultValue), out var entry);
      key = entry.Key;
      return res;
    }

    public K HigherKey(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Higher keys in an empty AVLTreeDictionary.");
      if (TryGetHigherKey(from, out K key)) {
        return key;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Higher key in the AVLTreeDictionary.");
    }

    public KeyValuePair<K, V> HigherEntry(K from) {
      if (IsEmpty()) throw new InvalidOperationException("Cannot have Higher keys in an empty AVLTreeDictionary.");
      if (TryGetHigherEntry(from, out KeyValuePair<K, V> entry)) {
        return entry;
      }
      else throw new ArgumentOutOfRangeException("from", "There is no Higher key in the AVLTreeDictionary.");
    }

    public KeyValuePair<K, V> LowestEntry() => BackingSet.LowestValue();
    public K LowestKey() => BackingSet.LowestValue().Key;
    public KeyValuePair<K, V> HighestEntry() => BackingSet.HighestValue();
    public K HighestKey() => BackingSet.HighestValue().Key;
    #endregion

    #region IDictionary<K, V>
    public void Add(K key, V value) {
      var kvp = new KeyValuePair<K, V>(key, value);
      // As a reminder, BackingSet's equality checker only checks the keys.
      if (BackingSet.Contains(kvp)) {
        throw new ArgumentException("The specified key is already in use.");
      }
      else {
        BackingSet.Add(kvp);
      }
    }

    public bool ContainsKey(K key) => BackingSet.Contains(new KeyValuePair<K, V>(key, DefaultValue));

    public bool Remove(K key) => BackingSet.Remove(new KeyValuePair<K, V>(key, DefaultValue));

    public bool TryGetValue(K key, out V value) {
      AVLTreeSet<KeyValuePair<K, V>>.NodeTriplet nodes = BackingSet.SearchAround(new KeyValuePair<K, V>(key, DefaultValue));
      if (nodes.HasEqualValue) {
        value = nodes.EqualValue.Value;
        return true;
      }
      else {
        value = default(V);
        return false;
      }
    }
    #endregion

    #region ICollection<KeyValuePair<K, V>>
    public void Add(KeyValuePair<K, V> entry) {
      // As a reminder, BackingSet's equality checker only checks the keys.
      if (BackingSet.Contains(entry)) {
        // ... and ReplaceValue doesn't check equality.
        BackingSet.ReplaceValue(entry, entry);
      }
      else {
        BackingSet.Add(entry);
      }
    }

    public void Clear() => BackingSet.Clear();

    public bool Contains(KeyValuePair<K, V> entry) {
      var nodes = BackingSet.SearchAround(entry);
      return (nodes.HasEqualValue && nodes.EqualValue.Value.Equals(entry.Value));
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int index) => BackingSet.CopyTo(array, index);

    public bool Remove(KeyValuePair<K, V> entry) {
      if (Contains(entry)) return BackingSet.Remove(entry);
      else return false;
    }
    #endregion

    #region IEnumerable<KeyValuePair<K, V>>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => BackingSet.GetEnumerator();
    #endregion

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => BackingSet.GetEnumerator();
    #endregion
    #endregion

    #region Private Methods
    private static Comparison<K> GetComparer() {
      if (typeof(IComparable<K>).IsAssignableFrom(typeof(K)) || typeof(System.IComparable).IsAssignableFrom(typeof(K))) {
        return Comparer<K>.Default.Compare;
      }
      else {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The type {0} cannot be compared. It must implement IComparable<T> or IComparable interface", typeof(K).FullName));
      }
    }
    #endregion
  }
}