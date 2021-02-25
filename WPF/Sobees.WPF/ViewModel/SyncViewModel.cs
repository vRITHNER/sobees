using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.ViewModelBase;
#if !SILVERLIGHT
using Sobees.BDataService;

#endif

namespace Sobees.ViewModel
{
  public class SyncViewModel : BWorkspaceViewModel
  {
    #region Fields

    private Visibility _mainVisibility = Visibility.Visible;
    private Visibility _waitingVisibility = Visibility.Collapsed;
#if !SILVERLIGHT
    private ServiceUser_SUS _user;
#endif
    private Visibility _connectedVisibility = Visibility.Collapsed;

    #endregion

    #region Properties

    public Visibility WaitingVisibility
    {
      get => _waitingVisibility;
        set
      {
        _waitingVisibility = value;
        RaisePropertyChanged();
      }
    }

    public Visibility ConnectedVisibility
    {
      get => _connectedVisibility;
        set
      {
        _connectedVisibility = value;
        RaisePropertyChanged();
      }
    }

#if !SILVERLIGHT
    public ServiceUser_SUS User
    {
      get => _user;
        set
      {
        _user = value;
        if (_user != null)
        {
          UserName = _user.nickName_SUS;
        }
        RaisePropertyChanged();
      }
    }
#endif

    private string _userName;

    public string UserName
    {
      get => _userName;
        set
      {
        _userName = value;
        RaisePropertyChanged();
      }
    }

    public Visibility MainVisibility
    {
      get => _mainVisibility;
        set
      {
        _mainVisibility = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        string dt =
          "<DataTemplate x:Name='dtSyncView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Sync.Views;assembly=Sobees' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:UcSync/> " +
          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        XmlReader xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
    }

    #endregion

    #region Commands

    public RelayCommand CreateAccountCommand { get; set; }
    public RelayCommand ConnectAccountCommand { get; set; }
    public RelayCommand GoBackCommand { get; set; }
    public RelayCommand VerifyConnectionCommand { get; set; }

    #endregion

    #region Constructors

    public SyncViewModel()
    {
      InitCommands();
      if (!string.IsNullOrEmpty(SobeesSettings.SyncUser))
      {
        UserName = SobeesSettings.SyncUser;
        IsConnected();
      }
    }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      CloseCommand = new RelayCommand(() => Messenger.Default.Send("CloseSync"));
      CreateAccountCommand = new RelayCommand(Connect);
      ConnectAccountCommand = new RelayCommand(Connect);
      VerifyConnectionCommand = new RelayCommand(VerifyConnection);
      GoBackCommand = new RelayCommand(GoBack);
      base.InitCommands();
    }

    private void VerifyConnection()
    {
      //User = SynchronizationHelper.GetUserFromSobeesToken();
      if (User == null) return;
      SobeesSettings.SyncUser = User.id_SUS.ToString();
      IsConnected();
      Messenger.Default.Send("CloseSync");
      Refresh();
    }

    private void IsConnected()
    {
      MainVisibility = Visibility.Collapsed;
      WaitingVisibility = Visibility.Collapsed;
      ConnectedVisibility = Visibility.Visible;
    }

    private void Connect()
    {
      StartWaiting();
      //   SynchronizationHelper.Connect();
      MainVisibility = Visibility.Collapsed;
      WaitingVisibility = Visibility.Visible;
    }

    private void GoBack()
    {
      StopWaiting();
      MainVisibility = Visibility.Visible;
      WaitingVisibility = Visibility.Collapsed;
    }


    public override void UpdateAll()
    {
      //SynchronizationHelper.GetLastUpdate();
      EndUpdateAll();
    }

    #endregion
  }
}