using System.Collections.Generic;

namespace Nixill.Random {
  public class WeightedRandom<T> {
    private SortedDictionary<int, T> Backing;

    public WeightedRandom() {
      Backing = new SortedDictionary<int, T>();
    }
  }
}