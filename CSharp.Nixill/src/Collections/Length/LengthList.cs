using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Nixill.Collections;

public class LengthList<TSumLength, TLength, TItem> : ICollection<LengthEntry<TLength, TItem>>
  where TSumLength : IAdditionOperators<TSumLength, TLength, TSumLength>, IAdditiveIdentity<TSumLength, TSumLength>,
    IComparable<TSumLength>, ISubtractionOperators<TSumLength, TSumLength, TLength>,
    ISubtractionOperators<TSumLength, TLength, TSumLength>
  where TLength : IAdditionOperators<TLength, TLength, TLength>, IAdditiveIdentity<TLength, TLength>,
    IComparable<TLength>, ISubtractionOperators<TLength, TLength, TLength>
{
  private AVLTreeDictionary<TSumLength, TItem> Items;

  public LengthList()
  {
    Items = new();
  }

  public LengthList(IEnumerable<LengthEntry<TLength, TItem>> items)
  {
    Items = new();

    foreach (var item in items)
    {
      Add(item);
    }
  }

  public int Count => Items.Count;

  public TSumLength TotalLength => (Items.Count > 0)
    ? Items.HighestKey()
    : TSumLength.AdditiveIdentity;

  public TItem this[TSumLength at]
  {
    get
    {
      if (!IsNonNegative(at))
      {
        throw new IndexOutOfRangeException("Only non-negative positions may be accessed in a LengthList.");
      }
      if (Items.TryGetHigherEntry(at, out var entry))
      {
        return entry.Value;
      }
      else
      {
        throw new IndexOutOfRangeException("The specified position exceeds the total length of the LengthList.");
      }
    }
  }

  public bool IsReadOnly => false;

  public void Add(LengthEntry<TLength, TItem> item) => Add(item.Length, item.Item);

  public void Add(TLength length, TItem item)
  {
    if (!IsPositive(length))
    {
      throw new ArgumentOutOfRangeException("Item lengths must be positive.");
    }

    TLength existing = LengthOf(item);
    if (IsPositive(existing))
    {
      Remove(item);
    }

    length += existing;
    TSumLength newTotal = Items.HighestKey() + length;
    Items[newTotal] = item;
  }

  public void Clear() => Items.Clear();

  public bool Contains(LengthEntry<TLength, TItem> item) => ComparesEqual(item.Length, LengthOf(item.Item));
  public bool Contains(TItem item) => IsPositive(LengthOf(item));

  public TLength LengthOf(TItem item)
  {
    var keys = Items.Where(x => x.Value.Equals(item));

    // Does it exist?
    if (keys.Any())
    {
      TSumLength key = keys.First().Key;

      // Is it the lowest key?
      if (ComparesEqual(key, Items.LowestKey()))
      {
        // The subtraction here makes it a TLength instead of a
        // TSumLength, just in case those are actually different types.
        return key - TSumLength.AdditiveIdentity;
      }
      else
      {
        return key - Items.LowerKey(key);
      }
    }
    else
    {
      return TLength.AdditiveIdentity;
    }
  }

  public void CopyTo(LengthEntry<TLength, TItem>[] array, int index)
  {
    if (index + Count > array.Length) throw new ArgumentException("Array is not long enough.");

    foreach (LengthEntry<TLength, TItem> item in this)
    {
      array[index++] = item;
    }
  }

  public IEnumerator<LengthEntry<TLength, TItem>> GetEnumerator()
  {
    TSumLength last = TSumLength.AdditiveIdentity;
    foreach (KeyValuePair<TSumLength, TItem> item in Items)
    {
      TLength length = item.Key - last;
      yield return new LengthEntry<TLength, TItem>()
      {
        Length = length,
        Item = item.Value
      };
      last = item.Key;
    }
  }

  public bool Remove(LengthEntry<TLength, TItem> item)
  {
    if (ComparesEqual(LengthOf(item.Item), item.Length))
    {
      return Remove(item.Length, item.Item);
    }
    else
    {
      return false;
    }
  }

  public bool Remove(TItem item)
  {
    TLength length = LengthOf(item);
    if (!IsPositive(length)) return false;

    // Find this object's key and remove it
    TSumLength key = Items.Where(x => x.Value.Equals(item)).First().Key;
    Items.Remove(key);

    // Decrement every higher key
    while (Items.TryGetHigherKey(key, out key))
    {
      TItem moving = Items[key];
      Items.Remove(key);
      Items.Add(key - length, moving);
    }

    // And return true
    return true;
  }

  public bool Remove(TLength length, TItem item)
  {
    TLength lengthOf = LengthOf(item);

    // If we're trying to remove more length of the item than exists, just
    // remove the whole item.
    if (ComparesGreater(length, lengthOf)) length = lengthOf;

    // If no length exists, then do nothing.
    if (!IsPositive(length)) return false;

    // Find this object's key
    TSumLength key = Items.Where(x => x.Value.Equals(item)).First().Key;

    // Decrement it and every higher key
    do
    {
      TItem moving = Items[key];
      Items.Remove(key);
      if (!Items.ContainsKey(key - length)) Items.Add(key - length, moving);
    } while (Items.TryGetHigherKey(key, out key));

    // And return true
    return true;
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private static bool IsZero<T>(T input) where T : IComparable<T>, IAdditiveIdentity<T, T>
    => input.CompareTo(T.AdditiveIdentity) == 0;

  private static bool IsPositive<T>(T input) where T : IComparable<T>, IAdditiveIdentity<T, T>
    => input.CompareTo(T.AdditiveIdentity) > 0;

  private static bool IsNonNegative<T>(T input) where T : IComparable<T>, IAdditiveIdentity<T, T>
    => input.CompareTo(T.AdditiveIdentity) >= 0;

  private static bool ComparesEqual<T>(T input, T target) where T : IComparable<T>
    => input.CompareTo(target) == 0;

  private static bool ComparesGreater<T>(T input, T target) where T : IComparable<T>
    => input.CompareTo(target) > 0;

  private static bool IsEqual<T>(T input, T target) => input.Equals(target);
}

public struct LengthEntry<A, V>
{
  public V Item;
  public A Length;
}