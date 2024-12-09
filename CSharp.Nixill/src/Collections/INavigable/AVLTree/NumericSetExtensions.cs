using System.Numerics;

namespace Nixill.Collections;

public static class NumericNavigableExtensions
{
  public static TOut LowerDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.Lower(from);

  public static bool TryGetLowerDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetLower(from, out TIn found))
    {
      distance = from - found;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut FloorDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.Floor(from);

  public static bool TryGetFloorDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetFloor(from, out TIn found))
    {
      distance = from - found;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut CeilingDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - from;

  public static bool TryGetCeilingDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetCeiling(from, out TIn found))
    {
      distance = found - from;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut HigherDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - from;

  public static bool TryGetHigherDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetHigher(from, out TIn found))
    {
      distance = found - from;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut HigherLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - set.Lower(from);

  public static bool TryGetHigherLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigher(from, out TIn foundHigher);
    bool existsLower = set.TryGetLower(from, out TIn foundLower);

    if (existsHigher)
    {
      if (existsLower)
      {
        distance = foundHigher - foundLower;
        return true;
      }
      else
      {
        distance = foundHigher - from;
        return false;
      }
    }
    else
    {
      if (existsLower)
      {
        distance = from - foundLower;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut HigherFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - set.Floor(from);

  public static bool TryGetHigherFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigher(from, out TIn foundHigher);
    bool existsFloor = set.TryGetFloor(from, out TIn foundFloor);

    if (existsHigher)
    {
      if (existsFloor)
      {
        distance = foundHigher - foundFloor;
        return true;
      }
      else
      {
        distance = foundHigher - from;
        return false;
      }
    }
    else
    {
      if (existsFloor)
      {
        distance = from - foundFloor;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut CeilingLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - set.Lower(from);

  public static bool TryGetCeilingLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeiling(from, out TIn foundCeiling);
    bool existsLower = set.TryGetLower(from, out TIn foundLower);

    if (existsCeiling)
    {
      if (existsLower)
      {
        distance = foundCeiling - foundLower;
        return true;
      }
      else
      {
        distance = foundCeiling - from;
        return false;
      }
    }
    else
    {
      if (existsLower)
      {
        distance = from - foundLower;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut CeilingFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - set.Floor(from);

  public static bool TryGetCeilingFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeiling(from, out TIn foundCeiling);
    bool existsFloor = set.TryGetFloor(from, out TIn foundFloor);

    if (existsCeiling)
    {
      if (existsFloor)
      {
        distance = foundCeiling - foundFloor;
        return true;
      }
      else
      {
        distance = foundCeiling - from;
        return false;
      }
    }
    else
    {
      if (existsFloor)
      {
        distance = from - foundFloor;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut TotalSetSize<TIn, TOut>(this INavigableSet<TIn> set)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    => set.HighestValue() - set.LowestValue();

  public static bool TryGetTotalSetSize<TIn, TOut>(this INavigableSet<TIn> set, out TOut value)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.Count != 0)
    {
      value = set.HighestValue() - set.LowestValue();
      return true;
    }
    else
    {
      value = default(TOut)!;
      return false;
    }
  }

  public static TOut LowerDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.LowerKey(from);

  public static bool TryGetLowerDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetLowerKey(from, out TIn found))
    {
      distance = from - found;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut FloorDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.FloorKey(from);

  public static bool TryGetFloorDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetFloorKey(from, out TIn found))
    {
      distance = from - found;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut CeilingDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - from;

  public static bool TryGetCeilingDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetCeilingKey(from, out TIn found))
    {
      distance = found - from;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut HigherDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - from;

  public static bool TryGetHigherDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.TryGetHigherKey(from, out TIn found))
    {
      distance = found - from;
      return true;
    }
    distance = default(TOut)!;
    return false;
  }

  public static TOut HigherLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - set.LowerKey(from);

  public static bool TryGetHigherLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigherKey(from, out TIn foundHigher);
    bool existsLower = set.TryGetLowerKey(from, out TIn foundLower);

    if (existsHigher)
    {
      if (existsLower)
      {
        distance = foundHigher - foundLower;
        return true;
      }
      else
      {
        distance = foundHigher - from;
        return false;
      }
    }
    else
    {
      if (existsLower)
      {
        distance = from - foundLower;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut HigherFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - set.FloorKey(from);

  public static bool TryGetHigherFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigherKey(from, out TIn foundHigher);
    bool existsFloor = set.TryGetFloorKey(from, out TIn foundFloor);

    if (existsHigher)
    {
      if (existsFloor)
      {
        distance = foundHigher - foundFloor;
        return true;
      }
      else
      {
        distance = foundHigher - from;
        return false;
      }
    }
    else
    {
      if (existsFloor)
      {
        distance = from - foundFloor;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut CeilingLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - set.LowerKey(from);

  public static bool TryGetCeilingLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeilingKey(from, out TIn foundCeiling);
    bool existsLower = set.TryGetLowerKey(from, out TIn foundLower);

    if (existsCeiling)
    {
      if (existsLower)
      {
        distance = foundCeiling - foundLower;
        return true;
      }
      else
      {
        distance = foundCeiling - from;
        return false;
      }
    }
    else
    {
      if (existsLower)
      {
        distance = from - foundLower;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut CeilingFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - set.FloorKey(from);

  public static bool TryGetCeilingFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeilingKey(from, out TIn foundCeiling);
    bool existsFloor = set.TryGetFloorKey(from, out TIn foundFloor);

    if (existsCeiling)
    {
      if (existsFloor)
      {
        distance = foundCeiling - foundFloor;
        return true;
      }
      else
      {
        distance = foundCeiling - from;
        return false;
      }
    }
    else
    {
      if (existsFloor)
      {
        distance = from - foundFloor;
        return false;
      }
      else
      {
        distance = default(TOut)!;
        return false;
      }
    }
  }

  public static TOut TotalSetSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    => set.HighestKey() - set.LowestKey();

  public static bool TryGetTotalSetSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, out TOut value)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    if (set.Count != 0)
    {
      value = set.HighestKey() - set.LowestKey();
      return true;
    }
    else
    {
      value = default(TOut)!;
      return false;
    }
  }
}