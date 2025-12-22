using Nixill.Utils.Extensions;
using Nixill.Objects;

namespace Nixill.Collections;

/// <summary>
///   Extension methods for grids.
/// </summary>
public static class GridExtensions
{
  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   that are both orthogonally adjacent to the original reference and
  ///   located within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<IntVector2> OrthogonallyAdjacentRefs<T>(this IGrid<T> grid, IntVector2 from)
    => grid.NearbyRefs(from, [(0, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   that are both diagonally (<em>not</em> orthogonally) adjacent to
  ///   the original reference and located within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<IntVector2> DiagonallyAdjacentRefs<T>(this IGrid<T> grid, IntVector2 from)
    => grid.NearbyRefs(from, [(1, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   that are both orthogonally or diagonally adjacent to the original
  ///   reference and located within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<IntVector2> EightAdjacentRefs<T>(this IGrid<T> grid, IntVector2 from)
    => grid.NearbyRefs(from, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   that are both vaguely near the original reference and located
  ///   within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <param name="offsets">
  ///   A sequence of offsets to check.
  /// </param>
  /// <param name="transforms">
  ///   A sequence of transformation functions to apply to each offset,
  ///   such that every combination of offset and transform is returned.
  ///   If this parameter is not specified, no transformation functions
  ///   except the identity are applied, but if it is, the identity must
  ///   be re-specified if desired.
  /// </param>
  /// <param name="distinct">
  ///   If true or unspecified, each distinct reference will be returned
  ///   only once, regardless of how many combinations of offset and
  ///   transform reach it.
  /// </param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<IntVector2> NearbyRefs<T>(this IGrid<T> grid, IntVector2 from,
    IEnumerable<IntVector2> offsets, IEnumerable<Func<IntVector2, IntVector2>> transforms = null!, bool distinct = true)
  {
    transforms ??= GridTransforms.Identity;

    foreach (IntVector2 offset in offsets.WithTranslations(transforms, distinct))
    {
      IntVector2 result = from + offset;

      if (grid.IsWithinGrid(result)) yield return result;
    }
  }

  static IEnumerable<IntVector2> WithTranslations
    (this IEnumerable<IntVector2> originals, IEnumerable<Func<IntVector2, IntVector2>> transforms, bool distinct)
  {
    if (distinct) return originals.WithTranslations2(transforms).Distinct();
    else return originals.WithTranslations2(transforms);
  }

  static IEnumerable<IntVector2> WithTranslations2
    (this IEnumerable<IntVector2> originals, IEnumerable<Func<IntVector2, IntVector2>> transforms)
  {
    foreach (Func<IntVector2, IntVector2> transform in transforms)
    {
      foreach (IntVector2 offset in originals)
      {
        yield return transform(offset);
      }
    }
  }

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are both orthogonally adjacent to
  ///   the original reference and located within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(IntVector2 Reference, T Item)> OrthogonallyAdjacentCells<T>(this IGrid<T> grid,
    IntVector2 from)
    => grid.NearbyCells(from, [(0, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are both diagonally (<em>not</em>
  ///   orthogonally) adjacent to the original reference and located
  ///   within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(IntVector2 Reference, T Item)> DiagonallyAdjacentCells<T>(this IGrid<T> grid,
    IntVector2 from)
    => grid.NearbyCells(from, [(1, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are both orthogonally or diagonally
  ///   adjacent to the original reference and located within the bounds
  ///   of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(IntVector2 Reference, T Item)> EightAdjacentCells<T>(this IGrid<T> grid, IntVector2 from)
    => grid.NearbyCells(from, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are both vaguely near the original
  ///   reference and located within the bounds of the grid.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to check.</param>
  /// <param name="from">The original reference.</param>
  /// <param name="offsets">
  ///   A sequence of offsets to check.
  /// </param>
  /// <param name="transforms">
  ///   A sequence of transformation functions to apply to each offset,
  ///   such that every combination of offset and transform is returned.
  ///   If this parameter is not specified, no transformation functions
  ///   except the identity are applied, but if it is, the identity must
  ///   be re-specified if desired.
  /// </param>
  /// <param name="distinct">
  ///   If true or unspecified, each distinct reference will be returned
  ///   only once, regardless of how many combinations of offset and
  ///   transform reach it.
  /// </param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(IntVector2 Reference, T Item)> NearbyCells<T>(this IGrid<T> grid, IntVector2 from,
    IEnumerable<IntVector2> offsets, IEnumerable<Func<IntVector2, IntVector2>> transforms = null!, bool distinct = true)
    => grid.NearbyRefs(from, offsets, transforms, distinct).Select(r => (r, grid[r]));

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are recursively orthogonally
  ///   adjacent to the original cell, within the grid, and meet a given
  ///   condition (such as being equal to the original cell).
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to flood select.</param>
  /// <param name="from">The original reference.</param>
  /// <param name="condition">The condition that should be met.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(T Item, IntVector2 Reference)> OrthogonalFloodSelect<T>(this IGrid<T> grid,
    IntVector2 from, Func<T, IntVector2, bool> condition)
    => grid.FloodSelect(from, condition, [(0, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are recursively orthogonally or
  ///   diagonally adjacent to the original cell, within the grid, and
  ///   meet a given condition (such as being equal to the original cell).
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to flood select.</param>
  /// <param name="from">The original reference.</param>
  /// <param name="condition">The condition that should be met.</param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(T item, IntVector2 Reference)> EightWayFloodSelect<T>(this IGrid<T> grid,
    IntVector2 from, Func<T, IntVector2, bool> condition)
    => grid.FloodSelect(from, condition, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  /// <summary>
  ///   Returns a sequence of <see cref="IntVector2"/> cell references
  ///   (along with their values) that are recursively vaguely near the
  ///   original cell, within the grid, and meet a given condition (such
  ///   as being equal to the original cell).
  /// </summary>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to flood select.</param>
  /// <param name="from">The original reference.</param>
  /// <param name="condition">The condition that should be met.</param>
  /// <param name="offsets">
  ///   A sequence of offsets to check.
  /// </param>
  /// <param name="transforms">
  ///   A sequence of transformation functions to apply to each offset,
  ///   such that every combination of offset and transform is returned.
  ///   If this parameter is not specified, no transformation functions
  ///   except the identity are applied, but if it is, the identity must
  ///   be re-specified if desired.
  /// </param>
  /// <returns>The sequence.</returns>
  public static IEnumerable<(T item, IntVector2 Reference)> FloodSelect<T>(this IGrid<T> grid, IntVector2 from,
    Func<T, IntVector2, bool> condition, IEnumerable<IntVector2> offsets,
    IEnumerable<Func<IntVector2, IntVector2>> transforms = null!)
  {
    List<IntVector2> refQueue = [from];
    HashSet<IntVector2> queued = [from];

    while (refQueue.Count >= 1)
    {
      IntVector2 rfc = refQueue.Pop();
      T item = grid[rfc]!;

      if (condition(item, rfc))
      {
        yield return (item, rfc);
        foreach (IntVector2 rfc2 in grid.NearbyRefs(rfc, offsets, transforms))
        {
          if (!queued.Contains(rfc2))
          {
            refQueue.Add(rfc2);
            queued.Add(rfc2);
          }
        }
      }
    }
  }

  /// <summary>
  ///   Returns a transposed view of the specified grid.
  /// </summary>
  /// <remarks>
  ///   If this is called on an already-transposed grid, the original grid
  ///   is returned.
  ///   <para/>
  ///   The view remains linked to the original grid, with all
  ///   modifications to one reflected in the other, just with the axes
  ///   swapped.
  /// </remarks>
  /// <typeparam name="T">
  ///   The type of objects the grid contains.
  /// </typeparam>
  /// <param name="grid">The grid to transpose.</param>
  /// <returns>The transposed grid.</returns>
  public static IGrid<T> GetTransposedGrid<T>(this IGrid<T> grid)
    => (grid is TransposedGrid<T> tGrid) ? tGrid.BackingGrid : new TransposedGrid<T>(grid);
}

/// <summary>
///   A selection of
///   <see cref="IEnumerable{T}">IEnumerable</see>&lt;<see cref="Func{T1, T2}">Func</see>&lt;<see cref="IntVector2"/>,
///   <see cref="IntVector2"/>&gt;&gt;s for the <c>transforms</c>
///   parameter of
///   <see cref="GridExtensions.NearbyCells{T}(IGrid{T}, IntVector2, IEnumerable{IntVector2}, IEnumerable{Func{IntVector2, IntVector2}}, bool)"/>,
///   <see cref="GridExtensions.NearbyRefs{T}(IGrid{T}, IntVector2, IEnumerable{IntVector2}, IEnumerable{Func{IntVector2, IntVector2}}, bool)"/>,
///   and
///   <see cref="GridExtensions.FloodSelect{T}(IGrid{T}, IntVector2, Func{T, IntVector2, bool}, IEnumerable{IntVector2}, IEnumerable{Func{IntVector2, IntVector2}})"/>.
/// </summary>
public static class GridTransforms
{
  /// <summary>
  ///   Read-only: A sequence of transform functions containing only the
  ///   identity function.
  /// </summary>
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Identity = [iv2 => iv2];

  /// <summary>
  ///   Read-only: A sequence of transform functions containing the
  ///   identity, right rotation, 180° rotation, and left rotation.
  /// </summary>
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Rotate90 =
  [
    iv2 => iv2,
    iv2 => iv2.RotateRight(),
    iv2 => iv2.RotateAround(),
    iv2 => iv2.RotateLeft()
  ];

  /// <summary>
  ///   Read-only: A sequence of transform functions containing the left
  ///   rotation and the identity.
  /// </summary>
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Previous =
  [
    iv2 => iv2.RotateLeft(),
    iv2 => iv2
  ];

  /// <summary>
  ///   Read-only: A sequence of transform functions containing the right
  ///   and 180° rotations.
  /// </summary>
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Forward =
  [
    iv2 => iv2.RotateRight(),
    iv2 => iv2.RotateAround()
  ];
}