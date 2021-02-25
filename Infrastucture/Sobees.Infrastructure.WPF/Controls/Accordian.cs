using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Accordian
  /// </summary>
  public class Accordian : ItemsControl
  {
    #region ExpandedItem

    // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ExpandedItemProperty = DependencyProperty.Register(
      "ExpandedItem", typeof (object), typeof (Accordian),
      new UIPropertyMetadata(null, OnExpandedItemChanged));



    /// <summary>
    /// Gets/Sets which item to expand
    /// </summary>
    public object ExpandedItem
    {
      get { return GetValue(ExpandedItemProperty); }
      set { SetValue(ExpandedItemProperty, value); }
    }

    private static void OnExpandedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var shelf = sender as Accordian;
      if (shelf != null)
      {
        shelf.OnExpandedItemChanged(e.OldValue, e.NewValue);
      }
    }

    protected virtual void OnExpandedItemChanged(object oldValue, object newValue)
    {
      var oldItem = ItemContainerGenerator.ContainerFromItem(oldValue) as AccordianItem;

      if (oldItem != null)
      {
        oldItem.IsExpanded = false;
      }
    }

    #endregion

    #region Source

    // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source", typeof(object), typeof(Accordian),
      new UIPropertyMetadata(null, OnSourceChanged));



    /// <summary>
    /// Gets/Sets which item to expand
    /// </summary>
    public object Source
    {
      get { return GetValue(SourceProperty); }
      set { SetValue(SourceProperty, value); }
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var self = sender as Accordian;
      self.Items.Clear();
      var items = e.NewValue as List<AccordianItem>;
      foreach (var item in items)
      {
        self.Items.Add(item);
      }

    }

    protected virtual void OnSourceChanged(object oldValue, object newValue)
    {
    }

    #endregion

    #region Constructors

    static Accordian()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof (Accordian), new FrameworkPropertyMetadata(typeof (Accordian)));
    }

    #endregion

    #region Overrides

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new AccordianItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is AccordianItem;
    }

    #endregion
  }
}