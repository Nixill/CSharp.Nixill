using System.Collections;

namespace Nixill.Collections;

public class Buffer<T> : ICollection<T>
{
  Queue<T> Queue = [];

  public int BufferSize { get; private set; }

  public int Count => Queue.Count;
  public bool IsReadOnly => ((ICollection<T>)Queue).IsReadOnly;

  public Buffer(int bufferSize)
  {
    BufferSize = bufferSize;
  }

  public Buffer(int bufferSize, IEnumerable<T> items)
  {
    BufferSize = bufferSize;
    Queue = new(items.TakeLast(bufferSize));
  }

  public (bool Bumped, T BumpedItem) Add(T item)
  {
    Queue.Enqueue(item);
    if (this.Count > this.BufferSize)
    {
      return (true, Queue.Dequeue());
    }
    else
    {
      return (false, default(T)!);
    }
  }

  public IEnumerable<T> ChangeCapacity(int newCapacity)
  {
    BufferSize = newCapacity;
    while (Count > BufferSize) yield return Queue.Dequeue();
  }

  public void Clear() => Queue.Clear();
  public bool Contains(T item) => Queue.Contains(item);
  public void CopyTo(T[] array, int arrayIndex) => Queue.CopyTo(array, arrayIndex);
  public IEnumerator<T> GetEnumerator() => Queue.GetEnumerator();
  bool ICollection<T>.Remove(T item) => ((ICollection<T>)Queue).Remove(item);

  void ICollection<T>.Add(T item) => this.Add(item);
  IEnumerator IEnumerable.GetEnumerator() => Queue.GetEnumerator();
}