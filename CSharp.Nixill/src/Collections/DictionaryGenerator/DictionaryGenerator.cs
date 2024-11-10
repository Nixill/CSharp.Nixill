using System;
using System.Collections;
using System.Collections.Generic;

namespace Nixill.Collections
{
  /// <summary>
  /// A class that wraps around an
  /// <see cref="IDictionary&lt;TKey, TValue&gt;" />, adding automatic
  /// generation of values from keys.
  /// </summary>
  /// <typeparam name="K">
  /// The type of the keys used in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  /// The type of the values used in the dictionary.
  /// </typeparam>
  public class DictionaryGenerator<K, V> : IDictionary<K, V> where K : notnull
  {
    /// <summary>
    /// The <see cref="Generator" /> used by this <c>DictionaryGenerator</c>.
    /// </summary>
    public Generator<K, V> Generator { get; }

    /// <summary>
    /// The <see cref="IDictionary&lt;TKey, TValue&gt;" /> contained by
    /// this <c>DictionaryGenerator</c>.
    /// </summary>
    public IDictionary<K, V> Dict { get; }

    /// <summary>
    /// Whether or not to store newly generated values when a key is not
    /// found in the dictionary during a get operation. Does not affect
    /// previously generated values, whether they were or weren't stored.
    /// </summary>
    public bool StoreGeneratedValues = true;

    // The primary generator
    public DictionaryGenerator(IDictionary<K, V> dict, Generator<K, V> gen, bool storeValues = true)
    {
      Dict = dict;
      Generator = gen;
      StoreGeneratedValues = storeValues;
    }

    public DictionaryGenerator()
    : this(new Dictionary<K, V>(), new DefaultGenerator<K, V>(), true)
    { }

    public DictionaryGenerator(bool storeValues)
    : this(new Dictionary<K, V>(), new DefaultGenerator<K, V>(), storeValues)
    { }

    public DictionaryGenerator(Generator<K, V> gen, bool storeValues = true)
    : this(new Dictionary<K, V>(), gen, storeValues)
    { }

    public DictionaryGenerator(Func<K, V> genFunc, bool storeValues = true)
    : this(new Dictionary<K, V>(), new FuncGenerator<K, V>(genFunc), storeValues)
    { }

    public DictionaryGenerator(IDictionary<K, V> dict, Func<K, V> genFunc, bool storeValues = true)
    : this(dict, new FuncGenerator<K, V>(genFunc), storeValues)
    { }

    public DictionaryGenerator(V item, bool storeValues = true)
    : this(new Dictionary<K, V>(), new SingleValueGenerator<K, V>(item), storeValues)
    { }

    public DictionaryGenerator(IDictionary<K, V> dict, V item, bool storeValues = true)
    : this(dict, new SingleValueGenerator<K, V>(item), storeValues)
    { }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    ///
    /// If the specified key is not found, a get operation automatically
    /// generates a value, sets it to that key (if the DictionaryGenerator
    /// is set to do so), and returns it.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    public V this[K key]
    {
      get
      {
        if (Dict.ContainsKey(key)) return Dict[key];
        else if (StoreGeneratedValues) return Add(key);
        else return Generator.Generate(key);
      }
      set => Dict[key] = value;
    }

    /// <summary>
    /// Adds the specified key, and an automatically-generated value, to
    /// the dictionary.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <returns>The added value.</returns>
    public V Add(K key)
    {
      if (key == null)
      {
        throw new ArgumentNullException("Null keys cannot be added to GeneratorDictionaries.");
      }
      else if (Dict.ContainsKey(key))
      {
        throw new ArgumentException("Key " + key.ToString() + " already exists in the Dictionary.");
      }
      else
      {
        V val = Generator.Generate(key);
        Dict[key] = val;
        return val;
      }
    }

    /// <summary>Adds the specified key and value to the dictionary.</summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    public void Add(K key, V value) => Dict.Add(key, value);

    /// <summary>
    /// Whether or not any value can be generated for a key without
    /// throwing an exception.
    ///
    /// See <see  cref="Generator.CanGenerateFrom(K)" /> for return value
    /// details.
    ///
    /// This method does not check current dictionary contents. In
    /// particular, it may return <c>true</c> for keys already contained
    /// in the dictionary.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    public bool? CanGenerateForKey(K key) => (key == null) ? false : Generator.CanGenerateFrom(key);

    /// <summary>
    /// Whether or not a specific value can be generated immediately.
    /// 
    /// See <see cref="Generator.CanGenerate(V)" /> for return value
    /// details.
    ///
    /// This method does not check current dictionary contents. In
    /// particular, it may return <c>true</c> for values that can only be
    /// generated from keys already contained in the dictionary, and may
    /// also return <c>false</c> for values already contained in the
    /// dictionary.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    public bool? CanGenerateValue(V val) => Generator.CanGenerate(val);

    public bool ContainsKey(K key) => Dict.ContainsKey(key);
    public bool Remove(K key) => Dict.Remove(key);
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
    public ICollection<K> Keys => Dict.Keys;
    public ICollection<V> Values => Dict.Values;

    public void Add(KeyValuePair<K, V> entry)
    {
      Add(entry.Key, entry.Value);
    }

    public void Clear()
    {
      Dict.Clear();
    }

    public bool Contains(KeyValuePair<K, V> entry) => Dict.ContainsKey(entry.Key) && (Dict[entry.Key]!.Equals(entry.Value));

    public void CopyTo(KeyValuePair<K, V>[] array, int index)
    {
      Dict.CopyTo(array, index);
    }

    public bool Remove(KeyValuePair<K, V> entry)
    {
      if (Contains(entry)) return Remove(entry.Key);
      else return false;
    }

    public int Count => Dict.Count;
    public bool IsReadOnly => Dict.IsReadOnly;
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => Dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Dict.GetEnumerator();
  }

  /// <summary>
  /// A class used by DictionaryGenerator to create values for given keys.
  /// </summary>
  public abstract class Generator<K, V> where K : notnull
  {
    /// <summary>
    /// Returns a value for the given key.
    /// </summary>
    public abstract V Generate(K key);

    /// <summary>
    /// Whether or not any value can be generated for a key without
    /// throwing an exception.
    ///
    /// More specifically, this method checks whether the <i>current
    /// state</i> of the Generator can accept this key and return a value.
    ///
    /// A return value of <c>true</c> means an immediate call to Generate
    /// with the given key will definitely succeed. A return value of
    /// <c>false</c> means an immediate call to Generate with the given
    /// key will definitely throw an exception. A return value of
    /// <c>null</c> means no such promises can be made either way.
    ///
    /// Any value returned by this method is invalidated if the internal
    /// state of this Generator changes after it is called.
    ///
    /// When overriding this method, <c>null</c> is always a safe return
    /// value.
    /// </summary>
    public bool? CanGenerateFrom(K key) => null;

    /// <summary>
    /// Whether or not a specific value can be generated immediately.
    ///
    /// More specifically, whether there is any key that would cause the
    /// given value (or another value for which <c>Equals()</c>, with the
    /// supplied value, returns true) to be generated immediately with the
    /// <i>current state</i> of the Generator.
    ///
    /// A return value of <c>true</c> means that there exists at least one
    /// key that could cause the value to be generated on the next call to
    /// <c>Generate()</c>. A return value of <c>false</c> means that there
    /// exists no key that could possibly cause the value to be generated.
    /// A return value of <c>null</c> means neither of the above promises
    /// can be made.
    ///
    /// Any value returned by this method is invalidated if the internal
    /// state of this Generator changes after it is called.
    ///
    /// When overriding this method, <c>null</c> is always a safe return
    /// value.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    public bool? CanGenerate(V val) => null;

    public DictionaryGenerator<K, V> Wrap(IDictionary<K, V> dict) => new DictionaryGenerator<K, V>(dict, this);

    public DictionaryGenerator<K, V> Wrap() => new DictionaryGenerator<K, V>(this);
  }

  public static class DictionaryGeneratorExtensions
  {
    public static DictionaryGenerator<K, V> WithGenerator<K, V>(this IDictionary<K, V> dict, Generator<K, V> gen)
      where K : notnull
      => new DictionaryGenerator<K, V>(dict, gen);

    public static DictionaryGenerator<K, V> CopyWithGenerator<K, V>(this IDictionary<K, V> dict, Generator<K, V> gen)
      where K : notnull
      => new DictionaryGenerator<K, V>(new Dictionary<K, V>(dict), gen);
  }
}