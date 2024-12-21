using System;
using System.Reflection;

namespace Nixill.Collections;

/// <summary>
///   A <see cref="Generator{K, V}"/> that always returns the same value.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
/// <typeparam name="V">
///   The type of the values in this generator.
/// </typeparam>
public class SingleValueGenerator<K, V> : Generator<K, V> where K : notnull
{
  /// <summary>
  ///   Get: The value this Generator returns.
  /// </summary>
  public V Val { get; }

  /// <summary>
  ///   Constructs a new SingleValueGenerator with a given value.
  /// </summary>
  /// <param name="val">The value to use.</param>
  public SingleValueGenerator(V val)
  {
    Val = val;
  }

  /// <summary>
  ///   Returns the initially supplied value, regardless of key.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><see cref="Val"/></returns>
  public override V Generate(K key) => Val;

  /// <summary>
  ///   Returns whether or not a value can be generated for the given
  ///   key without throwing an exception. This is <c>true</c> for any
  ///   key, even <c>null</c>.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><c>true</c>.</returns>
  public override bool? CanGenerateFrom(K key) => true;
}

/// <summary>
///   A Generator that returns the key as its own value.
/// </summary>
/// <typeparam name="T">
///   The type of both the keys and the values in this generator, not null.
/// </typeparam>
public class EchoGenerator<T> : Generator<T, T> where T : notnull
{
  /// <summary>
  ///   Returns (echoes) the supplied value.
  /// </summary>
  /// <param name="key">
  ///   The key for which a value should be generated.
  /// </param>
  /// <returns><c>key</c></returns>
  public override T Generate(T key) => key;

  /// <summary>
  ///   Returns whether or not a value can be generated for the given
  ///   key without throwing an exception. This is <c>true</c> for any
  ///   key, even <c>null</c>.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><c>true</c>.</returns>
  public override bool? CanGenerateFrom(T key) => true;
}

/// <summary>
///   A Generator that returns incrementally counted keys, starting with
///   zero.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
public class CountingGenerator<K> : Generator<K, int> where K : notnull
{
  /// <summary>
  ///   Get: The next number that will be Generated.
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  ///   Constructs a new <see cref="CountingGenerator{K}"/>.
  /// </summary>
  public CountingGenerator()
  {
    Count = 0;
  }

  /// <summary>
  ///   Returns the next integer in sequence.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns>
  ///   <see cref="Count"/>, which is then incremented by 1.
  /// </returns>
  public override int Generate(K key) => Count++;

  /// <summary>
  ///   Returns whether or not a value can be generated for the given
  ///   key without throwing an exception. This is <c>true</c> for any
  ///   key, including <c>null</c>.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><c>true</c>.</returns>
  public override bool? CanGenerateFrom(K key) => true;
}

/// <summary>
///   A <see cref="Generator{K, V}"/> based on an arbitrary
///   <see cref="Func{K, V}"/>.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
/// <typeparam name="V">
///   The type of the values in this generator.
/// </typeparam>
public class FuncGenerator<K, V> : Generator<K, V> where K : notnull
{
  /// <summary>
  ///   The <see cref="Func{K, V}"/> that is used to perform the actual
  ///   generation of values.
  /// </summary>
  public Func<K, V> GeneratingFunc { get; }

  /// <summary>
  ///   The <see cref="Func{K, bool?}"/> that checks whether or not a
  ///   value can be generated for a given key.
  /// </summary>
  public Func<K, bool?> KeyCheckFunc { get; }

  /// <summary>
  ///   Constructs a new <see cref="FuncGenerator{K, V}"/> with a given
  ///   generating func, and the default key check function (which will
  ///   always return <c>null</c>).
  /// </summary>
  public FuncGenerator(Func<K, V> func) : this(func, (key) => null) { }

  /// <summary>
  ///   Constructs a new <see cref="FuncGenerator{K, V}"/> with given
  ///   generating and key-checking <see cref="Func{K, V}"/>s.
  /// </summary>
  public FuncGenerator(Func<K, V> func, Func<K, bool?> keyCheck)
  {
    GeneratingFunc = func;
    KeyCheckFunc = keyCheck;
  }

  /// <summary>
  ///   Generates a value for the given key using the
  ///   <see cref="Func{K, V}"/> provided in the constructor.
  /// </summary>
  /// <param name="key">The key to use.</param>
  /// <returns>The generated value.</returns>
  public override V Generate(K key) => GeneratingFunc(key);

  /// <summary>
  ///   Returns whether or not a value can be generated for the given
  ///   key using the <see cref="Func{K, bool?}"/> provided in the
  ///   constructor.
  /// </summary>
  /// <param name="key">The key to use.</param>
  /// <returns>
  ///   Whether or not that key can be used to generate a value. Or,
  ///   <c>null</c> if no key-checking function was provided.
  /// </returns>
  public override bool? CanGenerateFrom(K key) => KeyCheckFunc(key);
}

/// <summary>
///   A Generator that generates values which are the HashCodes of the
///   keys used to generate them.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
public class HashCodeGenerator<K> : Generator<K, int> where K : notnull
{
  /// <summary>
  ///   Returns the <see cref="object.GetHashCode()">hash code</see> of
  ///   the key.
  /// </summary>
  /// <param name="key">The key to use.</param>
  /// <returns>The hash code of the input object.</returns>
  public override int Generate(K key) => key.GetHashCode();
}

/// <summary>
///   A Generator that generates values which are the string
///   representations of the keys used to generate them.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
public class ToStringGenerator<K> : Generator<K, string> where K : notnull
{
  /// <summary>
  ///   Returns the <see cref="object.ToString()">string
  ///   representation</see> of the key.
  /// </summary>
  /// <param name="key">The key to use.</param>
  /// <returns>The string representation of the input object.</returns>
  public override string Generate(K key) => key.ToString()!;
}

/// <summary>
///   A Generator that generates default values for a type.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
/// <typeparam name="V">
///   The type of the values in this generator.
/// </typeparam>
public class DefaultGenerator<K, V> : Generator<K, V> where K : notnull
{
  /// <summary>
  ///   Returns the default value for the type <c>V</c>.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><c>default(V)</c>.</returns>
  public override V Generate(K key) => default(V)!;
}

/// <summary>
///   A Generator that uses the empty constructor to create values.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
/// <typeparam name="V">
///   The type of the values in this generator. Must have a
///   parameterless constructor overload.
/// </typeparam>
public class EmptyConstructorGenerator<K, V> : Generator<K, V> where K : notnull where V : new()
{
  /// <summary>
  ///   Returns the value type initialized with a default constructor.
  /// </summary>
  /// <param name="key">This parameter is ignored.</param>
  /// <returns><c>new V()</c>.</returns>
  public override V Generate(K key) => new V();
}

/// <summary>
///   A Generator that uses a single-parameter constructor to create
///   values.
/// </summary>
/// <typeparam name="K">
///   The type of the keys in this generator, not null.
/// </typeparam>
/// <typeparam name="V"> 
///   The type of the values in this generator. Must have a constructor
///   overload that has a single parameter of type <c>K</c>.
/// </typeparam>
public class ConstructorGenerator<K, V> : Generator<K, V> where K : notnull
{
  ConstructorInfo Constructor;

  /// <summary>
  ///   Constructs a new <see cref="ConstructorGenerator{K, V}"/>.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   The type <c>V</c> does not have a constructor <c>V(K)</v>.
  /// </exception>
  public ConstructorGenerator()
  {
    Type kType = typeof(K);
    Type vType = typeof(V);

    Constructor = vType.GetConstructor([kType])!;

    if (Constructor == null || Constructor.IsPrivate)
      throw new InvalidOperationException($"Cannot create a ConstructorGenerator<{kType.Name},"
      + $"{vType.Name}> because {vType.Name} does not have a non-private constructor {vType.Name}({kType.Name}).");
  }

  public override V Generate(K key) => (V)Constructor.Invoke([key]);
}
