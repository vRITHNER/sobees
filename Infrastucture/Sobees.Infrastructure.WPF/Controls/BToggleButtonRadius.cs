#region

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#endregion

namespace Sobees.Infrastructure.Controls
{
  public class BToggleButtonRadius : ToggleButton
  {
    #region DependencyProperty declarations

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius",
                                                                                                 typeof (CornerRadius),
                                                                                                 typeof(BToggleButtonRadius),
                                                                                                 null);

    public static readonly DependencyProperty Content2Property = DependencyProperty.Register("Content2",
                                                                                             typeof (object),
                                                                                             typeof(BToggleButtonRadius),
                                                                                             new PropertyMetadata(null));

    public static readonly DependencyProperty HorizontalContent2AlignmentProperty =
      DependencyProperty.Register("HorizontalContent2Alignment",
                                  typeof (HorizontalAlignment),
                                  typeof(BToggleButtonRadius),
                                  null);

    public static readonly DependencyProperty VerticalContent2AlignmentProperty =
      DependencyProperty.Register("VerticalContent2Alignment",
                                  typeof (VerticalAlignment),
                                  typeof(BToggleButtonRadius),
                                  null);

    public static readonly DependencyProperty MarginContent2Property = DependencyProperty.Register("MarginContent2",
                                                                                                   typeof (Thickness),
                                                                                                   typeof(BToggleButtonRadius
                                                                                                     ),
                                                                                                   null);

    public static readonly DependencyProperty ContentTemplate2Property = DependencyProperty.Register("ContentTemplate2",
                                                                                                     typeof (
                                                                                                       DataTemplate),
                                                                                                     typeof (
                                                                                                       BToggleButtonRadius),
                                                                                                     null);

    #endregion

    #region Properties

    [Category("Seb"), Description("Represents the radii of a border's corners. The radii cannot be negative.")]
    public CornerRadius CornerRadius
    {
      get { return (CornerRadius) GetValue(CornerRadiusProperty); }
      set
      {
        SetValue(CornerRadiusProperty,
                 value);
      }
    }

    [Category("Content 2"),
     Description(
       "The Content property of this element can be set to any arbitrary value (including simple string values).")]
    public object Content2
    {
      get { return GetValue(Content2Property); }
      set
      {
        SetValue(Content2Property,
                 value);
      }
    }

    [Category("Content 2"), Description("Gets or sets the horizontal alignment of an element's content.")]
    public HorizontalAlignment HorizontalContent2Alignment
    {
      get { return (HorizontalAlignment) GetValue(HorizontalContent2AlignmentProperty); }
      set
      {
        SetValue(HorizontalContent2AlignmentProperty,
                 value);
      }
    }

    [Category("Content 2"), Description("Gets or sets the vertical alignment of an element's content.")]
    public VerticalAlignment VerticalContent2Alignment
    {
      get { return (VerticalAlignment) GetValue(VerticalContent2AlignmentProperty); }
      set
      {
        SetValue(VerticalContent2AlignmentProperty,
                 value);
      }
    }

    [Category("Content 2"), Description("Gets or sets the outer margin of an element.")]
    public Thickness MarginContent2
    {
      get { return (Thickness) GetValue(MarginContent2Property); }
      set
      {
        SetValue(MarginContent2Property,
                 value);
      }
    }

    [Category("Content 2"), Description("DataTemplate")]
    public DataTemplate ContentTemplate2
    {
      get { return (DataTemplate) GetValue(ContentTemplate2Property); }
      set
      {
        SetValue(ContentTemplate2Property,
                 value);
      }
    }

    #endregion
  }
}