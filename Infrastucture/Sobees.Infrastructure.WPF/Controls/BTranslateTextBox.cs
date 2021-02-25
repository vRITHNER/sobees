using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Sobees.Tools.Logging;
using Sobees.Tools.Web;

namespace Sobees.Infrastructure.Controls
{
  public abstract class BTranslateTextBox:TextBlock
  {
    #region Private fields

    #endregion

    #region Properties

    #endregion

    #region Dependency properties

    public static readonly DependencyProperty BTextProperty =
      DependencyProperty.Register("BText", typeof(string), typeof(BTranslateTextBox),
                                  new FrameworkPropertyMetadata(string.Empty, BTextChanged));
    public static readonly DependencyProperty IsTranslatedProperty =
      DependencyProperty.Register("IsTranslated", typeof(bool), typeof(BTranslateTextBox),
                                  new FrameworkPropertyMetadata(false, IsTranslatedChanged));

    public bool IsTranslated
    {
      get { return (bool)GetValue(IsTranslatedProperty); }
      set { SetValue(IsTranslatedProperty, value); }
    }

    public string BText
    {
      get { return (string)GetValue(BTextProperty); }
      set { SetValue(BTextProperty, value); }
    }


    #endregion

    #region Constructors

    /// <summary>
    /// property change event handler for BindingProperty
    /// </summary>
    public static void IsTranslatedChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BTranslateTextBox)depObj).OnIsTranslatedChanged(e);
    }

    /// <summary>
    /// property change event handler for BindingProperty
    /// </summary>
    public static void BTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BTranslateTextBox)depObj).OnTextChangedChanged(e);
    }

    protected abstract void OnTextChangedChanged(DependencyPropertyChangedEventArgs e);
    protected void OnIsTranslatedChanged(DependencyPropertyChangedEventArgs e)
    {
      OnTextChangedChanged(e);
    }

    #endregion

    #region Regular Hyperlink Click

    public static void HyperLinkUnloaded(object sender, RoutedEventArgs e)
    {
      var hyperLink = sender as Hyperlink;
      if (hyperLink == null) return;
      hyperLink.Unloaded -= HyperLinkUnloaded;
      hyperLink.Click -= RequestNavigate;
    }

    public static void RequestNavigate(object sender, RoutedEventArgs e)
    {
      try
      {
        WebHelper.NavigateToUrl(((Hyperlink)sender).NavigateUri.ToString());
      }
      catch (Exception ex)
      {
        if (sender != null)
          TraceHelper.Trace(sender.ToString(), ex);
        else
          TraceHelper.Trace("BTextBlock::RequestNavigate::", ex);
      }
    }

    #endregion

    #region Methods

    public abstract void DoStuffOnText(BTranslateTextBox block, string text);

    protected string CleanUrl(string displayLink)
    {
      if (!char.IsLetterOrDigit(displayLink[0]))
      {
        displayLink = displayLink.Remove(0, 1);
      }
      if (!char.IsLetterOrDigit(displayLink[displayLink.Length - 1]))
      {
        displayLink = displayLink.Remove(displayLink.Length - 1, 1);
      }
      return displayLink.Replace(";", "").Replace(",", "");
    }

    public abstract void FinalizeWork(TextBlock block, List<Object> lst, string text);

    public abstract object DoStuffOnWord(string word, BTranslateTextBox block, string text, List<object> lst);

    #endregion

    #region Events

    #endregion
  }
}
