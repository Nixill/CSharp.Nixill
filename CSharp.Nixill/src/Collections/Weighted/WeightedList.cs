using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections;

public class WeightedList<V> : IList<WeightedEntry<V>> {
  public WeightedEntry<V> this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

  public int Count => throw new System.NotImplementedException();

  public bool IsReadOnly => throw new System.NotImplementedException();

  public void Add(WeightedEntry<V> item) {
    throw new System.NotImplementedException();
  }

  public void Clear() {
    throw new System.NotImplementedException();
  }

  public bool Contains(WeightedEntry<V> item) {
    throw new System.NotImplementedException();
  }

  public void CopyTo(WeightedEntry<V>[] array, int arrayIndex) {
    throw new System.NotImplementedException();
  }

  public IEnumerator<WeightedEntry<V>> GetEnumerator() {
    throw new System.NotImplementedException();
  }

  public int IndexOf(WeightedEntry<V> item) {
    throw new System.NotImplementedException();
  }

  public void Insert(int index, WeightedEntry<V> item) {
    throw new System.NotImplementedException();
  }

  public bool Remove(WeightedEntry<V> item) {
    throw new System.NotImplementedException();
  }

  public void RemoveAt(int index) {
    throw new System.NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    throw new System.NotImplementedException();
  }
}

public struct WeightedEntry<V> {
  public V Item;
  public int StartAt;
  public int Weight;
}