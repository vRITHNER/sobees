#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Controls.Facebook.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Library.BFacebookLibV2.Exceptions;
using Sobees.Library.BGenericLib;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{


  public class CredentialsViewModel : BFacebookViewModel
  {
    private string _textConnection;

    private ObservableCollection<string> _accountsFacebook;

    public ObservableCollection<string> AccountsFacebook
    {
      get { return _accountsFacebook ?? (_accountsFacebook = new ObservableCollection<string>()); }
      set { _accountsFacebook = value; }
    }

    private bool _isFacebookConnected = true;

    public bool IsFacebookConnected
    {
      get { return _isFacebookConnected; }
      set
      {
        _isFacebookConnected = value;
        RaisePropertyChanged();
      }
    }

    private string _fbUrl = string.Empty;

    public string FbUrl
    {
      get { return _fbUrl; }
      set
      {
        _fbUrl = value;
        RaisePropertyChanged();
      }
    }

    public CredentialsViewModel()
    {
    }

    public CredentialsViewModel(FacebookViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      if (!string.IsNullOrEmpty(Settings.UserName))
      {
        var i = SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.Facebook));


        TextConnection = "Verify session validity and permissions";

        CurrentSession.ApplicationKey = BGlobals.FACEBOOK_WPF_API;
        CurrentSession.ApplicationSecret = BGlobals.FACEBOOK_WPF_SECRET;
        CurrentSession.SessionKey = SobeesSettings.Accounts[i].SessionKey;
        CurrentSession.SessionSecret = SobeesSettings.Accounts[i].Secret;
        CurrentSession.AccessToken = SobeesSettings.Accounts[i].AuthToken;

        CurrentSession.UserId = SobeesSettings.Accounts[i].UserId;
        if (Service != null) return;
        Service.Api.Users.GetLoggedInUserAsync(VerifySessionCompleted, null);
      }
      else
      {
        foreach (var account in SobeesSettings.Accounts)
        {
          switch (account.Type)
          {
            case EnumAccountType.Facebook:
              AccountsFacebook.Add(account.Login);
              IsFacebookConnected = false;
              break;
          }
        }
      }
    }

    #region Properties

    public string TextConnection
    {
      get { return _textConnection; }
      set
      {
        _textConnection = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Commands

    /// <summary>
    ///   Returns the command that, when invoked, attempts
    ///   to show settings for this control.
    /// </summary>
    public RelayCommand ConnectToFbCommand { get; private set; }

    //public RelayCommand<string> OpenFBCommand { get; set; }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      base.InitCommands();
      ConnectToFbCommand = new RelayCommand(ConnectToFb);
      //OpenFBCommand = new RelayCommand<string>(OpenFB);
      CloseCommand = new RelayCommand(() => FacebookViewModel.CloseControl());
    }

    private void ConnectToFb()
    {
      StartWaiting();
      TextConnection = "Connecting to Facebook";
      CurrentSession.LoginCompleted += SessionLoginCompleted;
      CurrentSession.Login();
    }

    private void SessionLoginCompleted(object sender, AsyncCompletedEventArgs e)
    {
      CurrentSession.LoginCompleted -= SessionLoginCompleted;
      try
      {
        if (e.Error == null)
        {
          TextConnection = "Get User Info";
          //if (Service == null) //TODO:VR-03-02-2013-Verify if this line must be uncommented
          {
            Service = BindingManager.CreateInstance(CurrentSession);
            Service.Api.Users.GetInfoAsync(CurrentSession.UserId, UserInfoCompleted, null);
          }
        }
        else
        {
          TextConnection = e.Error.Message;
          MessengerInstance.Send(new BMessage("ShowError", e.Error.Message));
          StopWaiting();
          var errorMsg = e.Error.Message.ToLower();
          if (errorMsg.Contains("failed") || errorMsg.Contains("session"))
          {
            //Due to oaouth modification, old subscription doesn't work anymore
            //User must reconnect their account
            //TextConnection = string.Format("Due to a security modification in Facebook service side, your old credentials don't work anymore; please remove your account in the globals settings and set a new Facebook service");
            TextConnection =
              new LocText("Sobees.Configuration.BGlobals:Resources:FbOldSecurityWarning").ResolveLocalizedValue();
            Messenger.Default.Send(new BMessage("DisplayPopupFb", TextConnection));
          }
        }
      }
      catch (Exception ex)
      {
        StopWaiting();
        TraceHelper.Trace(this, ex);
      }
    }

    private void VerifySessionCompleted(long userid, object state, FacebookException e)
    {
      if (e == null && string.IsNullOrEmpty(CurrentSession.CheckPermissions()))
      {
        Service.Api.Users.GetInfoAsync(CurrentSession.UserId, UserInfoCompleted, null);
        return;
      }
      if (e != null)
      {
        StopWaiting();
        TextConnection = e.Message;
        MessengerInstance.Send(new BMessage("ShowError", e.Message));
      }
      else
      {
        TextConnection = "Verify permissions";
        CurrentSession.Login();
      }
    }

    private void UserInfoCompleted(IList<User> users, object state, FacebookException e)
    {
      try
      {
        if (e == null)
        {
          AddUserToAccountsList(users[0]);
          Settings.UserName = users[0].name;
          StopWaiting();

          Application.Current.Dispatcher.BeginInvokeIfRequired(() => MessengerInstance.Send("Connected"));
        }
        else
        {
          StopWaiting();
          TextConnection = e.Message;
          MessengerInstance.Send(new BMessage("ShowError", e.Message));
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void AddUserToAccountsList(User user)
    {
      foreach (var account in SobeesSettings.Accounts)
      {
        if (account.Type != EnumAccountType.Facebook) continue;
        if (account.Login != user.name) continue;
        account.AuthToken = CurrentSession.AccessToken;
        account.Secret = CurrentSession.SessionSecret;
        account.SessionKey = CurrentSession.SessionKey;
        account.UserId = CurrentSession.UserId;
        account.PictureUrl = user.pic;
        return;
      }
      SobeesSettings.Accounts.Add(new UserAccount
      {
        AuthToken = CurrentSession.AccessToken,
        Secret = CurrentSession.SessionSecret,
        SessionKey = CurrentSession.SessionKey,
        UserId = CurrentSession.UserId,
        Login = user.name,
        Type = EnumAccountType.Facebook,
        PictureUrl = user.pic
      });
    }

    #endregion
  }
}

