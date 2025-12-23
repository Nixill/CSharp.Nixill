using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Collections;

public class BlockDictionary<K, V>
{
  public V DefaultValue { get; private set; }
  // TODO: MAKE THIS NOT PUBLIC (it's public for testing purposes)
  public AVLTreeDictionary<K, (V Exact, V Higher)> Backing { get; }
  IComparer<K> KeyComparer { get; }
  IEqualityComparer<V> ValueComparer { get; }

  public BlockDictionary(V defaultValue)
  {
    KeyComparer = Comparer<K>.Default;
    ValueComparer = EqualityComparer<V>.Default;
    DefaultValue = defaultValue;
    Backing = [];
  }

  public BlockDictionary(V defaultValue, IComparer<K> keyComparer)
  {
    KeyComparer = keyComparer;
    ValueComparer = EqualityComparer<V>.Default;
    DefaultValue = defaultValue;
    Backing = new(keyComparer);
  }

  public BlockDictionary(V defaultValue, IEqualityComparer<V> equalityComparer)
  {
    KeyComparer = Comparer<K>.Default;
    ValueComparer = equalityComparer;
    DefaultValue = defaultValue;
    Backing = [];
  }

  public BlockDictionary(V defaultValue, IComparer<K> keyComparer, IEqualityComparer<V> equalityComparer)
  {
    KeyComparer = keyComparer;
    ValueComparer = equalityComparer;
    DefaultValue = defaultValue;
    Backing = new(keyComparer);
  }

  public V this[K key]
  {
    get
    {
      if (!Backing.TryGetFloorEntry(key, out var entry)) return DefaultValue;
      if (KeyComparer.Compare(key, entry.Key) == 0) return entry.Value.Exact;
      return entry.Value.Higher;
    }
  }

  public void SetRange(K lowerBound, K upperBound, V value, bool lowerBoundInclusive = true,
    bool upperBoundInclusive = false)
  {
    // Step 0: Validate the arguments
    if (KeyComparer.Compare(lowerBound, upperBound) > 0) throw new ArgumentException("upperBound must be ≥ lowerBound");

    // Step 1: Set the upper bound
    if (Backing.TryGetValue(upperBound, out var higherEntry))
    {
      if (upperBoundInclusive)
      {
        (_, V upperValue) = higherEntry;
        Backing[upperBound] = (value, upperValue);
      }
      // otherwise do nothing
    }
    else
    {
      V valueAtHigher = this[upperBound];
      if (upperBoundInclusive)
      {
        Backing[upperBound] = (value, valueAtHigher);
      }
      else
      {
        Backing[upperBound] = (valueAtHigher, valueAtHigher);
      }
    }

    // Step 2: Set the lower bound
    if (lowerBoundInclusive)
    {
      Backing[lowerBound] = (value, value);
    }
    else
    {
      V lowerValue = this[lowerBound];
      Backing[lowerBound] = (lowerValue, value);
    }

    // Step 3: Remove keys in between
    var keysToRemove = Backing
      .GetKeySlice(lowerBound, upperBound, NavigationDirection.Higher, NavigationDirection.Lower)
      .ToArray();

    foreach (K key in keysToRemove) Backing.Remove(key);

    // Step 4: Merge adjacent entries
    if (Backing.TryGetValue(upperBound, out var upperEntry)
      && ValueComparer.Equals(upperEntry.Exact, upperEntry.Higher)
      && ValueComparer.Equals(upperEntry.Exact,
        (Backing.TryGetLowerEntry(upperBound, out var belowUpperEntry)
        ? belowUpperEntry.Value.Higher
        : DefaultValue)))
    {
      Backing.Remove(upperBound);
    }

    if (Backing.TryGetValue(lowerBound, out var lowerEntry)
      && ValueComparer.Equals(lowerEntry.Exact, lowerEntry.Higher)
      && ValueComparer.Equals(lowerEntry.Exact,
        (Backing.TryGetLowerEntry(lowerBound, out var belowlowerEntry)
        ? belowlowerEntry.Value.Higher
        : DefaultValue)))
    {
      Backing.Remove(lowerBound);
    }
  }
}