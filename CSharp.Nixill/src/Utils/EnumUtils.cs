using System.Reflection;

namespace Nixill.Utils;

public static class EnumUtils
{
  /// <summary>
  ///   Returns the constants of the given enum type that have the given
  ///   attribute attached.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <returns>The matching enum constants.</returns>
  public static IEnumerable<TEnum> ValuesWith<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Where(f => f.GetCustomAttributes<TAttribute>().Any())
      .Select(f => (TEnum)f.GetRawConstantValue()!);

  /// <summary>
  ///   Returns the constants of the given enum type for which the given
  ///   attribute exists and meets a specified condition.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <param name="condition">Condition to filter constants on.</param>
  /// <returns>The matching enum constants.</returns>
  public static IEnumerable<TEnum> ValuesWith<TEnum, TAttribute>(Predicate<TAttribute> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttribute<TEnum, TAttribute>(condition)
      .Select(tuple => tuple.Value);

  /// <summary>
  ///   Returns the constants of the given enum type that have the given
  ///   attribute attached, along with the single attribute instance in
  ///   question.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <returns>
  ///   The matching enum constants and attached attributes.
  /// </returns>
  public static IEnumerable<(TEnum Value, TAttribute Attribute)> ValuesWithAttribute<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Select(f => (Value: (TEnum)f.GetRawConstantValue()!, Attribute: f.GetCustomAttribute<TAttribute>()))
      .Where(f => f.Attribute != null)!;

  /// <summary>
  ///   Returns the constants of the given enum type for which the given
  ///   attribute exists and meets a specified condition, along with the
  ///   single attribute instance in question.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <param name="condition">Condition to filter constants on.</param>
  /// <returns>
  ///   The matching enum constants and attached attributes.
  /// </returns>
  public static IEnumerable<(TEnum Value, TAttribute Attribute)>
    ValuesWithAttribute<TEnum, TAttribute>(Predicate<TAttribute> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttribute<TEnum, TAttribute>()
      .Where(t => condition(t.Attribute));

  /// <summary>
  ///   Returns the constants of the given enum type that have the given
  ///   attribute attached, along with the attribute instances in question.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <returns>
  ///   The matching enum constants and attached attributes.
  /// </returns>
  public static IEnumerable<(TEnum Value, IEnumerable<TAttribute> Attributes)> ValuesWithAttributes<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Select(f => (Value: (TEnum)f.GetRawConstantValue()!, Attributes: f.GetCustomAttributes<TAttribute>()))
      .Where(f => f.Attributes.Any());

  /// <summary>
  ///   Returns the constants of the given enum type for which the given
  ///   attribute exists and meets a specified condition, along with the
  ///   attribute instances in question.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <param name="condition">Condition to filter constants on.</param>
  /// <returns>
  ///   The matching enum constants and attached attributes.
  /// </returns>
  public static IEnumerable<(TEnum Value, IEnumerable<TAttribute> Attributes)>
    ValuesWithAttributes<TEnum, TAttribute>(Predicate<IEnumerable<TAttribute>> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttributes<TEnum, TAttribute>()
      .Where(t => condition(t.Attributes));

  /// <summary>
  ///   Returns the single attribute of the given type on the given enum
  ///   constant.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <param name="value">The enum constant.</param>
  /// <returns>The attached attribute instance.</returns>
  /// <exception cref="InvalidCastException">
  ///   <c>value</c> isn't a constant of <c>TEnum</c>.
  /// </exception>
  public static TAttribute AttributeOf<TEnum, TAttribute>(TEnum value)
    where TEnum : struct, Enum where TAttribute : Attribute
  => (typeof(TEnum)
    .GetField(value.ToString())
      ?? throw new InvalidCastException($"No enum constant exists for ({typeof(TEnum).Name}).{value}"))
    .GetCustomAttribute<TAttribute>()!;

  /// <summary>
  ///   Returns the attributes of the given type on the given enum
  ///   constant.
  /// </summary>
  /// <typeparam name="TEnum">The enum type.</typeparam>
  /// <typeparam name="TAttribute">The attribute type.</typeparam>
  /// <param name="value">The enum constant.</param>
  /// <returns>The attached attribute instance.</returns>
  /// <exception cref="InvalidCastException">
  ///   <c>value</c> isn't a constant of <c>TEnum</c>.
  /// </exception>
  public static IEnumerable<TAttribute> AttributesOf<TEnum, TAttribute>(TEnum value)
    where TEnum : struct, Enum where TAttribute : Attribute
  => (typeof(TEnum)
    .GetField(value.ToString())
      ?? throw new InvalidCastException($"No enum constant exists for ({typeof(TEnum).Name}).{value}"))
    .GetCustomAttributes<TAttribute>();
}