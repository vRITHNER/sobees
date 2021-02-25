#region

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.Facebook.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml;
using Sobees.Library.BFacebookLibV2.Exceptions;
using Sobees.Library.BFacebookLibV2.Login;
using Sobees.Library.BFacebookLibV2.Objects.Users;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ListBox = System.Windows.Controls.ListBox;
using Path = System.Windows.Shapes.Path;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  

  public class FacebookViewModel : BServiceWorkspaceViewModel
  {
    #region Fields

    private bool _showAppsHome = true;
    private bool _showHome = true;
    private bool _showLinksHome = true;
    private bool _showMyHome;

    //private bool _showInBox;
    //private bool _showFriends;
    //private bool _showPagesHome = true;

    //private bool _showVideosHome = true;
    private bool _showStatusHome = true;

    #region PostingStatus

    private string _fileName;
    private string _imageName;
    private Visibility _isDelBtnImgShow = Visibility.Collapsed;
    private Visibility _isVisibleBtnLink = Visibility.Visible;
    private Visibility _isVisibleBtnPhoto = Visibility.Visible;
    private Visibility _isVisibleBtnVideo = Visibility.Visible;
    private string _linkUrl;

    private album _selectedAlbums;
    private string _videoName;
    private string _videoTitle;

    #endregion

    #region Workspaces

    private CredentialsViewModel _credentialsViewModel;
    private HomeWorkspaceViewModel _homeWorkspaceViewModel;

    private MyHomeWorkspaceViewModel _myHomeWorkspaceViewModel;
    private ProfileViewModel _profileViewModel;
    private SettingsViewModel _settingsViewModel;

    #endregion

    //private bool _showPhotosHome = true;
    //private bool _showOnline;
    //private bool _showBirthdays;
    //private bool _showEvents;
    //private bool _showEvent;

    #endregion

    #region Properties

    public override Object MenuItems
    {
      get
      {
        //Create Binding and items for Home
        var bindingH1 = new Binding("ShowStatusHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemH1 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleFeedH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowStatusOnly").ResolveLocalizedValue()
        };
        itemH1.SetBinding(ToggleButton.IsCheckedProperty, bindingH1);
        var bindingH2 = new Binding("ShowLinksHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemH2 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleLinksOnlyH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowLinksOnly").ResolveLocalizedValue()
        };
        itemH2.SetBinding(ToggleButton.IsCheckedProperty, bindingH2);

        //var bindingH3 = new Binding("ShowPhotosHome") {Source = this, Mode = BindingMode.TwoWay};
        //var itemH3 = new ToggleButton
        //               {
        //                 Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStylePhotoH2"))},
        //                 ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowAlbumsOnly").ResolveLocalizedValue()
        //               };
        //itemH3.SetBinding(ToggleButton.IsCheckedProperty, bindingH3);
        //var bindingH4 = new Binding("ShowPagesHome") {Source = this, Mode = BindingMode.TwoWay};
        //var itemH4 = new ToggleButton
        //               {
        //                 Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStylePagesH2"))},
        //                 ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowPagesOnly").ResolveLocalizedValue()
        //               };
        //itemH4.SetBinding(ToggleButton.IsCheckedProperty, bindingH4);
        var bindingH5 = new Binding("ShowAppsHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemH5 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleAppH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowAppsOnly").ResolveLocalizedValue()
        };
        itemH5.SetBinding(ToggleButton.IsCheckedProperty, bindingH5);
        var bindingH6 = new Binding("ShowVideosHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemH6 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleVideoH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowVideosOnly").ResolveLocalizedValue()
        };
        itemH6.SetBinding(ToggleButton.IsCheckedProperty, bindingH6);

        //Create Binding and items for Friends
        //var bindingF1 = new Binding("ShowOnline") { Source = this, Mode = BindingMode.TwoWay };
        //var itemF1 = new ToggleButton
        //                 {
        //                     Content =
        //                         new System.Windows.Shapes.Path
        //                             {
        //                                 Style = (Style) (Application.Current.TryFindResource("PathStyleFacebookOnlineH2"))
        //                             },
        //                     ToolTip =
        //                         new LocText("Sobees.Configuration.BGlobals:Resources:btnShowOnlineOnlyMyspace").
        //                         ResolveLocalizedValue()
        //                 };
        //  itemF1.SetBinding(ToggleButton.IsCheckedProperty, bindingF1);
        //Create Binding and items for Events
        //var bindingE1 = new Binding("ShowBirthsDay") { Source = this, Mode = BindingMode.TwoWay };
        //var itemE1 = new ToggleButton
        //                 {
        //                     Content =
        //                         new System.Windows.Shapes.Path
        //                             {
        //                                 Style =
        //                                     (Style)
        //                                     (Application.Current.TryFindResource("PathStyleFacebookBirthdayH2"))
        //                             },
        //                     ToolTip =
        //                         new LocText("Sobees.Configuration.BGlobals:Resources:btnShowBirthdayOnlyMyspace").
        //                         ResolveLocalizedValue()
        //                 };
        //  itemE1.SetBinding(ToggleButton.IsCheckedProperty, bindingE1);
        //var bindingE2 = new Binding("ShowEvent") { Source = this, Mode = BindingMode.TwoWay };
        //var itemE2 = new ToggleButton
        //                 {
        //                     Content =
        //                         new System.Windows.Shapes.Path
        //                             {Style = (Style) (Application.Current.TryFindResource("PathStyleFacebookEventH2"))},
        //                     ToolTip =
        //                         new LocText("Sobees.Configuration.BGlobals:Resources:btnShowEventOnlyMyspace").
        //                         ResolveLocalizedValue()
        //                 };
        //  itemE2.SetBinding(ToggleButton.IsCheckedProperty, bindingE2);

        //Create Binding and items for Myhome
        var bindingMh1 = new Binding("ShowStatusHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemMh1 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleFeedH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowStatusOnly").ResolveLocalizedValue()
        };
        itemMh1.SetBinding(ToggleButton.IsCheckedProperty, bindingMh1);
        var bindingMh2 = new Binding("ShowLinksHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemMh2 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleLinksOnlyH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowLinksOnly").ResolveLocalizedValue()
        };
        itemMh2.SetBinding(ToggleButton.IsCheckedProperty, bindingMh2);
        var bindingMh3 = new Binding("ShowPhotosHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemMh3 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStylePhotoH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowAlbumsOnly").ResolveLocalizedValue()
        };
        itemMh3.SetBinding(ToggleButton.IsCheckedProperty, bindingMh3);

        //var bindingMh4 = new Binding("ShowPagesHome") {Source = this, Mode = BindingMode.TwoWay};
        //var itemMh4 = new ToggleButton
        //                {
        //                  Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStylePagesH2"))},
        //                  ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowAlbumsOnly").ResolveLocalizedValue()
        //                };
        //itemMh4.SetBinding(ToggleButton.IsCheckedProperty, bindingMh4);
        var bindingMh5 = new Binding("ShowAppsHome") { Source = this, Mode = BindingMode.TwoWay };
        var itemMh5 = new ToggleButton
        {
          Content = new Image { Style = (Style)(Application.Current.TryFindResource("ImageStyleAppH2")) },
          ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowAlbumsOnly").ResolveLocalizedValue()
        };
        itemMh5.SetBinding(ToggleButton.IsCheckedProperty, bindingMh5);

        //var bindingMh6 = new Binding("ShowVideosHome") {Source = this, Mode = BindingMode.TwoWay};
        //var itemMh6 = new ToggleButton
        //                {
        //                  Content = new Image {Style = (Style) (Application.Current.TryFindResource("ImageStyleVideoH2"))},
        //                  ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:btnShowVideosOnly").ResolveLocalizedValue()
        //                };
        //itemMh6.SetBinding(ToggleButton.IsCheckedProperty, bindingMh6);
        //Create Binding and items for Inbox
        var bindingI1 = new Binding("ShowOnlyNewMails") { Source = this, Mode = BindingMode.TwoWay };
        var itemI1 = new ToggleButton
        {
          Content = new Path { Style = (Style)(Application.Current.TryFindResource("PathStyleFacebookOnlyNewMailH2")) },
          ToolTip =
            new LocText("Sobees.Configuration.BGlobals:Resources:btnShowNewMailOnlyMyspace").ResolveLocalizedValue()
        };
        itemI1.SetBinding(ToggleButton.IsCheckedProperty, bindingI1);

        //Create lists of submenu
        var lstItemH = new List<ToggleButton> { itemH1, itemH2, itemH5 };

        //var lstItemMh = new List<ToggleButton> {itemMh1, itemMh2, itemMh3, itemMh6};
        //var lstItemF = new List<ToggleButton>
        //                 {
        //                   itemF1
        //                 };
        //var lstItemE = new List<ToggleButton>
        //                 {
        //                   itemE1,
        //                   itemE2
        //                 };
        //var lstItemI = new List<ToggleButton>
        //                 {
        //                   itemI1
        //                 };

        //Create MenuItems
        //Home
        var accItem1 = new AccordianItem
        {
          Header = new Path { Style = (Style)(Application.Current.TryFindResource("PathStyleFacebookHomeH1")) },
          Content = new ListBox { ItemsSource = lstItemH },
        };
        var bindingAccItem1 = new Binding("ShowHome") { Source = this, Mode = BindingMode.TwoWay };
        accItem1.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabHome").ResolveLocalizedValue();
        accItem1.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem1);

        ////Friends
        //var accItem2 = new AccordianItem
        //{
        //  Header = new System.Windows.Shapes.Path { Style = (Style)(Application.Current.TryFindResource("PathStyleFacebookFriendsH1")) },
        //  Content = new ListBox { ItemsSource = lstItemF },
        //};
        //var bindingAccItem2 = new Binding("ShowFriends") { Source = this, Mode = BindingMode.TwoWay };
        //accItem2.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabFriends").ResolveLocalizedValue();
        //accItem2.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem2);

        ////Events
        //var accItem3 = new AccordianItem
        //{
        //  Header = new System.Windows.Shapes.Path { Style = (Style)(Application.Current.TryFindResource("PathStyleFacebookEventsH1")) },
        //  Content = new ListBox { ItemsSource = lstItemE },
        //};
        //var bindingAccItem3 = new Binding("ShowEvents") { Source = this, Mode = BindingMode.TwoWay };
        //accItem3.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabEvents").ResolveLocalizedValue();
        //accItem3.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem3);

        ////Inbox
        //var accItem4 = new AccordianItem
        //{
        //  Header = new System.Windows.Shapes.Path { Style = (Style)(Application.Current.TryFindResource("PathStyleFacebookInboxH1")) },
        //  Content = new ListBox { ItemsSource = lstItemI },
        //};
        //var bindingAccItem4 = new Binding("ShowInBox") { Source = this, Mode = BindingMode.TwoWay };
        //accItem4.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabMail").ResolveLocalizedValue();
        //accItem4.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem4);

        //MyHome
        //var accItem5 = new AccordianItem
        //                 {
        //                   Header = new Path {Style = (Style) (Application.Current.TryFindResource("PathStyleFacebookProfileH1"))},
        //                   Content = new ListBox {ItemsSource = lstItemMh},
        //                 };
        //var bindingAccItem5 = new Binding("ShowMyHome") {Source = this, Mode = BindingMode.TwoWay};
        //accItem5.ToolTip = new LocText("Sobees.Configuration.BGlobals:Resources:rdTabProfile").ResolveLocalizedValue();
        //accItem5.SetBinding(AccordianItem.IsExpandedProperty, bindingAccItem5);
        var lst = new List<AccordianItem>
        {
          accItem1,

          //accItem5,
          //accItem2,
          //accItem3,
          //accItem4
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

    public DesktopSession CurrentSession { get; private set; }

    #region Workspaces

    public SettingsViewModel SettingsViewModel => _settingsViewModel ?? (_settingsViewModel = new SettingsViewModel(this, ServiceMessenger));

    public ProfileViewModel ProfileViewModel => _profileViewModel ?? (_profileViewModel = new ProfileViewModel(this, ServiceMessenger));

    public HomeWorkspaceViewModel HomeWorkspaceViewModel => _homeWorkspaceViewModel ?? (_homeWorkspaceViewModel = new HomeWorkspaceViewModel(this, ServiceMessenger));

    public MyHomeWorkspaceViewModel MyHomeWorkspaceViewModel => _myHomeWorkspaceViewModel ??
                                                                (_myHomeWorkspaceViewModel = new MyHomeWorkspaceViewModel(this, ServiceMessenger));

    public CredentialsViewModel CredentialsViewModel => _credentialsViewModel ?? (_credentialsViewModel = new CredentialsViewModel(this, ServiceMessenger));

    #endregion

    #region Show Properties

    public bool IsPosting;

    public override BWorkspaceViewModel ProfileView
    {
      get { return _profileViewModel; }
      set { base.ProfileView = value; }
    }

    public override DataTemplate ProfileTemplate => _profileViewModel == null ? null : _profileViewModel.DataTemplateView;

    public override string ImageUser => SobeesSettings.Accounts[
      SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].PictureUrl;

    public bool ShowHome
    {
      get { return _showHome; }
      set
      {
        _showHome = value;
        if (_showHome)
        {
          CurrentColumnView = FacebookColomnView.Home;
          RaisePropertyChanged("MainViewModel");
          RaisePropertyChanged("MainTemplateView");
        }
        CloseProfile();
      }
    }

    public bool ShowMyHome
    {
      get { return _showMyHome; }
      set
      {
        _showMyHome = value;
        if (_showMyHome)
        {
          CurrentColumnView = FacebookColomnView.MyHome;
          RaisePropertyChanged("MainViewModel");
          RaisePropertyChanged("MainTemplateView");
        }
        CloseProfile();
      }
    }

    public bool ShowAppsHome
    {
      get { return _showAppsHome; }
      set
      {
        _showAppsHome = value;
        FilterChanged();
      }
    }

    public bool ShowStatusHome
    {
      get { return _showStatusHome; }
      set
      {
        _showStatusHome = value;
        FilterChanged();
      }
    }

    public bool ShowLinksHome
    {
      get { return _showLinksHome; }
      set
      {
        _showLinksHome = value;
        FilterChanged();
      }
    }

    public FacebookTypeView CurrentView { get; set; }

    public FacebookColomnView CurrentColumnView { get; set; }

    #region PostingStatus

    public bool IsUpdating;
    private ObservableCollection<album> _listAlbums;

    public ICollectionView LcvAlbum { get; set; }

    public byte[] ImageFileInfo { get; set; }

    public album SelectedAlbums
    {
      get { return _selectedAlbums; }
      set
      {
        _selectedAlbums = value;

        RaisePropertyChanged();
        RaisePropertyChanged("IsActiv");
      }
    }

    public ObservableCollection<album> ListAlbums
    {
      get { return _listAlbums ?? (_listAlbums = new ObservableCollection<album>()); }
      set { _listAlbums = value; }
    }

    public long AlbumId { get; set; }

    public string VideoName
    {
      get { return _videoName; }
      set
      {
        _videoName = value;
        RaisePropertyChanged();
      }
    }

    public string VideoTitle
    {
      get { return _videoTitle; }
      set
      {
        _videoTitle = value;
        RaisePropertyChanged();
      }
    }

    public string LinkUrl
    {
      get { return _linkUrl; }
      set
      {
        _linkUrl = value;
        RaisePropertyChanged();
      }
    }

    public string ImageName
    {
      get { return _imageName; }
      set
      {
        _imageName = value;
        RaisePropertyChanged();
        RaisePropertyChanged("IsImgShow");
      }
    }

    public string FileName
    {
      get { return _fileName; }
      set
      {
        _fileName = value;
        RaisePropertyChanged();
        RaisePropertyChanged("IsImgShow");
      }
    }

    public Visibility IsImgShow => String.IsNullOrEmpty(ImageName) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility IsDelBtnImgShow
    {
      get { return _isDelBtnImgShow; }
      set
      {
        _isDelBtnImgShow = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsVisibleBtnPhoto
    {
      get { return _isVisibleBtnPhoto; }
      set
      {
        _isVisibleBtnPhoto = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsVisibleBtnLink
    {
      get { return _isVisibleBtnLink; }
      set
      {
        _isVisibleBtnLink = value;
        RaisePropertyChanged();
      }
    }

    public Visibility IsVisibleBtnVideo
    {
      get { return _isVisibleBtnVideo; }
      set
      {
        _isVisibleBtnVideo = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region DataTemplate

    public override DataTemplate FrontTemplateView
    {
      get
      {
        switch (CurrentFrontView)
        {
        case FacebookFrontView.Settings:
          return SettingsViewModel.DataTemplateView;

        case FacebookFrontView.None:
          return null;

        case FacebookFrontView.MailBox:
          return null;

        case FacebookFrontView.Profile:
          return ProfileViewModel.DataTemplateView;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.FrontTemplateView = value; }
    }

    public override BWorkspaceViewModel FrontViewModel
    {
      get
      {
        switch (CurrentFrontView)
        {
        case FacebookFrontView.Settings:
          return SettingsViewModel;

        case FacebookFrontView.None:
          return null;

        case FacebookFrontView.MailBox:
          return null;

        case FacebookFrontView.Profile:
          return ProfileViewModel;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.FrontViewModel = value; }
    }

    public DataTemplate HomeDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:Home/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate FriendsDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:Friends/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate EventsDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:Events/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate EventsBirthsdayDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:EventsBirthsday/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate BirthsdayDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:Birthsday/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate CredentialsDataTemplate
    {
      get
      {
        const string DT =
          "<DataTemplate x:Name='dtMainView' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook' " +
          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" + "<Views:Credentials/> " + "</DataTemplate>";

        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        switch (CurrentView)
        {
        case FacebookTypeView.Credentials:
          return CredentialsDataTemplate;

        case FacebookTypeView.Base:
          return base.DataTemplateView;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainTemplateView = value; }
    }

    public override DataTemplate MainTemplateView
    {
      get
      {
        switch (CurrentColumnView)
        {
        case FacebookColomnView.Home:
          return HomeDataTemplate;

        case FacebookColomnView.MyHome:
          return HomeDataTemplate;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainTemplateView = value; }
    }

    public FacebookFrontView CurrentFrontView { get; set; }

    public override BWorkspaceViewModel MainViewModel
    {
      get
      {
        switch (CurrentColumnView)
        {
        case FacebookColomnView.Home:
          return HomeWorkspaceViewModel;

        case FacebookColomnView.MyHome:
          return MyHomeWorkspaceViewModel;

        default:
          throw new ArgumentOutOfRangeException();
        }
      }
      set { base.MainViewModel = value; }
    }

    #endregion

    #endregion

    #endregion

    #region Commands

    public RelayCommand UploadImageCommand { get; set; }

    public RelayCommand DeleteImageCommand { get; set; }

    #endregion

    #region Constructors

    public FacebookViewModel(BPosition bposition, BServiceWorkspace serviceWorkspace, string settings)
      : base(bposition, serviceWorkspace)
    {
      CurrentView = FacebookTypeView.Credentials;
      CurrentColumnView = FacebookColomnView.Home;
      CurrentFrontView = FacebookFrontView.None;
      CurrentSession = new DesktopSession(
        BGlobals.FACEBOOK_WPF_API,
        true,
        new List<Enums.ExtendedPermissions>
        {
          Enums.ExtendedPermissions.user_about_me,
          Enums.ExtendedPermissions.read_stream,
          Enums.ExtendedPermissions.user_friends,
          Enums.ExtendedPermissions.public_profile,
        });

      //Register to the Messenger
      ServiceMessenger.Register<string>(this, DoAction);
      ServiceMessenger.Register<BMessage>(this, DoActionMessage);

      //restore Settings
      if (string.IsNullOrEmpty(settings))
      {
        Settings = new FacebookSettings();
      }
      else
      {
        Settings = GenericSerializer.DeserializeObject(settings, typeof(FacebookSettings)) as FacebookSettings;
      }
      Task.Factory.StartNew(InitCommands);
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
      case "Connected":
        CurrentView = FacebookTypeView.Base;
        RaisePropertyChanged("DataTemplateView");
        Messenger.Default.Send("SaveSettings");
        break;

      case "ConnectedAfterLoginFB":
        CurrentView = FacebookTypeView.Base;
        RaisePropertyChanged("DataTemplateView");
        Messenger.Default.Send("SaveSettings");
        break;

      case "CloseProfile":
        CloseProfile();
        break;

      case "SaveSettingsFB":
        SaveSettings();
        CloseSettings();
        Messenger.Default.Send("SaveSettings");
        break;

      case "CloseSettingsFB":
        CloseSettings();
        break;

      case "UpdateViewFilter":
        ServiceMessenger.Send("UpdateView");
        break;

      case "GoOnline":
        Refresh();
        break;

      case "GoOffline":
        EndUpdateAll();
        break;
      }
      base.DoAction(param);
    }

    public override void DoActionMessage(BMessage message)
    {
      switch (message.Action)
      {
      case "ShowError":
        ShowErrorMsg(message.Parameter as string);
        break;

      case "ShowProfile":
        ShowUser(message.Parameter as FacebookUser);
        break;
      }
      base.DoActionMessage(message);
    }

    protected override void InitCommands()
    {
      DeleteImageCommand = new RelayCommand(DeleteImage);
      UploadImageCommand = new RelayCommand(UploadImage);
      base.InitCommands();
    }

    public override void DeleteUser(UserAccount account)
    {
      if (account.Type != EnumAccountType.Facebook || account.Login != Settings.UserName) return;
      IsClosed = true;
      CloseControl();
    }

    #endregion

    public override void CloseControl()
    {
      if (_settingsViewModel != null)
      {
        _settingsViewModel.Cleanup();
      }
      if (_profileViewModel != null)
      {
        _profileViewModel.Cleanup();
      }
      if (_homeWorkspaceViewModel != null)
      {
        _homeWorkspaceViewModel.Cleanup();
      }
      if (_myHomeWorkspaceViewModel != null)
      {
        _myHomeWorkspaceViewModel.Cleanup();
      }
      if (_credentialsViewModel != null)
      {
        _credentialsViewModel.Cleanup();
      }
      base.CloseControl();
    }

    protected override void CancelPostStatus()
    {
      IsStatusZoneOpen = false;
      Status = "";
      ImageName = "";
      FileName = "";
    }

    public override void UpdateAll()
    {
      IsWaiting = true;
      if (ShowHome)
        HomeWorkspaceViewModel.Refresh();

      if (ShowMyHome)
        MyHomeWorkspaceViewModel.Refresh();
    }

    public void ShowUser(User user)
    {
      ProfileViewModel.UpdateUser(user);
      RaisePropertyChanged("ProfileTemplate");
      RaisePropertyChanged("ProfileView");
    }

    public override void ShowSettings()
    {
      CurrentFrontView = FacebookFrontView.Settings;
      RaisePropertyChanged("FrontTemplateView");
      RaisePropertyChanged("FrontViewModel");
    }

    private void SaveSettings()
    {
      if (!SettingsViewModel.IsDirty)
        return;

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

        // Remove existant spams from current lists!
        SobeesSettings.Accounts[
          SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList.Clear();
        foreach (var spam in _settingsViewModel.Spams)
        {
          SobeesSettings.Accounts[
            SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook))].SpamList.Add(
              spam);
          TraceHelper.Trace(this, "Following SPAM was added: " + spam);
        }

        ServiceMessenger.Send("SettingsUpdated");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    public void CloseSettings()
    {
      CurrentFrontView = FacebookFrontView.None;
      RaisePropertyChanged("FrontTemplateView");
      RaisePropertyChanged("FrontViewModel");
      _settingsViewModel.Cleanup();
      _settingsViewModel = null;
    }

    public override void SetAccount(UserAccount account)
    {
      CurrentSession.ApplicationKey = BGlobals.FACEBOOK_WPF_API;
      CurrentSession.ApplicationSecret = BGlobals.FACEBOOK_WPF_SECRET;
      CurrentSession.AccessToken = account.AuthToken;
      CurrentSession.SessionKey = account.SessionKey;
      CurrentSession.SessionSecret = account.Secret;
      CurrentSession.UserId = account.UserId;
      if (Service == null)
      {
        Service = BindingManager.CreateInstance(CurrentSession);
      }
      Settings.UserName = account.Login;
      CurrentView = FacebookTypeView.Base;
      RaisePropertyChanged("DataTemplateView");
    }

    public void FilterChanged()
    {
      if (_profileViewModel != null)
        CloseProfile();
      MainViewModel.UpdateView();
    }

    private void CloseProfile()
    {
      if (_profileViewModel == null)
        return;
      _profileViewModel.Cleanup();
      _profileViewModel = null;
      RaisePropertyChanged("ProfileTemplate");
      RaisePropertyChanged("ProfileView");
    }

    #region Status

    protected override bool CanPostStatus()
    {
      return base.CanPostStatus() && !IsPosting;
    }

    protected override void PostStatus()
    {
      IsPosting = true;
      if (!(Status != null & Status != "")) return;
      //Dans le cas d'un album
      if (!string.IsNullOrEmpty(ImageName) & SelectedAlbums != null)
      {
        var fileinfo = new FileInfo(ImageName);
        using (var stream = fileinfo.OpenRead())
        {
          var binaryImageData = new byte[stream.Length];
          stream.Read(binaryImageData, 0, binaryImageData.Length);
          ImageFileInfo = binaryImageData;
          Service.Api.Photos.UploadAsync(SelectedAlbums.aid, Status, ImageFileInfo, fileinfo.Extension,
            OnGetPhotoSteamQueryCompleted, null);
        }
      }
      else
      {
        Service.Api.Status.SetAsync(Status, SetStatusCompleted, null);
      }
    }

    private void SetStatusCompleted(bool result, object state, FacebookException e)
    {
      if (e != null)
      {
        ShowErrorMsg(e.Message);
        return;
      }
      try
      {
        if (Application.Current == null || Application.Current.Dispatcher == null)
          return;
        Application.Current.Dispatcher.BeginInvoke(
          new Action(
            () =>
            {
              IsVisibleBtnLink = Visibility.Visible;
              IsVisibleBtnPhoto = Visibility.Visible;
              IsVisibleBtnVideo = Visibility.Visible;
              IsDelBtnImgShow = Visibility.Collapsed;
              IsPosting = false;
              if (SobeesSettings.CloseBoxPublication)
              {
                IsStatusZoneOpen = false;
              }
              ImageName = "";
              FileName = "";
              Status = "";
              Refresh();
            }));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OnGetPhotoSteamQueryCompleted(photo photo, object state, FacebookException e)
    {
      if (e != null)
      {
        ShowErrorMsg(e.Message);
        return;
      }
      try
      {
        if (Application.Current == null || Application.Current.Dispatcher == null)
          return;

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
              IsVisibleBtnLink = Visibility.Visible;
              IsVisibleBtnPhoto = Visibility.Visible;
              IsVisibleBtnVideo = Visibility.Visible;
              IsDelBtnImgShow = Visibility.Collapsed;
              IsPosting = false;
              ImageName = null;
              FileName = null;
              if (SobeesSettings.CloseBoxPublication)
                IsStatusZoneOpen = false;

              Status = "";
              ImageFileInfo = null;
            }));
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #region UploadPhoto

    private void DeleteImage()
    {
      try
      {
        ImageName = null;
        FileName = null;
        IsDelBtnImgShow = Visibility.Collapsed;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void UploadImage()
    {
      try
      {
        Service.Api.Photos.GetAlbumsAsync(CurrentSession.UserId, null, GetAlbumsCompleted, null);
        const string FILTER = "Images |*.bmp;*.jpg;*.gif;*.png;*.psd;*.tiff;*.jp2;*.iff;*.wbmp;*.xbm";
        var ofd = new OpenFileDialog
        {
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
          Filter = FILTER,
          RestoreDirectory = true
        };

        var dr = ofd.ShowDialog();
        if (dr != DialogResult.OK) return;
        ImageName = ofd.FileName;
        IsVisibleBtnLink = Visibility.Collapsed;
        IsVisibleBtnVideo = Visibility.Collapsed;
        IsVisibleBtnPhoto = Visibility.Collapsed;
        IsDelBtnImgShow = Visibility.Visible;
      }
      catch (Exception ex)
      {
        FileName = null;
        ImageName = null;
        TraceHelper.Trace(this, ex);
      }
    }

    private void GetAlbumsCompleted(IEnumerable<album> albums, object state, FacebookException e)
    {
      if (e != null)
      {
        ShowErrorMsg(e.Message);
        return;
      }
      try
      {
        foreach (var album in albums)
        {
          if (album.type == "profile")
            continue;

          var album1 = album;
          if ((from i in ListAlbums where i.name == album1.name select album1).Count() != 0)
            continue;
          Application.Current.Dispatcher.BeginInvoke(new Action(() =>
              {
                ListAlbums.Add(album1);
                SelectedAlbums = ListAlbums[0];
              }));
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #endregion
  }
}