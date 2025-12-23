using System.Collections;

namespace Nixill.Collections;

internal class AVLKeySet<K, V> : IReadOnlyNavigableSet<K>
{
  readonly AVLTreeDictionary<K, V> Backing;

  internal AVLKeySet(AVLTreeDictionary<K, V> backing) => Backing = backing;

  public int Count => Backing.Count;

  public K Ceiling(K from) => Backing.CeilingKey(from);
  public bool ContainsCeiling(K from) => Backing.ContainsCeiling(from);
  public bool ContainsFloor(K from) => Backing.ContainsFloor(from);
  public bool ContainsHigher(K from) => Backing.ContainsHigher(from);
  public bool ContainsLower(K from) => Backing.ContainsLower(from);
  public K Floor(K from) => Backing.FloorKey(from);

  IEnumerable<K> AsEnumerable()
  {
    foreach (var kvp in Backing)
    {
      yield return kvp.Key;
    }
  }

  public IEnumerator<K> GetEnumerator() => AsEnumerable().GetEnumerator();
  public K Higher(K from) => Backing.HigherKey(from);
  public K HighestValue() => Backing.HighestKey();
  public K Lower(K from) => Backing.LowerKey(from);
  public K LowestValue() => Backing.LowestKey();
  public bool TryGetCeiling(K from, out K value) => Backing.TryGetCeilingKey(from, out value);
  public bool TryGetFloor(K from, out K value) => Backing.TryGetFloorKey(from, out value);
  public bool TryGetHigher(K from, out K value) => Backing.TryGetHigherKey(from, out value);
  public bool TryGetLower(K from, out K value) => Backing.TryGetLowerKey(from, out value);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public IEnumerable<K> GetSlice(K lowerBound, K upperBound,
    NavigationDirection lowerBoundDirection = NavigationDirection.Floor,
    NavigationDirection upperBoundDirection = NavigationDirection.Ceiling)
    => Backing.GetKeySlice(lowerBound, upperBound, lowerBoundDirection, upperBoundDirection);
}