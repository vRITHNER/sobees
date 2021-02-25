#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.Facebook.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  public class SettingsViewModel : BSettingsViewModel
  {
    protected SettingsViewModel(Messenger messenger)
      : base(messenger)
    {
      InitCommands();
      InitFields();
    }

    public SettingsViewModel(BServiceWorkspaceViewModel model, Messenger messenger)
      : base(messenger)
    {
      _model = model;
      Settings = model.Settings as FacebookSettings;
      InitCommands();
      InitFields();
    }

    #region Fields

    private int _maxTweets = BGlobals.DEFAULT_NB_POST_TO_KEEP;
    private string _newSpam;
    private double _refreshTime;
    private int _rpp = BGlobals.DEFAULT_NB_POST_TO_GET;

    #endregion

    #region Properties

    private readonly BServiceWorkspaceViewModel _model;
    private ObservableCollection<string> _spams = new ObservableCollection<string>();

    public ObservableCollection<string> Spams
    {
      get { return _spams; }
      set { _spams = value; }
    }

    public string NewSpam
    {
      get { return _newSpam; }
      set
      {
        _newSpam = value;
        RaisePropertyChanged();

      }
    }

    public double RefreshTime
    {
      get
      {
        if (_refreshTime.Equals(0))
          _refreshTime = BGlobals.DEFAULT_REFRESH_TIME;

        return _refreshTime;
      }
      set
      {
        _refreshTime = value;
        IsDirty = true;
      }
    }

    public FacebookSettings Settings { get; set; }

    public int MaxTweets
    {
      get { return _maxTweets; }
      set
      {
        _maxTweets = value;
        IsDirty = true;
      }
    }

    public int Rpp
    {
      get { return _rpp; }
      set
      {
        _rpp = value;
        IsDirty = true;
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:UcSettings HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    #endregion

    #region Commands

    public RelayCommand SaveSettingsCommand { get; private set; }
    public RelayCommand CloseSettingsCommand { get; private set; }
    public RelayCommand AddSpamCommand { get; private set; }
    public RelayCommand<string> DeleteSpamCommand { get; private set; }

    #endregion

    #region Constructors

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      SaveSettingsCommand = new RelayCommand(() => MessengerInstance.Send("SaveSettingsFB"));
      CloseSettingsCommand = new RelayCommand(() => MessengerInstance.Send("CloseSettingsFB"));
      AddSpamCommand = new RelayCommand(AddSpam, () => !string.IsNullOrEmpty(NewSpam));
      DeleteSpamCommand = new RelayCommand<string>(DeleteSpam);
      base.InitCommands();
    }

    private void AddSpam()
    {
      if (!Spams.Contains(NewSpam))
      {
        Spams.Add(NewSpam);
      }
      NewSpam = string.Empty;
      IsDirty = true;
    }

    private void DeleteSpam(string spam)
    {
      Spams.Remove(spam);
      IsDirty = true;
    }

    private void InitFields()
    {
      try
      {
        IsDirty = false;
        _rpp = Settings.NbPostToGet;
        _maxTweets = Settings.NbMaxPosts;
        _refreshTime = Settings.RefreshTime;
        if (
          SobeesSettings.Accounts[
            SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList !=
          null)
        {
          foreach (
            var spam in
              SobeesSettings.Accounts[
                SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList)
          {
            Spams.Add(spam);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #region member Interface

    public bool IsDirty { get; set; }

    public string Title
    {
      get { return _model.ServiceType; }
      set { throw new NotImplementedException(); }
    }

    public DataTemplate SettingsControl
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Settings HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set { }
    }


    public BWorkspaceViewModel CurrentViewModel
    {
      get { return this; }
      set { }
    }

    #endregion member Interface
  }
}