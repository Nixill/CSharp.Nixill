using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nixill.Collections;

/// <summary>
///   A set whose values are enumerable sequences, stored as a tree of
///   those sequences.
/// </summary>
/// <typeparam name="T">
///   The type of elements in the sequences that form values in the set.
///   <para/>
///   Note that this is specifically the <em>elements of the
///   sequence</em>. For example, you would write <c>char</c> here if you
///   want to have a set of <c>string</c>s, or <c>int</c> if you want a
///   set of <c>int[]</c>s.
/// </typeparam>
public class RecursiveSet<T> : IRecursiveSet<T> where T : notnull
{
  /// <summary>
  ///   Creates a new, empty <see cref="RecursiveSet{T}"/>.
  /// </summary>
  public RecursiveSet() : this([], EqualityComparer<T>.Default) { }

  /// <summary>
  ///   Creates a new <see cref="RecursiveSet{T}"/> with the given initial
  ///   values.
  /// </summary>
  /// <param name="values">The initial values.</param>
  public RecursiveSet(IEnumerable<IEnumerable<T>> values) : this(values, EqualityComparer<T>.Default) { }

  /// <summary>
  ///   Creates a new, empty <see cref="RecursiveSet{T}"/> with the given
  ///   value element comparer.
  /// </summary>
  /// <param name="comparer">The comparer.</param>
  public RecursiveSet(IEqualityComparer<T> comparer) : this([], comparer) { }

  /// <summary>
  ///   Creates a new <see cref="RecursiveSet{T}"/> with the given initial
  ///   values and the given value element comparer.
  /// </summary>
  /// <param name="values">The initial values.</param>
  /// <param name="comparer">The comparer.</param>
  public RecursiveSet(IEnumerable<IEnumerable<T>> values, IEqualityComparer<T> comparer)
  {
    Backing = new(comparer);
    Comparer = comparer;
    foreach (var value in values) Add(value);
  }

  private RecursiveSet(RecursiveDictionary<T, bool> newBacking)
  {
    Backing = newBacking;
    Comparer = newBacking.Comparer;
  }

  RecursiveDictionary<T, bool> Backing;

  /// <summary>
  ///   Get: The comparer that checks for equality between value elements.
  /// </summary>
  public IEqualityComparer<T> Comparer { get; }

  /// <summary>
  ///   Get: Whether or not this <see cref="RecursiveSet{T}"/> has a
  ///   empty-sequence value.
  /// </summary>
  public bool HasEmptyValue => Backing.HasEmptyKeyValue;

  /// <summary>
  ///   Get: A collection of all the first-element prefixes in the set.
  /// </summary>
  public ICollection<T> Prefixes => Backing.Prefixes;

  /// <summary>
  ///   Get: The number of items contained by this
  ///   <see cref="RecursiveSet{T}"/>, including the empty value (if
  ///   applicable) and all children (if any).
  /// </summary>
  public int Count => Backing.Count;

  /// <summary>
  ///   Get: Whether or not this set is read-only (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => false;

  /// <summary>
  ///   Adds a sequence to the set.
  /// </summary>
  /// <param name="item">The sequence to add.</param>
  /// <returns>
  ///   <c>true</c> if the sequence was successfully added, <c>false</c>
  ///   otherwise (including if the sequence was already in the set).
  /// </returns>
  public bool Add(IEnumerable<T> item)
  {
    try
    {
      Backing.Add(item, true);
      return true;
    }
    catch (ArgumentException)
    {
      return false;
    }
  }

  /// <summary>
  ///   Removes all sequences from the set.
  /// </summary>
  public void Clear() => Backing.Clear();

  /// <summary>
  ///   Returns whether or not the set contains the given sequence.
  /// </summary>
  /// <param name="item">The sequence.</param>
  /// <returns>
  ///   <c>true</c> iff the sequence is in the set.
  /// </returns>
  public bool Contains(IEnumerable<T> item) => Backing.ContainsKey(item);

  /// <summary>
  ///   Returns whether or not the set contains any sequences with the
  ///   given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>The sequence.</returns>
  public bool ContainsPrefix(IEnumerable<T> prefix) => Backing.ContainsPrefix(prefix);

  /// <inheritdoc/>
  public void CopyTo(IEnumerable<T>[] array, int arrayIndex) => Backing.Keys.CopyTo(array, arrayIndex);

  /// <inheritdoc/>
  public void ExceptWith(IEnumerable<IEnumerable<T>> other)
  {
    foreach (IEnumerable<T> seq in other) Remove(seq);
  }

  /// <summary>
  ///   Returns an enumerator over the sequences in the set.
  /// </summary>
  /// <returns>The enumerator.</returns>
  public IEnumerator<IEnumerable<T>> GetEnumerator() => Backing.Keys.GetEnumerator();

  /// <summary>
  ///   Returns a <em>copy</em> of this set containing only the values
  ///   with the given prefix (the values in the copy do not have the
  ///   prefix).
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>The copy.</returns>
  public IRecursiveSet<T> GetPrefix(IEnumerable<T> prefix) => new RecursiveSet<T>(Backing.GetPrefix(prefix).Keys);

  /// <summary>
  ///   Tries to get the sub-set for a given prefix.
  /// </summary>
  /// <remarks>
  ///   If a sub-set is given, it is an <em>unlinked copy</em> of the
  ///   given set for the given prefix. Additionally, the values in the
  ///   sub-set have the specified prefix removed.
  /// </remarks>
  /// <param name="prefix">The prefix.</param>
  /// <param name="result">
  ///   If this method returns <c>true</c>, this is a copy of the sub-set
  ///   for the given prefix. Otherwise, this is <c>null</c>.
  /// </param>
  /// <returns>
  ///   Whether or not the set contains the given prefix.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public bool TryGetPrefix(IEnumerable<T> prefix, [MaybeNullWhen(false)] out IRecursiveSet<T> result)
  {
    if (prefix == null) throw new ArgumentNullException(nameof(prefix));
    if (Backing.TryGetPrefix(prefix, out var resultDict))
    {
      result = new RecursiveSet<T>((RecursiveDictionary<T, bool>)resultDict);
      return true;
    }
    result = null;
    return false;
  }

  /// <summary>
  ///   Removes all values from the set with the given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>
  ///   The number of entries removed, which is <c>0</c> if the given
  ///   prefix did not exist.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public int RemovePrefix(IEnumerable<T> prefix) => Backing.RemovePrefix(prefix);

  /// <summary>
  ///   Adds a sequence to the set.
  /// </summary>
  /// <param name="item">The sequence to add.</param>
  void ICollection<IEnumerable<T>>.Add(IEnumerable<T> item) => this.Add(item);

  /// <summary>
  ///   Removes a sequence from the set.
  /// </summary>
  /// <param name="item">The sequence to remove.</param>
  /// <returns>
  ///   <c>true</c> if the sequence was actually removed from the set,
  ///   <c>false</c> otherwise (including if the sequence was never in the
  ///   set in the first place).
  /// </returns>
  public bool Remove(IEnumerable<T> item) => Backing.Remove(item);

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
