using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Nixill.Collections;

/// <summary>
///   This is an implementation of <see cref="ISet<T>" /> backed by an
///   AVL tree.
/// </summary>
/// <remarks>
///   <para>
///     Much of the code from this class comes from Costin S and was
///     licensed under the MIT License. The original code is available
///     <see href="https://code.google.com/archive/p/self-balancing-avl-tree/">here</see>.
///   </para>
///   <para>
///     This class doesn't include the sections of code under the
///     <c>TREE_WITH_PARENT_POINTERS</c> or
///     <c>TREE_WITH_CONCAT_AND_SPLIT_OPERATIONS</c> defines.
///   </para>
/// </remarks>
/// <typeparam name="T">The type of the data stored in the nodes</typeparam>
public class AVLTreeSet<T> : INavigableSet<T>
{
  #region Fields

  private Node<T>? Root;
  private Comparison<T> Comparer;

  #endregion

  #region Properties

  /// <summary>
  ///   Get: The number of entries in this set.
  /// </summary>
  public int Count { get; private set; }

  /// <summary>
  ///   Get: Whether or not this set is read-only (<c>false</c>).
  /// </summary>
  public bool IsReadOnly => false;

  #endregion

  #region Ctor

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using the type's default <see cref="Comparer{T}"/>.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   The type isn't naturally comparable.
  /// </exception>
  public AVLTreeSet() : this([], GetComparer()) { }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using a specified <see cref="IComparer{T}"/>.
  /// </summary>
  /// <param name="comparer">
  ///   The comparer that compares the elements of the set.
  /// </param>
  public AVLTreeSet(IComparer<T> comparer) : this([], comparer.Compare) { }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using a specified comparison function.
  /// </summary>
  /// <param name="comparer">
  ///   The function that compares the elements of the set.
  /// </param>
  public AVLTreeSet(Comparison<T> comparer) : this([], comparer) { }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using the type's default <see cref="Comparer{T}"/> and a
  ///   pre-existing set of elements.
  /// </summary>
  /// <param name="elems">
  ///   The elements with which to pre-populate the set.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   The type isn't naturally comparable.
  /// </exception>
  public AVLTreeSet(IEnumerable<T> elems) : this(elems, GetComparer()) { }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using a specified <see cref="IComparer{T}"/> and a
  ///   pre-existing set of elements.
  /// </summary>
  /// <param name="elems">
  ///   The elements with which to pre-populate the set.
  /// </param>
  /// <param name="comparer">
  ///   The comparer that compares the elements of the set.
  /// </param>
  public AVLTreeSet(IEnumerable<T> elems, IComparer<T> comparer) : this(elems, comparer.Compare) { }

  /// <summary>
  ///   Initializes a new instance of the <see cref="AVLTreeSet{T}"/>
  ///   class, using a specified comparison function and a pre-existing
  ///   set of elements.
  /// </summary>
  /// <param name="elems">
  ///   The elements with which to pre-populate the set.
  /// </param>
  /// <param name="comparer">
  ///   The function that compares the elements of the set.
  /// </param>
  public AVLTreeSet(IEnumerable<T> elems, Comparison<T> comparer)
  {
    this.Comparer = comparer;

    if (elems != null)
    {
      foreach (var elem in elems)
      {
        this.Add(elem);
      }
    }
  }

  #endregion

  #region Delegates

  /// <summary>
  /// the visitor delegate
  /// </summary>
  /// <typeparam name="TNode">The type of the node.</typeparam>
  /// <param name="node">The node.</param>
  /// <param name="level">The level.</param>
  internal delegate void VisitNodeHandler<TNode>(TNode node, int level);

  #endregion

  #region Public Methods
  /// <summary>
  ///   Adds the specified item to the set.
  /// </summary>
  /// <param name="arg">The item to add.</param>
  /// <returns>
  ///   <c>true</c> if the item was successfully added, <c>false</c>
  ///   otherwise (which may also indicate that the set already
  ///   contained the element).
  /// </returns>
  public bool Add(T arg)
  {
    bool wasAdded = false;
    bool wasSuccessful = false;

    this.Root = this.Add(this.Root, arg, ref wasAdded, ref wasSuccessful);

    if (wasSuccessful) Count++;

    return wasSuccessful;
  }

  /// <summary>
  ///   Removes the specified item from the set.
  /// </summary>
  /// <param name="arg">The item to remove.</param>
  /// <returns>
  ///   <c>true</c> if the item was successfully removed, <c>false</c>
  ///   otherwise (which may also indicate that the set already
  ///   contained the element).
  /// </returns>
  public bool Delete(T arg)
  {
    bool wasSuccessful = false;

    if (this.Root != null)
    {
      bool wasDeleted = false;
      this.Root = this.Delete(this.Root, arg, ref wasDeleted, ref wasSuccessful);
    }

    if (wasSuccessful) Count--;

    return wasSuccessful;
  }

  /// <summary>
  ///   Attempts to get the lowest element in the set.
  /// </summary>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this is the lowest element
  ///   in the set. Otherwise, this is <c>default(T)</c>.
  /// </param>
  /// <returns>
  ///   Whether or not there are any elements in the set.
  /// </returns>
  /// <seealso cref="LowestValue()"/>
  public bool GetMin(out T value)
  {
    if (this.Root != null)
    {
      var min = FindMin(this.Root);
      if (min != null)
      {
        value = min.Data;
        return true;
      }
    }

    value = default(T)!;
    return false;
  }

  /// <summary>
  ///   Attempts to get the highest element in the set.
  /// </summary>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this is the highest element
  ///   in the set. Otherwise, this is <c>default(T)</c>.
  /// </param>
  /// <returns>
  ///   Whether or not there are any elements in the set.
  /// </returns>
  /// <seealso cref="HighestValue()"/>
  public bool GetMax(out T value)
  {
    if (this.Root != null)
    {
      var max = FindMax(this.Root);
      if (max != null)
      {
        value = max.Data;
        return true;
      }
    }

    value = default(T)!;
    return false;
  }

  /// <summary>
  ///   Returns whether or not the set contains the given element.
  /// </summary>
  /// <param name="arg">The element to find.</param>
  /// <returns><c>true</c> iff the set contains <c>arg</c>.</returns>
  public bool Contains(T arg)
  {
    return this.Search(this.Root, arg) != null;
  }

  /// <summary>
  ///   Deletes the lowest value from the set.
  /// </summary>
  /// <returns>
  ///   Whether or not any value was deleted — a return value of
  ///   <c>false</c> may indicate that the set was already empty.
  /// </returns>
  public bool DeleteMin()
  {
    if (this.Root != null)
    {
      bool wasDeleted = false, wasSuccessful = false;
      this.Root = this.DeleteMin(this.Root, ref wasDeleted, ref wasSuccessful);

      return wasSuccessful;
    }

    return false;
  }

  /// <summary>
  ///   Deletes the highest value from the set.
  /// </summary>
  /// <returns>
  ///   Whether or not any value was deleted — a return value of
  ///   <c>false</c> may indicate that the set was already empty.
  /// </returns>
  public bool DeleteMax()
  {
    if (this.Root != null)
    {
      bool wasDeleted = false, wasSuccessful = false;
      this.Root = this.DeleteMax(this.Root, ref wasDeleted, ref wasSuccessful);

      return wasSuccessful;
    }

    return false;
  }

  /// <summary>
  ///   Returns the height of the tree. 
  /// </summary>
  /// <returns>The AVL tree height.</returns>
  public int GetHeightLogN()
  {
    return this.GetHeightLogN(this.Root);
  }

  /// <summary>
  ///   Removes all values from this set.
  /// </summary>
  public void Clear()
  {
    this.Root = null;
    Count = 0;
  }

  /// <summary>
  ///   Prints this instance to the console.
  /// </summary>
  public void Print()
  {
    this.Visit((node, level) =>
    {
      Console.Write(new string(' ', 2 * level));
      Console.WriteLine("{0, 6}", node.Data);
    });
  }

  /// <summary>
  ///   Returns whether or not the set is empty.
  /// </summary>
  /// <returns><c>true</c> iff the set contains no elements.</returns>
  public bool IsEmpty() => Root == null;

  /// <summary>
  ///   Returns a triplet with, if they exist in the set, the value
  ///   equal to the given value, the highest value less than the given
  ///   value, and the lowest value greater than the given value.
  /// </summary>
  /// <param name="value">The value to search around.</param>
  /// <returns>The triplet.</returns>
  public NodeTriplet<T> SearchAround(T value)
  {
    return new NodeTriplet<T>(SearchBounded(value));
  }

  /// <summary>
  ///   Replaces a single element without rebalancing the tree.
  /// </summary>
  /// <param name="oldValue">The value being replaced.</param>
  /// <param name="newValue">The value to replace it with.</param>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The <c>newValue</c> doesn't fall between the same surrounding
  ///   elements as the <c>oldValue</c>.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   The <c>oldValue</c> is not present within the set.
  /// </exception>
  public void ReplaceValue(T oldValue, T newValue)
  {
    var nodes = SearchBounded(oldValue);
    if (nodes.Exact == null) throw new InvalidOperationException("The oldValue was not found in the set.");
    if (nodes.Higher != null && Comparer(newValue, nodes.Higher.Data) >= 0) throw new ArgumentOutOfRangeException("newValue", "Higher than or equal to the next node.");
    if (nodes.Lower != null && Comparer(nodes.Lower.Data, newValue) >= 0) throw new ArgumentOutOfRangeException("newValue", "Lower than or equal to the previous node.");
    nodes.Exact.Data = newValue;
  }

  #endregion

  #region Interface Implementations
  /// <summary>
  ///   Attempts to retrieve the highest value in the set that is less
  ///   than the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   value found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff a lower value exists.</returns>
  public bool TryGetLower(T from, out T value)
  {
    var nodes = SearchBounded(from);
    var node = nodes.Lower;

    if (node != null)
    {
      value = node.Data;
      return true;
    }
    else
    {
      value = default(T)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns whether or not a value exists in the set that is less
  ///   than the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns><c>true</c> iff a lower value exists.</returns>
  public bool ContainsLower(T from) => TryGetLower(from, out T placeholder);

  /// <summary>
  ///   Returns the highest value in the set that is less than the given
  ///   value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns>The lower value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The set is not empty, but <c>from<c/> is less than or equal to
  ///   the lowest value in the set.
  /// </exception>
  public T Lower(T from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have lower values in an empty AVLTreeSet.");
    if (TryGetLower(from, out T value))
    {
      return value;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no lower value in the AVLTreeSet.");
  }

  /// <summary>
  ///   Attempts to retrieve the highest value in the set that is less
  ///   than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   value found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff a lower or equal value exists.</returns>
  public bool TryGetFloor(T from, out T value)
  {
    var nodes = SearchBounded(from);
    var node = nodes.Exact ?? nodes.Lower;

    if (node != null)
    {
      value = node.Data;
      return true;
    }
    else
    {
      value = default(T)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns whether or not a value exists in the set that is less
  ///   than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns><c>true</c> iff a lower value exists.</returns>
  public bool ContainsFloor(T from) => TryGetFloor(from, out T placeholder);

  /// <summary>
  ///   Returns the highest value in the set that is less than or equal
  ///   to the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns>The lower or equal value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The set is not empty, but <c>from<c/> is less than the lowest
  ///   value in the set.
  /// </exception>
  public T Floor(T from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have floor values in an empty AVLTreeSet.");
    if (TryGetFloor(from, out T value))
    {
      return value;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no floor value in the AVLTreeSet.");
  }

  /// <summary>
  ///   Attempts to retrieve the lowest value in the set that is greater
  ///   than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   value found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff a higher or equal value exists.</returns>
  public bool TryGetCeiling(T from, out T value)
  {
    var nodes = SearchBounded(from);
    var node = nodes.Exact ?? nodes.Higher;

    if (node != null)
    {
      value = node.Data;
      return true;
    }
    else
    {
      value = default(T)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns whether or not a value exists in the set that is greater
  ///   than or equal to the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns><c>true</c> iff a higher or equal value exists.</returns>
  public bool ContainsCeiling(T from) => TryGetCeiling(from, out T placeholder);

  /// <summary>
  ///   Returns the lowest value in the set that is greater than the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns>The higher or equal value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The set is not empty, but <c>from<c/> is greater than the
  ///   highest value in the set.
  /// </exception>
  public T Ceiling(T from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have ceiling values in an empty AVLTreeSet.");
    if (TryGetCeiling(from, out T value))
    {
      return value;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no ceiling value in the AVLTreeSet.");
  }

  /// <summary>
  ///   Attempts to retrieve the lowest value in the set that is greater
  ///   than the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <param name="value">
  ///   If this method returns <c>true</c>, this parameter contains the
  ///   value found. Otherwise, it contains the default value for the type.
  /// </param>
  /// <returns><c>true</c> iff a higher value exists.</returns>
  public bool TryGetHigher(T from, out T value)
  {
    var nodes = SearchBounded(from);
    var node = nodes.Higher;

    if (node != null)
    {
      value = node.Data;
      return true;
    }
    else
    {
      value = default(T)!;
      return false;
    }
  }

  /// <summary>
  ///   Returns whether or not a value exists in the set that is greater
  ///   than the given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns><c>true</c> iff a higher value exists.</returns>
  public bool ContainsHigher(T from) => TryGetHigher(from, out T placeholder);

  /// <summary>
  ///   Returns the lowest value in the set that is greater than the
  ///   given value.
  /// </summary>
  /// <param name="from">The value to find a value near.</param>
  /// <returns>The higher value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   The set is not empty, but <c>from<c/> is greater than or equal
  ///   to the highest value in the set.
  /// </exception>
  public T Higher(T from)
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot have higher values in an empty AVLTreeSet.");
    if (TryGetHigher(from, out T value))
    {
      return value;
    }
    else throw new ArgumentOutOfRangeException("from", "There is no higher value in the AVLTreeSet.");
  }

  /// <summary>
  ///   Returns the lowest value in the set.
  /// </summary>
  /// <returns>The lowest value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  public T LowestValue()
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot get the lowest value of an empty AVLTreeSet.");
    GetMin(out T val);
    return val;
  }

  /// <summary>
  ///   Returns the highest value in the set.
  /// </summary>
  /// <returns>The highest value.</returns>
  /// <exception cref="InvalidOperationException">
  ///   The set is empty.
  /// </exception>
  public T HighestValue()
  {
    if (IsEmpty()) throw new InvalidOperationException("Cannot get the highest value of an empty AVLTreeSet.");
    GetMax(out T val);
    return val;
  }

  /// <summary>
  ///   Removes all elements in <c>elems</c> from this set.
  /// </summary>
  /// <param name="elems">The elements to remove.</param>
  public void ExceptWith(IEnumerable<T> elems)
  {
    Root = new AVLTreeSet<T>(this.Except(elems), Comparer).Root;
  }

  /// <summary>
  ///   Removes all elements from this set except those in <c>elems</c>.
  /// </summary>
  /// <param name="elems">The elements to keep.</param>
  public void IntersectWith(IEnumerable<T> elems)
  {
    Root = new AVLTreeSet<T>(this.Intersect(elems), Comparer).Root;
  }

  /// <summary>
  ///   Adds all elements to this set from <c>elems</c>, except those
  ///   which are already in this set, which are instead removed.
  /// </summary>
  /// <param name="elems">The elements to exclusive-or.</param>
  public void SymmetricExceptWith(IEnumerable<T> elems)
  {
    Root = new AVLTreeSet<T>(this.Except(elems).Union(elems.Except(this)), Comparer).Root;
  }

  /// <summary>
  ///   Adds all elements to this set from <c>elems</c> that are not
  ///   already a part of it.
  /// </summary>
  /// <param name="elems">The elements to add.</param>
  public void UnionWith(IEnumerable<T> elems)
  {
    Root = new AVLTreeSet<T>(this.Union(elems), Comparer).Root;
  }

  /// <summary>
  ///   Returns whether this set is a strict subset of <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "strict subset" means that this set contains some, but not all,
  ///   of the elements in <c>elems</c>, and this set does not contain
  ///   any elements that <c>elems</c> does not contain.
  ///   <para/>
  ///   An empty set is a strict subset of a non-empty set. It is not,
  ///   however, a strict subset of another empty set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set is a strict subset of <c>elems</c>.
  /// </returns>
  public bool IsProperSubsetOf(IEnumerable<T> elems) => elems.Except(this).Any();

  /// <summary>
  ///   Returns whether this set is a strict superset of <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "strict superset" means that this set contains all elements of
  ///   <c>elems</c>, and at least one element that <c>elems</c> does
  ///   not contain.
  ///   <para/>
  ///   A non-empty set is a strict superset of an empty set. Another
  ///   empty set, however, is not a strict superset of an empty set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set is a strict superset of <c>elems</c>.
  /// </returns>
  public bool IsProperSupersetOf(IEnumerable<T> elems) => this.Except(elems).Any();

  /// <summary>
  ///   Returns whether this set is a subset of <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "subset" means that this set contains some or all of the
  ///   elements of <c>elems</c>, and does not contain any elements that
  ///   <c>elems</c> does not.
  ///   <para/>
  ///   An empty set is a subset of any other set, even another empty set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set is a subset of <c>elems</c>.
  /// </returns>
  public bool IsSubsetOf(IEnumerable<T> elems) => !this.Except(elems).Any();

  /// <summary>
  ///   Returns whether this set is a superset of <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "superset" means that this set contains all of the elements of
  ///   <c>elems</c>, regardless of whether or not this set also
  ///   contains elements that <c>elems</c> does not.
  ///   <para/>
  ///   Any set, including an empty set, is a superset of an empty set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set is a superset of <c>elems</c>.
  /// </returns>
  public bool IsSupersetOf(IEnumerable<T> elems) => !elems.Except(this).Any();

  /// <summary>
  ///   Returns whether this set overlaps <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "overlaps" means the two sets have at least one element in common.
  ///   <para/>
  ///   An empty set does not overlap any other set, even another empty set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set overlaps <c>elems</c>.
  /// </returns>
  public bool Overlaps(IEnumerable<T> elems) => elems.Intersect(this).Any();

  /// <summary>
  ///   Returns whether this set is equal to <c>elems</c>.
  /// </summary>
  /// <remarks>
  ///   "equals" means the two sets both contain the same elements -
  ///   neither set has any elements that the other does not.
  ///   <para/>
  ///   An empty set equals itself, and no other set.
  /// </remarks>
  /// <param name="elems">The other set to check.</param>
  /// <returns>
  ///   <c>true</c> iff this set is equal to <c>elems</c>.
  /// </returns>
  public bool SetEquals(IEnumerable<T> elems) => elems.Intersect(this).Count() == Count;

  /// <inheritdoc/>
  void ICollection<T>.Add(T item) { Add(item); }

  /// <summary>
  ///   Copies this set to a one-dimensional array.
  /// </summary>
  /// <param name="array">
  ///   The array elements will be copied to.
  /// </param>
  /// <param name="index">
  ///   The index within the array at which to begin copying.
  /// </param>
  public void CopyTo(T[] array, int index)
  {
    foreach (T item in this)
    {
      array[index++] = item;
    }
  }

  /// <summary>
  ///   Removes an item from the set.
  /// </summary>
  /// <param name="item">The item to remove.</param>
  /// <returns>
  ///   <c>true</c> if the item was successfully removed, <c>false</c>
  ///   otherwise (which may indicate that the item was not in the set
  ///   in the first place).
  /// </returns>
  public bool Remove(T item) => Delete(item);

  /// <summary>
  ///   Returns an enumerator over the elements of the set.
  /// </summary>
  /// <remarks>
  ///   Elements are enumerated in sequential order.
  /// </remarks>
  /// <returns>The enumerator.</returns>
  public IEnumerator<T> GetEnumerator() => RecursiveEnumerate(Root).GetEnumerator();

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  #endregion

  #region Internal Methods

  internal int GetCount()
  {
    int count = 0;
    this.Visit((node, level) =>
    {
      count++;
    });
    return count;
  }

  #endregion

  #region Private Methods

  private IEnumerable<T> RecursiveEnumerate(Node<T>? node)
  {
    if (node == null) yield break;

    foreach (T item in RecursiveEnumerate(node.Left)) yield return item;
    yield return node.Data;
    foreach (T item in RecursiveEnumerate(node.Right)) yield return item;
  }

  private static Comparison<T> GetComparer()
  {
    if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(System.IComparable).IsAssignableFrom(typeof(T)))
    {
      return Comparer<T>.Default.Compare;
    }
    else
    {
      throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The type {0} cannot be compared. It must implement IComparable<T> or IComparable interface", typeof(T).FullName));
    }
  }

  /// <summary>
  /// Gets the height of the tree in log(n) time.
  /// </summary>
  /// <param name="node">The node.</param>
  /// <returns>The height of the tree. Runs in O(log(n)) where n is the number of nodes in the tree </returns>
  private int GetHeightLogN(Node<T>? node)
  {
    if (node == null)
    {
      return 0;
    }
    else
    {
      int leftHeight = this.GetHeightLogN(node.Left);
      if (node.Balance == 1)
      {
        leftHeight++;
      }

      return 1 + leftHeight;
    }
  }

  /// <summary>
  /// Adds the specified data to the tree identified by the specified argument.
  /// </summary>
  /// <param name="elem">The elem.</param>
  /// <param name="data">The data.</param>
  /// <returns></returns>
  private Node<T> Add(Node<T>? elem, T data, ref bool wasAdded, ref bool wasSuccessful)
  {
    if (elem == null)
    {
      elem = new Node<T> { Data = data, Left = null, Right = null, Balance = 0 };

      wasAdded = true;
      wasSuccessful = true;
    }
    else
    {
      int resultCompare = this.Comparer(data, elem.Data);

      if (resultCompare < 0)
      {
        var newLeft = Add(elem.Left, data, ref wasAdded, ref wasSuccessful);
        if (elem.Left != newLeft)
        {
          elem.Left = newLeft;
        }

        if (wasAdded)
        {
          --elem.Balance;

          if (elem.Balance == 0)
          {
            wasAdded = false;
          }
          else if (elem.Balance == -2)
          {
            int leftBalance = newLeft.Balance;
            if (leftBalance == 1)
            {
              int elemLeftRightBalance = newLeft.Right!.Balance;

              elem.Left = RotateLeft(newLeft);
              elem = RotateRight(elem);

              elem.Balance = 0;
              elem.Left!.Balance = elemLeftRightBalance == 1 ? -1 : 0;
              elem.Right!.Balance = elemLeftRightBalance == -1 ? 1 : 0;
            }
            else if (leftBalance == -1)
            {
              elem = RotateRight(elem);
              elem.Balance = 0;
              elem.Right!.Balance = 0;
            }

            wasAdded = false;
          }
        }
      }
      else if (resultCompare > 0)
      {
        var newRight = this.Add(elem.Right, data, ref wasAdded, ref wasSuccessful);
        if (elem.Right != newRight)
        {
          elem.Right = newRight;
        }

        if (wasAdded)
        {
          ++elem.Balance;
          if (elem.Balance == 0)
          {
            wasAdded = false;
          }
          else if (elem.Balance == 2)
          {
            int rightBalance = newRight.Balance;
            if (rightBalance == -1)
            {
              int elemRightLeftBalance = newRight.Left!.Balance;

              elem.Right = RotateRight(newRight);
              elem = RotateLeft(elem);

              elem.Balance = 0;
              elem.Left!.Balance = elemRightLeftBalance == 1 ? -1 : 0;
              elem.Right!.Balance = elemRightLeftBalance == -1 ? 1 : 0;
            }
            else if (rightBalance == 1)
            {
              elem = RotateLeft(elem);

              elem.Balance = 0;
              elem.Left!.Balance = 0;
            }

            wasAdded = false;
          }
        }
      }
    }

    return elem;
  }

  /// <summary>
  /// Deletes the specified arg. value from the tree.
  /// </summary>
  /// <param name="node">The node.</param>
  /// <param name="arg">The arg.</param>
  /// <returns></returns>
  private Node<T> Delete(Node<T> node, T arg, ref bool wasDeleted, ref bool wasSuccessful)
  {
    int cmp = this.Comparer(arg, node.Data);
    Node<T>? newChild = null;

    if (cmp < 0)
    {
      if (node.Left != null)
      {
        newChild = this.Delete(node.Left, arg, ref wasDeleted, ref wasSuccessful);
        if (node.Left != newChild)
        {
          node.Left = newChild;
        }

        if (wasDeleted)
        {
          node.Balance++;
        }
      }
    }
    else if (cmp == 0)
    {
      wasDeleted = true;
      if (node.Left != null && node.Right != null)
      {
        var min = FindMin(node.Right);
        T data = node.Data;
        node.Data = min!.Data;
        min.Data = data;

        wasDeleted = false;

        newChild = this.Delete(node.Right, data, ref wasDeleted, ref wasSuccessful);
        if (node.Right != newChild)
        {
          node.Right = newChild;
        }

        if (wasDeleted)
        {
          node.Balance--;
        }
      }
      else if (node.Left == null)
      {
        wasSuccessful = true;

        return node.Right!;
      }
      else
      {
        wasSuccessful = true;

        return node.Left;
      }
    }
    else
    {
      if (node.Right != null)
      {
        newChild = this.Delete(node.Right, arg, ref wasDeleted, ref wasSuccessful);
        if (node.Right != newChild)
        {
          node.Right = newChild;
        }

        if (wasDeleted)
        {
          node.Balance--;
        }
      }
    }

    if (wasDeleted)
    {
      if (node.Balance == 1 || node.Balance == -1)
      {
        wasDeleted = false;
      }
      else if (node.Balance == -2)
      {
        var nodeLeft = node.Left;
        int leftBalance = nodeLeft!.Balance;

        if (leftBalance == 1)
        {
          int leftRightBalance = nodeLeft.Right!.Balance;

          node.Left = RotateLeft(nodeLeft);
          node = RotateRight(node);

          node.Balance = 0;
          node.Left!.Balance = (leftRightBalance == 1) ? -1 : 0;
          node.Right!.Balance = (leftRightBalance == -1) ? 1 : 0;
        }
        else if (leftBalance == -1)
        {
          node = RotateRight(node);
          node.Balance = 0;
          node.Right!.Balance = 0;
        }
        else if (leftBalance == 0)
        {
          node = RotateRight(node);
          node.Balance = 1;
          node.Right!.Balance = -1;

          wasDeleted = false;
        }
      }
      else if (node.Balance == 2)
      {
        var nodeRight = node.Right;
        int rightBalance = nodeRight!.Balance;

        if (rightBalance == -1)
        {
          int rightLeftBalance = nodeRight.Left!.Balance;

          node.Right = RotateRight(nodeRight);
          node = RotateLeft(node);

          node.Balance = 0;
          node.Left!.Balance = (rightLeftBalance == 1) ? -1 : 0;
          node.Right!.Balance = (rightLeftBalance == -1) ? 1 : 0;
        }
        else if (rightBalance == 1)
        {
          node = RotateLeft(node);
          node.Balance = 0;
          node.Left!.Balance = 0;
        }
        else if (rightBalance == 0)
        {
          node = RotateLeft(node);
          node.Balance = -1;
          node.Left!.Balance = 1;

          wasDeleted = false;
        }
      }
    }

    return node;
  }

  /// <summary>
  /// Finds the min.
  /// </summary>
  /// <param name="node">The node.</param>
  /// <returns></returns>
  private static Node<T>? FindMin(Node<T> node)
  {
    while (node != null && node.Left != null)
    {
      node = node.Left;
    }

    return node;
  }

  /// <summary>
  /// Finds the max.
  /// </summary>
  /// <param name="node">The node.</param>
  /// <returns></returns>
  private static Node<T>? FindMax(Node<T> node)
  {
    while (node != null && node.Right != null)
    {
      node = node.Right;
    }

    return node;
  }

  /// <summary>
  /// Searches the specified subtree for the specified data.
  /// </summary>
  /// <param name="subtree">The subtree.</param>
  /// <param name="data">The data to search for.</param>
  /// <returns>null if not found, otherwise the node instance with the specified value</returns>
  private Node<T>? Search(Node<T>? subtree, T data)
  {
    if (subtree != null)
    {
      if (this.Comparer(data, subtree.Data) < 0)
      {
        return this.Search(subtree.Left, data);
      }
      else if (this.Comparer(data, subtree.Data) > 0)
      {
        return this.Search(subtree.Right, data);
      }
      else
      {
        return subtree;
      }
    }
    else
    {
      return null;
    }
  }

  /// <summary>
  /// Deletes the min element in the tree.
  /// Precondition: (node != null)
  /// </summary>
  /// <param name="node">The node.</param>
  /// <returns></returns>
  private Node<T>? DeleteMin(Node<T> node, ref bool wasDeleted, ref bool wasSuccessful)
  {
    Debug.Assert(node != null);

    if (node.Left == null)
    {
      wasDeleted = true;
      wasSuccessful = true;

      return node.Right;
    }

    node.Left = this.DeleteMin(node.Left, ref wasDeleted, ref wasSuccessful);
    if (wasDeleted)
    {
      node.Balance++;
    }

    if (wasDeleted)
    {
      if (node.Balance == 1 || node.Balance == -1)
      {
        wasDeleted = false;
      }
      else if (node.Balance == -2)
      {
        int leftBalance = node.Left!.Balance;
        if (leftBalance == 1)
        {
          int leftRightBalance = node.Left.Right!.Balance;

          node.Left = RotateLeft(node.Left);
          node = RotateRight(node);

          node.Balance = 0;
          node.Left!.Balance = (leftRightBalance == 1) ? -1 : 0;
          node.Right!.Balance = (leftRightBalance == -1) ? 1 : 0;
        }
        else if (leftBalance == -1)
        {
          node = RotateRight(node);
          node.Balance = 0;
          node.Right!.Balance = 0;
        }
        else if (leftBalance == 0)
        {
          node = RotateRight(node);
          node.Balance = 1;
          node.Right!.Balance = -1;

          wasDeleted = false;
        }
      }
      else if (node.Balance == 2)
      {
        int rightBalance = node.Right!.Balance;
        if (rightBalance == -1)
        {
          int rightLeftBalance = node.Right.Left!.Balance;

          node.Right = RotateRight(node.Right);
          node = RotateLeft(node);

          node.Balance = 0;
          node.Left!.Balance = (rightLeftBalance == 1) ? -1 : 0;
          node.Right!.Balance = (rightLeftBalance == -1) ? 1 : 0;
        }
        else if (rightBalance == 1)
        {
          node = RotateLeft(node);
          node.Balance = 0;
          node.Left!.Balance = 0;
        }
        else if (rightBalance == 0)
        {
          node = RotateLeft(node);
          node.Balance = -1;
          node.Left!.Balance = 1;

          wasDeleted = false;
        }
      }
    }

    return node;
  }

  /// <summary>
  /// Deletes the max element in the tree.
  /// Precondition: (node != null)
  /// </summary>
  /// <param name="node">The node.</param>
  /// <returns></returns>
  private Node<T>? DeleteMax(Node<T> node, ref bool wasDeleted, ref bool wasSuccessful)
  {
    Debug.Assert(node != null);

    if (node.Right == null)
    {
      wasDeleted = true;
      wasSuccessful = true;

      return node.Left;
    }

    node.Right = this.DeleteMax(node.Right, ref wasDeleted, ref wasSuccessful);
    if (wasDeleted)
    {
      node.Balance--;
    }

    if (wasDeleted)
    {
      if (node.Balance == 1 || node.Balance == -1)
      {
        wasDeleted = false;
      }
      else if (node.Balance == -2)
      {
        int leftBalance = node.Left!.Balance;
        if (leftBalance == 1)
        {
          int leftRightBalance = node.Left.Right!.Balance;

          node.Left = RotateLeft(node.Left);
          node = RotateRight(node);

          node.Balance = 0;
          node.Left!.Balance = (leftRightBalance == 1) ? -1 : 0;
          node.Right!.Balance = (leftRightBalance == -1) ? 1 : 0;
        }
        else if (leftBalance == -1)
        {
          node = RotateRight(node);
          node.Balance = 0;
          node.Right!.Balance = 0;
        }
        else if (leftBalance == 0)
        {
          node = RotateRight(node);
          node.Balance = 1;
          node.Right!.Balance = -1;

          wasDeleted = false;
        }
      }
      else if (node.Balance == 2)
      {
        int rightBalance = node.Right!.Balance;
        if (rightBalance == -1)
        {
          int rightLeftBalance = node.Right.Left!.Balance;

          node.Right = RotateRight(node.Right);
          node = RotateLeft(node);

          node.Balance = 0;
          node.Left!.Balance = (rightLeftBalance == 1) ? -1 : 0;
          node.Right!.Balance = (rightLeftBalance == -1) ? 1 : 0;
        }
        else if (rightBalance == 1)
        {
          node = RotateLeft(node);
          node.Balance = 0;
          node.Left!.Balance = 0;
        }
        else if (rightBalance == 0)
        {
          node = RotateLeft(node);
          node.Balance = -1;
          node.Left!.Balance = 1;

          wasDeleted = false;
        }
      }

    }

    return node;
  }

  /// <summary>
  /// Visits the tree using the specified visitor.
  /// </summary>
  /// <param name="visitor">The visitor.</param>
  private void Visit(VisitNodeHandler<Node<T>> visitor)
  {
    if (this.Root != null)
    {
      this.Root.Visit(visitor, 0);
    }
  }

  /// <summary>
  /// Rotates lefts this instance. 
  /// Precondition: (node != null && node.Right != null)
  /// </summary>
  /// <returns></returns>
  private static Node<T> RotateLeft(Node<T> node)
  {
    Debug.Assert(node != null && node.Right != null);

    var right = node.Right;
    var nodeLeft = node.Left;
    var rightLeft = right.Left;

    node.Right = rightLeft;
    right.Left = node;

    return right;
  }

  /// <summary>
  /// RotateRights this instance. 
  /// Precondition: (node != null && node.Left != null)
  /// </summary>
  /// <returns></returns>
  private static Node<T> RotateRight(Node<T> node)
  {
    Debug.Assert(node != null && node.Left != null);

    var left = node.Left;
    var leftRight = left.Right;
    node.Left = leftRight;

    left.Right = node;

    return left;
  }

  /// <summary>
  ///   Searches down the tree.
  /// </summary>
  private (Node<T>? Lower, Node<T>? Exact, Node<T>? Higher) SearchBounded(T value)
  {
    if (Root == null) return (null, null, null);

    Node<T>? lower = null, higher = null;
    Node<T> current = Root;

    while (true)
    {
      int comp = Comparer(value, current.Data);

      if (comp == 0)
      {
        if (current.Left != null)
        {
          lower = current.Left;
          while (lower.Right != null) lower = lower.Right;
        }
        if (current.Right != null)
        {
          higher = current.Right;
          while (higher.Left != null) higher = higher.Left;
        }
        return (lower, current, higher);
      }

      else if (comp < 0)
      {
        if (current.Left != null)
        {
          higher = current;
          current = current.Left;
        }
        else return (lower, null, current);
      }

      else
      {
        if (current.Right != null)
        {
          lower = current;
          current = current.Right;
        }
        else return (current, null, higher);
      }
    }
  }

  #endregion

  #region Nested Classes

  /// <summary>
  /// node class
  /// </summary>
  /// <typeparam name="TElem">The type of the elem.</typeparam>
  internal class Node<TElem>
  {
    #region Properties

    public Node<TElem>? Left { get; set; }

    public Node<TElem>? Right { get; set; }

    public required TElem Data { get; set; }

    public int Balance { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Visits (in-order) this node with the specified visitor.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    /// <param name="level">The level.</param>
    public void Visit(VisitNodeHandler<Node<TElem>> visitor, int level)
    {
      if (visitor == null)
      {
        return;
      }

      if (this.Left != null)
      {
        this.Left.Visit(visitor, level + 1);
      }

      visitor(this, level);

      if (this.Right != null)
      {
        this.Right.Visit(visitor, level + 1);
      }
    }

    #endregion
  }

  #endregion
}

/// <summary>
///   Represents a triplet of values as a search result around a
///   specific value.
/// </summary>
/// <typeparam name="T">
///   The type of items contained in this set.
/// </typeparam>
public class NodeTriplet<T>
{
  internal readonly NodeValue? Lesser;
  internal readonly NodeValue? Equal;
  internal readonly NodeValue? Greater;

  /// <summary>
  ///   Get: Whether or not this triplet has a value less than the original.
  /// </summary>
  public bool HasLesserValue => Lesser != null;

  /// <summary>
  ///   Get: Whether or not this triplet has a value equal to the original.
  /// </summary>
  public bool HasEqualValue => Equal != null;

  /// <summary>
  ///   Get: Whether or not this triplet has a value greater than the
  ///   original.
  /// </summary>
  public bool HasGreaterValue => Greater != null;

  /// <summary>
  ///   Get: The value that's less than the original.
  /// </summary>
  public T LesserValue
  {
    get
    {
      if (Lesser != null) return Lesser.Value;
      else throw new InvalidOperationException("No lesser value exists.");
    }
  }

  /// <summary>
  ///   Get: The value that's equal to the original.
  /// </summary>
  public T EqualValue
  {
    get
    {
      if (Equal != null) return Equal.Value;
      else throw new InvalidOperationException("No equal value exists.");
    }
  }

  /// <summary>
  ///   Get: The value that's greater than the original.
  /// </summary>
  public T GreaterValue
  {
    get
    {
      if (Greater != null) return Greater.Value;
      else throw new InvalidOperationException("No greater value exists.");
    }
  }

  internal NodeTriplet((AVLTreeSet<T>.Node<T>? L, AVLTreeSet<T>.Node<T>? E, AVLTreeSet<T>.Node<T>? G) nodes)
  {
    if (nodes.L != null) Lesser = new NodeValue(nodes.L.Data);
    if (nodes.E != null) Equal = new NodeValue(nodes.E.Data);
    if (nodes.G != null) Greater = new NodeValue(nodes.G.Data);
  }

  internal class NodeValue
  {
    public readonly T Value;
    public NodeValue(T val)
    {
      Value = val;
    }
  }
}
