#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Util;

#endregion

namespace Sobees
{
  /// <summary>
  ///   Interaction logic for mainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow() :
      base("MainWindow", true, null)
    {
      GetInstance = this;
      InitializeComponent();
      Loaded += MainWindowLoaded;
    }

    public static MainWindow GetInstance { get; private set; }

    #region Override

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == Key.F4)
        Messenger.Default.Send("ShowMultiPost");

      base.OnKeyDown(e);
    }

    protected override void OnStateChanged(EventArgs e)
    {
      try
      {
        switch (WindowState)
        {
          case WindowState.Maximized:
            biAlert.IconViewModel.TextMinimizeMenu =
              new LocText("Sobees.Configuration.BGlobals:Resources:trayMinimizeToTray").ResolveLocalizedValue();
            break;

          case WindowState.Minimized:
            if (BisMinimizeWindowInTray || BisMinimizeWindowOnceInTray) Visibility = Visibility.Collapsed;
            biAlert.IconViewModel.TextMinimizeMenu =
              new LocText("Sobees.Configuration.BGlobals:Resources:trayRestorebDule").ResolveLocalizedValue();
            biAlert.IconViewModel.IconText =
              new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Hidden").
                ResolveLocalizedValue(); //Properties.Resources.SystemTrayIconTooltip_Hidden;
            break;

          case WindowState.Normal:
            biAlert.IconViewModel.TextMinimizeMenu =
              new LocText("Sobees.Configuration.BGlobals:Resources:trayMinimizeToTray").ResolveLocalizedValue();
            biAlert.IconViewModel.IconText =
              new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Normal").
                ResolveLocalizedValue();
            break;
        }

        biAlert.IconViewModel.TextOpenMultiPost =
          new LocText("Sobees.Configuration.BGlobals:Resources:JumpListPostStatus").ResolveLocalizedValue();
        biAlert.IconViewModel.TextShowMenu =
          new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Normal").ResolveLocalizedValue();

        base.OnStateChanged(e);

        //
        UpdateFrame();
        Messenger.Default.Send(new BMessage("StateChanged", WindowState));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                            ex);
      }
    }

    #endregion

    #region Disabled UI Automation

    // Trying desperately to turn off UI Automation because it destroys
    // performance on touch enabled machines.

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return new FakeWindowsPeer(this);
    }

    private class FakeWindowsPeer : WindowAutomationPeer
    {
      public FakeWindowsPeer(Window window)
        : base(window)
      {
      }

      protected override List<AutomationPeer> GetChildrenCore()
      {
        return null;
      }
    }

    #endregion

    #region IsAppRunningInClickOnceMode

    /// <summary>
    ///   IsAppRunningInClickOnceMode Dependency Property
    /// </summary>
    public static readonly DependencyProperty IsAppRunningInClickOnceModeProperty =
      DependencyProperty.Register("IsAppRunningInClickOnceMode",
                                  typeof(bool),
                                  typeof(MainWindow),
                                  new FrameworkPropertyMetadata(false,
                                                                FrameworkPropertyMetadataOptions.None,
                                                                OnIsAppRunningInClickOnceModeChanged));

    /// <summary>
    ///   Gets or sets the IsAppRunningInClickOnceMode property.  This dependency property
    ///   indicates ....
    /// </summary>
    public bool IsAppRunningInClickOnceMode
    {
      get { return (bool)GetValue(IsAppRunningInClickOnceModeProperty); }
      set
      {
        SetValue(IsAppRunningInClickOnceModeProperty,
                 value);
      }
    }

    /// <summary>
    ///   Handles changes to the IsAppRunningInClickOnceMode property.
    /// </summary>
    private static void OnIsAppRunningInClickOnceModeChanged(DependencyObject d,
                                                             DependencyPropertyChangedEventArgs e)
    {
      ((MainWindow)d).OnIsAppRunningInClickOnceModeChanged(e);
    }

    /// <summary>
    ///   Provides derived classes an opportunity to handle changes to the IsAppRunningInClickOnceMode property.
    /// </summary>
    protected virtual void OnIsAppRunningInClickOnceModeChanged(DependencyPropertyChangedEventArgs e)
    {
      TraceHelper.Trace(this,
                        string.Format("OnIsAppRunningInClickOnceModeChanged::NewValue:{0}",
                                      e.NewValue));
    }

    #endregion

    private void MainWindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        // When the window loses focus take it as an opportunity to trim our workingset.
        Deactivated += (o, ee) => ProcessHelper.PerformAggressiveCleanup();
        Activated += (o, ee) => Messenger.Default.Send("ActivateWindows");

        UpdateFrame();

        Loaded -= MainWindowLoaded;
        Title = string.Format("{0} - Version {1}",
                              Title,
                              AssemblyHelper.GetEntryAssemblyVersion());
        InitDragDrop();
        OnStateChanged(null); //force to change the localized text in tray
        InitMemoryCleanupTimer();
        Messenger.Default.Register<string>(this, DoAction);
        Messenger.Default.Send("WindowLoaded");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
    }

    private void InitMemoryCleanupTimer()
    {
      //
    }

    private void DoAction(string param)
    {
      switch (param)
      {
        case "ShowSobees":
          MenuShowSobees(this, null);
          break;

        case "MinimizeSobeesToTray":
          MenuMinimizeToTray(this, null);
          break;

        case "ExitSobees":
          MenuCloseSobees(this, null);
          break;

        case "CloseSaveSettings":
          OnStateChanged(null); //force to change the localized text in tray
          RefreshSettings();
          break;

        case "SettingsRestored":
          RefreshSettings();
          break;
      }
    }

    private void RefreshSettings()
    {
      try
      {
        BisMinimizeWindowInTray = SobeesSettingsLocator.SobeesSettingsStatic.MinimizeWindowInTray;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    private void MenuShowSobees(MainWindow mainWindow, object o)
    {
      ShowAndActivateWindow();
    }

    private void MenuMinimizeToTray(MainWindow mainWindow, object o)
    {
      try
      {
        if (WindowState != WindowState.Minimized)
        {
          BisMinimizeWindowOnceInTray = true;
          WindowState = WindowState.Minimized;
        }
        else
          ShowAndActivateWindow();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
    }

    private void ShowAndActivateWindow()
    {
      try
      {
        Show();
        WindowState = WindowState.Normal;
        BisMinimizeWindowOnceInTray = false;
        Activate();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
                          (ex));
      }
    }

    private void MenuCloseSobees(MainWindow mainWindow, object o)
    {
      Close();
    }

    private void InitDragDrop()
    {
      DragEnter += (o,
                    de) =>
                     {
                       de.Effects = DragDropEffects.None;
                       DropTargetHelper.DragEnter(null, de.Data, de.GetPosition(this), DragDropEffects.None);
                       de.Handled = true;
                       base.OnDragEnter(de);
                     };

      DragOver += (o,
                   de) =>
                    {
                      de.Effects = DragDropEffects.None;
                      DropTargetHelper.DragOver(de.GetPosition(this), DragDropEffects.None);
                      de.Handled = true;
                      base.OnDragOver(de);
                    };

      DragLeave += (o,
                    de) =>
                     {
                       DropTargetHelper.DragLeave(de.Data);
                       de.Handled = true;
                       base.OnDragLeave(de);
                     };
    }

    private const int MARGIN_MOUSE_HANDLER_WIDTH = 6;

    private void UpdateFrame()
    {
      switch (WindowState)
      {
        ////TODO:Fine tune ...
        case WindowState.Maximized:
          grMain.Margin = new Thickness(-ResizeMargin.Left / 2 + MARGIN_MOUSE_HANDLER_WIDTH, -2, -ResizeMargin.Right / 2 + MARGIN_MOUSE_HANDLER_WIDTH, -ResizeMargin.Bottom / 2 + MARGIN_MOUSE_HANDLER_WIDTH);
          break;

        case WindowState.Normal:
          grMain.Margin = new Thickness(-ResizeMargin.Left / 2 + 2, -2, -ResizeMargin.Right / 2 + 2, -ResizeMargin.Bottom / 2 + 2);
          break;
      }
    }
  }
}