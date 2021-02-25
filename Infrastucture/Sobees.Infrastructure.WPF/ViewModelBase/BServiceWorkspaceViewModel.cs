#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Settings;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.ViewModelBase
{
  public class BServiceWorkspaceViewModel : BWorkspaceViewModel, IXmlSerializable
  {
    #region Fields

    public bool IsServiceWorkspace = true;
    private Visibility _closeControlVisibility = Visibility.Collapsed;
    private DataTemplate _dataTemplateView;

    private string _filterText;

    private bool _isSpecialZoneOpen;
    private bool _isStatusZoneOpen;
    private Messenger _serviceMessenger;
    private Visibility _settingsButtonVisibility;
    private string _status;

    #endregion

    #region Properties

    public override bool UseTimer => false;

    public override string DisplayName
    {
      get
      {
        var displayName = Settings != null ? Settings.UserName : base.DisplayName;
        if (BServiceWorkspace != null && string.IsNullOrEmpty(displayName))
          return BServiceWorkspace.DisplayName;

        return displayName;
      }
    }

    public virtual string ServiceType => BServiceWorkspace != null ? BServiceWorkspace.DisplayName : base.DisplayName;

    public string ServiceImageUrl => BServiceWorkspace?.Img;

    public string Status
    {
      get => _status;
      set
      {
        _status = value;
        RaisePropertyChanged();
        PostNewStatusCommand.RaiseCanExecuteChanged();
      }
    }

    public bool IsStatusZoneOpen
    {
      get => _isStatusZoneOpen;
      set
      {
        _isStatusZoneOpen = value;
        if (_isStatusZoneOpen)
        {
          Status = string.Empty;
        }

        if (string.IsNullOrEmpty(Status))
        {
          // Focus HACK
          Status = "Focus";
          Status = string.Empty;
        }

        RaisePropertyChanged();
        PostNewStatusCommand.RaiseCanExecuteChanged();
      }
    }

    public bool IsSpecialZoneOpen
    {
      get => _isSpecialZoneOpen;
      set
      {
        _isSpecialZoneOpen = value;
        RaisePropertyChanged();
        PostNewStatusCommand.RaiseCanExecuteChanged();
      }
    }

    public bool IsSpellCheckActivated
    {
      get => Settings != null && Settings.IsSpellCheckActivated;
      set
      {
        Settings.IsSpellCheckActivated = value;
        RaisePropertyChanged();
      }
    }

    public string FilterText
    {
      get => _filterText;
      set
      {
        _filterText = value;
        RaisePropertyChanged();
        PostNewStatusCommand.RaiseCanExecuteChanged();
      }
    }

    public virtual object MenuItems
    {
      get
      {
        var binding = new Binding("Test") {Source = this};
        var item = new ToggleButton {Content = "_1", Command = ShowSettingsCommand};
        item.SetBinding(ToggleButton.IsCheckedProperty, binding);
        var lstItem = new List<ToggleButton>
                      {
                        item,
                        new ToggleButton {Content = "_2"},
                        new ToggleButton {Content = "_3"},
                        new ToggleButton {Content = "_4"}
                      };

        var lst = new List<AccordianItem>
                  {
                    new AccordianItem
                    {
                      Header = new TextBlock {Text = "Menu 1"},
                      Content = new ListBox {ItemsSource = lstItem}
                    },
                    new AccordianItem
                    {
                      Header = new TextBlock {Text = "Menu 1"},
                      Content = new ListBox {ItemsSource = lstItem}
                    }
                  };

        return lst;
      }
    }

    public bool IsBtnStatuszoneOpen => true;

    public virtual bool IsRefreshVisible => true;

    public bool IsBtnSpecialzoneOpen => false;
    public BSettingsControlBase Settings { get; set; }
    public string ControlSettings { get; set; }
    public BServiceWorkspace BServiceWorkspace { get; set; }
    public bool IsSettingsOpen { get; set; }
    public virtual string ImageUser { get; set; }

    protected Messenger ServiceMessenger => _serviceMessenger ?? (_serviceMessenger = new Messenger());

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtDefault' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:ViewsBase='clr-namespace:Sobees.Infrastructure.ViewsBase;assembly=Sobees.Infrastructure'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<ViewsBase:ViewBase HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);

        return _dataTemplateView ?? (_dataTemplateView = XamlReader.Load(xmlReader) as DataTemplate);
      }
      set => base.DataTemplateView = value;
    }

    public virtual Visibility SettingsButtonVisibility
    {
      get => _settingsButtonVisibility;
      set
      {
        _settingsButtonVisibility = value;
        RaisePropertyChanged();
      }
    }

    public virtual Visibility CloseControlVisibility
    {
      get => _closeControlVisibility;
      set
      {
        _closeControlVisibility = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Constructors

    public BServiceWorkspaceViewModel()
    {
    }

    public BServiceWorkspaceViewModel(BPosition position, BServiceWorkspace serviceWorkspace)
      : this(position, serviceWorkspace, null)
    {
    }

    public BServiceWorkspaceViewModel(BPosition position, BServiceWorkspace serviceWorkspace,
                                      string settings)
      : base(position)
    {
      Messenger.Default.Register<BMessage>(this, DoActionMessage);
      Messenger.Default.Register<string>(this, DoAction);
      BServiceWorkspace = serviceWorkspace;
      ControlSettings = settings;
      IsMaxiMize = false;
    }

    #endregion

    #region Public Methods

    public virtual void DeleteUser(UserAccount account)
    {
    }

    #region ErrorMsg

    public void ShowErrorMsg(string errorMsg)
    {
      if (string.IsNullOrEmpty(errorMsg)) return;
      ErrorMsg = errorMsg;
      ErrorMsgVisibility = Visibility.Visible;
    }

    #endregion

    public virtual void SetAccount(UserAccount account)
    {
    }

    public virtual void RaiseStatusChanged()
    {
      // THIS METHOD CONCERN ONLY SILVERLIGHT -> check the "Status" Property
      PostNewStatusCommand.RaiseCanExecuteChanged();
    }

    #endregion

    #region Overrides

    public override void DoActionMessage(BMessage obj)
    {
      switch (obj.Action)
      {
        case "DeleteUser":
          DeleteUser(obj.Parameter as UserAccount);
          break;
      }
    }

    #endregion

    #region Commands

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to show settings for this control.
    /// </summary>
    public RelayCommand ShowSettingsCommand { get; private set; }

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to update the content of the control
    /// </summary>
    public RelayCommand RefreshCommand { get; private set; }

    /// <summary>
    ///   Returns the command that, when invoked, ask user for closing Control
    /// </summary>
    public RelayCommand CloseServiceCommand { get; protected internal set; }

    /// <summary>
    ///   Returns the command that, when invoked, ask user for closing Control
    /// </summary>
    public RelayCommand CancelCloseCommand { get; private set; }

    /// <summary>
    ///   Command who send the tweet
    /// </summary>
    public RelayCommand PostNewStatusCommand { get; private set; }

    /// <summary>
    ///   Command who cancel the tweet
    /// </summary>
    public RelayCommand PostCancelNewStatusCommand { get; set; }

    #endregion

    #region Methods

    /// <summary>
    ///   Request to save global settings
    /// </summary>
    public virtual bool UseAccount(UserAccount account)
    {
      return false;
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "SettingsUpdated":
          OnSettingsUpdated();
          break;
        case "GoOnline":
          BisNetworkAvailable = true;
          ServiceMessenger?.Send("Online");
          break;
        case "GoOffline":
          BisNetworkAvailable = false;
          ServiceMessenger?.Send("Offline");
          break;
        default:
          break;
      }
      base.DoAction(param);
    }

    private Visibility _clearTweetsVisibility = Visibility.Collapsed;

    public virtual Visibility ClearTweetsVisibility
    {
      get => _clearTweetsVisibility;
      set
      {
        _clearTweetsVisibility = value;
        RaisePropertyChanged();
      }
    }

    protected override void OnSettingsUpdated()
    {
      RaisePropertyChanged("MenuItems");
      RaisePropertyChanged("CloseButtonVisibility");

      base.OnSettingsUpdated();
    }

    protected override void InitCommands()
    {
      ShowSettingsCommand = new RelayCommand(ShowSettings);
      CloseServiceCommand = new RelayCommand(AskCloseControl);
      CloseCommand = new RelayCommand(CloseControl);
      MaximizeCommand = new RelayCommand(() =>
                                         {
                                           IsMaxiMize = !IsMaxiMize;
                                           Messenger.Default.Send(IsMaxiMize ? "Maximize" : "Minimize");
                                         });
      CancelCloseCommand = new RelayCommand(() => { CloseControlVisibility = Visibility.Collapsed; });
      RefreshCommand = new RelayCommand(Refresh);
      PostNewStatusCommand = new RelayCommand(PostStatus, CanPostStatus);
      PostCancelNewStatusCommand = new RelayCommand(CancelPostStatus, CanCancelPostStatus);
      base.InitCommands();
    }

    public virtual void CloseControl()
    {
      IsClosed = true;
      Messenger.Default.Send("ServiceClosed");
      Dispose(true);
    }

    private void AskCloseControl()
    {
      CloseControlVisibility = Visibility.Visible;
    }

    #endregion // SettingsCommand

    #region Settings

    public virtual void ShowSettings()
    {
      IsSettingsOpen = true;
      CloseButtonVisibility = Visibility.Collapsed;
      SettingsButtonVisibility = Visibility.Collapsed;
    }

    public virtual void Maximize()
    {
    }

    #endregion

    #region IXmlSerializable

    public XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      try
      {
        reader.MoveToContent();
        reader.Read();
        //reader.ReadStartElement("ServiceWorkspaceViewModel");
        if (reader.Name.Equals("IsServiceWorkspace"))
        {
          if (reader.IsEmptyElement)
          {
            reader.ReadStartElement("IsServiceWorkspace");
          }
          else
          {
            reader.ReadStartElement("IsServiceWorkspace");
            var isServiceWorkspace = reader.ReadContentAsString();
            if (!string.IsNullOrEmpty(isServiceWorkspace))
              IsServiceWorkspace = bool.Parse(isServiceWorkspace);
            reader.ReadEndElement();
          }
        }
        if (reader.Name.Equals("PositionInGrid"))
        {
          if (reader.IsEmptyElement)
          {
            reader.ReadStartElement("PositionInGrid");
          }
          else
          {
            reader.ReadStartElement("PositionInGrid");
            PositionInGrid =
              GenericSerializer.DeserializeObject(reader.ReadContentAsString(), typeof(BPosition)) as BPosition;
            reader.ReadEndElement();
          }
        }

        if (reader.Name.Equals("BServiceWorkspace"))
        {
          if (reader.IsEmptyElement)
          {
            reader.ReadStartElement("BServiceWorkspace");
          }
          else
          {
            reader.ReadStartElement("BServiceWorkspace");
            BServiceWorkspace =
              GenericSerializer.DeserializeObject(reader.ReadContentAsString(), typeof(BServiceWorkspace)) as
                BServiceWorkspace;
            reader.ReadEndElement();
          }
        }
        if (reader.Name.Equals("Settings"))
        {
          if (reader.IsEmptyElement)
          {
            reader.ReadStartElement("Settings");
          }
          else
          {
            reader.ReadStartElement("Settings");
            ControlSettings = reader.ReadContentAsString();
            reader.ReadEndElement();
          }
        }
        reader.ReadEndElement();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public void WriteXml(XmlWriter writer)
    {
      try
      {
        writer.WriteElementString("IsServiceWorkspace", IsServiceWorkspace.ToString());
        writer.WriteElementString("PositionInGrid", GenericSerializer.SerializeObject(PositionInGrid));
        writer.WriteElementString("BServiceWorkspace", GenericSerializer.SerializeObject(BServiceWorkspace));
        writer.WriteElementString("Settings", GetSettings());
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override string GetSettings()
    {
      return Settings == null ? null : GenericSerializer.SerializeObject(Settings);
    }

    #endregion

    #region Status

    protected virtual void CancelPostStatus()
    {
      IsStatusZoneOpen = false;
    }

    protected virtual bool CanCancelPostStatus()
    {
      return true;
    }

    protected virtual bool CanPostStatus()
    {
      return !string.IsNullOrEmpty(Status);
    }

    protected virtual void PostStatus()
    {
    }

    public virtual void NewSearch(string newSearch)
    {
    }

    #endregion
  }
}