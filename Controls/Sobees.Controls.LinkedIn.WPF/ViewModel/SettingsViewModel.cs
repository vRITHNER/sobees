using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;

namespace Sobees.Controls.LinkedIn.ViewModel
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
      Settings = model.Settings as LinkedInSettings;
      InitCommands();
      InitFields();
    }

    #region Fields

    private int _maxTweets = BGlobals.DEFAULT_NB_POST_TO_KEEP;
    private double _refreshTime;
    private int _rpp = BGlobals.DEFAULT_NB_POST_TO_GET;

    #endregion Fields

    #region Properties

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

    public LinkedInSettings Settings { get; set; }

    public int MaxTweets
    {
      get { return _maxTweets; }
      set
      {
        _maxTweets = value;
        IsDirty = true;
      }
    }

    public string Title
    {
      get { return "LinkedIn"; }
      set { throw new NotImplementedException(); }
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

    #endregion Properties

    #region Commands

    public RelayCommand SaveSettingsCommand { get; private set; }

    public RelayCommand CloseSettingsCommand { get; private set; }

    #endregion Commands

    #region Constructors

    #endregion Constructors

    #region Methods

    protected override void InitCommands()
    {
      SaveSettingsCommand = new RelayCommand(() => MessengerInstance.Send("SaveSettingsLI"));
      CloseSettingsCommand = new RelayCommand(() => MessengerInstance.Send("CloseSettingsLI"));
      base.InitCommands();
    }

    private void InitFields()
    {
      try
      {
        IsDirty = false;
        _rpp = Settings.NbPostToGet;
        _maxTweets = Settings.NbMaxPosts;
        _refreshTime = Settings.RefreshTime;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion Methods

    #region member Interface

    public bool IsDirty { get; set; }

    public DataTemplate SettingsControl
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn'>" +
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