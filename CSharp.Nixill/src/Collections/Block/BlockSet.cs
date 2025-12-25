using System.Collections;

namespace Nixill.Collections;

public class BlockSet<T> : IEnumerable<BlockSetRecord<T>>
{
  BlockDictionary<T, bool> Backing { get; }

  public BlockSet()
  {
    Backing = new BlockDictionary<T, bool>(false);
  }

  public BlockSet(IComparer<T> comparer)
  {
    Backing = new BlockDictionary<T, bool>(false, comparer);
  }

  public bool Contains(T test)
    => Backing[test];

  public void AddRange(T lowerBound, T upperBound, bool lowerBoundInclusive = true, bool upperBoundInclusive = false)
  {
    Backing.SetRange(lowerBound, upperBound, true, lowerBoundInclusive, upperBoundInclusive);
  }

  public void RemoveRange(T lowerBound, T upperBound, bool lowerBoundInclusive = true, bool upperBoundInclusive = false)
  {
    Backing.SetRange(lowerBound, upperBound, false, lowerBoundInclusive, upperBoundInclusive);
  }

  IEnumerable<BlockSetRecord<T>> AsEnumerable() => Backing
    .Select(bdr => new BlockSetRecord<T>(bdr.LowerBound, bdr.UpperBound));

  public IEnumerator<BlockSetRecord<T>> GetEnumerator() => AsEnumerable().GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();
}

public record struct BlockSetRecord<T>(BlockKey<T> LowerBound, BlockKey<T> UpperBound);
