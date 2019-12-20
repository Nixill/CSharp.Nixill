using System;
using System.Collections.Generic;

namespace Nixill.Utils {
  public class GeneratorDictionary<K, V> : Dictionary<K, V> {
    public Generator<K, V> Generator { get; protected set; }

    public GeneratorDictionary(Generator<K, V> func) {
      Generator = func;
    }

    public GeneratorDictionary(IDictionary<K, V> dictionary, Generator<K, V> func) : base(dictionary) {
      Generator = func;
    }

    public GeneratorDictionary(IEqualityComparer<K> comparer, Generator<K, V> func) : base(comparer) {
      Generator = func;
    }

    public GeneratorDictionary(int capacity, Generator<K, V> func) : base(capacity) {
      Generator = func;
    }

    public GeneratorDictionary(IDictionary<K, V> dictionary, IEqualityComparer<K> comparer, Generator<K, V> func) : base(dictionary, comparer) {
      Generator = func;
    }

    public GeneratorDictionary(int capacity, IEqualityComparer<K> comparer, Generator<K, V> func) : base(capacity, comparer) {
      Generator = func;
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