using System;
namespace Nixill.Utils {
  public class SingleValueGenerator<K, V> : Generator<K, V> {
    public V Val { get; }

    public SingleValueGenerator(V val) {
      Val = val;
    }

    public override V Generate(K key) => Val;
    public new bool? CanGenerate(V val) {
      if (Val == null) {
        return val == null;
      }
      else {
        return Val.Equals(val);
      }
    }
  }

  public class EchoGenerator<T> : Generator<T, T> {
    public override T Generate(T key) => key;
    public new bool? CanGenerate(T val) {
      return val != null;
    }
  }

  public class CountingGenerator<K> : Generator<K, int> {
    public int Count { get; private set; }
    public CountingGenerator() {
      Count = 0;
    }

    public override int Generate(K key) {
      if (Count == Int32.MaxValue) {
        return Count;
      }
      else {
        return Count++;
      }
    }
    public new bool? CanGenerate(int val) {
      return Count <= val;
    }
  }

  public class FuncGenerator<K, V> : Generator<K, V> {
    public Func<K, V> BackingFunc { get; }
    public Func<K, bool?> KeyCheckFunc { get; }
    public Func<V, bool?> ValCheckFunc { get; }

    public FuncGenerator(Func<K, V> func) : this(func, (key) => key != null, (val) => null) { }

    public FuncGenerator(Func<K, V> func, Func<K, bool?> keyCheck, Func<V, bool?> valCheck) {
      BackingFunc = func;
      KeyCheckFunc = keyCheck;
      ValCheckFunc = valCheck;
    }

    public override V Generate(K key) => BackingFunc.Invoke(key);
    public new bool? CanGenerateFrom(K key) => KeyCheckFunc.Invoke(key);
    public new bool? CanGenerate(V val) => ValCheckFunc.Invoke(val);
  }

  public class HashCodeGenerator<K> : Generator<K, int> {
    public override int Generate(K key) => key.GetHashCode();
  }

  public class ToStringGenerator<K> : Generator<K, string> {
    public override string Generate(K key) => key.ToString();
  }
}