#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.LinkedIn.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Tools.Logging;
using Sobees.Tools.Web;

#if SILVERLIGHT
using LinkedInServiceProxy;
using System.Windows.Browser;
#endif

#endregion

namespace Sobees.Controls.LinkedIn.ViewModel
{
  public class CredentialsViewModel : BLinkedInViewModel
  {
    private ObservableCollection<string> _accountsLinkedIn;

    public CredentialsViewModel(LinkedInViewModel model, Messenger messenger) : base(model, messenger)
    {
      if (!string.IsNullOrEmpty(Settings.UserName))
      {
        //Verify session validity
        LinkedInLibV2.Token = SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].SessionKey;
        LinkedInLibV2.TokenSecret = SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].Secret;
        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }
                             var result = LinkedInLibV2.GetCurrentProfile();
                             if (result != null)
                             {
                               SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].PictureUrl =
                                 result.ProfileImgUrl;
                               MessengerInstance.Send("ConnectedStored");
                             }
                             else
                             {
                               MessengerInstance.Send(new BMessage("ShowError", "Session expired"));
                             }
                           };

          worker.RunWorkerAsync();
        }
      }

      InitCommands();
    }

    public ObservableCollection<string> AccountsLinkedIn
    {
      get { return _accountsLinkedIn ?? (_accountsLinkedIn = new ObservableCollection<string>()); }
      set { _accountsLinkedIn = value; }
    }

    #region Fields

    private const string APPNAME = "CredentialViewModel";
    private Visibility _connectVisibility = Visibility.Visible;
    private Visibility _waitingCodeVisibility = Visibility.Collapsed;

    #endregion

    #region Properties

    //public string Code { get; set; }
    public string PinCode { get; set; }

    public Visibility ConnectVisibility
    {
      get { return _connectVisibility; }
      set
      {
        _connectVisibility = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt =
          "<DataTemplate x:Name='dtCredentialsView' " + "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
          "xmlns:Views='clr-namespace:Sobees.Controls.LinkedIn.Views;assembly=Sobees.Controls.LinkedIn' " + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
          "<Views:Credentials/> " + "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
      set { base.DataTemplateView = value; }
    }

    public Visibility WaitingCodeVisibility
    {
      get { return _waitingCodeVisibility; }
      set
      {
        _waitingCodeVisibility = value;
        RaisePropertyChanged();
      }
    }

    #endregion

    #region Commands

    public RelayCommand ConnectCommand { get; private set; }

    public RelayCommand<string> EnterSecurityCodeCommand { get; private set; }

    public RelayCommand<string> OpenLICommand { get; set; }

    #endregion

    #region Methods

    protected override void InitCommands()
    {
      OpenLICommand = new RelayCommand<string>(OpenLI);
      ConnectCommand = new RelayCommand(Connect);
      EnterSecurityCodeCommand = new RelayCommand<string>(EnterSecurityCode);
      CloseCommand = new RelayCommand(() => _linkedInViewModel.CloseControl());
      base.InitCommands();
    }

    private void Connect()
    {
      //if (!Debugger.IsAttached)
      //    Debugger.Launch();

      ConnectVisibility = Visibility.Collapsed;
      WaitingCodeVisibility = Visibility.Visible;

      using (var worker = new BackgroundWorker())
      {
        var result = "";
        worker.DoWork += delegate(object s, DoWorkEventArgs args)
                         {
                           if (worker.CancellationPending)
                           {
                             args.Cancel = true;
                             return;
                           }

                           result = LinkedInLibV2.AuthorizationLinkGet();

                           worker.RunWorkerCompleted += delegate { WebHelper.NavigateToUrl(result); };
                         };
        worker.RunWorkerAsync();
      }
    }

    private void EnterSecurityCode(string txt)
    {
      try
      {
        using (var worker = new BackgroundWorker())
        {
          var result = "";
          worker.DoWork += delegate(object s, DoWorkEventArgs args)
                           {
                             if (worker.CancellationPending)
                             {
                               args.Cancel = true;
                               return;
                             }

                             PinCode = txt;
                             LinkedInLibV2.Verifier = PinCode;
                             result = LinkedInLibV2.AccessTokenGet(LinkedInLibV2.Token);
                           };

          worker.RunWorkerCompleted += delegate { OnAuthCompleted(result); };
          worker.RunWorkerAsync();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    private void OpenLI(string account)
    {
    }

    #endregion

    #region Callback

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    private void OnAuthCompleted(string result)
    {
      var user = LinkedInLibV2.GetCurrentProfile();
      var response = LinkedInLibV2.ApiWebRequest("GET", "https://api.linkedin.com/v1/people/~", null);
      BLogManager.LogEntry(APPNAME + "::onAuthCompleted:Response:", response, true);

      if (SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn)) == -1)
      {
        var account = new UserAccount
                      {
                        Type = EnumAccountType.LinkedIn,
                        Secret = LinkedInLibV2.TokenSecret,
                        SessionKey = LinkedInLibV2.Token,
                        Login = user.NickName,
                        PictureUrl = user.ProfileImgUrl
                      };

        SobeesSettings.Accounts.Add(account);
        Settings.UserName = user.NickName;
      }
      else
      {
        SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].SessionKey = LinkedInLibV2.Token;
        SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].Secret = LinkedInLibV2.TokenSecret;
        SobeesSettings.Accounts[SobeesSettings.Accounts.IndexOf(new UserAccount(Settings.UserName, EnumAccountType.LinkedIn))].PictureUrl = user.ProfileImgUrl;
      }
      MessengerInstance.Send("Connected");
    }

    #endregion

    #region Utilities

    #endregion
  }
}