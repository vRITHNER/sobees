#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Sobees.Tools.Helpers;

#endregion

namespace Sobees.Infrastructure.Controls
{
  public class BTextBoxWatermarked : TextBox
  {
    #region Properties

    // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelProperty =
      DependencyProperty.Register("Label",
                                  typeof (string),
                                  typeof (BTextBoxWatermarked),
                                  new UIPropertyMetadata("Label"));

    // Using a DependencyProperty as the backing store for LabelStyle.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelStyleProperty =
      DependencyProperty.Register("LabelStyle",
                                  typeof (Style),
                                  typeof (BTextBoxWatermarked),
                                  new UIPropertyMetadata(null));


    private static readonly DependencyPropertyKey HasTextPropertyKey =
      DependencyProperty.RegisterReadOnly("HasText",
                                          typeof (bool),
                                          typeof (BTextBoxWatermarked),
                                          new PropertyMetadata(false));

    public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

    public string Label
    {
      get { return (string) GetValue(LabelProperty); }
      set { SetValue(LabelProperty, value); }
    }

    public Style LabelStyle
    {
      get { return (Style) GetValue(LabelStyleProperty); }
      set { SetValue(LabelStyleProperty, value); }
    }

    public bool HasText
    {
      get { return (bool) GetValue(HasTextProperty); }
      private set { SetValue(HasTextPropertyKey, value); }
    }

    #endregion

    private AdornerLabel myAdornerLabel;
    private AdornerLayer myAdornerLayer;

    public BTextBoxWatermarked()
      : base()
    {
      if (BDesignerHelper.IsInDesignModeStatic)
        return;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      //myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
      myAdornerLabel = new AdornerLabel(this, Label, LabelStyle);
      UpdateAdorner(this);

      var focusProp = DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof (FrameworkElement));
      if (focusProp != null)
      {
        focusProp.AddValueChanged(this, delegate { UpdateAdorner(this); });
      }

      var containsTextProp = DependencyPropertyDescriptor.FromProperty(HasTextProperty, typeof (BTextBoxWatermarked));
      if (containsTextProp != null)
      {
        containsTextProp.AddValueChanged(this, delegate { UpdateAdorner(this); });
      }
    }

    protected override void OnTextChanged(TextChangedEventArgs e)
    {
      HasText = Text != "";

      base.OnTextChanged(e);
    }

    //protected override void OnDragEnter(DragEventArgs e)
    //{
    //  myAdornerLayer.RemoveAdorners<AdornerLabel>(this); // requires AdornerExtensions.cs

    //  base.OnDragEnter(e);
    //}

    //protected override void OnDragLeave(DragEventArgs e)
    //{
    //  UpdateAdorner(this);

    //  base.OnDragLeave(e);
    //}

    private void UpdateAdorner(FrameworkElement elem)
    {
      if (((BTextBoxWatermarked) elem).HasText || elem.IsFocused)
      {
        // Hide the Shadowed Label
        ToolTip = Label;
        //myAdornerLayer.RemoveAdorners<AdornerLabel>(elem); // requires AdornerExtensions.cs
      }
      else
      {
        // Show the Shadowed Label
        ToolTip = null;
        //if (!myAdornerLayer.Contains<AdornerLabel>(elem)) // requires AdornerExtensions.cs
        //  myAdornerLayer.Add(myAdornerLabel);
      }
    }
  }

  //public static class AdornerExtensions
  //{
  //  public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
  //  {
  //    var adorners = adr.GetAdorners(elem);

  //    if (adorners == null) return;

  //    for (var i = adorners.Length - 1; i >= 0; i--)
  //    {
  //      if (adorners[i] is T)
  //        adr.Remove(adorners[i]);
  //    }
  //  }

  //  public static bool Contains<T>(this AdornerLayer adr, UIElement elem)
  //  {
  //    var adorners = adr.GetAdorners(elem);

  //    if (adorners == null) return false;

  //    for (var i = adorners.Length - 1; i >= 0; i--)
  //    {
  //      if (adorners[i] is T)
  //        return true;
  //    }
  //    return false;
  //  }

  //  public static void RemoveAll(this AdornerLayer adr, UIElement elem)
  //  {
  //    try
  //    {
  //      var adorners = adr.GetAdorners(elem);

  //      if (adorners == null) return;

  //      foreach (var toRemove in adorners)
  //        adr.Remove(toRemove);
  //    }
  //    catch
  //    {
  //    }
  //  }

  //  public static void RemoveAllRecursive(this AdornerLayer adr, UIElement element)
  //  {
  //    try
  //    {
  //      Action<UIElement> recurse = null;
  //      recurse = ((Action<UIElement>) delegate(UIElement elem)
  //                                       {
  //                                         adr.RemoveAll(elem);
  //                                         if (elem is Panel)
  //                                         {
  //                                           foreach (UIElement e in ((Panel) elem).Children)
  //                                             recurse(e);
  //                                         }
  //                                         else if (elem is Decorator)
  //                                         {
  //                                           recurse(((Decorator) elem).Child);
  //                                         }
  //                                         else if (elem is ContentControl)
  //                                         {
  //                                           if (((ContentControl) elem).Content is UIElement)
  //                                             recurse(((ContentControl) elem).Content as UIElement);
  //                                         }
  //                                       });

  //      recurse(element);
  //    }
  //    catch
  //    {
  //    }
  //  }
  //}
}