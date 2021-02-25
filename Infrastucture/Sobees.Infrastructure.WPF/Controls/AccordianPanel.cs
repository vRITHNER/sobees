using System;
using System.Windows;
using System.Windows.Controls;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// AccordianPanel
  /// The arrangement of children is similar to DockPanel
  /// </summary>
  public class AccordianPanel : Panel
  {
    public static readonly DependencyProperty ChildToFillProperty = DependencyProperty.Register(
      "ChildToFill", typeof (UIElement), typeof (AccordianPanel),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Gets/Sets Which child to fill the rest space
    /// </summary>
    public UIElement ChildToFill
    {
      get { return (UIElement) GetValue(ChildToFillProperty); }
      set { SetValue(ChildToFillProperty, value); }
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      UIElementCollection internalChildren = InternalChildren;

      int count = internalChildren.Count;

      // If ChildToFill is not specified, set it to the last child
      int childToFillIndex = ChildToFill == null ? count - 1 : internalChildren.IndexOf(ChildToFill);

      double y = 0.0;

      var rectForFill = new Rect(0, 0, arrangeSize.Width, arrangeSize.Height);

      if (childToFillIndex != -1)
      {
        // Arrange elements before ChildToFill in sequence
        for (int i = 0; i < childToFillIndex + 1; i++)
        {
          UIElement element = internalChildren[i];
          if (element != null)
          {
            Size desiredSize = element.DesiredSize;
            var finalRect = new Rect(0, y, Math.Max(0.0, arrangeSize.Width), Math.Max(0.0, arrangeSize.Height - y));
            if (i < childToFillIndex)
            {
              finalRect.Height = desiredSize.Height;
              y += desiredSize.Height;
              element.Arrange(finalRect);
            }
            else
            {
              // The rect for the rest of children
              rectForFill = finalRect;
            }
          }
        }

        y = 0.0;

        // Arrange the elements after ChildToFill in negative sequence（Including ChildToFill）
        for (int i = count - 1; i > childToFillIndex; i--)
        {
          UIElement element = internalChildren[i];
          if (element != null)
          {
            Size desiredSize = element.DesiredSize;
            var finalRect = new Rect(0, arrangeSize.Height - y - desiredSize.Height, Math.Max(0.0, arrangeSize.Width),
                                     Math.Max(0.0, desiredSize.Height));

            element.Arrange(finalRect);
            y += desiredSize.Height;
          }
        }
        rectForFill.Height -= y;
        InternalChildren[childToFillIndex].Arrange(rectForFill);
      }

      return arrangeSize;
    }

    // Modified from DockPanel
    protected override Size MeasureOverride(Size constraint)
    {
      UIElementCollection internalChildren = InternalChildren;
      double num = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      int index = 0;
      int count = internalChildren.Count;

      while (index < count)
      {
        UIElement element = internalChildren[index];
        if (element != null)
        {
          var availableSize = new Size(Math.Max(0.0, (constraint.Width - num3)),
                                       Math.Max(0.0, (constraint.Height - num4)));
          element.Measure(availableSize);
          Size desiredSize = element.DesiredSize;

          num = Math.Max(num, num3 + desiredSize.Width);
          num4 += desiredSize.Height;
        }
        index++;
      }
      num = Math.Max(num, num3);
      return new Size(num, Math.Max(num2, num4));
    }
  }
}