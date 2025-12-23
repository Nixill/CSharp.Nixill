using NUnit.Framework;
using Nixill.Objects;
using Nixill.Collections;

namespace Nixill.Test;

public class TreeTests
{
  [Test]
  public void SliceTest()
  {
    AVLTreeSet<int> set = [3, 5, 10, 14, 16, 20];
    Assert.AreEqual(set.Count, 6);

    AVLTreeSet<int> slice = [.. set.GetSlice(12, 18, NavigationDirection.Higher, NavigationDirection.Lower)];
    Assert.AreEqual(slice.Count, 2);
  }
}