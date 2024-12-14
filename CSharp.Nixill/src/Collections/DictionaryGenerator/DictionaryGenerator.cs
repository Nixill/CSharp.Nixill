using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections
{
  /// <summary>
  ///   A class that wraps around an
  ///   <see cref="IDictionary&lt;TKey, TValue&gt;" />, adding automatic
  ///   generation of entries from keys.
  /// </summary>
  /// <typeparam name="K">
  ///   The type of the keys used in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  ///   The type of the values used in the dictionary.
  /// </typeparam>
  public class DictionaryGenerator<K, V> : IDictionary<K, V> where K : notnull
  {
    /// <summary>
    ///   Get: The <see cref="Generator{K, V}" /> used by this 
    ///   <see cref="DictionaryGenerator{K, V}"/>.
    /// </summary>
    public Generator<K, V> Generator { get; }

    /// <summary>
    ///   Get: The <see cref="IDictionary{K, V}" /> wrapped by this
    ///   <see cref="DictionaryGenerator{K, V}"/>.
    /// </summary>
    public IDictionary<K, V> Dictionary { get; }

    /// <summary>
    ///   Get or set: Whether or not to store newly generated entries when
    ///   a key is not found in the dictionary during a get operation.
    ///   Does not affect previously generated entries, whether they were
    ///   or weren't stored.
    /// </summary>
    public bool StoreGeneratedValues = true;

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping the given <see cref="IDictionary{TKey, TValue}"/> and
    ///   using the given <see cref="Generator{K, V}"/> to generate entries.
    /// </summary>
    /// <remarks>
    ///   This method doesn't create a new copy of the dictionary being
    ///   wrapped; instead, the existing dictionary instance is wrapped.
    /// </remarks>
    /// <param name="dict">The dictionary to wrap.</param>
    /// <param name="gen">The generator to use.</param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(IDictionary<K, V> dict, Generator<K, V> gen, bool storeValues = true)
    {
      Dictionary = dict;
      Generator = gen;
      StoreGeneratedValues = storeValues;
    }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping a new <see cref="Dictionary{K, V}"/> and using a
    ///   <see cref="DefaultGenerator{K, V}"/> to generate entries.
    /// </summary>
    /// <remarks>
    ///   When using this constructor, newly generated entries are stored
    ///   in the dictionary upon generation.
    /// </remarks>
    public DictionaryGenerator()
    : this(new Dictionary<K, V>(), new DefaultGenerator<K, V>(), true)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping a new <see cref="Dictionary{K, V}"/> and using a
    ///   <see cref="DefaultGenerator{K, V}"/> to generate entries.
    /// </summary>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(bool storeValues)
    : this(new Dictionary<K, V>(), new DefaultGenerator<K, V>(), storeValues)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping a new <see cref="Dictionary{K, V}"/> and using the
    ///   given <see cref="Generator{K, V}"/> to generate entries.
    /// </summary>
    /// <param name="gen">The generator to use.</param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(Generator<K, V> gen, bool storeValues = true)
    : this(new Dictionary<K, V>(), gen, storeValues)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping a new <see cref="Dictionary{K, V}"/> and using a
    ///   <see cref="FuncGenerator{K, V}"/> powered by the given
    ///   <see cref="Func{K, V}"/> to generate entries.
    /// </summary>
    /// <param name="genFunc">
    ///   The function that generates entry values.
    /// </param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(Func<K, V> genFunc, bool storeValues = true)
    : this(new Dictionary<K, V>(), new FuncGenerator<K, V>(genFunc), storeValues)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping the given <see cref="IDictionary{TKey, TValue}"/> and
    ///   using a <see cref="FuncGenerator{K, V}"/> powered by the given
    ///   <see cref="Func{K, V}"/> to generate entries.
    /// </summary>
    /// <remarks>
    ///   This method doesn't create a new copy of the dictionary being
    ///   wrapped; instead, the existing dictionary instance is wrapped.
    /// </remarks>
    /// <param name="dict">The dictionary to wrap.</param>
    /// <param name="genFunc">
    ///   The function that generates entry values.
    /// </param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(IDictionary<K, V> dict, Func<K, V> genFunc, bool storeValues = true)
    : this(dict, new FuncGenerator<K, V>(genFunc), storeValues)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping a new <see cref="Dictionary{K, V}"/> and using the
    ///   given value to generate new entries.
    /// </summary>
    /// <param name="item">The value to put in new entries.</param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(V item, bool storeValues = true)
    : this(new Dictionary<K, V>(), new SingleValueGenerator<K, V>(item), storeValues)
    { }

    /// <summary>
    ///   Constructs a new <see cref="DictionaryGenerator{K, V}"/>,
    ///   wrapping the given <see cref="IDictionary{TKey, TValue}"/> and
    ///   using the given value to generate new entries.
    /// </summary>
    /// <remarks>
    ///   This method doesn't create a new copy of the dictionary being
    ///   wrapped; instead, the existing dictionary instance is wrapped.
    /// </remarks>
    /// <param name="dict">The dictionary to wrap.</param>
    /// <param name="item">The value to put in new entries.</param>
    /// <param name="storeValues">
    ///   Whether or not newly generated entries should be stored in the
    ///   dictionary upon generation.
    /// </param>
    public DictionaryGenerator(IDictionary<K, V> dict, V item, bool storeValues = true)
    : this(dict, new SingleValueGenerator<K, V>(item), storeValues)
    { }

    /// <summary>
    ///   Get or set: The value associated with the specified key.
    /// </summary>
    /// <remarks>
    ///   If the specified key is not found, a get operation automatically
    ///   generates an entry, saves it to the dictionary (if the
    ///   <see cref="DictionaryGenerator{K, V}"/> is set to do so), and
    ///   then returns it.
    /// </remarks>
    /// <param name="key">The key of the entry to get or set.</param>
    public V this[K key]
    {
      get
      {
        if (Dictionary.ContainsKey(key)) return Dictionary[key];
        else if (StoreGeneratedValues) return Add(key);
        else return Generator.Generate(key);
      }
      set => Dictionary[key] = value;
    }

    /// <summary>
    ///   Adds the specified key, and an automatically-generated value, to
    ///   the dictionary.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <returns>The added value.</returns>
    public V Add(K key)
    {
      if (key == null)
      {
        throw new ArgumentNullException("Null keys cannot be added to GeneratorDictionaries.");
      }
      else if (Dictionary.ContainsKey(key))
      {
        throw new ArgumentException("Key " + key.ToString() + " already exists in the Dictionary.");
      }
      else
      {
        V val = Generator.Generate(key);
        Dictionary[key] = val;
        return val;
      }
    }

    /// <summary>
    ///   Adds the specified key and value to the dictionary.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    public void Add(K key, V value) => Dictionary.Add(key, value);

    /// <summary>
    ///   Returns whether or not any value can be generated for a key
    ///   without throwing an exception.
    /// </summary>
    /// <remarks>
    ///   This method considers the current state of the generator, but
    ///   not its history, nor the current or past state of the
    ///   dictionary.
    /// </remarks>
    /// <param name="key">The key to check for.</param>
    /// <returns>
    ///   <c>true</c> if it is safe to generate a value for the given key,
    ///   <c>false</c> if the given key will cause an exception, or
    ///   <c>null</c> if the answer is not known.
    /// </returns>
    public bool? CanGenerateForKey(K key) => (key == null) ? false : Generator.CanGenerateFrom(key);

    /// <summary>
    ///   Returns whether or not the wrapped dictionary contains an entry
    ///   with the given key.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <returns>
    ///   <c>true</c> iff an entry exists with the given key.
    /// </returns>
    public bool ContainsKey(K key) => Dictionary.ContainsKey(key);

    /// <summary>
    ///   Removes the entry with the given key from the dictionary.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>
    ///   <c>true</c> iff an entry was removed with the given key;
    ///   <c>false</c> otherwise (which may include if no such entry existed).
    /// </returns>
    public bool Remove(K key) => Dictionary.Remove(key);

    /// <summary>
    ///   Retrieves the value for a given key, if it's present in the
    ///   dictionary.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <param name="value">
    ///   If the method returns <c>true</c>, this is the value associated
    ///   with the given key. If this method returns <c>false</c>, this is
    ///   <c>default(V)</c>.
    /// </param>
    /// <returns>Whether or not such an entry was found.</returns>
    public bool TryGetValue(K key, out V value)
    {
      try
      {
        value = this[key];
        return true;
      }
      catch (KeyNotFoundException)
      {
        value = default(V)!;
        return false;
      }
    }

    /// <summary>
    ///   Get: A read-only collection of the keys in the dictionary.
    /// </summary>
    public ICollection<K> Keys => Dictionary.Keys;

    /// <summary>
    ///   Get: A read-only collection of the values in the dictionary.
    /// </summary>
    public ICollection<V> Values => Dictionary.Values;

    /// <summary>
    ///   Adds an entry to the dictionary.
    /// </summary>
    /// <param name="entry">The entry to add.</param>
    public void Add(KeyValuePair<K, V> entry)
    {
      Add(entry.Key, entry.Value);
    }

    /// <summary>
    ///   Clears the dictionary.
    /// </summary>
    public void Clear()
    {
      Dictionary.Clear();
    }

    /// <summary>
    ///   Returns whether or not the dictionary contains a specified
    ///   entry.
    /// </summary>
    /// <param name="entry">The entry to check for.</param>
    /// <returns>
    ///   <c>true</c> iff the specified entry was matched (both key and value).
    /// </returns>
    public bool Contains(KeyValuePair<K, V> entry) => Dictionary.ContainsKey(entry.Key) && (Dictionary[entry.Key]!.Equals(entry.Value));

    /// <summary>
    ///   Copies the entries of this dictionary to an Array, starting at a
    ///   particular Array index.
    /// </summary>
    /// <param name="array">
    ///   The one-dimensional Array that is the destination of the entries
    ///   copied from this dictionary. The Array must have zero-based indexing.
    /// </param>
    /// <param name="index">
    ///   The zero-based index in array at which copying begins.
    /// </param>
    public void CopyTo(KeyValuePair<K, V>[] array, int index)
    {
      Dictionary.CopyTo(array, index);
    }

    /// <summary>
    ///   Removes an entry from the dictionary, if both key and value match.
    /// </summary>
    /// <param name="entry">The entry to remove</param>
    /// <returns>
    ///   <c>true</c> iff the specified entry was removed; false otherwise
    ///   (which may mean that the specified key was not found, or not
    ///   associated with the specified value, etc.).
    /// </returns>
    public bool Remove(KeyValuePair<K, V> entry)
    {
      if (Contains(entry)) return Remove(entry.Key);
      else return false;
    }

    /// <summary>
    ///   Get: The number of objects in this dictionary.
    /// </summary>
    public int Count => Dictionary.Count;

    /// <summary>
    ///   Whether or not the wrapped dictionary is read-only.
    /// </summary>
    public bool IsReadOnly => Dictionary.IsReadOnly;

    /// <summary>
    ///   Gets an enumerator over the entries of the wrapped dictionary.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => Dictionary.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
  }

  /// <summary>
  ///   A class used by DictionaryGenerator to create values for given keys.
  /// </summary>
  public abstract class Generator<K, V> where K : notnull
  {
    /// <summary>
    ///   Returns a value for the given key.
    /// </summary>
    public abstract V Generate(K key);

    /// <summary>
    ///   Whether or not any value can be immediately generated for a key
    ///   without throwing an exception.
    /// </summary>
    /// <remarks>
    ///   More specifically, this method checks whether the <em>current
    ///   state</em> of the Generator can accept this key and return a
    ///   value.
    ///   <para/>
    ///   Any value returned by this method is invalidated if the internal
    ///   state of this Generator changes after it is called.
    ///   <para/>
    ///   When overriding this method, <c>null</c> is always a safe return
    ///   value.
    /// </remarks>
    /// <param name="key">The key to check.</param>
    /// <returns>
    ///   <c>true</c> means that immediately calling
    ///   <see cref="Generate(K)"/> with the given key will successfully
    ///   return a value. <c>false</c> means that immediately calling
    ///   <see cref="Generate(K)"/> with the given key will throw an
    ///   exception. <c>null</c> means the method cannot make a promise
    ///   either way, or the information is not known.
    /// </returns>
    public virtual bool? CanGenerateFrom(K key) => null;

    /// <summary>
    ///   Returns a <see cref="DictionaryGenerator{K, V}"/> using this
    ///   <see cref="Generator{K, V}"/> to wrap the given
    ///   <see cref="IDictionary{K, V}"/>.
    /// </summary>
    /// <remarks>
    ///   This method doesn't create a new copy of the dictionary being
    ///   wrapped; instead, the existing dictionary instance is wrapped.
    /// </remarks>
    /// <param name="dict">The dictionary to wrap.</param>
    /// <returns>The DictionaryGenerator.</returns>
    public DictionaryGenerator<K, V> Wrap(IDictionary<K, V> dict) => new DictionaryGenerator<K, V>(dict, this);

    /// <summary>
    ///   Returns a <see cref="DictionaryGenerator{K, V}"/> using this
    ///   <see cref="Generator{K, V}"/> to wrap a new, empty
    ///   <see cref="Dictionary{K, V}"/>.
    /// </summary>
    /// <returns>The DictionaryGenerator.</returns>
    public DictionaryGenerator<K, V> Wrap() => new DictionaryGenerator<K, V>(this);
  }

  /// <summary>
  ///   DictionaryGenerator wrapper extension methods.
  /// </summary>
  public static class DictionaryGeneratorExtensions
  {
    /// <summary>
    ///   Wraps this dictionary in a
    ///   <see cref="DictionaryGenerator{K, V}"/> with the given
    ///   <see cref="Generator{K, V}"/>.
    /// </summary>
    /// <remarks>
    ///   This method doesn't create a new copy of the dictionary being
    ///   wrapped; instead, the existing dictionary instance is wrapped.
    /// </remarks>
    /// <typeparam name="K">
    ///   The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="V">
    ///   The type of values in the dictionary.
    /// </typeparam>
    /// <param name="dict">The dictionary being wrapped.</param>
    /// <param name="gen">The generator to use.</param>
    /// <returns>The DictionaryGenerator.</returns>
    public static DictionaryGenerator<K, V> WithGenerator<K, V>(this IDictionary<K, V> dict, Generator<K, V> gen)
      where K : notnull
      => new DictionaryGenerator<K, V>(dict, gen);

    /// <summary>
    ///   Copies this dictionary and wraps the copy in a
    ///   <see cref="DictionaryGenerator{K, V}"/> with the given
    ///   <see cref="Generator{K, V}"/>.
    /// </summary>
    /// <typeparam name="K">
    ///   The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="V">
    ///   The type of values in the dictionary.
    /// </typeparam>
    /// <param name="dict">The dictionary being wrapped.</param>
    /// <param name="gen">The generator to use.</param>
    /// <returns>The DictionaryGenerator.</returns>
    public static DictionaryGenerator<K, V> CopyWithGenerator<K, V>(this IDictionary<K, V> dict, Generator<K, V> gen)
      where K : notnull
      => new DictionaryGenerator<K, V>(new Dictionary<K, V>(dict), gen);
  }
}