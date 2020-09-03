using System;
using System.Collections;
using System.Collections.Generic;

class TreeSet<T> : ISet<T> {
  public int Count { get; private set; }
  public bool IsReadOnly => false;

  private Comparison<T> Comparer;
  private TreeNode RootNode;

  // These private methods assist in the public methods below.
  #region
  // If the given item is a member of the set, returns its node.
  // Otherwise, returns the node that would be its parent.
  // For example, in this int tree:
  //                      15
  //          07                      23
  //    03          11          19          27
  // 01    05    09    13    17    21    25    29
  // Any number <= 2 returns the node with 1
  // Any number >= 28 returns the node with 29
  // Any other odd number returns its own node
  // Any other even number returns the closest *lowest level* odd node
  private Tuple<TreeNode, int> FindSelfOrParent(TreeNode from, T item) {
    T val = from.Value;
    int comp = Comparer.Invoke(item, val);
    if (comp == 0) return new Tuple<TreeNode, int>(from, comp);
    if (comp < 0) {
      if (from.LesserNode != null) return FindSelfOrParent(from.LesserNode, item);
      else return new Tuple<TreeNode, int>(from, comp);
    }
    // comp > 0
    if (from.GreaterNode != null) return FindSelfOrParent(from.GreaterNode, item);
    else return new Tuple<TreeNode, int>(from, comp);
  }

  // Returns an enumerator through this node's...
  // • LesserNode enumerator's return values
  // • Actual value
  // • GreaterNode enumerator's return valies
  // ... in that order.
  private IEnumerable<T> NodeEnumerator(TreeNode from) {
    // First, the LesserNode...
    if (from.LesserNode != null) {
      foreach (T val in NodeEnumerator(from.LesserNode)) {
        yield return val;
      }
    }

    // Then this value itself...
    yield return from.Value;

    // Then the GreaterNode...
    if (from.GreaterNode != null) {
      foreach (T val in NodeEnumerator(from.GreaterNode)) {
        yield return val;
      }
    }
  }
  #endregion

  public bool Add(T item) {
    // If set is empty:
    if (RootNode == null) {
      RootNode = new TreeNode(item);
      Count = 1;
      return true;
    }

    Tuple<TreeNode, int> selfOrParent = FindSelfOrParent(RootNode, item);
    int comp = selfOrParent.Item2;
    TreeNode node = selfOrParent.Item1;
    // Item is already in set
    if (comp == 0) return false;
    // Otherwise make a new TreeNode
    TreeNode newNode = new TreeNode(item);
    // Figure out which branch to put it on
    if (comp < 0) node.LesserNode = newNode;
    else node.GreaterNode = newNode;
    // Add to the count
    Count += 1;
    return true;
  }

  public void Clear() {
    RootNode = null;
    Count = 0;
  }

  public bool Contains(T item) {
    if (RootNode == null) return false;
    Tuple<TreeNode, int> node = FindSelfOrParent(RootNode, item);
    if (node.Item2 == 0) return true;
    else return false;
  }

  public void CopyTo(T[] array, int arrayIndex) {
    throw new System.NotImplementedException();
  }

  public void ExceptWith(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public IEnumerator<T> GetEnumerator() {
    foreach (T val in NodeEnumerator(RootNode)) {
      yield return val;
    }
  }

  public void IntersectWith(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool IsProperSubsetOf(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool IsProperSupersetOf(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool IsSubsetOf(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool IsSupersetOf(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool Overlaps(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public bool Remove(T item) {
    throw new System.NotImplementedException();
  }

  public bool SetEquals(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public void SymmetricExceptWith(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  public void UnionWith(IEnumerable<T> other) {
    throw new System.NotImplementedException();
  }

  void ICollection<T>.Add(T item) {
    throw new System.NotImplementedException();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    throw new System.NotImplementedException();
  }

  private class TreeNode {
    public T Value;
    public TreeNode LesserNode;
    public TreeNode GreaterNode;
    public TreeNode(T item) {
      Value = item;
    }
  }
}