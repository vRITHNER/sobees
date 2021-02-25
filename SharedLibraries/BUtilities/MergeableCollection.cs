﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;


namespace Sobees.Library.BUtilities
{
  internal interface IMergeable<T> : IEquatable<T>
  {
    /// <summary>Immutable foreign key for the object </summary>
    string FKID { get; }

    /// <summary>Merge the properties of another object of like type with this one.</summary>
    /// <param name="other">The object to merge</param>
    void Merge(T other);
  }

  internal class MergeableCollection<T> : IList<T>, INotifyCollectionChanged where T : class
  {
    private class _Comparer : IComparer<T>
    {
      private bool _dontCompare = false;
      private Comparison<T> _defaultComparison;
      private Comparison<T> _customComparison;
      private Comparison<T> _trueComparison;

      public _Comparer()
      {
        if (typeof(T).GetInterface(typeof(IComparable<T>).Name) != null)
        {
          _defaultComparison = (left, right) =>
                                 {
                                   var comparableLeft = (IComparable<T>)left;
                                   if (comparableLeft == null)
                                   {
                                     return -1;
                                   }

                                   return comparableLeft.CompareTo(right);
                                 };
        }

        _trueComparison = _defaultComparison;
      }

      public Comparison<T> Comparison
      {
        get { return _customComparison; }
        set
        {
          Assert.IsFalse(_dontCompare);
          if (!_dontCompare)
          {
            _customComparison = value;
            _trueComparison = _customComparison ?? _defaultComparison;
          }
        }
      }

      public bool CanCompare => _trueComparison != null;

      public void StopComparisons()
      {
        _trueComparison = null;
        _dontCompare = true;
      }

      public int Compare(T x, T y)
      {
        Assert.IsTrue(CanCompare);
        return _trueComparison(x, y);
      }

      public IEnumerable<T> OrderedList(IEnumerable<T> original)
      {
        Assert.IsNotNull(original);

        if (!CanCompare)
        {
          return original;
        }

        return original.OrderBy(x => x, this);
      }
    }

    private readonly bool _areItemsMergable;
    private readonly bool _areItemsNotifiable;
    private readonly ObservableCollection<T> _items;
    private _Comparer _itemComparer = new _Comparer();

    public object SyncRoot { get; private set; }

    public MergeableCollection()
      : this(null, true)
    {}

    public MergeableCollection(bool sort)
      : this(null, sort)
    {}

    public MergeableCollection(IEnumerable<T> dataObjects)
      : this(dataObjects, true)
    {}

    public MergeableCollection(IEnumerable<T> dataObjects, bool sort)
    {
      SyncRoot = new object();

      if (!sort)
      {
        _itemComparer.StopComparisons();
      }

      // We don't really want to constrain based on the type being IMergeable or comparable
      _areItemsMergable = typeof(T).GetInterface(typeof(IMergeable<T>).Name) != null;
      _areItemsNotifiable = typeof(T).GetInterface(typeof(INotifyPropertyChanged).Name) != null;

      if (dataObjects == null)
      {
        _items = new ObservableCollection<T>();
      }
      else
      {
        _items = new ObservableCollection<T>(_itemComparer.OrderedList(dataObjects));
        if (_areItemsNotifiable)
        {
          lock (SyncRoot)
          {
            foreach (INotifyPropertyChanged item in _items)
            {
              item.PropertyChanged += _OnItemChanged;
            }
          }
        }
      }
    }

    public Comparison<T> CustomComparison
    {
      get { return _itemComparer.Comparison; }
      set
      {
        if (_itemComparer.Comparison != value)
        {
          _itemComparer.Comparison = value;
          RefreshSort();
        }
      }
    }

    public void RefreshSort()
    {
      if (_itemComparer.CanCompare)
      {
        lock (SyncRoot)
        {
          var copyList = new List<T>(_items);
          // Clear the list first so we don't bubble-sort just to reorder.
          _Merge(null, false);
          _Merge(copyList, false);
        }
      }
    }

    /// <summary>
    /// Merges data from another collection.
    /// </summary>
    /// <param name="newCollection">The data object collection that contains new data.</param>
    /// <param name="add">
    /// If true, combine the new collection with existing content, otherwise replace the list.
    /// </param>
    public void Merge(IEnumerable<T> newCollection, bool add)
    {
      _Merge(newCollection, add);
    }

    private void _Merge(IEnumerable<T> newCollection, bool add)
    {
      lock (SyncRoot)
      {
        // Go-go partial template specialization!
        // incrementally correctly merging collections causes memory to blow up right now...
        if (_areItemsMergable)
        {
          _RichMerge(newCollection, add);
        }
        else
        {
          _SimpleMerge(newCollection, add);
        }
      }
    }

    public T FindFKID(string id)
    {
      lock (SyncRoot)
      {
        if (!_areItemsMergable)
        {
          throw new InvalidOperationException("This can only be used on collections with Mergeable items.");
        }

        int index = _FindIndex(0, p => p.FKID == id);
        if (index == -1)
        {
          return null;
        }
        return _items[index];
      }
    }

    private int _FindIndex(int startIndex, Predicate<IMergeable<T>> match)
    {
      int count = Count - startIndex;
      for (int i = startIndex; i < startIndex + count; ++i)
      {
        if (match((IMergeable<T>)this[i]))
        {
          return i;
        }
      }

      return -1;
    }

    private bool _VerifyInsertionPoint(int index)
    {
      lock (SyncRoot)
      {
        if (_itemComparer.Comparison == null)
        {
          // We don't have any way of determining a correct order.  Whatever we have is fine.
          return true;
        }

        // Make sure that the item at index is not less than the one before it
        // and not greater than the one after it.
        // If this fails, we need to update the list.

        if (index != 0)
        {
          if (_itemComparer.Compare(_items[index - 1], _items[index]) > 0)
          {
            return false;
          }
        }

        if (index < _items.Count - 1)
        {
          if (_itemComparer.Compare(_items[index], _items[index+1]) > 0)
          {
            return false;
          }
        }

        return true;
      }
    }

    private int _FindInsertionPoint(T item)
    {
      Assert.IsNotNull(item);

      if (_itemComparer.Comparison != null)
      {
        for (int i = 0; i < _items.Count; ++i)
        {
          if (_itemComparer.Compare(item, _items[i]) <= 0)
          {
            return i;
          }
        }
      }

      return -1;
    }

    /// <summary>
    /// Safe version of Clear that removes references to this from the items being removed.
    /// </summary>
    private void _MergeClear()
    {
      if (_areItemsNotifiable)
      {
        foreach (var item in _items)
        {
          _SafeRemoveNotify(item);
        }
      }
      _items.Clear();
    }

    private void _RichMerge(IEnumerable<T> newCollection, bool additive)
    {
      lock (SyncRoot)
      {
        if (newCollection == null)
        {
          _MergeClear();
          return;
        }

        if (!additive)
        {
          int index = -1;
          foreach (T newItem in _itemComparer.OrderedList(newCollection))
          {
            var mergableItem = newItem as IMergeable<T>;

            ++index;
            int oldIndex = _FindIndex(index, p => p.FKID == mergableItem.FKID);
            if (oldIndex == -1)
            {
              _items.Insert(index, newItem);
              _SafeAddNotify(newItem);
              continue;
            }
            else if (oldIndex != index)
            {
              var item = _items[oldIndex];
              _items.RemoveAt(oldIndex);
              _items.Insert(index, item);
            }

            ((IMergeable<T>)this[index]).Merge(newItem);
          }

          if (index != -1)
          {
            ++index;
            _RemoveRange(index);
          }
          else
          {
            _MergeClear();
          }
        }
        else
        {
          foreach (var item in newCollection)
          {
            var mergableItem = (IMergeable<T>)item;

            int index = _FindIndex(0, p => p.FKID == mergableItem.FKID);
            if (index == -1)
            {
              index = _FindInsertionPoint(item);

              if (-1 == index)
              {
                _items.Add(item);
              }
              else
              {
                _items.Insert(index, item);
              }
              _SafeAddNotify(item);
            }
            else
            {
              ((IMergeable<T>)_items[index]).Merge(item);
            }
          }
        }

        //Assert.Implies(_areItemsComparable || _customComparison != null, () => _items.AreSorted(_customComparison));
      }
    }

    private void _RemoveRange(int index)
    {
      while (index < _items.Count)
      {
        _SafeRemoveNotify(_items[index]);
        _items.RemoveAt(index);
      }
    }

    private void _SimpleMerge(IEnumerable<T> newCollection, bool add)
    {
      lock (SyncRoot)
      {
        if (!add)
        {
          // This just replaces the entire collection.
          _MergeClear();

          if (newCollection == null)
          {
            return;
          }

          foreach (T item in _itemComparer.OrderedList(newCollection))
          {
            _items.Add(item);
            _SafeAddNotify(item);
          }
        }
        else
        {
          foreach (var item in newCollection)
          {
            if (!_items.Contains(item))
            {
              int index = _FindInsertionPoint(item);

              if (-1 == index)
              {
                _items.Add(item);
              }
              else
              {
                _items.Insert(index, item);
              }
              _SafeAddNotify(item);
            }
          }
        }
        //Assert.Implies(_areItemsComparable || _customComparison != null, () => _items.AreSorted(_customComparison));
      }
    }

    private void _SafeAddNotify(T item)
    {
      Assert.IsNotNull(item);
      if (_areItemsNotifiable)
      {
        ((INotifyPropertyChanged)item).PropertyChanged += _OnItemChanged;
      }
    }

    private void _SafeRemoveNotify(T item)
    {
      Assert.IsNotNull(item);
      if (_areItemsNotifiable)
      {
        ((INotifyPropertyChanged)item).PropertyChanged -= _OnItemChanged;
      }
    }

    private void _OnItemChanged(object sender, PropertyChangedEventArgs e)
    {
      var item = sender as T;
      if (item != null)
      {
        lock (SyncRoot)
        {
          int currentIndex = _items.IndexOf(item);
          if (-1 != currentIndex)
          {
            if (!_VerifyInsertionPoint(currentIndex))
            {
              _items.RemoveAt(currentIndex);
              int newIndex = _FindInsertionPoint(item);
              if (newIndex == -1 || newIndex == _items.Count)
              {
                _items.Add(item);
              }
              else
              {
                _items.Insert(newIndex, item);
              }
            }
          }

          //Assert.IsTrue(_items.AreSorted(_customComparison));
        }
      }
    }

    #region IList<T> Members

    public int IndexOf(T item) { return _items.IndexOf(item); }

    public T this[int index]
    {
      get { return _items[index]; }
      set { throw new NotSupportedException(); }
    }

    #region Unsupported Mutable IList<T> Members
    void IList<T>.Insert(int index, T item) { throw new NotSupportedException(); }
    void IList<T>.RemoveAt(int index) { throw new NotSupportedException(); }
    #endregion

    #endregion

    #region ICollection<T> Members

    public bool Contains(T item) { return _items.Contains(item); }
    public void CopyTo(T[] array, int arrayIndex) { _items.CopyTo(array, arrayIndex); }
    public int Count => _items.Count;
    public bool IsReadOnly => true;

    public void Clear() { Merge(null, false); }
        
    public void Add(T item)
    { 
      Verify.IsNotNull(item, "item");
      Merge(new [] { item }, true);
    }

    public bool Remove(T item)
    { 
      Verify.IsNotNull(item, "item");

      lock (SyncRoot)
      {
        return _items.Remove(item);
      }
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator() { return _items.GetEnumerator(); }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

    #endregion

    #region INotifyCollectionChanged Members

    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add { _items.CollectionChanged += value; }
      remove { _items.CollectionChanged -= value; }
    }

    #endregion
  }
}


