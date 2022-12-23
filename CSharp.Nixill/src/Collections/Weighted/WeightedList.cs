using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Nixill.Collections;

public class WeightedList<TSumWeight, TWeight, TItem> : ICollection<WeightedEntry<TWeight, TItem>>
  where TSumWeight : IAdditionOperators<TSumWeight, TWeight, TSumWeight>, IAdditiveIdentity<TSumWeight, TSumWeight>,
    IComparable<TSumWeight>, ISubtractionOperators<TSumWeight, TSumWeight, TWeight>,
    ISubtractionOperators<TSumWeight, TWeight, TSumWeight>
  where TWeight : IAdditionOperators<TWeight, TWeight, TWeight>, IAdditiveIdentity<TWeight, TWeight>,
    IComparable<TWeight>, ISubtractionOperators<TWeight, TWeight, TWeight>
{
  private AVLTreeDictionary<TSumWeight, TItem> Items;

  public WeightedList()
  {
    Items = new();
  }

  public WeightedList(IEnumerable<WeightedEntry<TWeight, TItem>> items)
  {
    Items = new();

    foreach (var item in items)
    {
      Add(item);
    }
  }

  public int Count => Items.Count;

  public TSumWeight TotalWeight => (Items.Count > 0)
    ? Items.HighestKey()
    : TSumWeight.AdditiveIdentity;

  public TItem this[TSumWeight at]
  {
    get
    {
      if (!IsNonNegative(at))
      {
        throw new IndexOutOfRangeException("Only positive positions may be accessed in a WeightedList.");
      }
      if (Items.TryGetHigherEntry(at, out var entry))
      {
        return entry.Value;
      }
      else
      {
        throw new IndexOutOfRangeException("The specified position exceeds the total weight of the WeightedList.");
      }
    }
  }

  public bool IsReadOnly => false;

  public void Add(WeightedEntry<TWeight, TItem> item) => Add(item.Weight, item.Item);

  public void Add(TWeight weight, TItem item)
  {
    if (!IsPositive(weight))
    {
      throw new ArgumentOutOfRangeException("Item weights must be positive.");
    }

    TWeight existing = WeightOf(item);
    if (IsPositive(existing))
    {
      Remove(item);
    }

    weight += existing;
    TSumWeight newTotal = Items.HighestKey() + weight;
    Items[newTotal] = item;
  }

  public void Clear() => Items.Clear();

  public bool Contains(WeightedEntry<TWeight, TItem> item) => ComparesEqual(item.Weight, WeightOf(item.Item));
  public bool Contains(TItem item) => IsPositive(WeightOf(item));

  public TWeight WeightOf(TItem item)
  {
    var keys = Items.Where(x => x.Value.Equals(item));

    // Does it exist?
    if (keys.Any())
    {
      TSumWeight key = keys.First().Key;

      // Is it the lowest key?
      if (ComparesEqual(key, Items.LowestKey()))
      {
        // The subtraction here makes it a TWeight instead of a
        // TSumWeight, just in case those are actually different types.
        return key - TSumWeight.AdditiveIdentity;
      }
      else
      {
        return key - Items.LowerKey(key);
      }
    }
    else
    {
      return TWeight.AdditiveIdentity;
    }
  }

  public void CopyTo(WeightedEntry<TWeight, TItem>[] array, int index)
  {
    foreach (WeightedEntry<TWeight, TItem> item in this)
    {
      array[index++] = item;
    }
  }

  public IEnumerator<WeightedEntry<TWeight, TItem>> GetEnumerator()
  {
    TSumWeight last = TSumWeight.AdditiveIdentity;
    foreach (KeyValuePair<TSumWeight, TItem> item in Items)
    {
      TWeight weight = item.Key - last;
      yield return new WeightedEntry<TWeight, TItem>()
      {
        Weight = weight,
        Item = item.Value
      };
      last = item.Key;
    }
  }

  public bool Remove(WeightedEntry<TWeight, TItem> item) => Remove(item.Weight, item.Item);

  public bool Remove(TItem item)
  {
    TWeight weight = WeightOf(item);
    if (!IsPositive(weight)) return false;

    // Find this object's key and remove it
    TSumWeight key = Items.Where(x => x.Value.Equals(item)).First().Key;
    Items.Remove(key);

    // Decrement every higher key
    while (Items.TryGetHigherKey(key, out key))
    {
      TItem moving = Items[key];
      Items.Remove(key);
      Items.Add(key - weight, moving);
    }

    // And return true
    return true;
  }

  public bool Remove(TWeight weight, TItem item)
  {
    TWeight weightOf = WeightOf(item);

    // If we're trying to remove more weight of the item than exists, just
    // remove the whole item.
    if (ComparesGreater(weight, weightOf)) weight = weightOf;

    // If no weight exists, then do nothing.
    if (!IsPositive(weight)) return false;

    // Find this object's key
    TSumWeight key = Items.Where(x => x.Value.Equals(item)).First().Key;

    // Decrement it and every higher key
    do
    {
      TItem moving = Items[key];
      Items.Remove(key);
      if (!Items.ContainsKey(key - weight)) Items.Add(key - weight, moving);
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

public struct WeightedEntry<A, V>
{
  public V Item;
  public A Weight;
}