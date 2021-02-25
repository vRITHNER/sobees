using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// AccordianItem
  /// </summary>
  public class AccordianItem : HeaderedContentControl
  {
    #region IsExpanded

    // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
      "IsExpanded", typeof (bool), typeof (AccordianItem),
      new PropertyMetadata(false, OnIsExpandedChanged));

    public bool IsExpanded
    {
      get { return (bool) GetValue(IsExpandedProperty); }
      set { SetValue(IsExpandedProperty, value); }
    }

    private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var item = sender as AccordianItem;
      if (item != null)
      {
        item.OnIsExpandedChanged(e);
      }
    }

    protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
    {
      var newValue = (bool) e.NewValue;

      if (newValue)
      {
        OnExpanded();
      }
      else
      {
        OnCollapsed();
      }
    }

    #endregion

    #region Expand Events

    public static RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
      "Collapsed", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AccordianItem));

    public static RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
      "Expanded", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (AccordianItem));

    /// <summary>
    /// Raised when selected
    /// </summary>
    public event RoutedEventHandler Expanded
    {
      add { AddHandler(ExpandedEvent, value); }
      remove { RemoveHandler(ExpandedEvent, value); }
    }

    /// <summary>
    /// Raised when unselected
    /// </summary>
    public event RoutedEventHandler Collapsed
    {
      add { AddHandler(CollapsedEvent, value); }
      remove { RemoveHandler(CollapsedEvent, value); }
    }

    protected virtual void OnExpanded()
    {
      Accordian parentAccordian = ParentAccordian;
      if (parentAccordian != null)
      {
        parentAccordian.ExpandedItem = this;
      }
      RaiseEvent(new RoutedEventArgs(ExpandedEvent, this));
    }

    protected virtual void OnCollapsed()
    {
      RaiseEvent(new RoutedEventArgs(CollapsedEvent, this));
    }

    #endregion

    #region ExpandCommand

    public static RoutedCommand ExpandCommand = new RoutedCommand("Expand", typeof (AccordianItem));

    private static void OnExecuteExpand(object sender, ExecutedRoutedEventArgs e)
    {
      var item = sender as AccordianItem;
      if (item != null)
        if (!item.IsExpanded)
        {
          item.IsExpanded = true;
        }
    }

    private static void CanExecuteExpand(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = sender is AccordianItem;
    }

    #endregion

    #region ParentAccordian

    private Accordian ParentAccordian => ItemsControl.ItemsControlFromItemContainer(this) as Accordian;

    #endregion

    #region Constructor

    static AccordianItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof (AccordianItem),
                                               new FrameworkPropertyMetadata(typeof (AccordianItem)));

      var expandCommandBinding = new CommandBinding(ExpandCommand, OnExecuteExpand, CanExecuteExpand);
      CommandManager.RegisterClassCommandBinding(typeof (AccordianItem), expandCommandBinding);
    }

    #endregion
  }
}