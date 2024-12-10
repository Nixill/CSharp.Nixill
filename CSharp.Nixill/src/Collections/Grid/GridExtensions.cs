namespace Nixill.Collections;

public static class GridExtensions
{
  public static IEnumerable<(T? Item, GridReference Reference)> OrthogonallyAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(0, -1)]);

  public static IEnumerable<(T? Item, GridReference Reference)> DiagonallyAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(1, -1)]);

  public static IEnumerable<(T? Item, GridReference reference)> EightAdjacentCells<T>(this IGrid<T> grid,
    GridReference from)
    => grid.NearbyCells(from, [(0, -1), (1, -1)]);

  public static IEnumerable<(T? Item, GridReference reference)> NearbyCells<T>(this IGrid<T> grid, GridReference from,
    IEnumerable<IntVector2> offsets, IEnumerable<Func<IntVector2, IntVector2>> transforms = null!, bool distinct = true)
  {
    transforms ??= GridTransforms.Rotate90;

    foreach (IntVector2 offset in offsets.WithTranslations(transforms, distinct))
    {
      GridReference result = from + offset;

      if (grid.IsWithinGrid(result)) yield return (grid[result], result);
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
}

public static class GridTransforms
{
  public static readonly IEnumerable<Func<IntVector2, IntVector2>> Rotate90 =
  [
    iv2 => iv2,
    iv2 => iv2.RotateRight(),
    iv2 => iv2.RotateAround(),
    iv2 => iv2.RotateLeft()
  ];
}