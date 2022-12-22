using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Nixill.Collections;

public class WeightedList<TSumWeight, TWeight, TItem> : ICollection<WeightedEntry<TWeight, TItem>>
  where TSumWeight : IAdditionOperators<TSumWeight, TWeight, TSumWeight>, IAdditiveIdentity<TSumWeight, TSumWeight>,
    IComparable<TSumWeight>, ISubtractionOperators<TSumWeight, TWeight, TSumWeight>
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

  public bool IsReadOnly => false;

  public void Add(WeightedEntry<TWeight, TItem> item) => Add(item.Item, item.Weight);

  public void Add(TItem item, TWeight weight)
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

  public void Clear()
  {
    throw new System.NotImplementedException();
  }

  public bool Contains(WeightedEntry<TWeight, TItem> item)
  {
    throw new System.NotImplementedException();
  }

  public bool Contains(TItem item) { }
  public TWeight WeightOf(TItem item) { }

  public void CopyTo(WeightedEntry<TWeight, TItem>[] array, int arrayIndex)
  {
    throw new System.NotImplementedException();
  }

  public IEnumerator<WeightedEntry<TWeight, TItem>> GetEnumerator()
  {
    throw new System.NotImplementedException();
  }

  public bool Remove(WeightedEntry<TWeight, TItem> item)
  {
    TWeight weightOf = WeightOf(item.Item);
    if (IsEqual(item.Weight, weightOf)) return Remove(item);
    else return false;
  }

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

  IEnumerator IEnumerable.GetEnumerator()
  {
    throw new System.NotImplementedException();
  }

  private static bool IsPositive<T>(T input) where T : IComparable<T>, IAdditiveIdentity<T, T>
    => input.CompareTo(T.AdditiveIdentity) > 0;

  private static bool IsEqual<T>(T input, T target) => input.Equals(target);
}

public struct WeightedEntry<A, V>
{
  public V Item;
  public A Weight;
}