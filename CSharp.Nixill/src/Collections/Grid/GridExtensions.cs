using Nixill.Utils.Extensions;

namespace Nixill.Collections;

public static class GridExtensions
{
  public static IEnumerable<GridReference> OrthogonallyAdjacentRefs<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyRefs(from, [(0, -1)], GridTransforms.Rotate90);

  public static IEnumerable<GridReference> DiagonallyAdjacentRefs<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyRefs(from, [(1, -1)], GridTransforms.Rotate90);

  public static IEnumerable<GridReference> EightAdjacentRefs<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyRefs(from, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  public static IEnumerable<GridReference> NearbyRefs<T>(this IGrid<T> grid, GridReference from,
    IEnumerable<IntVector2> offsets, IEnumerable<Func<IntVector2, IntVector2>> transforms = null!, bool distinct = true)
  {
    transforms ??= GridTransforms.Identity;

    foreach (IntVector2 offset in offsets.WithTranslations(transforms, distinct))
    {
      GridReference result = from + offset;

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

  public static IEnumerable<(T Item, GridReference Reference)> OrthogonallyAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(0, -1)], GridTransforms.Rotate90);

  public static IEnumerable<(T Item, GridReference Reference)> DiagonallyAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(1, -1)], GridTransforms.Rotate90);

  public static IEnumerable<(T Item, GridReference reference)> EightAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  public static IEnumerable<(T Item, GridReference reference)> NearbyCells<T>(this IGrid<T> grid, GridReference from,
    IEnumerable<IntVector2> offsets, IEnumerable<Func<IntVector2, IntVector2>> transforms = null!, bool distinct = true)
    => grid.NearbyRefs(from, offsets, transforms, distinct).Select(r => (grid[r], r));

  public static IEnumerable<(T Item, GridReference Reference)> OrthogonalFloodSelect<T>(this IGrid<T> input,
    GridReference from, Func<T, GridReference, bool> condition)
    => input.FloodSelect(from, condition, [(0, -1)], GridTransforms.Rotate90);

  public static IEnumerable<(T item, GridReference Reference)> EightWayFloodSelect<T>(this IGrid<T> input,
    GridReference from, Func<T, GridReference, bool> condition)
    => input.FloodSelect(from, condition, [(0, -1), (1, -1)], GridTransforms.Rotate90);

  public static IEnumerable<(T item, GridReference Reference)> FloodSelect<T>(this IGrid<T> grid, GridReference from,
    Func<T, GridReference, bool> condition, IEnumerable<IntVector2> offsets,
    IEnumerable<Func<IntVector2, IntVector2>> transforms = null!)
  {
    List<GridReference> refQueue = [from];
    HashSet<GridReference> queued = [from];

    while (refQueue.Count >= 1)
    {
      GridReference rfc = refQueue.Pop();
      T item = grid[rfc]!;

      if (condition(item, rfc))
      {
        yield return (item, rfc);
        foreach (GridReference rfc2 in grid.NearbyRefs(rfc, offsets, transforms))
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

  public static IGrid<T> GetTransposedGrid<T>(this IGrid<T> grid)
    => (grid is TransposedGrid<T> tGrid) ? tGrid.BackingGrid : new TransposedGrid<T>(grid);
}

public static class GridTransforms
{
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Identity = [iv2 => iv2];

  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Rotate90 =
  [
    iv2 => iv2,
    iv2 => iv2.RotateRight(),
    iv2 => iv2.RotateAround(),
    iv2 => iv2.RotateLeft()
  ];

  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Previous =
  [
    iv2 => iv2.RotateLeft(),
    iv2 => iv2
  ];

  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Forward =
  [
    iv2 => iv2.RotateRight(),
    iv2 => iv2.RotateAround()
  ];
}