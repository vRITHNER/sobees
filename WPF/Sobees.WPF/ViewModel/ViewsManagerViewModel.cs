#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLocalizeLib;
using Sobees.MultiPost;
using Sobees.Settings.Views;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;
using Sobees.Windows.Extensions;
using Application = System.Windows.Forms.Application;

#endregion

namespace Sobees.ViewModel
{
  public class ViewsManagerViewModel : BViewModelBase
  {
    private const string APPNAME = "ViewsManagerViewModel";

    #region Fields

    private BTemplate _bserviceWorkspaceGridTemplate1;
    private BTemplate _bserviceWorkspaceGridTemplate2;

    // private SyncViewModel _syncViewModel;
    private ChangeTemplateViewModel _changeTemplateViewModel1;

    private Duration _fadeDuration = new Duration(new TimeSpan(0, 0, 0, 1));
    private string _filterText;
    private About.Views.About _frmAbout;
    private FirstUse.Views.FirstUse _frmFirstUset;
    private MultiPostWindow _frmMultiPostWindow;
    private SettingWindow _frmSetting;
    public ObservableCollection<string> _listViews;
    private int _selectedServiceIndex;
    private int _selectedView = -1;
    private TemplateLoaderViewModel _templateLoaderViewModel1;
    private DispatcherTimer _timerupdate;
    private ObservableCollection<DataTemplate> _viewsCollection;
    private DeferredAction deferredAction;

    #endregion Fields

    #region Constructors

    public ViewsManagerViewModel()
    {
      InitCommands();

      CurrentFrontView1 = EnumTypeFrontView.WaitingStart;

      //CurrentFrontView1 = EnumTypeFrontView.None;
      MessengerInstance = Messenger.Default;
      Messenger.Default.Register<BMessage>(this, DoActionMessage);
      Messenger.Default.Register<string>(this, DoAction);
      ViewsCollection.Add(ServiceTemplate1);

      IsUpdateVisibility = Visibility.Collapsed;
      IsUpdateAvailable = false;
      var file = string.Format("{0}bDule\\bDuleSettingAutoBackup.xml", BGlobals.FOLDERBASESOBEES);
      ListViews.Add(string.Format("{0}\\{1}.png", BGlobals.FOLDER, 0));
      ListViews.Add(!File.Exists(string.Format("{0}\\{1}.png", BGlobals.FOLDER, 1))
        ? "/Sobees.Templates;Component/Images/Services/search.png"
        : string.Format("{0}\\{1}.png", BGlobals.FOLDER, 1));

      CurrentViewTemplate = ViewsCollection[0];

      ViewsCollection.Add(SearchTemplate);

      InitializeTimerUpdate();
    }

    #endregion Constructors

    #region DataTemplate

    public int SelectedServiceIndex
    {
      get
      {
        if (BServiceWorkspaces1 != null)
        {
          if (_selectedServiceIndex < 0 || _selectedServiceIndex > BServiceWorkspaces1.Count)
          {
            _selectedServiceIndex = 0;
          }
        }
        else
        {
          _selectedServiceIndex = 0;
        }
        return _selectedServiceIndex;
      }
      set
      {
        _selectedServiceIndex = value;
        RaisePropertyChanged("SelectedService");
        ProcessHelper.PerformAggressiveCleanup();
      }
    }

    /// <summary>
    ///   SelectedService
    /// </summary>
    public BViewModelBase SelectedService
    {
      get
      {
        try
        {
          if (SelectedServiceIndex == -1)
            return null;

          return BServiceWorkspaces1 != null && BServiceWorkspaces1.Count > 0
            ? BServiceWorkspaces1[SelectedServiceIndex]
            : null;
        }
        catch (Exception e)
        {
          return null;
        }
      }
    }

    public BViewModelBase ServiceViewModel => this;

    public DataTemplate CurrentViewTemplate { get; set; }

    public int SelectedView
    {
      get => _selectedView;
        set
      {
        _selectedView = value;
        ChangeView(_selectedView);
      }
    }

    public ObservableCollection<string> ListViews
    {
      get => _listViews ?? (_listViews = new ObservableCollection<string>());
        set => _listViews = value;
    }

    public ObservableCollection<DataTemplate> ViewsCollection
    {
      get => _viewsCollection ?? (_viewsCollection = new ObservableCollection<DataTemplate>());
        set => _viewsCollection = value;
    }

    public DataTemplate ServiceTemplate1
    {
      get
      {
        var view = SobeesSettingsLocator.SobeesSettingsStatic.ViewState ? "MainViewTab" : "MainViewColumn";

        var dt =
          string.Format(
            "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ><Views:{0}/> </DataTemplate>",
            view);

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate SearchTemplate
    {
      get
      {
        //var view = SobeesSettingsLocator.SobeesSettingsStatic.ViewState ? "MainViewTab" : "MainViewColumn";
        const string dt =
          "<DataTemplate x:Name='dtSearchView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ><Views:SearchView/> </DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate WaitingTemplate
    {
      get
      {
        const string dt =
          "<DataTemplate x:Name='dtSearchView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ><Views:Waiting/> </DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    //public DataTemplate SyncTemplate
    //{
    //  get { return SyncViewModel.DataTemplateView; }
    //}

    public DataTemplate ChangeViewtemplate
    {
      get
      {
        const string dt =
          "<DataTemplate x:Name='dtChangeView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ><Views:ChangeView/> </DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate MainView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:MainView/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate FirstLauch
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:FirstLaunchControl/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate ChangeLayoutTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:ChangeTemplate/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate TemplateLoaderTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:TemplateLoader/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate SettingsTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:SettingsControl/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public override DataTemplate DataTemplateView
    {
      get => SobeesSettingsLocator.SobeesSettingsStatic.ViewState ? MainView : MainView;
        set => base.DataTemplateView = value;
    }

    public BViewModelBase FrontViewModel
    {
      get
      {
        switch (CurrentFrontView1)
        {
          case EnumTypeFrontView.None:
            return null;
          case EnumTypeFrontView.ChangeLayout:
            return ChangeTemplateViewModel1;
          case EnumTypeFrontView.TemplateLoader:
            return TemplateLoaderViewModel1;
          case EnumTypeFrontView.ChangeView:
            return this;
            //case EnumTypeFrontView.SyncAccount:
            //  return SyncViewModel;
        }
        return null;
      }
    }

    public DataTemplate FrontTemplate
    {
      get
      {
        switch (CurrentFrontView1)
        {
          case EnumTypeFrontView.None:
            return null;
          case EnumTypeFrontView.ChangeLayout:
            return ChangeTemplateViewModel1.DataTemplateView;
          case EnumTypeFrontView.TemplateLoader:
            return TemplateLoaderViewModel1.DataTemplateView;
          case EnumTypeFrontView.ChangeView:
            return ChangeViewtemplate;
            //case EnumTypeFrontView.SyncAccount:
            //  return SyncTemplate;
          case EnumTypeFrontView.WaitingStart:
            return WaitingTemplate;
        }
        return null;
      }
    }

    #endregion DataTemplate

    #region Properties

    private bool _showSearch;
    private bool _showServices1 = true;
    private string _stringSearch;

    public ObservableCollection<BServiceWorkspaceViewModel> BServiceWorkspaces1 => BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1;

    public Visibility IsGlobalFilterVisible
    {
      get => SobeesSettingsLocator.SobeesSettingsStatic.ShowGlobalFilter ? Visibility.Visible : Visibility.Collapsed;
        set => RaisePropertyChanged();
    }

    public string StringSearch
    {
      get => _stringSearch;
        set
      {
        _stringSearch = value;
        RaisePropertyChanged();
      }
    }

    public EnumTypeFrontView CurrentFrontView1 { get; set; }

    public TemplateLoaderViewModel TemplateLoaderViewModel1
    {
      get => _templateLoaderViewModel1 ?? (_templateLoaderViewModel1 = new TemplateLoaderViewModel(EnumView.First));
        set => _templateLoaderViewModel1 = value;
    }

    //public SyncViewModel SyncViewModel
    //{
    //  get { return _syncViewModel ?? (_syncViewModel = new SyncViewModel()); }
    //  set { _syncViewModel = value; }
    //}

    public ChangeTemplateViewModel ChangeTemplateViewModel1
    {
      get => _changeTemplateViewModel1 ?? (_changeTemplateViewModel1 = new ChangeTemplateViewModel(EnumView.First));
        set => _changeTemplateViewModel1 = value;
    }

    public bool IsChangeTemplate => _changeTemplateViewModel1 != null;

    public BTemplate BServiceWorkspaceGridTemplate1
    {
      get => _bserviceWorkspaceGridTemplate1 ?? (_bserviceWorkspaceGridTemplate1 = new BTemplate
                                                                                   {
                                                                                       IsUserSelectedbTemplate = false,
                                                                                       BPositions =
                                                                                           new List<BPosition>
                                                                                           {
                                                                                               new BPosition
                                                                                               {
                                                                                                   Col = 0,
                                                                                                   ColSpan = 1,
                                                                                                   Height = -1,
                                                                                                   HorizontalAlignment =
                                                                                                       HorizontalAlignment.Stretch,
                                                                                                   Orientation =
                                                                                                       Orientation.Vertical,
                                                                                                   Row = 0,
                                                                                                   RowSpan = 1,
                                                                                                   UnitType =
                                                                                                       GridUnitType.Star,
                                                                                                   VerticalAlignment =
                                                                                                       VerticalAlignment.Stretch,
                                                                                                   Width = -1
                                                                                               },
                                                                                           },
                                                                                       Columns = 1,
                                                                                       Rows = 1,
                                                                                       GridSplitterPositions =
                                                                                           new List<BPosition>()
                                                                                   });
        set
      {
        if (_bserviceWorkspaceGridTemplate1 == null)
          _bserviceWorkspaceGridTemplate1 = new BTemplate();

        _bserviceWorkspaceGridTemplate1.BPositions = value.BPositions;
        _bserviceWorkspaceGridTemplate1.Columns = value.Columns;
        _bserviceWorkspaceGridTemplate1.GridSplitterPositions = value.GridSplitterPositions;
        _bserviceWorkspaceGridTemplate1.ImgUrl = value.ImgUrl;
        _bserviceWorkspaceGridTemplate1.Rows = value.Rows;
        _bserviceWorkspaceGridTemplate1.IsUserSelectedbTemplate = value.IsUserSelectedbTemplate;
        _bserviceWorkspaceGridTemplate1.DependencyPropertyDescriptorHack = value.DependencyPropertyDescriptorHack;
        RaisePropertyChanged();
      }
    }

    public int FontsizeValue => (int) SobeesSettingsLocator.SobeesSettingsStatic.FontSizeValue;

    public BTemplate BServiceWorkspaceGridTemplate2
    {
      get => _bserviceWorkspaceGridTemplate2 ?? (_bserviceWorkspaceGridTemplate2 = new BTemplate
                                                                                   {
                                                                                       IsUserSelectedbTemplate = false,
                                                                                       BPositions = new List<BPosition>
                                                                                                    {
                                                                                                        new BPosition
                                                                                                        {
                                                                                                            Col = 0,
                                                                                                            ColSpan = 1,
                                                                                                            Height = -1,
                                                                                                            HorizontalAlignment = HorizontalAlignment.Stretch,
                                                                                                            Orientation = Orientation.Vertical,
                                                                                                            Row = 0,
                                                                                                            RowSpan = 1,
                                                                                                            UnitType = GridUnitType.Star,
                                                                                                            VerticalAlignment = VerticalAlignment.Stretch,
                                                                                                            Width = -1
                                                                                                        },
                                                                                                    },
                                                                                       Columns = 1,
                                                                                       Rows = 1,
                                                                                       GridSplitterPositions = new List<BPosition>()
                                                                                   });
        set
      {
        if (_bserviceWorkspaceGridTemplate2 == null)
          _bserviceWorkspaceGridTemplate2 = new BTemplate();

        _bserviceWorkspaceGridTemplate2.BPositions = value.BPositions;
        _bserviceWorkspaceGridTemplate2.Columns = value.Columns;
        _bserviceWorkspaceGridTemplate2.GridSplitterPositions = value.GridSplitterPositions;
        _bserviceWorkspaceGridTemplate2.ImgUrl = value.ImgUrl;
        _bserviceWorkspaceGridTemplate2.Rows = value.Rows;
        _bserviceWorkspaceGridTemplate2.IsUserSelectedbTemplate = value.IsUserSelectedbTemplate;
        _bserviceWorkspaceGridTemplate2.DependencyPropertyDescriptorHack = value.DependencyPropertyDescriptorHack;
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
      }
    }

    public bool IsSettingOpened { get; set; }

    public bool IsMultiPostOpened { get; set; }

    public bool IsAsyncConnected => !string.IsNullOrEmpty(SobeesSettingsLocator.SobeesSettingsStatic.SyncUser);

    public bool ShowServices1
    {
      get => _showServices1;
        set
      {
        _showServices1 = true;
        if (value)
        {
          _showSearch = false;
          ChangeView(0);
          RaisePropertyChanged("ShowSearch");
        }
        else
        {
          ShowSearch = true;
        }

        RaisePropertyChanged();
      }
    }

    public bool ShowSearch
    {
      get => _showSearch;
        set
      {
        _showSearch = true;
        if (value)
        {
          _showServices1 = false;
          ChangeView(1);
          RaisePropertyChanged("ShowServices1");
        }
        else
        {
          ShowServices1 = true;
        }
        RaisePropertyChanged();
      }
    }

    public Visibility ChangeViewVisibility => string.IsNullOrEmpty(SobeesSettingsLocator.SobeesSettingsStatic.WordSearch)
      ? Visibility.Collapsed
      : Visibility.Visible;

    #endregion Properties

    #region Commands

    private BRelayCommand _textChangedFilterCommand;
    public RelayCommand ShowSettingsCommand { get; private set; }

    public RelayCommand ShowNextViewsCommand { get; private set; }

    public RelayCommand<string> RequestNavigateCommand { get; private set; }

    public RelayCommand ShowAboutCommand { get; private set; }

    public RelayCommand SaveKeywordsCommand { get; private set; }

    public RelayCommand RemoveFilterCommand { get; private set; }

    public BRelayCommand TextChangedFilterCommand => _textChangedFilterCommand ?? (_textChangedFilterCommand = new BRelayCommand(TextChangedFilter));

    public RelayCommand ShowMultiPostCommand { get; private set; }

    public RelayCommand ShowTemplateCommand { get; private set; }

    public RelayCommand AddColumnCommand { get; private set; }

    /// <summary>
    ///   Command to Close and ReOpen the application
    /// </summary>
    public RelayCommand UpdateClickOnceCommand
    {
      get;
      private set;
    }

    //internal void UpdateClickOnce()
    //{
    //  TraceHelper.Trace(this, "SobeesViewModel::UpdateClickOnce");
    //  Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
    //  {
        
    //    var shortcutFile = GetShortcutPath();
    //    var proc = new Process { StartInfo = { FileName = shortcutFile, UseShellExecute = true } };

    //    proc.Start();
    //    System.Windows.Application.Current.Shutdown();

    //    Application.Restart();
    //    System.Windows.Application.Current.Shutdown();
    //  });
    //}

    internal void UpdateClickOnce()
    {
      TraceHelper.Trace(this, "SobeesViewModel::UpdateClickOnce");
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action( () =>
      {
        var shortcutFile = GetShortcutPath();
        var proc = new Process { StartInfo = { FileName = shortcutFile, UseShellExecute = true } };

        proc.Start();
        System.Windows.Application.Current.Shutdown();

        //Application.Restart();
        //System.Windows.Application.Current.Shutdown();
      }));
    }


    public static string GetShortcutPath()
    {
      var file = String.Format(@"{0}\{1}\{2}.appref-ms", Environment.GetFolderPath(Environment.SpecialFolder.Programs), GetPublisher(), GetDeploymentInfo().Name.Replace(".application", ""));
      if (File.Exists(file)) return file;
      const string publisherFolderName = "sobees Ltd";
      file = String.Format(@"{0}\{1}\{2}.appref-ms", Environment.GetFolderPath(Environment.SpecialFolder.Programs), publisherFolderName, GetDeploymentInfo().Name.Replace(".application", ""));
      return file;
    }

    public static string GetPublisher()
    {
      TraceHelper.Trace(APPNAME, "GetPublisher::START");

      XDocument xDocument;
      using (var memoryStream = new MemoryStream(AppDomain.CurrentDomain.ActivationContext.DeploymentManifestBytes))
      using (var xmlTextReader = new XmlTextReader(memoryStream))
        xDocument = XDocument.Load(xmlTextReader);

      if (xDocument.Root == null)
        return null;

      var description = xDocument.Root.Elements().First(e => e.Name.LocalName == "description");
      var publisher = description.Attributes().First(a => a.Name.LocalName == "publisher");

      if (publisher == null) return null;

      TraceHelper.Trace(APPNAME, string.Format("GetPublisher::publisher.Value:{0}", publisher.Value));

      TraceHelper.Trace(APPNAME, "GetPublisher::END");
      return publisher.Value;
    }

    private static ApplicationId GetDeploymentInfo()
    {
      var appSecurityInfo = new System.Security.Policy.ApplicationSecurityInfo(AppDomain.CurrentDomain.ActivationContext);
      return appSecurityInfo.DeploymentId;
    }

    #endregion Commands

    #region Methods

    /// <summary>
    ///   Init commands
    /// </summary>
    protected override void InitCommands()
    {
      IsSettingOpened = false;
      IsMultiPostOpened = false;
      RaisePropertyChanged("IsSettingOpened");
      RemoveFilterCommand = new RelayCommand(RemoveFilter);
      RaisePropertyChanged("IsMultiPostOpened");
      ShowSettingsCommand = new RelayCommand(ShowSettings, () => !IsSettingOpened);
      ShowMultiPostCommand = new RelayCommand(ShowMultiPost);
      ShowAboutCommand = new RelayCommand(ShowAbout);
      ShowTemplateCommand = new RelayCommand(ShowTemplate);
      AddColumnCommand = new RelayCommand(ShowFirstUse);
      ShowNextViewsCommand = new RelayCommand(ShowNextViews);
      RequestNavigateCommand = new RelayCommand<string>(GoToweb);
      SaveKeywordsCommand = new RelayCommand(() => OpenSearch(true));
      UpdateClickOnceCommand = new RelayCommand(UpdateClickOnce);
      base.InitCommands();
    }

    private void OpenSearch(bool goToSearch)
    {
      BViewModelLocator.SearchViewModelStatic.StringSearch = StringSearch;
      BViewModelLocator.SearchViewModelStatic.SaveKeywords();
      StringSearch = string.Empty;
      if (goToSearch)
      {
        ChangeView(1);
        ShowSearch = true;
      }
      RaisePropertyChanged("ChangeViewVisibility");
    }

    private static void GoToweb(string url)
    {
      if (!string.IsNullOrEmpty(url))
        WebHelper.NavigateToUrl(url);
    }

    /// <summary>
    ///   Settings Window
    /// </summary>
    private void ShowSettings()
    {
      IsSettingOpened = true;
      RaisePropertyChanged("IsSettingOpened");

      _frmSetting = new SettingWindow((MainWindow) System.Windows.Application.Current.MainWindow)
      {
        ShowInTaskbar = false,
        Owner = System.Windows.Application.Current.MainWindow
      };
      _frmSetting.Closed += (o, args) => CloseSettings();
      _frmSetting.ShowDialogWindow(System.Windows.Application.Current.MainWindow.GetWindowLocation());
    }

    private void CloseSettings()
    {
      if (_frmSetting != null)
      {
        _frmSetting.Close();
        _frmSetting = null;
      }

      IsSettingOpened = false;
      UpdateMainView();
      RaisePropertyChanged("IsSettingOpened");
      BViewModelLocator.DisposeSettings();
    }

    private void CloseFirstUse()
    {
      if (BServiceWorkspaces1.Count == 0)
      {
        if (BViewModelLocator.FirstLaunchControlViewModelStatic.ListTrends != null &&
            BViewModelLocator.FirstLaunchControlViewModelStatic.ListTrends.Count > 0)
        {
          AddColumn1(new UserAccount(BViewModelLocator.FirstLaunchControlViewModelStatic.ListTrends[0].Content,
            EnumAccountType.TwitterSearch));
        }
      }
      if (_frmFirstUset != null)
        _frmFirstUset.Close();
      _frmFirstUset = null;
      if (CurrentFrontView1 == EnumTypeFrontView.WaitingStart)
      {
        CurrentFrontView1 = EnumTypeFrontView.None;
        RaisePropertyChanged("FrontViewModel");
        RaisePropertyChanged("FrontTemplate");
      }

      BViewModelLocator.DisposeUcFirstLauchControl();
    }

    private void CloseAbout()
    {
      if (_frmAbout == null)
        return;
      _frmAbout.Close();
      _frmAbout = null;
    }

    private void UpdateMainView()
    {
      RaisePropertyChanged("IsGlobalFilterVisible");
    }

    /// <summary>
    ///   About Window
    /// </summary>
    private void ShowAbout()
    {
      _frmAbout = new About.Views.About((MainWindow) System.Windows.Application.Current.MainWindow)
      {
        ShowInTaskbar = false,
        Owner = System.Windows.Application.Current.MainWindow
      };
      _frmAbout.ShowDialogWindow(System.Windows.Application.Current.MainWindow.GetWindowLocation());
    }

    /// <summary>
    ///   About Window
    /// </summary>
    private void ShowFirstUse()
    {
      System.Windows.Application.Current.Dispatcher.BeginInvokeIfRequired(() =>
      {
        if (_frmFirstUset == null)
        {
          _frmFirstUset =
            new FirstUse.Views.FirstUse(
              (MainWindow) System.Windows.Application.Current.MainWindow)
            {
              ShowInTaskbar = false,
              Owner = System.Windows.Application.Current.MainWindow
            };
          _frmFirstUset.Closed += (o, args) => CloseFirstUse();
          _frmFirstUset.ShowDialogWindow(
            System.Windows.Application.Current.MainWindow.GetWindowLocation());
        }
        else
        {
          _frmFirstUset.Activate();
        }
      });
    }

    private void ShowTemplate()
    {
      CurrentFrontView1 = EnumTypeFrontView.TemplateLoader;
      RaisePropertyChanged("FrontViewModel");
      RaisePropertyChanged("FrontTemplate");
    }

    /// <summary>
    ///   MultiPost Window
    /// </summary>
    private void ShowMultiPost()
    {
      IsMultiPostOpened = true;
      RaisePropertyChanged("IsMultiPostOpened");

      if (_frmMultiPostWindow == null)
      {
        _frmMultiPostWindow = new MultiPostWindow((MainWindow) System.Windows.Application.Current.MainWindow)
        {
          ShowInTaskbar = true,
          Title = new LocText("Sobees.Configuration.BGlobals:Resources:JumpListPostStatus").ResolveLocalizedValue()
        };

        _frmMultiPostWindow.Closed += (o, args) =>
        {
          CloseMultipost();
          MessengerInstance.Send("MultiPostClosed");
          IsMultiPostOpened = false;
          RaisePropertyChanged("IsMultiPostOpened");
          _frmMultiPostWindow = null;
        };

        _frmMultiPostWindow.ShowWindow(System.Windows.Application.Current.MainWindow.GetWindowLocation());
      }
      else
        _frmMultiPostWindow.Activate();
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "NoSettingsRestored":
          ShowFirstUse();
          break;
        case "CloseFirstUse":
          CloseFirstUse();
          break;
        case "CloseSettings":
          CloseSettings();
          break;
        case "CloseAbout":
          CloseAbout();
          break;
        case "ShowSettings":
          ShowSettings();
          break;
        case "SettingsRestored":
          try
          {
            for (var index = 0; index < SobeesSettingsLocator.SobeesSettingsStatic.Accounts.Count; index++)
            {
              var userAccount = SobeesSettingsLocator.SobeesSettingsStatic.Accounts[index];
              if (userAccount.Type != EnumAccountType.Twitter)
                continue;
              if (string.IsNullOrEmpty(userAccount.PasswordHash))
                continue;
              var isfind = false;
              foreach (var i in BServiceWorkspaces1.Where(i => i.Settings.UserName == userAccount.Login))
              {
                isfind = true;
              }
              if (!isfind)
              {
                SobeesSettingsLocator.SobeesSettingsStatic.Accounts.Remove(userAccount);
              }
            }
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
          }
          if (!string.IsNullOrEmpty(SobeesSettingsLocator.SobeesSettingsStatic.WordSearch))
          {
            _stringSearch = SobeesSettingsLocator.SobeesSettingsStatic.WordSearch;
            OpenSearch(false);
          }
          RaisePropertyChanged("IsAsyncConnected");
          CurrentFrontView1 = EnumTypeFrontView.None;
          RaisePropertyChanged("FrontViewModel");
          RaisePropertyChanged("FrontTemplate");
          UpdateMainView();
          break;
        case "CloseMultipost":
          CloseMultipost();
          break;
        case "ShowMultiPost":
          ShowMultiPost();
          break;
        case "ActivateWindows":
          ActivateWindows();
          break;
        case "CloseSync":
          ClearFrontView();
          //if (_syncViewModel != null) _syncViewModel.Cleanup();
          //_syncViewModel = null;
          RaisePropertyChanged("IsAsyncConnected");
          break;
        case "CloseFront1":
          ClearFrontView();
          if (_templateLoaderViewModel1 != null)
            _templateLoaderViewModel1.Cleanup();
          _templateLoaderViewModel1 = null;
          break;
        case "CloseChangeTemplate1":
          ClearFrontView();
          if (_changeTemplateViewModel1 != null)
            _changeTemplateViewModel1.Cleanup();
          _changeTemplateViewModel1 = null;
          break;
        case "ViewStateChanged":
          RaisePropertyChanged("ServiceTemplate1");
          if (CurrentViewTemplate == ViewsCollection[0])
          {
            ViewsCollection[0] = ServiceTemplate1;
            CurrentViewTemplate = ViewsCollection[0];
          }
          else
          {
            ViewsCollection[0] = ServiceTemplate1;
          }

          break;
      }
      base.DoAction(param);
    }

    private void ActivateWindows()
    {
      if (_frmFirstUset != null)
        _frmFirstUset.Activate();
      if (_frmSetting != null)
        _frmSetting.Activate();
    }

    protected void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
        case "TemplateChoosed1":
          Messenger.Default.Send("OpenChangeTemplate1");
          if (_templateLoaderViewModel1 != null)
            _templateLoaderViewModel1.Cleanup();
          _templateLoaderViewModel1 = null;
          CurrentFrontView1 = EnumTypeFrontView.ChangeLayout;
          ChangeTemplateViewModel1.SetSelectedTemplate(message.Parameter as BTemplate);
          RaisePropertyChanged("FrontViewModel");
          RaisePropertyChanged("FrontTemplate");

          break;
        case "NewAccountFirstUse":
          if ((message.Parameter as UserAccount).Type == EnumAccountType.TwitterSearch)
          {
            foreach (var service in BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1)
            {
              if (service.BServiceWorkspace == null)
                continue;
              if (service.BServiceWorkspace.ClassName != "TwitterSearchViewModel")
                continue;
              service.SetAccount(message.Parameter as UserAccount);
              Messenger.Default.Send("SaveSettings");
              return;
            }
          }

          AddColumn1(message.Parameter as UserAccount);
          Messenger.Default.Send("SaveSettings");
          break;
      }
    }

    private void ClearFrontView()
    {
      CurrentFrontView1 = EnumTypeFrontView.None;
      RaisePropertyChanged("FrontViewModel");
      RaisePropertyChanged("FrontTemplate");
    }

    public void UpdateView()
    {
      RaisePropertyChanged("BServiceWorkspaceGridTemplate1");
      BServiceWorkspaceGridTemplate1 = BServiceWorkspaceGridTemplate1;
    }

    private void AddColumn1(UserAccount account)
    {
      ClearFrontView();
      foreach (var model in BServiceWorkspaces1)
      {
        if (model.IsMaxiMize)
        {
          model.IsMaxiMize = false;
          Messenger.Default.Send("Minimize");
        }
        if (model.IsServiceWorkspace)
          continue;
        BServiceWorkspaceHelper.LoadServiceWorkspace(account, model.PositionInGrid, model as SourceLoaderViewModel);
        return;
      }
      var nb = BServiceWorkspaces1.Count(model => model.IsServiceWorkspace);
      if (nb >= 5)
        return;

      if (BServiceWorkspaces1.Count == 0)
      {
        //If there is no service, we add a Search
        BServiceWorkspaceHelper.LoadServiceWorkspace(account, BServiceWorkspaceGridTemplate1.BPositions[0], null);
        return;
      }
      var tempTemplate = new BTemplate {BPositions = new List<BPosition>()};
      foreach (var position in BServiceWorkspaceGridTemplate1.BPositions)
      {
        tempTemplate.BPositions.Add(position);
      }
      tempTemplate.Columns = BServiceWorkspaceGridTemplate1.Columns + 1;
      tempTemplate.IsUserSelectedbTemplate = true;
      tempTemplate.Rows = BServiceWorkspaceGridTemplate1.Rows;
      var pos = new BPosition
      {
        Col = BServiceWorkspaceGridTemplate1.Columns,
        ColSpan = 1,
        Height = -1,
        HorizontalAlignment = HorizontalAlignment.Left,
        Orientation = Orientation.Vertical,
        Row = 0,
        RowSpan = BServiceWorkspaceGridTemplate1.Rows,
        UnitType = GridUnitType.Star,
        VerticalAlignment = VerticalAlignment.Top,
        Width = -1
      };
      foreach (var position in tempTemplate.BPositions)
      {
        position.Width = -1;
        position.Height = -1;
      }
      tempTemplate.GridSplitterPositions = new List<BPosition>();
      foreach (var gridSplitterPosition in BServiceWorkspaceGridTemplate1.GridSplitterPositions)
      {
        tempTemplate.GridSplitterPositions.Add(gridSplitterPosition);
      }
      tempTemplate.GridSplitterPositions.Add(new BPosition(BServiceWorkspaceGridTemplate1.Columns - 1, 0,
        BServiceWorkspaceGridTemplate1.Rows, 1,
        Orientation.Vertical, VerticalAlignment.Stretch,
        HorizontalAlignment.Right));
      tempTemplate.BPositions.Add(pos);
      BServiceWorkspaceGridTemplate1 = tempTemplate;
      BServiceWorkspaceHelper.LoadServiceWorkspace(account, pos, null);
    }

    private void CloseMultipost()
    {
      IsMultiPostOpened = false;
      RaisePropertyChanged("IsMultiPostOpened");
      BViewModelLocator.MultiPostViewModelStatic.GoToBaseState();
      if (_frmMultiPostWindow != null)
        _frmMultiPostWindow.Close();
    }

    #endregion Methods

    #region Filter

    private void TextChangedFilter(object obj)
    {
      var objs = BRelayCommand.CheckParams(obj);
      if (objs == null || objs[1] == null)
        return;
      FilterText = ((TextBox) objs[1]).Text;
      if (deferredAction == null)
        deferredAction = DeferredAction.Create(delegate
        {
          if (FilterText.Length <= 2 && !string.IsNullOrEmpty(FilterText))
            return;
          SobeesSettingsLocator.SobeesSettingsStatic.Filter = FilterText;
          Messenger.Default.Send("UpdateViewFilter");
        });

      // Defer applying search criteria until time has elapsed.
      deferredAction.Defer(TimeSpan.FromSeconds(1.0));
    }

    private void RemoveFilter()
    {
      SobeesSettingsLocator.SobeesSettingsStatic.Filter = string.Empty;
      FilterText = string.Empty;
      ProcessHelper.PerformAggressiveCleanup();
    }

    #endregion Filter

    #region MultiViews

    public void ShowNextViews()
    {
      _selectedView = -1;
      Messenger.Default.Send("CreatePicture");
      CurrentFrontView1 = EnumTypeFrontView.ChangeView;
      RaisePropertyChanged("FrontViewModel");
      RaisePropertyChanged("FrontTemplate");
    }

    private void ChangeView(int view)
    {
      if (view <= -1 || view >= ViewsCollection.Count)
        return;
      if (view == 0)
      {
        _showServices1 = true;
        _showSearch = false;
      }
      if (view == 1)
      {
        _showServices1 = false;
        _showSearch = true;
      }
      RaisePropertyChanged("ShowSearch");
      RaisePropertyChanged("ShowServices1");
      CurrentViewTemplate = ViewsCollection[view];
      RaisePropertyChanged("CurrentViewTemplate");
      CurrentFrontView1 = EnumTypeFrontView.None;
      RaisePropertyChanged("FrontViewModel");
      RaisePropertyChanged("FrontTemplate");
    }

    #endregion MultiViews

    #region Check Update ClickOnce

    private ApplicationDeployment _applicationDeployment;

    private bool _isUpdateAvailable;
    private Visibility _isUpdateVisibility;
    private bool _processing; 

    protected bool IsUpdateAvailable
    {
      get => _isUpdateAvailable;
        set
      {
        _isUpdateAvailable = value;
        TraceHelper.Trace(this, string.Format("ClickOnce Update: -> IsUpdateAvailable =:{0}", value));
        IsUpdateVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        TraceHelper.Trace(this, string.Format("ClickOnce Update: -> IsUpdateVisibility =:{0}", IsUpdateVisibility));
        RaisePropertyChanged();
        RaisePropertyChanged("IsUpdateVisibility");
      }
    }

    public Visibility IsUpdateVisibility
    {
      get => _isUpdateVisibility;
        set
      {
        _isUpdateVisibility = value;
        RaisePropertyChanged();
      }
    }

    private void InitializeTimerUpdate()
    {
      TraceHelper.Trace(this, "Check for ClickOnce Running Instance");
      if (!ApplicationDeployment.IsNetworkDeployed)
      {
        TraceHelper.Trace(this, "No ClickOnce instance... Auto Update Timer not started ! ");
        return;
      }

      TraceHelper.Trace(this, "ClickOnce instance detected... Check Update and Auto Update Timer Initializing... ");

      //FirstTime check and let timer for rest of time
      TimerUpdateTick(this, null);

      _timerupdate = new DispatcherTimer();
      _timerupdate.Tick += TimerUpdateTick;

      _timerupdate.Interval = new TimeSpan(0, 0, Convert.ToInt32(BGlobals.DEFAULT_TIMER_UPDATE), 0);

      TraceHelper.Trace(this, "ClickOnce instance -> Auto Update Timer started ! ");

      _timerupdate.Start();

      TraceHelper.Trace(this,
        string.Format("ClickOnce Auto Update's timer initialized and started to run every {0} minutes.",
          BGlobals.DEFAULT_TIMER_UPDATE));
    }

    private void TimerUpdateTick(object sender, EventArgs e)
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
                          
                           if (_processing) return;

                           Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() => CheckUpdate(null));
                          //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => CheckUpdate(null)));
                         };
      worker.RunWorkerAsync();
      }
    }

    /// <summary>
    ///   CheckUpdate
    /// </summary>
    /// <param name="param"></param>
    private void CheckUpdate(object param)
    {
      if (_processing) return;

      IsUpdateAvailable = false;

      try
      {
        TraceHelper.Trace(this, "Check for ClickOnce deployement");
        if (!ApplicationDeployment.IsNetworkDeployed)
          return;

        try
        {
          Messenger.Default.Send("SaveSettings");
        }
        catch (Exception e)
        {
          TraceHelper.Trace(this, e);
        }

        TraceHelper.Trace(this, "ClickOnce Update Initialization");
        _processing = true;

        _applicationDeployment = ApplicationDeployment.CurrentDeployment;
        //Event to restart the application, otherwise changes will not reflect
        _applicationDeployment.UpdateCompleted += ApplicationDeploymentUpdateCompleted;
        //_applicationDeployment.CheckForUpdateCompleted += ApplicationDeploymentCheckForUpdateCompleted;
        //Event to show the progressbar
        _applicationDeployment.UpdateProgressChanged += ApplicationDeploymentUpdateProgressChanged;

        //Call the method to invoke the update process
        try
        {
          TraceHelper.Trace(this, "ClickOnce Update CheckForUpdate");
          if (_applicationDeployment.CheckForUpdate(false) || Application.ProductVersion == new Version(0, 8, 7, 4).ToString())
          {
            TraceHelper.Trace(this, "ClickOnce Update Forced to migrate to 0.8.8.1");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => _applicationDeployment.UpdateAsync()));
          }
          else
            _processing = false;
        }
        catch (DeploymentDownloadException dde)
        {
          TraceHelper.Trace(this,
            "The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " +
            dde.Message);
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(FinishClickOnceUpdate));
          
        }
        catch (InvalidDeploymentException ide)
        {
          TraceHelper.Trace(this,
            "Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " +
            ide.Message);
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(FinishClickOnceUpdate));
        }
        catch (InvalidOperationException ioe)
        {
          TraceHelper.Trace(this,
            "This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(FinishClickOnceUpdate));
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        _processing = false;
      }
    }

    private void ApplicationDeploymentCheckForUpdateCompleted(object sender,
      CheckForUpdateCompletedEventArgs e)
    {
      try
      {
        if (e.UpdateAvailable)
        {
          TraceHelper.Trace(this, "ClickOnce Update Available:YES");
          IsUpdateAvailable = false;

          //Async UPDATE
          TraceHelper.Trace(this, "ClickOnce Update Download Asynchronously:Starting...");
          _applicationDeployment.UpdateAsync();
        }
        else
        {
          TraceHelper.Trace(this, "ClickOnce Update Available:NO");
          FinishClickOnceUpdate();
        }
      }
      catch (Exception)
      {
        TraceHelper.Trace(this,
          "The server cannont be contacted. Try later.");
        FinishClickOnceUpdate();
      }
    }

    private void ApplicationDeploymentUpdateCompleted(object sender, AsyncCompletedEventArgs e)
    {
      TraceHelper.Trace(this, "A ClickOnce Update has been sucessfully installed. Please restart Sobees !");
      _processing = false;
      if (e.Cancelled || e.Error != null)
      {
        TraceHelper.Trace(this, "Could not install the latest version of the application.");
        return;
      }
      IsUpdateAvailable = true;
      FinishClickOnceUpdate();
    }

    private void FinishClickOnceUpdate()
    {
      TraceHelper.Trace(this, "ClickOnce Update Process Finish");
      _applicationDeployment.UpdateProgressChanged -= ApplicationDeploymentUpdateProgressChanged;
      _applicationDeployment.UpdateCompleted -= ApplicationDeploymentUpdateCompleted;
      _applicationDeployment.CheckForUpdateCompleted -= ApplicationDeploymentCheckForUpdateCompleted;
      _processing = false;
      TraceHelper.Trace(this,
        string.Format("ClickOnce::_timerupdate::Interval|IsEnabled:{0}|{1}", _timerupdate.Interval,
          _timerupdate.IsEnabled));
    }

    //to show the progressbar
    private static void ApplicationDeploymentUpdateProgressChanged(object sender,
      DeploymentProgressChangedEventArgs e)
    {
      TraceHelper.Trace("ClickOnce::AdUpdateProgressChanged::Percentage Value", e.ProgressPercentage.ToString());
    }

    #endregion Check Update ClickOnce
  }
}