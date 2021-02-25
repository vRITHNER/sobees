#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Twitter.Cls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class SettingsViewModel : BTwitterViewModel, IBSettingsViewModel
  {
    #region Fields

    private int _maxTweets = 200;
    private string _newSpam;
    private int _rpp = 100;
    private bool _showApiUsage;
    private double _slDmValue;

    private double _slFriendsValue;
    private double _slListValue;
    private double _slRepliesValue;
    private double _slUserValue;

    private ObservableCollection<string> _spams = new ObservableCollection<string>();

    private bool _viewRrafIcon;
    private bool _viewState;
    private bool _viewStateTweets;

    // private bool _showFactery;

    #endregion

    #region Properties

    private bool _showDMHome;
    private bool _showRepliesHome;

    public double SlFriendsValue
    {
      get
      {
        if (_slFriendsValue.Equals(0))
          _slFriendsValue = DoubleValueAttribute.GetDoubleValue(EnumTwitterType.Friends);

        return _slFriendsValue;
      }
      set
      {
        _slFriendsValue = value;
        IsDirty = true;
      }
    }

    public double SlListValue
    {
      get
      {
        if (_slListValue.Equals(0))
          _slListValue = DoubleValueAttribute.GetDoubleValue(EnumTwitterType.List);

        return _slListValue;
      }
      set
      {
        _slListValue = value;
        IsDirty = true;
      }
    }

    public double SlRepliesValue
    {
      get
      {
        if (_slRepliesValue.Equals(0))
          _slRepliesValue = DoubleValueAttribute.GetDoubleValue(EnumTwitterType.Replies);

        return _slRepliesValue;
      }
      set
      {
        _slRepliesValue = value;
        IsDirty = true;
      }
    }

    public double SlDmsValue
    {
      get
      {
        if (_slDmValue.Equals(0))
          _slDmValue = DoubleValueAttribute.GetDoubleValue(EnumTwitterType.DirectMessages);

        return _slDmValue;
      }
      set
      {
        _slDmValue = value;
        IsDirty = true;
      }
    }

    public double SlUserValue
    {
      get
      {
        if (_slUserValue.Equals(0))
          _slUserValue = DoubleValueAttribute.GetDoubleValue(EnumTwitterType.User);

        return _slUserValue;
      }
      set
      {
        _slUserValue = value;
        IsDirty = true;
      }
    }

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

    public bool ShowRepliesHome
    {
      get { return _showRepliesHome; }
      set
      {
        _showRepliesHome = value;
        IsDirty = true;
      }
    }

    public bool ShowDMHome
    {
      get { return _showDMHome; }
      set
      {
        IsDirty = true;
        _showDMHome = value;
      }
    }

    public bool ViewRrafIcon
    {
      get { return _viewRrafIcon; }
      set
      {
        _viewRrafIcon = value;
        IsDirty = true;
      }
    }

    //public bool ShowFactery
    //{
    //  get { return _showFactery; }
    //  set
    //  {
    //    _showFactery = value;
    //    IsDirty = true;
    //  }
    //}

    public bool ShowApiUsage
    {
      get { return _showApiUsage; }
      set
      {
        _showApiUsage = value;
        IsDirty = true;
      }
    }

    public bool ViewState
    {
      get { return _viewState; }
      set
      {
        _viewState = value;
        IsDirty = true;
      }
    }

    public bool ViewStateTweets
    {
      get { return _viewStateTweets; }
      set
      {
        _viewStateTweets = value;
        IsDirty = true;
      }
    }

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
        RaisePropertyChanged("NewSpam");
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:UcSettings HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    #endregion

    #region Commands

    public RelayCommand AddSpamCommand { get; private set; }

    public RelayCommand<string> DeleteSpamCommand { get; private set; }

    public RelayCommand SaveSettingsCommand { get; private set; }

    public RelayCommand CloseSettingsCommand { get; private set; }

    #endregion

    #region Constructors

    public SettingsViewModel(TwitterViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      InitCommands();
      InitFields();
    }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      AddSpamCommand = new RelayCommand(AddSpam, () => !string.IsNullOrEmpty(NewSpam));
      DeleteSpamCommand = new RelayCommand<string>(DeleteSpam);
      SaveSettingsCommand = new RelayCommand(() => MessengerInstance.Send("SaveSettingsTW"));
      CloseSettingsCommand = new RelayCommand(() => MessengerInstance.Send("CloseSettingsTW"));
      base.InitCommands();
    }

    private void InitFields()
    {
      try
      {
        IsDirty = false;
        _showApiUsage = Settings.ShowApiUsage;
        _rpp = Settings.NbPostToGet;
        _maxTweets = Settings.NbMaxPosts;
        _slRepliesValue = Settings.SlRepliesValue;
        _slDmValue = Settings.SlDmsValue;
        _slFriendsValue = Settings.SlFriendsValue;
        _slListValue = Settings.SlListValue;
        _slUserValue = Settings.SlUserValue;
        _showDMHome = Settings.ShowDMHome;
        _showRepliesHome = Settings.ShowRepliesHome;

        if (CurrentAccount.SpamList != null)
        {
          foreach (var spam in CurrentAccount.SpamList)
          {
            _spams.Add(spam);
          }
        }
        _viewState = Settings.ViewState;
        _viewStateTweets = Settings.ViewStateTweets;
        _viewRrafIcon = Settings.ViewRrafIcon;

        // _showFactery = Settings.ShowFactery;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
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

    #endregion

    #region member Interface

    public bool IsDirty { get; set; }

    public DataTemplate SettingsControl
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Settings HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
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

    public string Title
    {
      get { return _twitterViewModel.ServiceType; }
      set { throw new NotImplementedException(); }
    }

    public void CloseSettings()
    {
      throw new NotImplementedException();
    }

    public void SaveSettings()
    {
      throw new NotImplementedException();
    }

    #endregion member Interface
  }
}