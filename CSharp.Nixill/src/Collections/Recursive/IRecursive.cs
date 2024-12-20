using System.Diagnostics.CodeAnalysis;

namespace Nixill.Collections;

/// <summary>
///   Interface for recursive dictionaries, where a value is stored in a
///   subtree for each element in the key sequence.
/// </summary>
/// <typeparam name="K">
///   The type of elements in the sequences that form keys in the dictionary.
///   <para/>
///   Note that this is specifically the <em>elements of the
///   sequence</em>. For example, you would write <c>char</c> here if you
///   want to have a dictionary of <c>string</c>s, or <c>int</c> if you
///   want a dictionary of <c>int[]</c>s.
/// </typeparam>
/// <typeparam name="V">
///   The type of values in the dictionary.
/// </typeparam>
public interface IRecursiveDictionary<K, V> : IDictionary<IEnumerable<K>, V> where K : notnull
{
  /// <summary>
  ///   Get: Whether or not the dictionary has a value for the empty key.
  /// </summary>
  public bool HasEmptyKeyValue { get; }

  /// <summary>
  ///   Get: A collection of all the first-element prefixes contained in
  ///   this set.
  /// </summary>
  public ICollection<K> Prefixes { get; }

  /// <summary>
  ///   Whether the dictionary contains any keys with the given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>
  ///   <c>true</c> iff at least one key in the dictionary starts with
  ///   this prefix.
  /// </returns>
  public bool ContainsPrefix(IEnumerable<K> prefix);

  /// <summary>
  ///   Tries to get the sub-dictionary for a given prefix.
  /// </summary>
  /// <remarks>
  ///   If a sub-dicttionary is given, it is an <em>unlinked copy</em> of
  ///   the given dictionary for the given prefix. Additionally, the keys
  ///   in the sub-dictionary have the specified prefix removed.
  /// </remarks>
  /// <param name="prefix">The prefix.</param>
  /// <param name="result">
  ///   If this method returns <c>true</c>, this is a copy of the
  ///   sub-dictionary for the given prefix, not linked to the original.
  ///   Otherwise, this is <c>null</c>.
  /// </param>
  /// <returns>
  ///   Whether or not the dictionary contains the given prefix.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public bool TryGetPrefix(IEnumerable<K> prefix, [MaybeNullWhen(false)] out IRecursiveDictionary<K, V> result);

  /// <summary>
  ///   Gets the sub-dictionary for a given prefix.
  /// </summary>
  /// <remarks>
  ///   The sub-dicttionary is an <em>unlinked copy</em> of the given
  ///   dictionary for the given prefix. Additionally, the keys in the
  ///   sub-dictionary have the specified prefix removed.
  /// </remarks>
  /// <param name="prefix">The prefix.</param>
  /// <returns>The sub-dictionary.</returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  /// <exception cref="KeyNotFoundException">
  ///   The given prefix does not exist in the dictionary.
  /// </exception>
  public IRecursiveDictionary<K, V> GetPrefix(IEnumerable<K> prefix);

  /// <summary>
  ///   Removes all entries from the dictionary with the given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>
  ///   The number of entries removed, which is <c>0</c> if the given
  ///   prefix did not exist.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public int RemovePrefix(IEnumerable<K> prefix);
}

/// <summary>
///   Interface for recursive sets, where subtrees are used to store sets
///   by their elements.
/// </summary>
/// <typeparam name="T">
///   The type of elements in the sequences that form items in the set.
///   <para/>
///   Note that this is specifically the <em>elements of the
///   sequence</em>. For example, you would write <c>char</c> here if you
///   want to have a set of <c>string</c>s, or <c>int</c> if you want a 
///   set of <c>int[]</c>s.
/// </typeparam>
public interface IRecursiveSet<T> : ICollection<IEnumerable<T>>
{
  /// <summary>
  ///   Get: Whether or not the set contains the empty-sequence value.
  /// </summary>
  public bool HasEmptyValue { get; }

  /// <summary>
  ///   Get: A collection of all the first-element prefixes contained in
  ///   this set.
  /// </summary>
  public ICollection<T> Prefixes { get; }

  /// <summary>
  ///   Whether the set contains any values with the given prefix.
  /// </summary>
  /// <param name="prefix">The prefix.</param>
  /// <returns>
  ///   <c>true</c> iff at least one value in the set starts with this prefix.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  public bool ContainsPrefix(IEnumerable<T> prefix);

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
  public bool TryGetPrefix(IEnumerable<T> prefix, [MaybeNullWhen(false)] out IRecursiveSet<T> result);

  /// <summary>
  ///   Gets the sub-set for a given prefix.
  /// </summary>
  /// <remarks>
  ///   The sub-set is an <em>unlinked copy</em> of the given set for the
  ///   given prefix. Additionally, the values in the sub-set have the
  ///   specified prefix removed.
  /// </remarks>
  /// <param name="prefix">The prefix.</param>
  /// <returns>The sub-set.</returns>
  /// <exception cref="ArgumentNullException">
  ///   <c>prefix</c> is <c>null</c>.
  /// </exception>
  /// <exception cref="KeyNotFoundException">
  ///   The given prefix does not exist in the set.
  /// </exception>
  public IRecursiveSet<T> GetPrefix(IEnumerable<T> prefix);

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
  public int RemovePrefix(IEnumerable<T> prefix);
}

