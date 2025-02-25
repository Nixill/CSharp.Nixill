using System.Numerics;

namespace Nixill.Collections;

/// <summary>
///   Extensions to get the gaps between entries in a navigable set or
///   dictionary with subtractible keys.
/// </summary>
public static class NumericNavigableExtensions
{
  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is less than or equal to the lowest value in the
  ///   set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut LowerDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.Lower(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a lesser value exists.</returns>
  public static bool TryGetLowerDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is less than the lowest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut FloorDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.Floor(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a lesser or equal value exists.</returns>
  public static bool TryGetFloorDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut CeilingDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - from;

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a greater or equal value exists.</returns>
  public static bool TryGetCeilingDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest value in
  ///   the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut HigherDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - from;

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a greater value exists.</returns>
  public static bool TryGetHigherDistance<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest value in
  ///   the set, or less than or equal to the lowest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut HigherLowerSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - set.Lower(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a value greater than the given value and a
  ///   value less than the given value exist in the set.
  /// </returns>
  public static bool TryGetHigherLowerSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigher(from, out TIn foundHigher);
    bool existsLower = set.TryGetLower(from, out TIn foundLower);

    if (existsHigher && existsLower)
    {
      distance = foundHigher - foundLower;
      return true;
    }
    else
    {
      distance = default(TOut)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest value in
  ///   the set, or less than the lowest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut HigherFloorSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Higher(from) - set.Floor(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a value greater than the given value and a
  ///   value less than or equal to than the given value exist in the set.
  /// </returns>
  public static bool TryGetHigherFloorSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigher(from, out TIn foundHigher);
    bool existsFloor = set.TryGetFloor(from, out TIn foundFloor);

    if (existsHigher && existsFloor)
    {
      distance = foundHigher - foundFloor;
      return true;
    }
    else
    {
      distance = default(TOut)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest value in the set, or
  ///   less than or equal to the lowest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut CeilingLowerSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - set.Lower(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a value greater than or equal to the given
  ///   value and a value less than the given value exist in the set.
  /// </returns>
  public static bool TryGetCeilingLowerSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeiling(from, out TIn foundCeiling);
    bool existsLower = set.TryGetLower(from, out TIn foundLower);

    if (existsCeiling && existsLower)
    {
      distance = foundCeiling - foundLower;
      return true;
    }
    else
    {
      distance = default(TOut)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest value in the set, or
  ///   less than the lowest value in the set.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The set contains no elements.
  /// </exception>
  public static TOut CeilingFloorSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.Ceiling(from) - set.Floor(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest value in the set greater than or equal to the
  ///       given value.
  ///     </item>
  ///     <item>
  ///       The largest value in the set less than or equal to the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a value greater than or equal to the given
  ///   value and a value less than or equal to than the given value exist
  ///   in the set.
  /// </returns>
  public static bool TryGetCeilingFloorSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeiling(from, out TIn foundCeiling);
    bool existsFloor = set.TryGetFloor(from, out TIn foundFloor);

    if (existsCeiling && existsFloor)
    {
      distance = foundCeiling - foundFloor;
      return true;
    }
    else
    {
      distance = default(TOut)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>The largest value in the set.</item>
  ///     <item>The smallest value in the set.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <returns>The difference.</returns>
  public static TOut TotalSetSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    => set.HighestValue() - set.LowestValue();

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The largest value in the set.</item>
  ///     <item>The smallest value in the set.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">The type of values in the set.</typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The set being queried.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff the set contains at least one value.
  /// </returns>
  public static bool TryGetTotalSetSize<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, out TOut value)
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

  public static bool TryGetNearestValue<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TIn value,
    bool lowerOrEqual = false, bool higherOrEqual = false)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    where TOut : IComparable<TOut>
  => set.TryGetNearestValue(from, (m, s) => m - s, (l, r) => l.CompareTo(r), out value, lowerOrEqual, higherOrEqual);

  public static bool TryGetNearestValue<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from,
    Func<TIn, TIn, TOut> subtract, Comparison<TOut> compare, out TIn value, bool lowerOrEqual = false,
    bool higherOrEqual = false)
  {
    TIn lower;
    TIn higher;

    bool hasLower = lowerOrEqual
      ? set.TryGetFloor(from, out lower)
      : set.TryGetLower(from, out lower);
    bool hasHigher = higherOrEqual
      ? set.TryGetCeiling(from, out higher)
      : set.TryGetHigher(from, out higher);

    value = default(TIn)!;

    if (hasLower && hasHigher)
    {
      if (compare(subtract(from, lower), subtract(higher, from)) <= 0) value = lower;
      else value = higher;
    }
    else if (hasLower) value = lower;
    else if (hasHigher) value = higher;
    else return false;

    return true;
  }

  public static TIn NearestValue<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, out TIn value,
    bool lowerOrEqual = false, bool higherOrEqual = false)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    where TOut : IComparable<TOut>
  => set.NearestValue(from, (m, s) => m - s, (l, r) => l.CompareTo(r), out value, lowerOrEqual, higherOrEqual);

  public static TIn NearestValue<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from,
    Func<TIn, TIn, TOut> subtract, Comparison<TOut> compare, out TIn value, bool lowerOrEqual = false,
    bool higherOrEqual = false)
  => set.TryGetNearestValue(from, subtract, compare, out value, lowerOrEqual, higherOrEqual)
    ? value
    : throw new InvalidOperationException("Cannot find higher or lower value.");

  public static bool TryGetNearestValueInRange<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from, TIn lowerBound,
    TIn higherBound, out TIn value, bool lowerOrEqual = false, bool higherOrEqual = false)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>, IComparable<TIn>
    where TOut : IComparable<TOut>
  => set.TryGetNearestValueInRange(from, (l, r) => l.CompareTo(r), (m, s) => m - s, (l, r) => l.CompareTo(r),
    lowerBound, higherBound, out value, lowerOrEqual, higherOrEqual);

  public static bool TryGetNearestValueInRange<TIn, TOut>(this IReadOnlyNavigableSet<TIn> set, TIn from,
    Comparison<TIn> compareIn, Func<TIn, TIn, TOut> subtract, Comparison<TOut> compareOut, TIn lowerBound,
    TIn higherBound, out TIn value, bool lowerOrEqual = false, bool higherOrEqual = false)
  {
    if (compareIn(higherBound, lowerBound) < 0) throw new ArgumentOutOfRangeException("higherBound < lowerBound");

    bool hasLower;
    bool hasHigher;
    TIn lower;
    TIn higher;

    if (compareIn(from, lowerBound) < 0)
    {
      hasLower = false;
      lower = default(TIn)!;
      hasHigher = (higherOrEqual || compareIn(from, higherBound) < 0)
        ? set.TryGetCeiling(from, out higher)
        : set.TryGetHigher(from, out higher);
    }
    else if (compareIn(from, higherBound) > 0)
    {
      hasHigher = false;
      higher = default(TIn)!;
      hasLower = (lowerOrEqual || compareIn(from, lowerBound) > 0)
        ? set.TryGetFloor(from, out lower)
        : set.TryGetLower(from, out lower);
    }
    else
    {
      hasLower = lowerOrEqual
        ? set.TryGetFloor(from, out lower)
        : set.TryGetLower(from, out lower);
      hasHigher = higherOrEqual
        ? set.TryGetCeiling(from, out higher)
        : set.TryGetHigher(from, out higher);
    }

    if (compareIn(lower, lowerBound) < 0) hasLower = false;
    if (compareIn(higher, higherBound) > 0) hasHigher = false;

    value = default(TIn)!;

    if (hasLower && hasHigher)
    {
      if (compareOut(subtract(from, lower), subtract(higher, from)) <= 0) value = lower;
      else value = higher;
    }
    else if (hasLower) value = lower;
    else if (hasHigher) value = higher;
    else return false;

    return true;
  }
}