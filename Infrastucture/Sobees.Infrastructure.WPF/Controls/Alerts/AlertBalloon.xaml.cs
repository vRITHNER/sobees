using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GenericLib.NotifyIcon;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.NotifyIcon;

namespace Sobees.Infrastructure.Controls.Alerts
{
  /// <summary>
  /// Interaction logic for AlertBalloon.xaml
  /// </summary>
  public partial class AlertBalloon : UserControl
  {
    private bool isClosing;

    public AlertBalloon()
    {
      InitializeComponent();


#if SILVERLIGHT
      if (DesignerProperties.IsInDesignTool)
        return;
#else
      if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        return;
#endif

      TaskbarIcon.AddBalloonClosingHandler(this,
                                           OnBalloonClosing);
    }

    public AlertBalloon(int nb)
    {
      InitializeComponent();

#if SILVERLIGHT
      if (DesignerProperties.IsInDesignTool)
        return;
#else
      if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        return;
#endif

      TaskbarIcon.AddBalloonClosingHandler(this,
                                           OnBalloonClosing);
      lblNumber.Text = nb.ToString();
      //if (collection.Count == 1 || collection.Count > 1)
      //{
      //  lblUser1.Text = collection[0].User.NickName;
      //  lblTitle1.Text = collection[0].Title;
      //}
      //if (collection.Count == 2 || collection.Count > 2)
      //{
      //  lblUser2.Text = collection[1].User.NickName;
      //  lblTitle2.Text = collection[1].Title;
      //}
      //if (collection.Count == 3 || collection.Count > 3)
      //{
      //  lblUser3.Text = collection[2].User.NickName;
      //  lblTitle3.Text = collection[2].Title;
      //}
    }

    public AlertBalloon(int nb, string name, EnumAccountType type)
    {
      InitializeComponent();

#if SILVERLIGHT
      if (DesignerProperties.IsInDesignTool)
        return;
#else
      if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        return;
#endif

      TaskbarIcon.AddBalloonClosingHandler(this,
                                           OnBalloonClosing);
      lblAccount.Text = name;
      lblNumber.Text = nb.ToString();
      //if (collection.Count == 1 || collection.Count > 1)
      //{
      //  lblUser1.Text = collection[0].User.NickName;
      //  lblTitle1.Text = collection[0].Title;
      //}
      //if (collection.Count == 2 || collection.Count > 2)
      //{
      //  lblUser2.Text = collection[1].User.NickName;
      //  lblTitle2.Text = collection[1].Title;
      //}
      //if (collection.Count == 3 || collection.Count > 3)
      //{
      //  lblUser3.Text = collection[2].User.NickName;
      //  lblTitle3.Text = collection[2].Title;
      //}
      if (type == EnumAccountType.Twitter)
      {
        imgTwitter.Visibility = Visibility.Visible;
      }
      else if (type == EnumAccountType.Facebook)
      {
        imgFacebook.Visibility = Visibility.Visible;
      }
      else if (type == EnumAccountType.TwitterSearch)
      {
        imgTwitterSearch.Visibility = Visibility.Visible;
      }
      if (nb > 1)
      {
        lblMessage.Text = "messages ";
      }
    }

    /// <summary>
    /// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
    /// and setting the "Handled" property to true, we suppress the popup
    /// from being closed in order to display the fade-out animation.
    /// </summary>
    private void OnBalloonClosing(object sender, RoutedEventArgs e)
    {
      e.Handled = true;
      isClosing = true;
    }


    /// <summary>
    /// Resolves the <see cref="TaskbarIcon"/> that displayed
    /// the balloon and requests a close action.
    /// </summary>
    private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
    {
      //the tray icon assigned this attached property to simplify access
      TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
      taskbarIcon.CloseBalloon();
    }

    /// <summary>
    /// If the users hovers over the balloon, we don't close it.
    /// </summary>
    private void grid_MouseEnter(object sender, MouseEventArgs e)
    {
      //if we're already running the fade-out animation, do not interrupt anymore
      //(makes things too complicated for the sample)
      if (isClosing) return;

      //the tray icon assigned this attached property to simplify access
      TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
      taskbarIcon.ResetBalloonCloseTimer();
    }


    /// <summary>
    /// Closes the popup once the fade-out animation completed.
    /// The animation was triggered in XAML through the attached
    /// BalloonClosing event.
    /// </summary>
    private void OnFadeOutCompleted(object sender, EventArgs e)
    {
      var pp = (Popup) Parent;
      pp.IsOpen = false;
    }

    private void imgClose_Click(object sender, RoutedEventArgs e)
    {
      //the tray icon assigned this attached property to simplify access
      TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
      taskbarIcon.CloseBalloon();
    }

    private void grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      //the tray icon assigned this attached property to simplify access
      TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
      taskbarIcon.CloseBalloon();
    }

  }
}