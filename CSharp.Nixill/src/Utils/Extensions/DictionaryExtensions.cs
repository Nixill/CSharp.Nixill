using System.Numerics;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Extensions for dictionaries that use numeric values.
/// </summary>
public static class DictionaryExtensions
{
  /// <summary>
  ///   Adds a value to either the value associated with a given key in
  ///   the dictionary or the additive identity of the value type, then
  ///   saves that sum to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TAdd">
  ///   The type of values being added to the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="add">The value to add.</param>
  /// <returns>The sum of the addition.</returns>
  public static TValue PlusOrSet<TKey, TValue, TAdd>(this IDictionary<TKey, TValue> dictionary, TKey key, TAdd add)
    where TValue : IAdditionOperators<TValue, TAdd, TValue>, IAdditiveIdentity<TValue, TValue>
  => PlusOrSet(dictionary, key, add, () => TValue.AdditiveIdentity);

  /// <summary>
  ///   Adds a value to either the value associated with a given key in
  ///   the dictionary or a given default value, then saves that sum to
  ///   the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TAdd">
  ///   The type of values being added to the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="add">The value to add.</param>
  /// <param name="defaultValue">The default value.</param>
  /// <returns>The sum of the addition.</returns>
  public static TValue PlusOrSet<TKey, TValue, TAdd>(this IDictionary<TKey, TValue> dictionary, TKey key, TAdd add,
    TValue defaultValue) where TValue : IAdditionOperators<TValue, TAdd, TValue>
  => PlusOrSet(dictionary, key, add, () => defaultValue);

  /// <summary>
  ///   Adds a value to either the value associated with a given key in
  ///   the dictionary or a given default value, then saves that sum to
  ///   the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TAdd">
  ///   The type of values being added to the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="add">The value to add.</param>
  /// <param name="defaultValue">
  ///   A function to generate the default value; only called if the key
  ///   is not found.
  /// </param>
  /// <returns>The sum of the addition.</returns>
  public static TValue PlusOrSet<TKey, TValue, TAdd>(this IDictionary<TKey, TValue> dictionary, TKey key, TAdd add,
    Func<TValue> defaultValue) where TValue : IAdditionOperators<TValue, TAdd, TValue>
  => dictionary[key] = (dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue()) + add;

  /// <summary>
  ///   Subtracts a value from either the value associated with a given
  ///   key in the dictionary or the additive identity of the value type,
  ///   then saves that difference to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TSubtract">
  ///   The type of values being subtracted from the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="subtract">The value to subtract.</param>
  /// <returns>The difference of the subtraction.</returns>
  public static TValue MinusOrSet<TKey, TValue, TSubtract>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TSubtract subtract) where TValue : ISubtractionOperators<TValue, TSubtract, TValue>, IAdditiveIdentity<TValue, TValue>
  => MinusOrSet(dictionary, key, subtract, () => TValue.AdditiveIdentity);

  /// <summary>
  ///   Subtracts a value from either the value associated with a given
  ///   key in the dictionary or a given default value, then saves that
  ///   difference to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TSubtract">
  ///   The type of values being subtracted from the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="subtract">The value to subtract.</param>
  /// <param name="defaultValue">The default value.</param>
  /// <returns>The difference of the subtraction.</returns>
  public static TValue MinusOrSet<TKey, TValue, TSubtract>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TSubtract subtract, TValue defaultValue) where TValue : ISubtractionOperators<TValue, TSubtract, TValue>
  => MinusOrSet(dictionary, key, subtract, () => defaultValue);

  /// <summary>
  ///   Subtracts a value from either the value associated with a given
  ///   key in the dictionary or a given default value, then saves that
  ///   difference to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TSubtract">
  ///   The type of values being subtracted from the values in the dictionary.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="subtract">The value to subtract.</param>
  /// <param name="defaultValue">
  ///   A function to generate the default value; only called if the key
  ///   is not found.
  /// </param>
  /// <returns>The difference of the subtraction.</returns>
  public static TValue MinusOrSet<TKey, TValue, TSubtract>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TSubtract subtract, Func<TValue> defaultValue) where TValue : ISubtractionOperators<TValue, TSubtract, TValue>
  => dictionary[key] = (dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue()) - subtract;

  /// <summary>
  ///   Multiplies a value into either the value associated with a given
  ///   key in the dictionary or the multiplicative identity of the value
  ///   type, then saves that product to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TMultiply">
  ///   The type of values by which the values in the dictionary are being
  ///   multiplied.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="multiplyBy">The value to multiply.</param>
  /// <returns>The product of the multiplication.</returns>
  public static TValue TimesOrSet<TKey, TValue, TMultiply>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TMultiply multiplyBy)
    where TValue : IMultiplyOperators<TValue, TMultiply, TValue>, IMultiplicativeIdentity<TValue, TValue>
  => TimesOrSet(dictionary, key, multiplyBy, () => TValue.MultiplicativeIdentity);

  /// <summary>
  ///   Multiplies a value into either the value associated with a given
  ///   key in the dictionary or a given default value, then saves that
  ///   product to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TMultiply">
  ///   The type of values by which the values in the dictionary are being
  ///   multiplied.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="multiplyBy">The value by which to multiply.</param>
  /// <param name="defaultValue">The default value.</param>
  /// <returns>The product of the multiplication.</returns>
  public static TValue TimesOrSet<TKey, TValue, TMultiply>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TMultiply multiplyBy, TValue defaultValue) where TValue : IMultiplyOperators<TValue, TMultiply, TValue>
  => TimesOrSet(dictionary, key, multiplyBy, () => defaultValue);

  /// <summary>
  ///   Multiplies a value into either the value associated with a given
  ///   key in the dictionary or a given default value, then saves that
  ///   product to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TMultiply">
  ///   The type of values by which the values in the dictionary are being
  ///   multiplied.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="multiplyBy">The value by which to multiply.</param>
  /// <param name="defaultValue">
  ///   A function to generate the default value; only called if the key
  ///   is not found.
  /// </param>
  /// <returns>The product of the multiplication.</returns>
  public static TValue TimesOrSet<TKey, TValue, TMultiply>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TMultiply multiplyBy, Func<TValue> defaultValue) where TValue : IMultiplyOperators<TValue, TMultiply, TValue>
  => dictionary[key] = (dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue()) * multiplyBy;

  /// <summary>
  ///   Divides a value into either the value associated with a given key
  ///   in the dictionary or the multiplicative identity of the value
  ///   type, then saves that quotient to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TDivide">
  ///   The type of values by which the values in the dictionary are being
  ///   divided.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="divideBy">The value by which to divide.</param>
  /// <returns>The quotient of the division.</returns>
  public static TValue DivideOrSet<TKey, TValue, TDivide>(this IDictionary<TKey, TValue> dictionary, TKey key, TDivide divideBy)
    where TValue : IDivisionOperators<TValue, TDivide, TValue>, IMultiplicativeIdentity<TValue, TValue>
  => DivideOrSet(dictionary, key, divideBy, () => TValue.MultiplicativeIdentity);

  /// <summary>
  ///   Divides a value into either the value associated with a given key
  ///   in the dictionary or a given default value, then saves that
  ///   quotient to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TDivide">
  ///   The type of values by which the values in the dictionary are being
  ///   divided.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="divideBy">The value by which to divide.</param>
  /// <param name="defaultValue">The default value.</param>
  /// <returns>The quotient of the division.</returns>
  public static TValue DivideOrSet<TKey, TValue, TDivide>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TDivide divideBy, TValue defaultValue) where TValue : IDivisionOperators<TValue, TDivide, TValue>
  => DivideOrSet(dictionary, key, divideBy, () => defaultValue);

  /// <summary>
  ///   Divides a value into either the value associated with a given key
  ///   in the dictionary or a given default value, then saves that
  ///   quotient to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TDivide">
  ///   The type of values by which the values in the dictionary are being
  ///   divided.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="divideBy">The value by which to divide.</param>
  /// <param name="defaultValue">
  ///   A function to generate the default value; only called if the key
  ///   is not found.
  /// </param>
  /// <returns>The quotient of the division.</returns>
  public static TValue DivideOrSet<TKey, TValue, TDivide>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TDivide divideBy, Func<TValue> defaultValue) where TValue : IDivisionOperators<TValue, TDivide, TValue>
  => dictionary[key] = (dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue()) / divideBy;

  /// <summary>
  ///   Divides a value into either the value associated with a given key
  ///   in the dictionary or a given default value, then saves the
  ///   remainder of that division to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TDivide">
  ///   The type of values by which the values in the dictionary are being
  ///   divided.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="divideBy">The value by which to divide.</param>
  /// <param name="defaultValue">The default value.</param>
  /// <returns>The remainder of the division.</returns>
  public static TValue ModuloOrSet<TKey, TValue, TDivide>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TDivide divideBy, TValue defaultValue) where TValue : IModulusOperators<TValue, TDivide, TValue>
  => ModuloOrSet(dictionary, key, divideBy, () => defaultValue);

  /// <summary>
  ///   Divides a value into either the value associated with a given key
  ///   in the dictionary or a given default value, then saves the
  ///   remainder of that division to the dictionary and returns it.
  /// </summary>
  /// <typeparam name="TKey">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TDivide">
  ///   The type of values by which the values in the dictionary are being
  ///   divided.
  /// </typeparam>
  /// <param name="dictionary">The dictionary.</param>
  /// <param name="key">The key.</param>
  /// <param name="divideBy">The value by which to divide.</param>
  /// <param name="defaultValue">
  ///   A function to generate the default value; only called if the key
  ///   is not found.
  /// </param>
  /// <returns>The remainder of the division.</returns>
  public static TValue ModuloOrSet<TKey, TValue, TDivide>(this IDictionary<TKey, TValue> dictionary, TKey key,
    TDivide divideBy, Func<TValue> defaultValue) where TValue : IModulusOperators<TValue, TDivide, TValue>
  => dictionary[key] = (dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue()) % divideBy;
}