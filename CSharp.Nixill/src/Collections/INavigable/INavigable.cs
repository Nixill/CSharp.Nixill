using System.Collections.Generic;
using System;

namespace Nixill.Collections
{
  /// <summary>
  ///   A sorted set that can be navigated using methods such as
  ///   <c>Lower</c>, <c>Floor</c>, <c>Ceiling</c>, and <c>Higher</c>.
  /// </summary>
  public interface INavigableSet<T> : ISet<T>
  {
    /// <summary>
    ///   Gets the element with the highest value less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Lower(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a lesser value
    ///   than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsLower(T from);

    /// <summary>
    ///   Gets the element with the highest value less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lower value, if such a
    ///   value exists. Otherwise, this contains the default value for the
    ///   type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLower(T from, out T value);

    /// <summary>
    ///   Gets the element with the highest value less than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Floor(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a value lower than
    ///   or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsFloor(T from);

    /// <summary>
    ///   Gets the element with the highest value less than or equal to a
    ///   given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal value,
    ///   if such a value exists. Otherwise, this contains the default
    ///   value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloor(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value greater than or equal to
    ///   a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No higher or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Ceiling(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a value greater
    ///   than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsCeiling(T from);

    /// <summary>
    ///   Gets the element with the lowest value greater than or equal to
    ///   a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater or equal
    ///   value, if such a value exists. Otherwise, this contains the
    ///   default value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeiling(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Higher(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a higher value
    ///   than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsHigher(T from);

    /// <summary>
    ///   Gets the element with the lowest value greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater value, if such
    ///   a value exists. Otherwise, this contains the default value for
    ///   the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigher(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T LowestValue();

    /// <summary>
    ///   Gets the element with the highest value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T HighestValue();
  }

  /// <summary>
  ///   A sorted Dictionary that can be navigated using methods such as
  ///   <c>LowerKey</c>, <c>FloorKey</c>, <c>CeilingKey</c>, and
  ///   <c>HigherKey</c>.
  /// </summary>
  public interface INavigableDictionary<K, V> : IDictionary<K, V>
  {
    /// <summary>
    ///   Gets the highest key less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K LowerKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is less than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsLower(K from);

    /// <summary>
    ///   Gets the highest key less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lower key, if such a
    ///   key exists. Otherwise, this contains the default value for the
    ///   type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLowerKey(K from, out K value);

    /// <summary>
    ///   Gets the highest key less than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K FloorKey(K from);

    /// <summary>
    ///   Returns whether or not an key exists that is less than or equal
    ///   to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsFloor(K from);

    /// <summary>
    ///   Gets the highest key less than or equal to a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal key,
    ///   if such a key exists. Otherwise, this contains the default value
    ///   for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloorKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key greater than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No higher or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K CeilingKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is greater than or
    ///   equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsCeiling(K from);

    /// <summary>
    ///   Gets the lowest key greater than or equal to a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater or equal key,
    ///   if such a key exists. Otherwise, this contains the default value
    ///   for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeilingKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K HigherKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is greater than a given
    ///   value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsHigher(K from);

    /// <summary>
    ///   Gets the lowest key greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater key, if such a
    ///   key exists. Otherwise, this contains the default value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigherKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K LowestKey();

    /// <summary>
    ///   Gets the highest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K HighestKey();

    /// <summary>
    ///   Gets the entry with the highest key less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> LowerEntry(K from);

    /// <summary>
    ///   Gets the entry with the highest key less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the lower
    ///   key, if such a key exists. Otherwise, this contains an entry
    ///   with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLowerEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the highest key less than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> FloorEntry(K from);

    /// <summary>
    ///   Gets the highest entry with a key less than or equal to a given
    ///   value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal key,
    ///   if such a key exists. Otherwise, this contains an entry with the
    ///   default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloorEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key greater than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> CeilingEntry(K from);

    /// <summary>
    ///   Gets the entry with the lowest key greater than or equal to a
    ///   given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the greater
    ///   or equal key, if such a key exists. Otherwise, this contains an
    ///   entry with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeilingEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> HigherEntry(K from);

    /// <summary>
    ///   Gets the entry with the lowest key greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the greater
    ///   key, if such a key exists. Otherwise, this contains an entry
    ///   with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigherEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> LowestEntry();

    /// <summary>
    ///   Gets the entry with the highest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> HighestEntry();
  }

  /// <summary>
  ///   A read-only sorted set that can be navigated using methods such as
  ///   <c>Lower</c>, <c>Floor</c>, <c>Ceiling</c>, and <c>Higher</c>.
  /// </summary>
  public interface IReadOnlyNavigableSet<T> : IReadOnlyCollection<T>
  {
    /// <summary>
    ///   Gets the element with the highest value less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Lower(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a lesser value
    ///   than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsLower(T from);

    /// <summary>
    ///   Gets the element with the highest value less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lower value, if such a
    ///   value exists. Otherwise, this contains the default value for the
    ///   type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLower(T from, out T value);

    /// <summary>
    ///   Gets the element with the highest value less than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Floor(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a value lower than
    ///   or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsFloor(T from);

    /// <summary>
    ///   Gets the element with the highest value less than or equal to a
    ///   given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal value,
    ///   if such a value exists. Otherwise, this contains the default
    ///   value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloor(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value greater than or equal to
    ///   a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No higher or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Ceiling(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a value greater
    ///   than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsCeiling(T from);

    /// <summary>
    ///   Gets the element with the lowest value greater than or equal to
    ///   a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater or equal
    ///   value, if such a value exists. Otherwise, this contains the
    ///   default value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeiling(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T Higher(T from);

    /// <summary>
    ///   Returns whether or not an element exists with a higher value
    ///   than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsHigher(T from);

    /// <summary>
    ///   Gets the element with the lowest value greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such an element exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater value, if such
    ///   a value exists. Otherwise, this contains the default value for
    ///   the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigher(T from, out T value);

    /// <summary>
    ///   Gets the element with the lowest value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T LowestValue();

    /// <summary>
    ///   Gets the element with the highest value.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The set contains no elements.
    /// </exception>
    public T HighestValue();
  }

  /// <summary>
  ///   A read-only sorted Dictionary that can be navigated using methods
  ///   such as <c>LowerKey</c>, <c>FloorKey</c>, <c>CeilingKey</c>, and
  ///   <c>HigherKey</c>.
  /// </summary>
  public interface IReadOnlyNavigableDictionary<K, V> : IReadOnlyDictionary<K, V>
  {
    /// <summary>
    ///   Gets the highest key less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K LowerKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is less than a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsLower(K from);

    /// <summary>
    ///   Gets the highest key less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lower key, if such a
    ///   key exists. Otherwise, this contains the default value for the
    ///   type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLowerKey(K from, out K value);

    /// <summary>
    ///   Gets the highest key less than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K FloorKey(K from);

    /// <summary>
    ///   Returns whether or not an key exists that is less than or equal
    ///   to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsFloor(K from);

    /// <summary>
    ///   Gets the highest key less than or equal to a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal key,
    ///   if such a key exists. Otherwise, this contains the default value
    ///   for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloorKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key greater than or equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No higher or equal value exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K CeilingKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is greater than or
    ///   equal to a given value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsCeiling(K from);

    /// <summary>
    ///   Gets the lowest key greater than or equal to a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater or equal key,
    ///   if such a key exists. Otherwise, this contains the default value
    ///   for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeilingKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K HigherKey(K from);

    /// <summary>
    ///   Returns whether or not a key exists that is greater than a given
    ///   value.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool ContainsHigher(K from);

    /// <summary>
    ///   Gets the lowest key greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the greater key, if such a
    ///   key exists. Otherwise, this contains the default value for the type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigherKey(K from, out K value);

    /// <summary>
    ///   Gets the lowest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K LowestKey();

    /// <summary>
    ///   Gets the highest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public K HighestKey();

    /// <summary>
    ///   Gets the entry with the highest key less than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lower key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> LowerEntry(K from);

    /// <summary>
    ///   Gets the entry with the highest key less than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the lower
    ///   key, if such a key exists. Otherwise, this contains an entry
    ///   with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetLowerEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the highest key less than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No lesser or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> FloorEntry(K from);

    /// <summary>
    ///   Gets the highest entry with a key less than or equal to a given
    ///   value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the lesser or equal key,
    ///   if such a key exists. Otherwise, this contains an entry with the
    ///   default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetFloorEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key greater than or equal to a
    ///   given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater or equal key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> CeilingEntry(K from);

    /// <summary>
    ///   Gets the entry with the lowest key greater than or equal to a
    ///   given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the greater
    ///   or equal key, if such a key exists. Otherwise, this contains an
    ///   entry with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetCeilingEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key greater than a given value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   No greater key exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> HigherEntry(K from);

    /// <summary>
    ///   Gets the entry with the lowest key greater than a given value.
    /// </summary>
    /// <returns>
    ///   Whether or not such a key exists.
    /// </returns>
    /// <param name="value">
    ///   When this method exits, this contains the entry with the greater
    ///   key, if such a key exists. Otherwise, this contains an entry
    ///   with the default values for the types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The given value is null.
    /// </exception>
    public bool TryGetHigherEntry(K from, out KeyValuePair<K, V> value);

    /// <summary>
    ///   Gets the entry with the lowest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> LowestEntry();

    /// <summary>
    ///   Gets the entry with the highest key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///   The dictionary contains no elements.
    /// </exception>
    public KeyValuePair<K, V> HighestEntry();
  }
}