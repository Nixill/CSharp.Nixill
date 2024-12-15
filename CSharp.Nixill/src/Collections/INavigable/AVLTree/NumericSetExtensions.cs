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
  public static TOut LowerDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static TOut FloorDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static TOut CeilingDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static TOut HigherDistance<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static TOut HigherLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static bool TryGetHigherLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
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
  public static TOut HigherFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static bool TryGetHigherFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
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
  public static TOut CeilingLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static bool TryGetCeilingLowerSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
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
  public static TOut CeilingFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from)
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
  public static bool TryGetCeilingFloorSize<TIn, TOut>(this INavigableSet<TIn> set, TIn from, out TOut distance)
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
  public static TOut TotalSetSize<TIn, TOut>(this INavigableSet<TIn> set)
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is less than or equal to the lowest key in the
  ///   dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut LowerDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.LowerKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a lesser key exists.</returns>
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is less than the lowest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut FloorDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => from - set.FloorKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The given value itself.</item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a lesser or equal key exists.</returns>
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut CeilingDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - from;

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a greater or equal key exists.</returns>
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest key in the
  ///   dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut HigherDistance<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - from;

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>The given value itself.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>Whether or not a greater key exists.</returns>
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

  /// <summary>
  ///   Returns the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest key in the
  ///   dictionary, or less than or equal to the lowest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut HigherLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - set.LowerKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a key greater than the given value and a key
  ///   less than the given value exist in the dictionary.
  /// </returns>
  public static bool TryGetHigherLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigherKey(from, out TIn foundHigher);
    bool existsLower = set.TryGetLowerKey(from, out TIn foundLower);

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
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than or equal to the highest key in the
  ///   dictionary, or less than the lowest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut HigherFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.HigherKey(from) - set.FloorKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a key greater than the given value and a key
  ///   less than or equal to than the given value exist in the dictionary.
  /// </returns>
  public static bool TryGetHigherFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsHigher = set.TryGetHigherKey(from, out TIn foundHigher);
    bool existsFloor = set.TryGetFloorKey(from, out TIn foundFloor);

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
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest key in the dictionary,
  ///   or less than or equal to the lowest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut CeilingLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - set.LowerKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than the given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <param name="distance">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff both a key greater than or equal to the given
  ///   value and a key less than the given value exist in the dictionary.
  /// </returns>
  public static bool TryGetCeilingLowerSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeilingKey(from, out TIn foundCeiling);
    bool existsLower = set.TryGetLowerKey(from, out TIn foundLower);

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
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="from">The value to search near.</param>
  /// <returns>The difference.</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The given value is greater than the highest key in the dictionary,
  ///   or less than the lowest key in the dictionary.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The dictionary contains no entries.
  /// </exception>
  public static TOut CeilingFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  => set.CeilingKey(from) - set.FloorKey(from);

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>
  ///       The smallest key in the dictionary greater than or equal to
  ///       the given value.
  ///     </item>
  ///     <item>
  ///       The largest key in the dictionary less than or equal to the
  ///       given value.
  ///     </item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
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
  public static bool TryGetCeilingFloorSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set, TIn from, out TOut distance)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
  {
    bool existsCeiling = set.TryGetCeilingKey(from, out TIn foundCeiling);
    bool existsFloor = set.TryGetFloorKey(from, out TIn foundFloor);

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
  ///     <item>The largest key in the dictionary.</item>
  ///     <item>The smallest key in the dictionary.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <returns>The difference.</returns>
  public static TOut TotalSetSize<TIn, TValue, TOut>(this INavigableDictionary<TIn, TValue> set)
    where TIn : ISubtractionOperators<TIn, TIn, TOut>
    => set.HighestKey() - set.LowestKey();

  /// <summary>
  ///   Attempts to get the difference between:
  ///   <list type="bullet">
  ///     <item>The largest key in the dictionary.</item>
  ///     <item>The smallest key in the dictionary.</item>
  ///   </list>
  /// </summary>
  /// <typeparam name="TIn">
  ///   The type of keys in the dictionary.
  /// </typeparam>
  /// <typeparam name="TValue">
  ///   The type of values in the dictionary.
  /// </typeparam>
  /// <typeparam name="TOut">
  ///   The type of value given when one <c>TIn</c> is subtracted from
  ///   another.
  /// </typeparam>
  /// <param name="set">The dictionary being queried.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this is the given distance.
  ///   Otherwise, this is <c>default(TIn)</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> iff the dictionary contains at least one entry.
  /// </returns>
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