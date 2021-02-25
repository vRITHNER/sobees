using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.ViewModelBase;
#if !SILVERLIGHT
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml;
using Sobees.Infrastructure.Cls;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Threading.Extensions;
using Sobees.Infrastructure.NotifyIcon;
using Sobees.Infrastructure.Controls.Alerts;
#endif

namespace Sobees.ViewModel
{

  public class IconViewModel : BWorkspaceViewModel
  {
    #region Fields


    private readonly List<UserControl> WaitAlertList;
#if !SILVERLIGHT
    private string _iconText = new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Normal").ResolveLocalizedValue(); //Properties.Resources.SystemTrayIconTooltip_Normal;
    private string _textShowMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayShowbDule").ResolveLocalizedValue(); //Properties.Resources.ShowSobees;
    private string _textMinimizeMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayMinimizeToTray").ResolveLocalizedValue(); //Properties.Resources.MinimizeinTray;
    private string _textExitMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayQuitbDule").ResolveLocalizedValue(); //Properties.Resources.ExitSobees;;
    private string _textOpenMultiPost = new LocText("Sobees.Configuration.BGlobals:Resources:JumpListPostStatus").ResolveLocalizedValue(); //Properties.Resources.ExitSobees;
    private TaskbarIcon _taskbarIcon;
#else
    private string _iconText;
    private string _textShowMenu;
    private string _textMinimizeMenu;
    private string _textExitMenu;
    private string _textOpenMultiPost;
#endif

    #endregion

    #region Properties

    public string IconText
    {
      get => _iconText;
        set
      {
        _iconText = value;
        RaisePropertyChanged();
      }
    }

    public string TextShowMenu
    {
      get => _textShowMenu;
        set
      {
        _textShowMenu = value;
        RaisePropertyChanged();
      }
    }

    public string TextMinimizeMenu
    {
      get => _textMinimizeMenu;
        set
      {
        _textMinimizeMenu = value;
        RaisePropertyChanged();
      }
    }

    public string TextExitMenu
    {
      get => _textExitMenu;
        set
      {
        _textExitMenu = value;
        RaisePropertyChanged();
      }
    }

    public string TextOpenMultiPost
    {
      get => _textOpenMultiPost;
        set
      {
        _textOpenMultiPost = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        var dt =
          "<DataTemplate x:Name='dtMainView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:NotifyIcon/> " +
          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
      set { base.DataTemplateView = value; }
    }

    #endregion

    #region Constructors

    public IconViewModel()
    {
      WaitAlertList = new List<UserControl>();
      Messenger.Default.Register<BMessage>(this, DoActionMessage);
      Messenger.Default.Register<BAlerteMessage>(this, DoActionAlerteMessage);
      InitCommands();
    }

    private void DoActionAlerteMessage(BAlerteMessage param)
    {
      switch (param.Action)
      {
#if !SILVERLIGHT
        case "ShowAlerts":
          ShowAlertesIcone(param.List, param.Name, param.Type);
          break;
#endif
      }
    }
#if !SILVERLIGHT
    private void ShowAlertesIcone(ICollection<Entry> collection, string name, EnumAccountType type)
    {
      ShowBalloon(collection.Count, name, type);
    }
#endif
    private void DoActionMessage(BMessage param)
    {
      switch (param.Action)
      {
#if !SILVERLIGHT
        case "StateChanged":
          StateChanged(param.Parameter is WindowState ? (WindowState)param.Parameter : WindowState.Normal);
          break;
#endif
      }
    }
    protected override void InitCommands()
    {
      base.InitCommands();
      DoubleClickCommand = new RelayCommand(DoubleClickIcone);
      ShowSobeesCommand = new RelayCommand(ShowSobees);
      MinimizeSobeesToTrayCommand = new RelayCommand(MinimizeSobeesToTray);
      ShowMultiPostCommand = new RelayCommand(OpenMultiPost);
      ExitSobeesCommand = new RelayCommand(ExitSobees);
    }
#if !SILVERLIGHT
    private void StateChanged(WindowState state)
    {
      switch (state)
      {
        case WindowState.Maximized:
          TextMinimizeMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayMinimizeToTray").ResolveLocalizedValue(); //Properties.Resources.MinimizeinTray;
          break;
        case WindowState.Minimized:
          TextMinimizeMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayRestorebDule").ResolveLocalizedValue(); //Properties.Resources.RestoreSobees;
          IconText = new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Hidden").ResolveLocalizedValue(); //Properties.Resources.SystemTrayIconTooltip_Hidden;
          break;
        case WindowState.Normal:
          TextMinimizeMenu = new LocText("Sobees.Configuration.BGlobals:Resources:trayMinimizeToTray").ResolveLocalizedValue(); //Properties.Resources.MinimizeinTray;
          IconText = new LocText("Sobees.Configuration.BGlobals:Resources:traySystemTrayIconTooltip_Normal").ResolveLocalizedValue(); //Properties.Resources.SystemTrayIconTooltip_Normal;
          break;
      }
    }
#endif
    #endregion

    #region Commands

    public RelayCommand DoubleClickCommand { get; set; }
    private BRelayCommand _setTaskbarIconCommand;

    public BRelayCommand SetTaskbarIconCommand
    {
      get
      {
        if (_setTaskbarIconCommand == null)
        {
          _setTaskbarIconCommand = new BRelayCommand(SetTaskbarIcon);
        }
        return _setTaskbarIconCommand;
      }
    }

    public RelayCommand ShowSobeesCommand { get; set; }
    public RelayCommand MinimizeSobeesToTrayCommand { get; set; }
    public RelayCommand ShowMultiPostCommand { get; set; }
    public RelayCommand ExitSobeesCommand { get; set; }

    #endregion

    private static void ShowSobees()
    {
      Messenger.Default.Send("ShowSobees");
    }

    private static void DoubleClickIcone()
    {
      Messenger.Default.Send("ShowSobees");
    }

    private static void MinimizeSobeesToTray()
    {
      Messenger.Default.Send("MinimizeSobeesToTray");
    }

    private void OpenMultiPost()
    {
      Messenger.Default.Send("ShowMultiPost");
    }

    private void ExitSobees()
    {
      Messenger.Default.Send("ExitSobees");
    }

    private void SetTaskbarIcon(object param)
    {
      object[] objs = BRelayCommand.CheckParams(param);
      if (objs == null || objs[1] == null) return;
#if !SILVERLIGHT
      _taskbarIcon = objs[1] as TaskbarIcon;
#endif
    }

#if !SILVERLIGHT
    public void ShowBalloon(int nb)
    {
      if (_taskbarIcon == null) return;
      Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
      {
        var alert = new AlertBalloon(nb);
        WaitAlertList.Add(alert);
        if (WaitAlertList.Count == 1)
        {
          _taskbarIcon.ShowCustomBalloon(alert,
                                         PopupAnimation.Fade,
                                         4000);
        }
      });
    }

    public void ShowBalloon(int nb, string name, EnumAccountType type)
    {
      if (_taskbarIcon == null) return;


      using (var worker = new BackgroundWorker())
      {

        worker.DoWork += delegate(object s,
                                       DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
          {
            var alert = new AlertBalloon(nb, name, type);
            WaitAlertList.Add(alert);
            if (WaitAlertList.Count == 1)
            {
              _taskbarIcon.ShowCustomBalloon(alert,
                                             PopupAnimation.Fade,
                                             4000);
            }
            alert.Unloaded += AlertUnloaded;
            alert.MouseLeftButtonUp += AlertMouseLeftButtonUp;
          });


        };

        worker.RunWorkerAsync();
      }

    }

    private void AlertMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Application.Current.Dispatcher.BeginInvokeIfRequired(ShowSobees);
    }

    private void AlertUnloaded(object sender, RoutedEventArgs e)
    {

      using (var worker = new BackgroundWorker())
      {

        worker.DoWork += delegate(object s,
                                       DoWorkEventArgs args)
        {
          if (worker.CancellationPending)
          {
            args.Cancel = true;
            return;
          }

          Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                                 {
                                                                   var alert = sender as UserControl;
                                                                   if (alert != null)
                                                                   {
                                                                     alert.Unloaded -= AlertUnloaded;
                                                                     alert.MouseLeftButtonUp -= AlertMouseLeftButtonUp;
                                                                     WaitAlertList.Remove(alert);
                                                                   }
                                                                   if (WaitAlertList.Count > 0)
                                                                   {
                                                                     if (_taskbarIcon != null)
                                                                       _taskbarIcon.ShowCustomBalloon(WaitAlertList[0],
                                                                                                      PopupAnimation.
                                                                                                        Fade,
                                                                                                      4000);
                                                                   }
                                                                 });

        };

        worker.RunWorkerAsync();

      }

    }
#endif
  }
}

