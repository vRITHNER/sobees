#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Helpers;
using Sobees.Infrastructure.Model;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Infrastructure.ViewModelBase
{
  public abstract class BWorkspaceViewModel : BViewModelBase
  {
    #region Fields

    private Visibility _errorMsgVisibility = Visibility.Collapsed;
    private string _errorMsg;
    private DispatcherTimer _timerError;
    private DispatcherTimer _timer;
    private Visibility _newMessageVisibility = Visibility.Collapsed;
    private Visibility _closeButtonVisibility;
    private bool _isWaiting;
    private int _numberNewMessage;
    private Visibility _isAnyDataVisibility = Visibility.Visible;
    protected bool _isUpdating;

    #endregion // Fields

    #region Properties

    public string ErrorMsg
    {
      get => _errorMsg;
        set
      {
        _errorMsg = value;
        RaisePropertyChanged();
      }
    }

    public Visibility ErrorMsgVisibility
    {
      get => _errorMsgVisibility;
        set
      {
        _errorMsgVisibility = value;
        RaisePropertyChanged();

        if (value != Visibility.Visible) return;

        Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
                                                               {
                                                                 if (_timerError != null)
                                                                 {
                                                                   _timerError.Stop();
                                                                 }
                                                                 else
                                                                 {
                                                                   _timerError = new DispatcherTimer();
                                                                   _timerError.Tick += ((sender, e) =>
                                                                                          {
                                                                                            _timerError.Stop();
                                                                                            _timerError = null;
                                                                                            ErrorMsgVisibility = Visibility.Collapsed;
                                                                                          });
                                                                 }
                                                                 _timerError.Interval = TimeSpan.FromSeconds(BGlobals.DEFAULT_ERROR_VISIBILITY);

                                                                 _timerError.Start();
                                                               });
      }
    }

    public SobeesSettings SobeesSettings
    {
      get => SobeesSettingsLocator.SobeesSettingsStatic;
        set => SobeesSettingsLocator.SobeesSettingsStatic = null;
    }

    public string Filter => SobeesSettingsLocator.SobeesSettingsStatic.Filter;

    public virtual Visibility CloseButtonVisibility
    {
      get => _closeButtonVisibility;
        set
      {
        _closeButtonVisibility = value;
        RaisePropertyChanged();
      }
    }

    public Visibility NewMessageVisibility
    {
      get => _newMessageVisibility;
        set
      {
        _newMessageVisibility = value;
        RaisePropertyChanged();
      }
    }

    public virtual void ShowUserProfile(User user)
    {
    }

    public int NumberNewMessage
    {
      get
      {
        if (_numberNewMessage < 0) _numberNewMessage = 0;
        return _numberNewMessage;
      }
      set
      {
        NewMessageVisibility = value == 0 ? Visibility.Collapsed : Visibility.Visible;
        _numberNewMessage = value;
        RaisePropertyChanged();
      }
    }

    public bool IsWaiting
    {
      get => _isWaiting;
        set
      {
        _isWaiting = value;
        RaisePropertyChanged();
      }
    }

    public bool IsClosed { get; set; }

    public bool IsMaxiMize { get; set; }

    public bool IsMaxiMizeOld;

    public BPosition PositionInGrid { get; set; }

    public Visibility IsAnyDataVisibility
    {
      get => _isAnyDataVisibility;
        set
      {
        _isAnyDataVisibility = value;
        RaisePropertyChanged();
      }
    }

    public virtual DataTemplate FrontTemplateView { get; set; }

    public virtual BWorkspaceViewModel FrontViewModel { get; set; }

    public virtual DataTemplate MainTemplateView { get; set; }

    public virtual BWorkspaceViewModel MainViewModel { get; set; }

    public virtual DataTemplate ProfileTemplate { get; set; }

    public virtual BWorkspaceViewModel ProfileView { get; set; }

    #endregion

    #region Constructor

    protected BWorkspaceViewModel()
    {
      //set initial value of BisNetworkAvailable
      BisNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
    }

    protected BWorkspaceViewModel(BPosition position)
      : this()
    {
      PositionInGrid = position;
    }

    protected void Dispose(bool disposing)
    {
      DeleteTimer();
      base.Cleanup();
      ProcessHelper.PerformAggressiveCleanup();
    }

    #endregion // Constructor

    #region CloseCommand

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to remove this workspace from the user interface.
    /// </summary>
    public ICommand CloseCommand { get; protected set; }

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to maximize the control.
    /// </summary>
    public RelayCommand MaximizeCommand { get; protected set; }

    public RelayCommand<string> GoToWebCommand { get; protected set; }

    public RelayCommand PopupOpenCommand { get; protected set; }

    public RelayCommand PopupCloseCommand { get; protected set; }

    #endregion // CloseCommand

    #region Methods

    /// <summary>
    ///   Used when a string arrived into the Messenger
    /// </summary>
    /// <param name="param">A string that represents the fonction to execute.</param>
    public override void DoAction(string param)
    {
      switch (param)
      {
        case "SettingsUpdated":
          OnSettingsUpdated();
          break;

        case "Online":
          BisNetworkAvailable = true;
          EndUpdateAll();
          Refresh();
          break;

        case "Offline":
          BisNetworkAvailable = false;
          EndUpdateAll();
          break;

        default:
          break;
      }
      base.DoAction(param);
    }

#if !SILVERLIGHT

    /// <summary>
    ///   Send the news messages to display the alerts.
    /// </summary>
    /// <param name="name">Name of the Account who have new Messages. For exemple twitter account</param>
    /// <param name="newEntries">The list of the new message</param>
    public void ShowAlerts(string name, List<Entry> newEntries)
    {
      Messenger.Default.Send(new AlertsInfo { NewEntries = newEntries, type = AccountType });
    }

#endif

    protected virtual void OnSettingsUpdated()
    {
    }

    public virtual void UpdateAll()
    {
      BLogManager.LogEntry(APPNAME, "UpdateAll", "START", true);
    }

    /// <summary>
    ///   Refresh
    /// </summary>
    public void Refresh()
    {
      try
      {
        if (_timer != null)
        {
          var interval = (int)GetRefreshTime();
          if (_timer.Interval.Minutes != interval)
          {
            _timer.Interval = new TimeSpan(0, interval, 0);
            StopTimer();
            StartTimer();
          }
        }
        else
        {
          InitializeTimer();
        }

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

                               if (CanUpdateAll())
                                 Application.Current.Dispatcher.BeginInvokeIfRequired(UpdateAll);
                             };

          //worker.RunWorkerCompleted += delegate { Application.Current.Dispatcher.BeginInvokeIfRequired(EndUpdateAll); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        EndUpdateAll();
      }
    }

    public virtual bool CanUpdateAll()
    {
      if (!NetworkHelper.GetStatus())
        return false;

      if (_isUpdating)
        return false;

      StartWaiting();
      _isUpdating = true;
      return true;
    }

    public const string APPNAME = "BWorkspaceViewModel";

    public virtual void EndUpdateAll()
    {
      BLogManager.LogEntry(APPNAME, "UpdateAll", "END", true);
      _isUpdating = false;
      StopWaiting();
    }

    public virtual void StartWaiting()
    {
      IsWaiting = true;
    }

    public virtual void StopWaiting()
    {
      IsWaiting = false;
    }

    protected override void InitCommands()
    {
      if (GoToWebCommand == null)
        GoToWebCommand = new RelayCommand<string>(GoToweb);

      PopupOpenCommand = new RelayCommand(() => { VisibilityPopup = Visibility.Visible; });

      PopupCloseCommand = new RelayCommand(() =>
                                             {
                                               PopupMessageContentTw = string.Empty;
                                               PopupMessageContentFb = string.Empty;
                                               VisibilityPopup = Visibility.Collapsed;
                                             });

      base.InitCommands();
    }

    public virtual string GetSettings()
    {
      return null;
    }

    public virtual void SetSettings(string serializedSettings)
    {
    }

    #region PopupWindow

    private Visibility _visibilityPopup = Visibility.Collapsed;

    public Visibility VisibilityPopup
    {
      get => _visibilityPopup;
        set
      {
        _visibilityPopup = value;
        RaisePropertyChanged();
      }
    }

    private string _popupMessagecontentFb = "";

    public string PopupMessageContentFb
    {
      get => _popupMessagecontentFb;
        set
      {
        _popupMessagecontentFb = value;

        RaisePropertyChanged();
      }
    }

    private string _popupMessagecontentTw = "";

    public string PopupMessageContentTw
    {
      get => _popupMessagecontentTw;
        set
      {
        _popupMessagecontentTw = value;

        RaisePropertyChanged();
      }
    }

    private bool _messagePopupIsOpen;
    private bool _messagePopupIsClose;

    public bool MessagePopupIsOpen
    {
      get => _messagePopupIsOpen;
        set
      {
        _messagePopupIsOpen = value;
        RaisePropertyChanged();
      }
    }

    public bool MessagePopupIsClosed
    {
      get => _messagePopupIsClose;
        set
      {
        _messagePopupIsClose = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Timer Methods

    public virtual double GetRefreshTime()
    {
      return BGlobals.DEFAULT_REFRESH_TIME_SERVICE;
    }

    public void InitializeTimer()
    {
      if (!UseTimer) return; //TODO check if need this line
      if (_timer != null)
      {
        var interval = (int)GetRefreshTime();
        if (_timer.Interval.Minutes != interval)
        {
          _timer.Interval = new TimeSpan(0, interval, 0);
        }
        StopTimer();
        StartTimer();
        return;
      }

      _timer = new DispatcherTimer();
      SetTimerInterval(GetRefreshTime());
      _timer.Tick += delegate { Refresh(); };
      StartTimer();
      TraceHelper.Trace(this,
        $"Timer initialized and started to run every {GetRefreshTime()} minutes.");
    }

    public virtual bool UseTimer => true;

    public void TimerTick(object sender, EventArgs e)
    {
      Refresh();
    }


    public void SetTimerInterval(DispatcherTimer timer, int newIntervalInMinute)
    {
      if (timer != null)
      {
        timer.Interval = new TimeSpan(0, newIntervalInMinute, 0);

        TraceHelper.Trace(this,
          $"{ToString()} -> new Timer interval -> {newIntervalInMinute} minutes.");
      }
    }


    public void SetTimerInterval(double newIntervalInMinute)
    {
      if (_timer != null)
      {
        _timer.Interval = new TimeSpan(0, (int)newIntervalInMinute, 0);
      }
      else
      {
        InitializeTimer();
      }
      TraceHelper.Trace(this, $"{ToString()} -> new Timer interval -> {newIntervalInMinute} minutes.");
    }

    public void StartTimer(DispatcherTimer timer)
    {
      timer?.Start();

      TraceHelper.Trace(this, $"{ToString()} -> Timer started!");
    }

    public void StartTimer()
    {
      _timer?.Start();

      //TraceHelper.Trace(this, string.Format("{0} -> Timer started!", ToString()));
    }

    public void StopTimer()
    {
      _timer?.Stop();

      //TraceHelper.Trace(this, string.Format("{0} -> Timer stopped!", ToString()));
    }

    public void DeleteTimer()
    {
      if (_timer != null)
      {
        _timer.Tick -= TimerTick;
        StopTimer();
        _timer = null;
      }
    }

    #endregion

    public virtual void UpdateView()
    {
    }

    #region utilities

    protected static void GoToweb(string url)
    {
      if (string.IsNullOrEmpty(url)) return;
      if (url.Contains("<a href"))
      {
        url = url.Split('"')[1];
      }
      if (!string.IsNullOrEmpty(url)) WebHelper.NavigateToUrl(url);
    }

    #endregion

    #endregion

    public virtual void ShowUserProfile(string user)
    {
    }

    public virtual void ShowHideErrorMsg(string errorMsg = null)
    {
      //  errorMsg = "An error occured, please try again later.";
      ErrorMsg = errorMsg;
      ErrorMsgVisibility = string.IsNullOrEmpty(errorMsg) ? Visibility.Collapsed : Visibility.Visible;
    }

    public static bool BisNetworkAvailable { get; set; }
  }
}