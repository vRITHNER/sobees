using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLinkedInLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;

namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class LinkedInViewModel : BServiceWorkspaceViewModel
  {
    #region Fields

    private CredentialsViewModel _credentialsViewModel;
    private FriendsViewModel _friendsViewModel;
    private HomeViewModel _homeViewModel;
    private ProfileViewModel _profileViewModel;
    private SettingsViewModel _settingsViewModel;
    private SearchViewModel _searchViewModel;
    private bool _isShowAll;
    private bool _showHome = true;
    private bool _showFriends;
    private bool _showSearch;

    public override double GetRefreshTime()
    {
      return Settings.RefreshTime;
    }

    #endregion

    #region Properties

    public ObservableCollection<LinkedInUser> Friends;
    private OAuthLinkedInV2 _linkedInLibV2;

    public OAuthLinkedInV2 LinkedInLibV2 => _linkedInLibV2 ??
                                            (_linkedInLibV2 = new OAuthLinkedInV2(BGlobals.LINKEDIN_WPF_KEY, BGlobals.LINKEDIN_WPF_SECRET));

    public override string ImageUser => SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].PictureUrl;

    #region Workspace

    public CredentialsViewModel CredentialsViewModel
    {
      get
      {
        if (_credentialsViewModel == null)
        {
          _credentialsViewModel = new CredentialsViewModel(this, ServiceMessenger);
        }
        return _credentialsViewModel;
      }
    }

    public FriendsViewModel FriendsViewModel
    {
      get
      {
        if (_friendsViewModel == null)
        {
          _friendsViewModel = new FriendsViewModel(this, ServiceMessenger);
        }
        return _friendsViewModel;
      }
    }

    public HomeViewModel HomeViewModel
    {
      get
      {
        if (_homeViewModel == null)
        {
          _homeViewModel = new HomeViewModel(this, ServiceMessenger);
        }
        return _homeViewModel;
      }
    }

    public ProfileViewModel ProfileViewModel
    {
      get
      {
        if (_profileViewModel == null)
        {
          _profileViewModel = new ProfileViewModel(this, ServiceMessenger);
        }
        return _profileViewModel;
      }
    }


    public SettingsViewModel SettingsViewModel
    {
      get
      {
        if (_settingsViewModel == null)
        {
          _settingsViewModel = new SettingsViewModel(this, ServiceMessenger);
        }
        return _settingsViewModel;
      }
    }

    public SearchViewModel SearchViewModel
    {
      get
      {
        if (_searchViewModel == null)
        {
          _searchViewModel = new SearchViewModel(this, ServiceMessenger);
        }
        return _searchViewModel;
      }
    }

    #endregion

    #region Filter

    public bool ShowCONN
    {
      get { return ((LinkedInSettings) Settings).ShowCONN; }
      set
      {
        ((LinkedInSettings) Settings).ShowCONN = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowSTAT
    {
      get { return ((LinkedInSettings) Settings).ShowSTAT; }
      set
      {
        ((LinkedInSettings) Settings).ShowSTAT = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowAPPS
    {
      get { return ((LinkedInSettings) Settings).ShowAPPS; }
      set
      {
        ((LinkedInSettings) Settings).ShowAPPS = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowJOBS
    {
      get { return ((LinkedInSettings) Settings).ShowJOBS; }
      set
      {
        ((LinkedInSettings) Settings).ShowJOBS = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowJGRP
    {
      get { return ((LinkedInSettings) Settings).ShowJGRP; }
      set
      {
        ((LinkedInSettings) Settings).ShowJGRP = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowRECU
    {
      get { return ((LinkedInSettings) Settings).ShowRECU; }
      set
      {
        ((LinkedInSettings) Settings).ShowRECU = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowPRFU
    {
      get { return ((LinkedInSettings) Settings).ShowPRFU; }
      set
      {
        ((LinkedInSettings) Settings).ShowPRFU = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool ShowOTHER
    {
      get { return ((LinkedInSettings) Settings).ShowOTHER; }
      set
      {
        ((LinkedInSettings) Settings).ShowOTHER = value;
        RaisePropertyChanged();
        MainViewModel.UpdateView();
      }
    }

    public bool IsShowAll
    {
      get { return _isShowAll; }
      set
      {
        _isShowAll = value;
        ShowAPPS = value;
        ShowCONN = value;
        ShowJGRP = value;
        ShowJOBS = value;
        ShowOTHER = value;
        ShowPRFU = value;
        ShowRECU = value;
        ShowSTAT = value;
        RaisePropertyChanged();
      }
    }

    public bool ShowHome
    {
      get { return _showHome; }
      set
      {
        _showHome = value;
        if (_showHome)
        {
          CurrentView = LinkedInTypeView.Home;
          RaisePropertyChanged("MainViewModel");
          RaisePropertyChanged("MainTemplateView");
        }
      }
    }

    public bool ShowFriends
    {
      get { return _showFriends; }
      set
      {
        _showFriends = value;
        if (_showFriends)
        {
          CurrentView = LinkedInTypeView.Friends;
          RaisePropertyChanged("MainViewModel");
          RaisePropertyChanged("MainTemplateView");
        }
      }
    }

    public bool ShowSearch
    {
      get { return _showSearch; }
      set
      {
        _showSearch = value;
        if (_showSearch)
        {
          CurrentView = LinkedInTypeView.Search;
          RaisePropertyChanged("MainViewModel");
          RaisePropertyChanged("MainTemplateView");
        }
      }
    }

    #endregion

    #region ViewManaging

    public LinkedInTypeView CurrentView { get; set; }
    public LinkedInFrontView CurrentFrontView { get; set; }

    public LinkedInMainTypeView CurrentMainView { get; set; }

    public override DataTemplate MainTemplateView
    {
      get
      {
        switch (CurrentView)
        {
          case LinkedInTypeView.Home:
            return HomeViewModel.DataTemplateView;
          case LinkedInTypeView.Friends:
            return FriendsViewModel.DataTemplateView;
          case LinkedInTypeView.Search:
            return SearchViewModel.DataTemplateView;
          case LinkedInTypeView.Credentials:
            return null;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainTemplateView = value; }
    }

#if !SILVERLIGHT
    public override object MenuItems
    {
      get
      {
        //Create Binding and items for Home
        var bindingH0 = new Binding("IsShowAll") {Source = this, Mode = BindingMode.TwoWay};
        var itemH0 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinHomeH1"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtAll").ResolveLocalizedValue()
                       };
        itemH0.SetBinding(ToggleButton.IsCheckedProperty, bindingH0);
        var bindingH1 = new Binding("ShowCONN") {Source = this, Mode = BindingMode.TwoWay};
        var itemH1 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinConnectH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInConnectionupdates").ResolveLocalizedValue()
                       };
        itemH1.SetBinding(ToggleButton.IsCheckedProperty, bindingH1);
        var bindingH2 = new Binding("ShowSTAT") {Source = this, Mode = BindingMode.TwoWay};
        var itemH2 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinStatusH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInStatusupdates").ResolveLocalizedValue()
                       };
        itemH2.SetBinding(ToggleButton.IsCheckedProperty, bindingH2);
        var bindingH3 = new Binding("ShowAPPS") {Source = this, Mode = BindingMode.TwoWay};
        var itemH3 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinAppsH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInApplicationupdates").ResolveLocalizedValue()
                       };
        itemH3.SetBinding(ToggleButton.IsCheckedProperty, bindingH3);
        var bindingH4 = new Binding("ShowJOBS") {Source = this, Mode = BindingMode.TwoWay};
        var itemH4 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinJobsH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInPostedAJob").ResolveLocalizedValue()
                       };
        itemH4.SetBinding(ToggleButton.IsCheckedProperty, bindingH4);
        var bindingH5 = new Binding("ShowJGRP") {Source = this, Mode = BindingMode.TwoWay};
        var itemH5 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinRECUH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInJoinedAGroup").ResolveLocalizedValue()
                       };
        itemH5.SetBinding(ToggleButton.IsCheckedProperty, bindingH5);
        var bindingH6 = new Binding("ShowRECU") {Source = this, Mode = BindingMode.TwoWay};
        var itemH6 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinJGRPH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedinRecommendations").ResolveLocalizedValue()
                       };
        itemH6.SetBinding(ToggleButton.IsCheckedProperty, bindingH6);
        var bindingH7 = new Binding("ShowPRFU") {Source = this, Mode = BindingMode.TwoWay};
        var itemH7 = new ToggleButton
                       {
                         Content =
                           new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinPRFUH2"))},
                         ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:txtLinkedInChangedProfile").ResolveLocalizedValue()
                       };
        itemH7.SetBinding(ToggleButton.IsCheckedProperty, bindingH7);

        //Create lists of submenu
        var lstItemH = new List<ToggleButton>
                         {
                           itemH0,
                           itemH1,
                           itemH2,
                           itemH3,
                           itemH4,
                           itemH5,
                           itemH6,
                           itemH7
                         };
        //Create MenuItems
        //Home        
        var accItem1 = new AccordianItem
                         {
                           Header = new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleFacebookHomeH1"))},
                           ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabHome").ResolveLocalizedValue(),
                           Content = new ListBox {ItemsSource = lstItemH},
                         };
        var bindingAccItem1 = new Binding("ShowHome") {Source = this, Mode = BindingMode.TwoWay};

        accItem1.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem1);
        //Friends
        var accItem2 = new AccordianItem
                         {
                           Header = new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleFacebookFriendsH1"))},
                           ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabConnections").ResolveLocalizedValue(),
                         };
        var bindingAccItem2 = new Binding("ShowFriends") {Source = this, Mode = BindingMode.TwoWay};
        accItem2.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem2);
        //Events
        var accItem3 = new AccordianItem
                         {
                           Header = new System.Windows.Shapes.Path {Style = (Style) (Application.Current.TryFindResource("PathStyleLinkedinSearchH1"))},
                           ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabSearch").ResolveLocalizedValue(),
                         };
        var bindingAccItem3 = new Binding("ShowSearch") {Source = this, Mode = BindingMode.TwoWay};

        accItem3.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem3);
        var lst = new List<AccordianItem>
                    {
                      accItem1,
                      accItem2,
                      accItem3
                    };
        var acc = new Accordian
                    {
                      Source = lst,
                      VerticalAlignment = VerticalAlignment.Top,
                      HorizontalAlignment = HorizontalAlignment.Stretch,
                      ExpandedItem = accItem1
                    };
        return acc;
      }
    }
#else
    public override Object MenuItems
    {
        get
        {
            //Create Binding and items for Home
            var bindingH0 = new Binding("IsShowAll") { Source = this, Mode = BindingMode.TwoWay };
            var itemH0 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinHomeH1"]) } };
            ToolTipService.SetToolTip(itemH0, LocalizationManager.GetString("txtAll"));
            itemH0.SetBinding(ToggleButton.IsCheckedProperty, bindingH0);

            var bindingH1 = new Binding("ShowCONN") { Source = this, Mode = BindingMode.TwoWay };
            var itemH1 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinConnectH2"]) } };
            ToolTipService.SetToolTip(itemH1, LocalizationManager.GetString("txtLinkedInConnectionupdates"));
            itemH1.SetBinding(ToggleButton.IsCheckedProperty, bindingH1);

            var bindingH2 = new Binding("ShowSTAT") { Source = this, Mode = BindingMode.TwoWay };
            var itemH2 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinStatusH2"]) } };
            ToolTipService.SetToolTip(itemH2, LocalizationManager.GetString("txtLinkedInStatusupdates"));
            itemH2.SetBinding(ToggleButton.IsCheckedProperty, bindingH2);

            var bindingH3 = new Binding("ShowAPPS") { Source = this, Mode = BindingMode.TwoWay };
            var itemH3 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinAppsH2"]) } };
            ToolTipService.SetToolTip(itemH3, LocalizationManager.GetString("txtLinkedInApplicationupdates"));
            itemH3.SetBinding(ToggleButton.IsCheckedProperty, bindingH3);

            var bindingH4 = new Binding("ShowJOBS") { Source = this, Mode = BindingMode.TwoWay };
            var itemH4 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinJobsH2"]) } };
            ToolTipService.SetToolTip(itemH4, LocalizationManager.GetString("txtLinkedInPostedAJob"));
            itemH4.SetBinding(ToggleButton.IsCheckedProperty, bindingH4);

            var bindingH5 = new Binding("ShowJGRP") { Source = this, Mode = BindingMode.TwoWay };
            var itemH5 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinRECUH2"]) } };
            ToolTipService.SetToolTip(itemH5, LocalizationManager.GetString("txtLinkedInJoinedAGroup"));
            itemH5.SetBinding(ToggleButton.IsCheckedProperty, bindingH5);

            var bindingH6 = new Binding("ShowRECU") { Source = this, Mode = BindingMode.TwoWay };
            var itemH6 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinJGRPH2"]) } };
            ToolTipService.SetToolTip(itemH6, LocalizationManager.GetString("txtLinkedinRecommendations"));
            itemH6.SetBinding(ToggleButton.IsCheckedProperty, bindingH6);

            var bindingH7 = new Binding("ShowPRFU") { Source = this, Mode = BindingMode.TwoWay };
            var itemH7 = new ToggleButton { Content = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinPRFUH2"]) } };
            ToolTipService.SetToolTip(itemH7, LocalizationManager.GetString("txtLinkedInChangedProfile"));
            itemH7.SetBinding(ToggleButton.IsCheckedProperty, bindingH7);

            //Create lists of submenu
            var lstItemH = new List<ToggleButton>
                         {
                           itemH0,
                           itemH1,
                           itemH2,
                           itemH3,
                           itemH4,
                           itemH5,
                           itemH6,
                           itemH7
                         };


            //Create MenuItems
            //Home     
            var accItem1 = new AccordionItem
            {
             
              Header = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinHomeH1"]) },
              Content = new ListBox { ItemsSource = lstItemH },
            };
            var bindingAccItem1 = new Binding("ShowHome") { Source = this, Mode = BindingMode.TwoWay };
            accItem1.SetBinding(AccordionItem.IsSelectedProperty, bindingAccItem1);
            ToolTipService.SetToolTip(accItem1, LocalizationManager.GetString("rdTabHome")); 


            //Friends
            var accItem2 = new AccordionItem
            {
              Header = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleFacebookFriendsH1"]) }, 
            };
            var bindingAccItem2 = new Binding("ShowFriends") { Source = this, Mode = BindingMode.TwoWay };
            accItem2.SetBinding(AccordionItem.IsSelectedProperty, bindingAccItem2);
            ToolTipService.SetToolTip(accItem2, LocalizationManager.GetString("rdTabConnections")); 


            //Search
            var accItem3 = new AccordionItem
            {
              Header = new ContentControl { Style = (Style)(Application.Current.Resources["CCStyleLinkedinSearchH1"]) }, 
            };
            var bindingAccItem3 = new Binding("ShowSearch") { Source = this, Mode = BindingMode.TwoWay };
            accItem3.SetBinding(AccordionItem.IsSelectedProperty, bindingAccItem3);
            ToolTipService.SetToolTip(accItem3, LocalizationManager.GetString("rdTabSearch"));


            var lst = new List<AccordionItem>
                    {
                      accItem1,
                      accItem2,
                      accItem3
                    };

            var acc = new Accordion
            {
              VerticalAlignment = VerticalAlignment.Top,
              HorizontalAlignment = HorizontalAlignment.Stretch,
              SelectedItem = accItem1,
              ItemsSource = lst,
            };            
            return acc;

        }
    }
#endif

    public override DataTemplate DataTemplateView
    {
      get
      {
        switch (CurrentMainView)
        {
          case LinkedInMainTypeView.Credentials:
            return CredentialsViewModel.DataTemplateView;
          case LinkedInMainTypeView.Base:
            return base.DataTemplateView;
          default:
            return base.DataTemplateView;
        }
      }
      set { base.DataTemplateView = value; }
    }

    public override BWorkspaceViewModel MainViewModel
    {
      get
      {
        switch (CurrentView)
        {
          case LinkedInTypeView.Home:
            return HomeViewModel;
          case LinkedInTypeView.Friends:
            return FriendsViewModel;
          case LinkedInTypeView.Search:
            return SearchViewModel;
          case LinkedInTypeView.Credentials:
            return null;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainViewModel = value; }
    }

    public override DataTemplate FrontTemplateView
    {
      get
      {
        switch (CurrentFrontView)
        {
          case LinkedInFrontView.None:
            return null;
          case LinkedInFrontView.Settings:
            return SettingsViewModel.DataTemplateView;
          case LinkedInFrontView.Profile:
            return ProfileViewModel.DataTemplateView;
        }
        return base.FrontTemplateView;
      }
      set { base.FrontTemplateView = value; }
    }

    public override BWorkspaceViewModel FrontViewModel
    {
      get
      {
        switch (CurrentFrontView)
        {
          case LinkedInFrontView.None:
            return null;
          case LinkedInFrontView.Settings:
            return SettingsViewModel;
          case LinkedInFrontView.Profile:
            return ProfileViewModel;
        }
        return base.FrontViewModel;
      }
      set { base.FrontViewModel = value; }
    }

    #endregion

    #endregion

    #region Status

    protected override bool CanCancelPostStatus()
    {
      return true;
    }

    protected override void CancelPostStatus()
    {
      try
      {
        Status = "";
      }
      catch (Exception)
      {
      }
      IsStatusZoneOpen = false;
    }

    protected override bool CanPostStatus()
    {
      return base.CanPostStatus();
    }

    protected override void PostStatus()
    {
      try
      {
        var statusTextTemp = RemoveAccentFromString(Status);
#if !SILVERLIGHT
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
                               try
                               {
                                 if (LinkedInLibV2.PostShare(statusTextTemp))
                                 {
                                   if (SobeesSettings.CloseBoxPublication)
                                   {
                                     IsStatusZoneOpen = false;
                                   }

                                   Status = "";
                                 }
                               }
                               catch (Exception ex)
                               {
                                 var txtError = new LocText("Sobees.Configuration.BGlobals:Resources:errorLinkedIn").ResolveLocalizedValue();
                                 if (MessengerInstance != null)
                                   MessengerInstance.Send(new BMessage("ShowError", txtError));
                                 TraceHelper.Trace(this, ex);
                               }
                             };

          worker.RunWorkerAsync();
        }
#else
        StatusTextTemp = HttpUtility.UrlDecode(StatusTextTemp);
        StatusTextTemp = HttpUtility.HtmlEncode(StatusTextTemp);
        ProxyLinkedIn.SetStatusAsyncAsync(Globals.LINKEDIN_SL_KEY, 
                                  Globals.LINKEDIN_SL_SECRET, 
                                  SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].SessionKey,
                                  SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].Secret,
                                  StatusTextTemp);
#endif
      }
      catch (Exception ex)
      {
        EndUpdateAll();
        TraceHelper.Trace(this, ex);
      }
    }

    public static string RemoveAccentFromString(string chaine)
    {
      chaine = chaine.Replace("“", "''");
      chaine = chaine.Replace("”", "''");
      var txt = chaine;
      try
      {
        // Déclaration de variables
        const string accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
        const string sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

        // Conversion des chaines en tableaux de caractères
        var tableauSansAccent = sansAccent.ToCharArray();
        var tableauAccent = accent.ToCharArray();

        // Pour chaque accent
        for (var i = 0; i < accent.Length; i++)
        {
          // Remplacement de l'accent par son équivalent sans accent dans la chaîne de caractères
          chaine = chaine.Replace(tableauAccent[i].ToString(),
                                  tableauSansAccent[i].ToString());
        }

        chaine = chaine.Replace("&", " ");
      }
      catch (Exception ex)
      {
        return txt;
      }
      // Retour du résultat
      return chaine;
    }

    #endregion

    #region Constructors

    public LinkedInViewModel(BPosition bposition, BServiceWorkspace serviceWorkspace, string settings)
      : base(bposition, serviceWorkspace)
    {
      CurrentView = LinkedInTypeView.Credentials;
      CurrentMainView = LinkedInMainTypeView.Credentials;
      //Register to the Messenger
      ServiceMessenger.Register<string>(this, DoAction);
      ServiceMessenger.Register<BMessage>(this, DoActionMessage);
      //restore Settings
      if (string.IsNullOrEmpty(settings))
      {
        Settings = new LinkedInSettings();
      }
      else
      {
        Settings = GenericSerializer.DeserializeObject(settings, typeof (LinkedInSettings)) as LinkedInSettings;
      }
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "Connected":
          CurrentView = LinkedInTypeView.Home;
          CurrentMainView = LinkedInMainTypeView.Base;
          RaisePropertyChanged("DataTemplateView");
          Messenger.Default.Send("SaveSettings");

          break;

        case "ConnectedAfterLoginSL":
          CurrentView = LinkedInTypeView.Home;
          CurrentMainView = LinkedInMainTypeView.Base;
          RaisePropertyChanged("DataTemplateView");
          Messenger.Default.Send("SaveSettings");
          break;

        case "ConnectedStored":
          CurrentView = LinkedInTypeView.Home;
          CurrentMainView = LinkedInMainTypeView.Base;
          RaisePropertyChanged("DataTemplateView");
          //Messenger.Default.Send("SaveSettings");
          break;
        case "CloseProfile":
          CurrentFrontView = LinkedInFrontView.None;
          _profileViewModel.Cleanup();
          _profileViewModel = null;
          RaisePropertyChanged("FrontTemplateView");
          RaisePropertyChanged("FrontViewModel");
          break;
        case "SaveSettingsLI":
          SaveSettings();
          CloseSettings();
          Messenger.Default.Send("SaveSettings");
          break;
        case "CloseSettingsLI":
          CloseSettings();
          break;
        case "UpdateViewFilter":
          ServiceMessenger.Send("UpdateView");
          break;
      }
      base.DoAction(param);
    }


    private void CloseSettings()
    {
      CurrentFrontView = LinkedInFrontView.None;
      RaisePropertyChanged("FrontTemplateView");
      RaisePropertyChanged("FrontViewModel");
      _settingsViewModel.Cleanup();
      _settingsViewModel = null;
    }

    private void SaveSettings()
    {
      if (!SettingsViewModel.IsDirty)
      {
        return;
      }
      try
      {
        if (Settings.NbMaxPosts != _settingsViewModel.MaxTweets)
        {
          Settings.NbMaxPosts = _settingsViewModel.MaxTweets;
        }

        if (Settings.NbPostToGet != _settingsViewModel.Rpp)
        {
          Settings.NbPostToGet = _settingsViewModel.Rpp;
        }
        if (Settings.RefreshTime != _settingsViewModel.RefreshTime)
        {
          Settings.RefreshTime = _settingsViewModel.RefreshTime;
        }
        ServiceMessenger.Send("SettingsUpdated");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public override void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
        case "ShowError":
          ShowErrorMsg(message.Parameter as string);
          break;
        case "ShowProfile":
          OpenProfile(message.Parameter as string);
          break;
      }
      base.DoActionMessage(message);
    }

    public override void DeleteUser(UserAccount account)
    {
      if (account.Type == EnumAccountType.LinkedIn && account.Login == Settings.UserName)
      {
        IsClosed = true;
        CloseControl();
      }
    }

    private void OpenProfile(string id)
    {
      CurrentFrontView = LinkedInFrontView.Profile;
      ProfileViewModel.ShowUser(id);
      RaisePropertyChanged("FrontTemplateView");
      RaisePropertyChanged("FrontViewModel");
    }

    #endregion

    public override void ShowSettings()
    {
      CurrentFrontView = LinkedInFrontView.Settings;
      RaisePropertyChanged("FrontTemplateView");
      RaisePropertyChanged("FrontViewModel");
    }

    public override void SetAccount(UserAccount account)
    {
      try
      {
        LinkedInLibV2.Token = account.SessionKey;
        LinkedInLibV2.TokenSecret = account.Secret;
        Settings.UserName = account.Login;
        CurrentView = LinkedInTypeView.Home;
        CurrentMainView = LinkedInMainTypeView.Base;
        RaisePropertyChanged("DataTemplateView");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }


    public override void UpdateAll()
    {
      try
      {
        if (Friends == null)
        {
#if SILVERLIGHT
          ProxyLinkedIn.GetConnectionsAsyncAsync(Globals.LINKEDIN_SL_KEY,
                                    Globals.LINKEDIN_SL_SECRET,
                                    SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].SessionKey,
                                    SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].Secret);
          return;
#endif
        }
        if (HomeViewModel != null) HomeViewModel.Refresh();
        EndUpdateAll();
        //if (FriendsViewModel != null) FriendsViewModel.Refresh();

        //if (SearchViewModel != null) SearchViewModel.Refresh();
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
        EndUpdateAll();
      }
    }
  }
}