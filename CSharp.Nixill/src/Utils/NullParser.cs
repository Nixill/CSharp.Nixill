namespace Nixill.Utils;

public static class NumParser
{
  public static bool? Bool(string input) => bool.TryParse(input, out bool result) ? result : null;
  public static byte? Byte(string input) => byte.TryParse(input, out byte result) ? result : null;
  public static sbyte? SByte(string input) => sbyte.TryParse(input, out sbyte result) ? result : null;
  public static char? Char(string input) => char.TryParse(input, out char result) ? result : null;
  public static decimal? Decimal(string input) => decimal.TryParse(input, out decimal result) ? result : null;
  public static double? Double(string input) => double.TryParse(input, out double result) ? result : null;
  public static float? Float(string input) => float.TryParse(input, out float result) ? result : null;
  public static int? Int(string input) => int.TryParse(input, out int result) ? result : null;
  public static uint? UInt(string input) => uint.TryParse(input, out uint result) ? result : null;
  public static long? Long(string input) => long.TryParse(input, out long result) ? result : null;
  public static ulong? ULong(string input) => ulong.TryParse(input, out ulong result) ? result : null;
  public static short? Short(string input) => short.TryParse(input, out short result) ? result : null;
  public static ushort? UShort(string input) => ushort.TryParse(input, out ushort result) ? result : null;

  public static bool Bool(string input, bool defaultValue) => bool.TryParse(input, out bool result) ? result : defaultValue;
  public static byte Byte(string input, byte defaultValue) => byte.TryParse(input, out byte result) ? result : defaultValue;
  public static sbyte SByte(string input, sbyte defaultValue) => sbyte.TryParse(input, out sbyte result) ? result : defaultValue;
  public static char Char(string input, char defaultValue) => char.TryParse(input, out char result) ? result : defaultValue;
  public static decimal Decimal(string input, decimal defaultValue) => decimal.TryParse(input, out decimal result) ? result : defaultValue;
  public static double Double(string input, double defaultValue) => double.TryParse(input, out double result) ? result : defaultValue;
  public static float Float(string input, float defaultValue) => float.TryParse(input, out float result) ? result : defaultValue;
  public static int Int(string input, int defaultValue) => int.TryParse(input, out int result) ? result : defaultValue;
  public static uint UInt(string input, uint defaultValue) => uint.TryParse(input, out uint result) ? result : defaultValue;
  public static long Long(string input, long defaultValue) => long.TryParse(input, out long result) ? result : defaultValue;
  public static ulong ULong(string input, ulong defaultValue) => ulong.TryParse(input, out ulong result) ? result : defaultValue;
  public static short Short(string input, short defaultValue) => short.TryParse(input, out short result) ? result : defaultValue;
  public static ushort UShort(string input, ushort defaultValue) => ushort.TryParse(input, out ushort result) ? result : defaultValue;
}