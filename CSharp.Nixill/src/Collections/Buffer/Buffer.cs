using System.Collections;

namespace Nixill.Collections;

/// <summary>
///   A buffer, which is a queue with a maximum size that will
///   automatically discard its oldest values if more values are inserted
///   than that maximum size.
/// </summary>
/// <typeparam name="T">
///   The type of items contained in this <see cref="Buffer{T}"/>.
/// </typeparam>
public class Buffer<T> : ICollection<T>
{
  Queue<T> Queue = [];

  /// <summary>
  ///   Get: The size (item limit) of this <see cref="Buffer{T}"/>.
  /// </summary>
  /// <seealso cref="ChangeCapacity(int)">
  ///   Method to change this value
  /// </seealso>
  public int BufferSize { get; private set; }

  /// <summary>
  ///   Get: The number of items currently within this
  ///   <see cref="Buffer{T}"/>.
  /// </summary>
  public int Count => Queue.Count;

  /// <summary>
  ///   Get: Whether or not this <see cref="Buffer{T}"/> is read-only
  ///   (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => ((ICollection<T>)Queue).IsReadOnly;

  /// <summary>
  ///   Creates a new <see cref="Buffer{T}"/> with a given buffer size.
  /// </summary>
  /// <param name="bufferSize">The buffer size.</param>
  public Buffer(int bufferSize)
  {
    BufferSize = bufferSize;
  }

  /// <summary>
  ///   Creates a new <see cref="Buffer{T}"/> with a given buffer size and
  ///   initial queue.
  /// </summary>
  /// <remarks>
  ///   If there are more items in <c>items</c> than the given
  ///   <c>bufferSize</c>, items from the front are discarded, leaving
  ///   only the final <c>bufferSize</c> items. This is the same as if the
  ///   items had been added in order after the <see cref="Buffer{T}"/>
  ///   was created.
  /// </remarks>
  /// <param name="bufferSize">The buffer size.</param>
  /// <param name="items">The initial contents.</param>
  public Buffer(int bufferSize, IEnumerable<T> items)
  {
    BufferSize = bufferSize;
    Queue = new(items.TakeLast(bufferSize));
  }

  /// <summary>
  ///   Adds a new item to this <see cref="Buffer{T}"/>. If this causes
  ///   the <see cref="Buffer{T}"/> to have more items than its
  ///   <see cref="BufferSize"/>, the oldest item in this
  ///   <see cref="Buffer{T}"/> is removed ("bumped") and returned.
  /// </summary>
  /// <param name="item">The item to add.</param>
  /// <returns>
  ///   <c>bool</c>: Whether or not an item was bumped. <c>T</c>: The
  ///   bumped item, or <c>default(T)</c> if no item was bumped.
  /// </returns>
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

  /// <summary>
  ///   Changes the <see cref="BufferSize"/> of this
  ///   <see cref="Buffer{T}"/>. If this operation shrinks the BufferSize
  ///   below the number of items contained in the
  ///   <see cref="Buffer{T}"/>, items are removed ("bumped") from the
  ///   front and yielded until the BufferSize is not exceeded.
  /// </summary>
  /// <param name="newCapacity">The new capacity.</param>
  /// <returns>
  ///   All of the items that were bumped out of this
  ///   <see cref="Buffer{T}"/>, or an empty enumerable if no items were
  ///   bumped.
  /// </returns>
  public IEnumerable<T> ChangeCapacity(int newCapacity)
  {
    BufferSize = newCapacity;
    while (Count > BufferSize) yield return Queue.Dequeue();
  }

  /// <inheritdoc/>
  public void Clear() => Queue.Clear();

  /// <inheritdoc/>
  public bool Contains(T item) => Queue.Contains(item);

  /// <inheritdoc/>
  public void CopyTo(T[] array, int arrayIndex) => Queue.CopyTo(array, arrayIndex);

  /// <inheritdoc/>
  public IEnumerator<T> GetEnumerator() => Queue.GetEnumerator();

  /// <inheritdoc/>
  bool ICollection<T>.Remove(T item) => ((ICollection<T>)Queue).Remove(item);

  /// <inheritdoc/>
  void ICollection<T>.Add(T item) => this.Add(item);

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => Queue.GetEnumerator();
}