using System;
using System.Collections.Generic;

namespace Nixill.Utils {
  /// <summary>
  /// A class that extends <a cref="Dictionary">Dictionary</a>, adding
  /// automatic generation of values from keys.
  /// </summary>
  /// <typeparam name="K">
  /// The type of the keys used in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  /// The type of the values used in the dictionary.
  /// </typeparam>
  public class GeneratorDictionary<K, V> : Dictionary<K, V> {
    /// <summary>
    /// The <a cref="Generator">Generator</a> used by this
    /// <c>GeneratorDictionary</c>.
    /// </summary>
    public Generator<K, V> Generator { get; }

    /// <summary>
    /// Creates a new, empty Dictionary with the default initial capacity
    /// and default equality comparer.
    /// </summary>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(Generator<K, V> gen) {
      Generator = gen;
    }

    /// <summary>
    /// Copies an existing IDictionary into a new GeneratorDictionary.
    /// </summary>
    /// <param name="dictionary">The IDictionary to copy.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(IDictionary<K, V> dictionary, Generator<K, V> gen) : base(dictionary) {
      Generator = gen;
    }

    public GeneratorDictionary(IEqualityComparer<K> comparer, Generator<K, V> gen) : base(comparer) {
      Generator = gen;
    }

    public GeneratorDictionary(int capacity, Generator<K, V> gen) : base(capacity) {
      Generator = gen;
    }

    public GeneratorDictionary(IDictionary<K, V> dictionary, IEqualityComparer<K> comparer, Generator<K, V> gen) : base(dictionary, comparer) {
      Generator = gen;
    }

    public GeneratorDictionary(int capacity, IEqualityComparer<K> comparer, Generator<K, V> gen) : base(capacity, comparer) {
      Generator = gen;
    }

    public new V this[K key] {
      get {
        try {
          return base[key];
        }
        catch (KeyNotFoundException) {
          return Add(key);
        }
      }
      set => base[key] = value;
    }

    public V Add(K key) {
      if (key == null) {
        throw new ArgumentNullException("Null keys cannot be added to GeneratorDictionaries.");
      }
      else if (ContainsKey(key)) {
        throw new ArgumentException("Key " + key.ToString() + " already exists in map.");
      }
      else {
        V val = Generator.Generate(key);
        base[key] = val;
        return val;
      }
    }

    public bool? CanGenerateForKey(K key) => Generator.CanGenerateFrom(key);
    public bool? CanGenerateValue(V val) => Generator.CanGenerate(val);
  }

  public abstract class Generator<K, V> {
    public abstract V Generate(K key);
    public bool? CanGenerateFrom(K key) => key != null;
    public bool? CanGenerate(V val) => null;
  }
}