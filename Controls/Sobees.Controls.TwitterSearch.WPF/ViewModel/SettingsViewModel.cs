#region

using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.TwitterSearch.Cls;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.TwitterSearch.ViewModel
{
  public class SettingsViewModel : BWorkspaceViewModel, IBSettingsViewModel
  {
    private readonly TwitterSearchViewModel _model;

    public SettingsViewModel(TwitterSearchViewModel model, Messenger messenger)
    {
      MessengerInstance = messenger;
      _model = model;
      InitCommands();
      InitFields();
    }

    #region Commands

    public RelayCommand SaveSettingsCommand { get; private set; }
    public RelayCommand CloseSettingsCommand { get; private set; }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      SaveSettingsCommand = new RelayCommand(() => MessengerInstance.Send("SaveSettingsTS"));
      CloseSettingsCommand = new RelayCommand(() => MessengerInstance.Send("CloseSettingsTS"));
      base.InitCommands();
    }

    private void InitFields()
    {
      try
      {
        IsDirty = false;
        _viewStateTweets = Settings.ViewStateTweets;
        _viewRrafIcon = Settings.ViewRrafIcon;
        _slRefreshTimeFF = Settings.RefreshTimeFF;
        _slRefreshTimeOR = Settings.RefreshTimeOR;
        _slRefreshTimeTS = Settings.RefreshTimeTS;
        //_isUseFactery = Settings.ShowFactery;
        //_isUseFriendFeed = Settings.ShowFriendFeed;
        //_isUseOneRiot = Settings.ShowOneRiot;
        _isUseTwitterSearch = Settings.ShowTwitter;
        _rpp = Settings.NbPostToGet;
        _maxTweet = Settings.NbMaxPosts;
        _isUseFacebook = Settings.ShowFacebook;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #region Fields

    private bool _isUseFacebook;
    private bool _isUseTwitterSearch;
    private int _maxTweet = BGlobals.DEFAULT_NB_POST_TO_KEEP;
    private int _rpp = BGlobals.DEFAULT_NB_POST_TO_GET_Twitter;
    private double _slRefreshTimeFF;
    private double _slRefreshTimeOR;
    private double _slRefreshTimeTS;
    private bool _viewRrafIcon;
    private bool _viewStateTweets;

    #endregion

    #region Properties

    public TwitterSearchSettings Settings => _model.Settings as TwitterSearchSettings;

    public override DataTemplate DataTemplateView
    {
      get
      {
        var dt =
          "<DataTemplate x:Name='dtGroupsView' " +
          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
          "xmlns:Views='clr-namespace:Sobees.Infrastructure.Controls;assembly=Sobees.Infrastructure'>" +
          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
          "<Views:UcSettings HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
          "</Grid>" +
          "</DataTemplate>";

#if SILVERLIGHT
        return XamlReader.Load(dt) as DataTemplate;
#else
        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
#endif
      }
    }

    public bool IsUseTwitterSearch
    {
      get => _isUseTwitterSearch;
      set
      {
        _isUseTwitterSearch = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    //public bool IsUseFactery
    //{
    //  get { return _isUseFactery; }
    //  set
    //  {
    //    _isUseFactery = value;
    //    RaisePropertyChanged("IsUseFactery");
    //    IsDirty = true;
    //  }
    //}
    //public bool IsUseOneRiot
    //{
    //  get { return _isUseOneRiot; }
    //  set
    //  {
    //    _isUseOneRiot = value;
    //    RaisePropertyChanged("IsUseOneRiot");
    //    IsDirty = true;
    //  }
    //}

    public bool IsUseFacebook
    {
      get => _isUseFacebook;
      set
      {
        _isUseFacebook = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    //public bool IsUseFriendFeed
    //{
    //  get { return _isUseFriendFeed; }
    //  set
    //  {
    //    _isUseFriendFeed = value;
    //    RaisePropertyChanged("IsUseFriendFeed");
    //    IsDirty = true;
    //  }
    //}

    public bool ViewStateTweets
    {
      get => _viewStateTweets;
      set
      {
        _viewStateTweets = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    public double SlRefreshTimeFF
    {
      get => _slRefreshTimeFF;
      set
      {
        _slRefreshTimeFF = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    public double SlRefreshTimeOR
    {
      get => _slRefreshTimeOR;
      set
      {
        _slRefreshTimeOR = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    public double SlRefreshTimeTS
    {
      get => _slRefreshTimeTS;
      set
      {
        _slRefreshTimeTS = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    public int Rpp
    {
      get => _rpp;
      set
      {
        _rpp = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }

    public int MaxTweet
    {
      get => _maxTweet;
      set
      {
        _maxTweet = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
    }


    public bool ViewRrafIcon
    {
      get => _viewRrafIcon;
      set
      {
        _viewRrafIcon = value;
        RaisePropertyChanged();
        IsDirty = true;
      }
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
                          "xmlns:Views='clr-namespace:Sobees.Controls.TwitterSearch.Views;assembly=Sobees.Controls.TwitterSearch'>" +
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
      get => _model.ServiceType;
      set => throw new NotImplementedException();
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