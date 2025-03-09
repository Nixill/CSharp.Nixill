namespace Nixill.Utils.Extensions;

public static class ValueExtensions
{
  public static T AssignTo<T>(this T value, out T variable)
  {
    variable = value;
    return value;
  }

  public static T ValueIfDefault<T>(this T value, T defaultValue)
    => object.ReferenceEquals(value, default(T)) ? defaultValue : value;

  public static T ValueIfDefault<T>(this T value, Func<T> defaultValue)
    => object.ReferenceEquals(value, default(T)) ? defaultValue() : value;

  public static string ValueIfEmptyString(this string? value, string defaultValue)
    => (value == null || value == "") ? defaultValue : value;

  public static string ValueIfEmptyString(this string? value, Func<string> defaultValue)
    => (value == null || value == "") ? defaultValue() : value;
}