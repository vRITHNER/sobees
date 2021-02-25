#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Twitter.Cls;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Library.BTwitterLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.Controls.Twitter.ViewModel
{
  public class ListEditViewModel : BTwitterViewModel
  {
    #region Methods

    protected ListEditViewModel(TwitterViewModel model, Messenger messenger, TwitterWorkspaceSettings settings)
      : base(model, messenger, settings)
    {
    }

    public ListEditViewModel(TwitterViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      MessengerInstance.Register<string>(this, DoAction);
      InitCommands();

      SearchUser();
    }

    #region Fields

    private string _currentList;
    private Visibility _editVisibility = Visibility.Collapsed;
    private ObservableCollection<User> _listMembers;

    private string _userToAdd;

    private string _userToSearch;

    private string Login => Settings.UserName;

    #endregion Fields

    #region Properties

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtGroupsView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:bDule.Controls.Twitter.Views;assembly=bDule.Controls.Twitter'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:List HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public string UserToAdd
    {
      get { return _userToAdd; }
      set
      {
        _userToAdd = value;
        RaisePropertyChanged("UserToAdd");
      }
    }

    public string UserToSearch
    {
      get { return _userToSearch; }
      set
      {
        _userToSearch = value;
        RaisePropertyChanged("UserToSearch");
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

    public Visibility EditVisibility
    {
      get
      {
        {
          return _editVisibility;
        }
      }
      set
      {
        _editVisibility = value;
        RaisePropertyChanged("EditVisibility");
      }
    }

    #endregion Properties

    #region

    public List<User> Friends => _twitterViewModel.Friends;

    #endregion

    #region Constructor

    #endregion

    #region Commands

    public RelayCommand<User> DeleteCommand { get; set; }

    public RelayCommand AddUserCommand { get; set; }

    public RelayCommand SearchUserCommand { get; set; }

    #endregion

    public override void UpdateAll()
    {
      try
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
                                 foreach (
                                   var user in
                                     TwitterLibV11.GetListMembers(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                                                  CurrentAccount.Secret, Login,
                                                                  _currentList, out error, ProxyHelper.GetConfiguredWebProxy(SobeesSettings)))

                                 {
                                   if (ListMembers.Contains(user)) continue;
                                   var item = user;
                                   Application.Current.Dispatcher.BeginInvokeIfRequired(() => ListMembers.Add(item));
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
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void Delete(User user)
    {
      try
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
                                 if (user == null) return;
                                 if (TwitterLibV11.DeleteListMembers(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                                                     CurrentAccount.Secret, Login,
                                                                     _currentList, user.Id,
                                                                     ProxyHelper.GetConfiguredWebProxy(
                                                                       SobeesSettings)))
                                 {
                                   ErrorMsg = string.Empty;
                                   Application.Current.Dispatcher.BeginInvokeIfRequired(() => ListMembers.Remove(user));
                                 }
                                 else
                                 {
                                   ErrorMsg = "Error while deleting user from list";
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
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddUser()
    {
      try
      {
        if (ListMembers.Any(member => UserToAdd.ToLower().Equals(member.NickName.ToLower())))
        {
          ErrorMsg =
            new LocText("Sobees.Configuration.BGlobals:Resources:txtListMembersExist").ResolveLocalizedValue();
          return;
        }

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
                                 User user = TwitterLibV11.GetUserInfo(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                                                       CurrentAccount.Secret, UserToAdd, out error,
                                                                       ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                 if (user == null)
                                 {
                                   ErrorMsg = error.Contains("limit")
                                                ? error
                                                : new LocText("Sobees.Configuration.BGlobals:Resources:txtListBadMember").ResolveLocalizedValue();
                                 }
                                 else
                                 {
                                   if (
                                     !TwitterLibV11.AddToList(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET, CurrentAccount.SessionKey,
                                                              CurrentAccount.Secret, Login,
                                                              _currentList, user.Id,
                                                              ProxyHelper.GetConfiguredWebProxy(SobeesSettings)))
                                   {
                                     ErrorMsg =
                                       new LocText("Sobees.Configuration.BGlobals:Resources:txtListBadMember").ResolveLocalizedValue();
                                   }
                                   else
                                   {
                                     ErrorMsg = string.Empty;
                                     UserToAdd = string.Empty;
                                     Application.Current.Dispatcher.BeginInvokeIfRequired(() => ListMembers.Add(user));
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
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected override void InitCommands()
    {
      DeleteCommand = new RelayCommand<User>(Delete);
      AddUserCommand = new RelayCommand(AddUser, () => !string.IsNullOrEmpty(UserToAdd));
      SearchUserCommand = new RelayCommand(SearchUser);
      base.InitCommands();
    }

    private void SearchUser()
    {
      try
      {
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }


    public void EditList(TwitterListShow show)
    {
      ListMembers.Clear();
      UserToAdd = string.Empty;
      UserToSearch = string.Empty;
      if (show != null)
      {
        EditVisibility = Visibility.Visible;
        _currentList = show.Id;
      }
      else
      {
        EditVisibility = Visibility.Collapsed;
      }
      UpdateAll();
    }
  }

  #endregion
}

public class NicknameComparer : IComparer<User>
{
  #region IComparer<User> Members

  public int Compare(User x, User y)
  {
    return (System.String.Compare(x.NickName.ToLower(), y.NickName.ToLower(), System.StringComparison.Ordinal));
  }

  #endregion
}