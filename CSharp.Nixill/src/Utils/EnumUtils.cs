using System;
using System.Collections.Generic;

namespace Nixill.Utils;

public static class EnumUtils
{
  public static IEnumerable<TEnum> ValuesWith<TEnum, TAttribute>()
    where TEnum : System.Enum where TAttribute : Attribute
  {
    Type enumType = typeof(TEnum);

    yield break;
  }
}