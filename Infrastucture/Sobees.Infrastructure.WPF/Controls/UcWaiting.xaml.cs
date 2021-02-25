#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  ///   Interaction logic for UcWaiting.xaml
  /// </summary>
  public partial class UcWaiting
  {
    #region Fields

    private readonly Storyboard _animateStoryboard;
    private readonly Storyboard _hideStoryboard;
    private readonly Storyboard _showStoryboard;

    #endregion

    #region Properties

    // Using a DependencyProperty as the backing store for IsAnimating.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsAnimatingProperty =
      DependencyProperty.Register("IsAnimating",
                                  typeof(bool),
                                  typeof(UcWaiting),
                                  new PropertyMetadata(false,
                                                       (s,
                                                        e) =>
                                                       {
                                                         var sender = s as UcWaiting;
                                                         if (sender != null)
                                                         {
                                                           if ((bool)e.NewValue)
                                                             sender.Run();
                                                           else
                                                             sender.Stop();
                                                         }
                                                       }));

    public bool IsAnimating
    {
      get { return (bool)GetValue(IsAnimatingProperty); }
      set
      {
        SetValue(IsAnimatingProperty,
                 value);
      }
    }

    #endregion

    #region Constructors

    public UcWaiting()
    {
      InitializeComponent();

      if (DesignerProperties.GetIsInDesignMode(this))
        return;

      _showStoryboard = Resources["ShowStoryboard"] as Storyboard;
      _hideStoryboard = Resources["HideStoryboard"] as Storyboard;
      _animateStoryboard = Resources["AnimateStoryboard"] as Storyboard;

      if (_animateStoryboard == null
          || _showStoryboard == null
          || _hideStoryboard == null)
        throw new Exception("Storyboard not found");

      _hideStoryboard.Completed += HideStoryboardCompleted;
    }

    #endregion

    #region Methods

    public void Run()
    {
      try
      {
        _animateStoryboard.Begin();
        _showStoryboard.Begin();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    public void Stop()
    {
      try
      {
        _hideStoryboard.Begin();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          ex);
      }
    }

    #endregion

    #region Events

    private void HideStoryboardCompleted(object sender,
                                         EventArgs e)
    {
      _showStoryboard.Stop();
      _hideStoryboard.Stop();
      _animateStoryboard.Stop();
    }

    #endregion
  }
}