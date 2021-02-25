#region

using System;
using System.Windows;
using System.Windows.Media;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Helpers
{
  public class WPFHelper
  {
    /// <summary>
    ///   Return the parent of an element according to the type specified.
    /// </summary>
    /// <typeparam name = "T">The type of the parent element you want to find.</typeparam>
    /// <param name = "item">The element from which one you search for the parent.</param>
    /// <returns>The parent, in the visual tree, of the element, according to the specified type.</returns>
    public static T FindParentOfType<T>(DependencyObject item) where T : DependencyObject
    {
      var parent = VisualTreeHelper.GetParent(item);
      do
      {
        parent = VisualTreeHelper.GetParent(parent);
      } while (parent != null && parent.GetType() != typeof (T));

      return parent as T;
    }

    public static T FindUIParentOfType<T>(UIElement item) where T : DependencyObject
    {
      try
      {
        var element = item;
        do
        {
          element = VisualTreeHelper.GetParent(element) as UIElement;
        } while (element != null && element.GetType() != null && (element.GetType() != typeof (T) && element.GetType().BaseType != typeof (T)));

        return element as T;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("bTools.General.WPFHelper:FindUIParentOfType -> ", ex);
      }
      return null;
    }

    public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
    {
      for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        var child = VisualTreeHelper.GetChild(obj, i);
        if (child != null && child is childItem)
          return (childItem) child;
        var childOfChild = FindVisualChild<childItem>(child);
        if (childOfChild != null)
          return childOfChild;
      }
      return null;
    }
  }
}