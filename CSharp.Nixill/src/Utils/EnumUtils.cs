using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nixill.Utils;

public static class EnumUtils
{
  public static IEnumerable<TEnum> ValuesWith<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Where(f => f.GetCustomAttributes<TAttribute>().Any())
      .Select(f => (TEnum)f.GetRawConstantValue());

  public static IEnumerable<TEnum> ValuesWith<TEnum, TAttribute>(Predicate<TAttribute> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttribute<TEnum, TAttribute>(condition)
      .Select(tuple => tuple.Value);

  public static IEnumerable<(TEnum Value, TAttribute Attribute)> ValuesWithAttribute<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Select(f => (Value: (TEnum)f.GetRawConstantValue(), Attribute: f.GetCustomAttribute<TAttribute>()))
      .Where(f => f.Attribute != null);

  public static IEnumerable<(TEnum Value, TAttribute Attribute)>
    ValuesWithAttribute<TEnum, TAttribute>(Predicate<TAttribute> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttribute<TEnum, TAttribute>()
      .Where(t => condition(t.Attribute));

  public static IEnumerable<(TEnum value, IEnumerable<TAttribute> Attributes)> ValuesWithAttributes<TEnum, TAttribute>()
    where TEnum : struct, Enum where TAttribute : Attribute
    => typeof(TEnum).GetFields()
      .Select(f => (Value: (TEnum)f.GetRawConstantValue(), Attributes: f.GetCustomAttributes<TAttribute>()))
      .Where(f => f.Attributes.Any());

  public static IEnumerable<(TEnum value, IEnumerable<TAttribute> Attributes)>
    ValuesWithAttributes<TEnum, TAttribute>(Predicate<IEnumerable<TAttribute>> condition)
    where TEnum : struct, Enum where TAttribute : Attribute
    => ValuesWithAttributes<TEnum, TAttribute>()
      .Where(t => condition(t.Attributes));

  public static TAttribute AttributeOf<TEnum, TAttribute>(TEnum value)
    where TEnum : struct, Enum where TAttribute : Attribute
  => (typeof(TEnum)
    .GetField(value.ToString())
      ?? throw new InvalidCastException($"No enum constant exists for ({typeof(TEnum).Name}).{value}"))
    .GetCustomAttribute<TAttribute>();


  public static IEnumerable<TAttribute> AttributesOf<TEnum, TAttribute>(TEnum value)
    where TEnum : struct, Enum where TAttribute : Attribute
  => (typeof(TEnum)
    .GetField(value.ToString())
      ?? throw new InvalidCastException($"No enum constant exists for ({typeof(TEnum).Name}).{value}"))
    .GetCustomAttributes<TAttribute>();
}