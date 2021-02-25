#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Twitter.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Library.BGenericLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class ListViewModel : BTwitterViewModel
  {
    #region Constructors

    protected ListViewModel(TwitterViewModel model, Messenger messenger, TwitterWorkspaceSettings settings)
      : base(model, messenger, settings)
    {
    }

    public ListViewModel(TwitterViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      if (IsInDesignModeStatic)
      {
        ListMembers.Add(new User
                          {
                            Name = "Mickey Mouse",
                            NickName = "Mickey",
                            ProfileImgUrl = "http://s.twimg.com/a/1257878623/images/default_profile_0_normal.png"
                          });
        return;
      }
      MessengerInstance.Register<string>(this, DoAction);

      InitCommands();
    }

    protected void Dispose()
    {
      _deleteCommand = null;
      _addUserCommand = null;
      if (_listMembers != null) ListMembers.Clear();
      _selectedList = null;
      _selectedUser = null;
      if (_twitterList != null) TwitterList.Clear();

      base.Dispose();
    }

    #endregion Constructors

    #region Fields

    private static List<Brush> _colorList;
    private readonly string _currentList;
    private Visibility _createNewListVisibility = Visibility.Collapsed;
    private int _currentIndexListSelected = -1;
    private bool _isPrivate = true;
    private ObservableCollection<User> _listMembers;
    private string _nameNewList;
    private TwitterListShow _selectedList;
    private User _selectedUser;
    private ObservableCollection<TwitterListShow> _twitterList;
    private string _userToAdd;

    #endregion Fields

    #region Properties

    private ListEditViewModel _editListView;

    public ObservableCollection<TwitterListShow> TwitterList
    {
      get
      {
        if (_twitterList == null)
        {
          _twitterList = new ObservableCollection<TwitterListShow>();
          UpdateList();
        }
        return _twitterList;
      }
    }

    public List<Brush> ColorList => _colorList ?? (_colorList = new List<Brush>
    {
      Brushes.Blue.Clone(),
      Brushes.Red.Clone(),
      Brushes.Yellow.Clone(),
      Brushes.Green.Clone(),
      Brushes.Pink.Clone(),
      Brushes.DarkViolet.Clone()
    });

    public User UserAddSL { get; set; }

    public List<User> Friends => null;

    public Visibility DetailsViewVisibility => SelectedList == null ? Visibility.Collapsed : Visibility.Visible;

    public Visibility CreateNewListVisibility
    {
      get { return _createNewListVisibility; }
      set
      {
        _createNewListVisibility = value;
        RaisePropertyChanged("CreateNewListVisibility");
      }
    }

    public int CurrentIndexListSelected
    {
      get { return _currentIndexListSelected; }
      set
      {
        _currentIndexListSelected = value;
        if (value > -1 && value < TwitterList.Count)
        {
          SelectedList = TwitterList[value];
        }
        RaisePropertyChanged("DetailsViewVisibility");
      }
    }

    public TwitterListShow SelectedList
    {
      get { return _selectedList; }
      set
      {
        _selectedList = value;
        EditListView.EditList(null);
        RaisePropertyChanged("SelectedList");
      }
    }

    public User SelectedUser
    {
      get { return _selectedUser; }
      set
      {
        _selectedUser = value;

        if (value != null)
        {
          UserToAdd = value.NickName;
        }
        RaisePropertyChanged("SelectedUser");
      }
    }

    public ObservableCollection<User> ListMembers
    {
      get { return _listMembers ?? (_listMembers = new ObservableCollection<User>()); }
      set
      {
        _listMembers = value;
        RaisePropertyChanged("ListMembers");
      }
    }

    public bool IsPrivate
    {
      get { return _isPrivate; }
      set
      {
        _isPrivate = value;
        RaisePropertyChanged("IsPrivate");
      }
    }

    public string NameNewList
    {
      get { return _nameNewList; }
      set
      {
        _nameNewList = value;
        RaisePropertyChanged("NameNewList");
      }
    }

    public string UserToAdd
    {
      get { return _userToAdd; }
      set
      {
        _userToAdd = value;
        RaisePropertyChanged("UserToAdd");
        RaisePropertyChanged("CanAddUser");
      }
    }

    public bool CanAddUser => !string.IsNullOrEmpty(UserToAdd);

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Twitter.Views;assembly=Sobees.Controls.Twitter'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:List HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public DataTemplate EditListTemplate
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Controls='clr-namespace:Sobees.Controls.Twitter.Controls;assembly=Sobees.Controls.Twitter'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Controls:UcEditList HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public ListEditViewModel EditListView
    {
      get
      {
        return _editListView ??
               (_editListView = new ListEditViewModel(_twitterViewModel, MessengerInstance as Messenger));
      }
      set { _editListView = value; }
    }

    #endregion Properties

    #region Commands

    #region Fields

    private BRelayCommand _addUserCommand;
    private BRelayCommand _deleteCommand;

    #endregion Fields

    public BRelayCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new BRelayCommand(Delete));

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to update the content of the control
    /// </summary>
    public RelayCommand<TwitterListShow> EditListCommand { get; private set; }

    public RelayCommand SaveCommand { get; private set; }

    public RelayCommand ShowcreateListCommand { get; private set; }

    public RelayCommand CancelCommand { get; private set; }

    public RelayCommand CancelNewListCommand { get; private set; }

    public RelayCommand SaveNewListCommand { get; private set; }

    public BRelayCommand AddUserCommand => _addUserCommand ?? (_addUserCommand = new BRelayCommand(AddUser));

    #endregion Commands

    #region Methods

    public void LoadListUser()
    {
      string error;

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
                               foreach (var user in
                                 TwitterLibV11.GetListMembers(BGlobals.TWITTER_OAUTH_KEY,
                                                                BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                                                CurrentAccount.Secret, CurrentAccount.Login,
                                                                _currentList, out error,
                                                                ProxyHelper.GetConfiguredWebProxy(SobeesSettings))
                                                .Where(user => !ListMembers.Contains(user)))
                               {
                                 ListMembers.Add(user);
                               }
                             }
                             catch (Exception ex)
                             {
                               TraceHelper.Trace(this, ex);
                             }
                           };

        worker.RunWorkerAsync();
      }
    }

    private void Delete(object obj)
    {
      var objs = BRelayCommand.CheckParams(obj);
      var user = objs[0] as User;

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
                               if (user != null)
                               {
                                 if (TwitterLibV11.DeleteListMembers(BGlobals.TWITTER_OAUTH_KEY,
                                                                       BGlobals.TWITTER_OAUTH_SECRET,
                                                                       CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                                       CurrentAccount.Login,
                                                                       _currentList, user.Id,
                                                                       ProxyHelper.GetConfiguredWebProxy(
                                                                         SobeesSettings)))
                                 {
                                   ErrorMsg = string.Empty;
                                   ListMembers.Remove(user);
                                 }
                                 else
                                 {
                                   ErrorMsg = "Error while deleting user from list";
                                 }
                               }
                             }
                             catch (Exception ex)
                             {
                               TraceHelper.Trace(this, ex);
                             }
                           };

        worker.RunWorkerAsync();
      }
    }

    private void AddUser(object obj)
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

                             try
                             {
                               string error;
                               var user = TwitterLibV11.GetUserInfo(BGlobals.TWITTER_OAUTH_KEY,
                                                                    BGlobals.TWITTER_OAUTH_SECRET,
                                                                    CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                                    UserToAdd, out error,
                                                                    ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                               if (user != null)
                               {
                                 if (
                                   !TwitterLibV11.AddToList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                                              CurrentAccount.SessionKey, CurrentAccount.Secret,
                                                              CurrentAccount.Login,
                                                              _currentList, user.Id,
                                                              ProxyHelper.GetConfiguredWebProxy(SobeesSettings)))
                                 {
                                   ErrorMsg = "Wrong User Name";
                                 }
                                 else
                                 {
                                   ErrorMsg = string.Empty;
                                   UserToAdd = string.Empty;
                                   ListMembers.Add(user);
                                 }
                               }
                               else
                               {
                                 ErrorMsg = error.Contains("limit") ? error : "Wrong User Name";
                               }
                             }
                             catch (Exception ex)
                             {
                               TraceHelper.Trace(this, ex);
                             }
                           };

        worker.RunWorkerAsync();
      }
    }

    public override void DoAction(string param)
    {
      switch (param)
      {
        case "ListUpdated":
          UpdateList();
          break;
      }
      base.DoAction(param);
    }

    /// <summary>
    /// UpdateList
    /// </summary>
    private void UpdateList()
    {
      if (_twitterList == null) return;
      _twitterList.Clear();
      foreach (var list in _twitterViewModel.TwitterListOwn)
      {
        var twitterListShow = new TwitterListShow(list)
                                {
                                  IsShowed = false,
                                  CanEdit =
                                    list.FullName.ToLower()
                                        .Contains(string.Format("@{0}", CurrentAccount.Login.ToLower()))
                                };
        foreach (var model in _twitterViewModel.TwitterWorkspaces)
        {
          if (model.WorkspaceSettings.Type != EnumTwitterType.List ||
              list.FullName.ToLower() != model.WorkspaceSettings.GroupName.ToLower()) continue;
          twitterListShow.IsShowed = true;
          twitterListShow.ColorIcon = model.WorkspaceSettings.Color;
          break;
        }
        _twitterList.Add(twitterListShow);
      }

      foreach (var list in _twitterViewModel.TwitterList)
      {
        if (!list.FullName.ToLower().Contains(string.Format("@{0}", CurrentAccount.Login.ToLower())))
        {
          var twitterListShow = new TwitterListShow(list) {IsShowed = false};

          _twitterList.Add(twitterListShow);
        }
      }
    }

    protected override void InitCommands()
    {
      SaveCommand = new RelayCommand(Save);
      CancelCommand = new RelayCommand(Cancel);
      CancelNewListCommand = new RelayCommand(() =>
                                                {
                                                  CreateNewListVisibility = Visibility.Collapsed;
                                                  NameNewList = string.Empty;
                                                });
      SaveNewListCommand = new RelayCommand(SaveNewList);
      ShowcreateListCommand = new RelayCommand(() => { CreateNewListVisibility = Visibility.Visible; });
      EditListCommand = new RelayCommand<TwitterListShow>(list => EditListView.EditList(list));
      base.InitCommands();
    }

    private void SaveNewList()
    {
      var mode = IsPrivate ? "private" : "public";
      TwitterLibV11.CreateNewList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                    CurrentAccount.Secret, Settings.UserName, mode, NameNewList,
                                    ProxyHelper.GetConfiguredWebProxy(
                                      SobeesSettings));
      CreateNewListVisibility = Visibility.Collapsed;
      MessengerInstance.Send("UpdateList");
    }

    private void Cancel()
    {
      MessengerInstance.Send("CloseList");
    }

    private void Save()
    {
      Cancel();
    }

    #endregion Methods
  }
}