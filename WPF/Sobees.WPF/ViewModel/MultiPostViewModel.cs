#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Library.BServicesLib;
using Sobees.Library.BTwitterLib;
using Sobees.Library.BTwitterLib.Response;
using Sobees.Tools.Logging;
using Sobees.Tools.Web;
using CheckBox = System.Windows.Controls.CheckBox;
//using Sobees.Library.BFacebookLibV2.Exceptions;
//using Sobees.Library.BFacebookLibV2.Login;

#endregion

namespace Sobees.ViewModel
{
    public class MultiPostViewModel : BMultiPostViewModel
    {
        #region Fields

        private string _errorMsg;
        private Visibility _errorMsgVisibility = Visibility.Collapsed;

        private bool _isWaiting;

        private ObservableCollection<UserAccount> _listAccountsSelected;

        private string _newPostMsg = "";
        private Visibility _visibilityMultiPost = Visibility.Visible;
        private Visibility _visibilityValidation = Visibility.Collapsed;

        private string errorMsg;

        #endregion

        #region Properties

#if !SILVERLIGHT

        public List<User> Friends
        {
            get => _friends ?? (_friends = new List<User>());
            set => _friends = value;
        }

#endif

        public bool IsWaiting
        {
            get => _isWaiting;
            set
            {
                _isWaiting = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<UserAccount> ListAccountsSelected
        {
            get => _listAccountsSelected ?? (_listAccountsSelected = new ObservableCollection<UserAccount>());
            set => _listAccountsSelected = value;
        }

        public ObservableCollection<UserAccount> Accounts => SobeesSettings.Accounts;

        public string NewPostMsg
        {
            get => _newPostMsg;
            set
            {
                _newPostMsg = value;
                RaisePropertyChanged();
            }
        }

        public Visibility VisibilityValidation
        {
            get => _visibilityValidation;
            set
            {
                _visibilityValidation = value;
                RaisePropertyChanged();
                RaisePropertyChanged("VisibilityPostTweets");
            }
        }

        public Visibility VisibilityPostTweets => VisibilityValidation == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

        private static SobeesSettings SobeesSettings => SobeesSettingsLocator.SobeesSettingsStatic;

        public Visibility VisibilityMultiPost
        {
            get => _visibilityMultiPost;
            set
            {
                _visibilityMultiPost = value;

                RaisePropertyChanged();
            }
        }

        public string ErrorMsg
        {
            get => _errorMsg;
            set
            {
                _errorMsg = value;
                RaisePropertyChanged();
            }
        }

        public Visibility ErrorMsgVisibility
        {
            get => _errorMsgVisibility;
            set
            {
                _errorMsgVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Public Methods

        public void GoToBaseState()
        {
            NewPostMsg = string.Empty;
            if (ListAccountsSelected != null) ListAccountsSelected.Clear();
            ErrorMsg = string.Empty;
            ErrorMsgVisibility = Visibility.Collapsed;
            VisibilityValidation = Visibility.Collapsed;
            VisibilityMultiPost = Visibility.Visible;
        }

        #endregion

        #region Constructor

        public MultiPostViewModel(Messenger messenger)
            : base(messenger)
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                InitCommands();
                LoadFriendsList();
            }
        }

        public MultiPostViewModel()
            : base(Messenger.Default)
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                InitCommands();
                LoadFriendsList();
            }
        }

        #endregion Constructor

        #region Commands

        private BRelayCommand _checkInsideListCommand;

        private List<User> _friends;
        public BRelayCommand CheckInsideListCommand => _checkInsideListCommand ?? (_checkInsideListCommand = new BRelayCommand(CheckInsideList));

        public RelayCommand CloseMultiCommand { get; set; }

        public RelayCommand ShortenUrlCommand { get; private set; }

        public RelayCommand SendMultiCommand { get; private set; }

        public RelayCommand RepostCommand { get; private set; }

        public RelayCommand UploadImageCommand { get; private set; }

        /// <summary>
        ///     Command for tweetshrink the text.
        /// </summary>
        public RelayCommand TweetShrinkCommand { get; private set; }

        #endregion Commands

        #region Methods

        #region Public Methods

        /// <summary>
        ///     Used when a string arrived into the Messenger
        /// </summary>
        /// <param name="param">A string that represents the fonction to execute.</param>
        protected new virtual void DoAction(string param)
        {
            switch (param)
            {
                case "SettingsUpdated":
                    //OnSettingsUpdated();
                    break;

                case "Offline":
                    break;

                default:
                    break;
            }
            base.DoAction(param);
        }

        #endregion Public Methods

        #region Private Methods

        private void LoadFriendsList()
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
                                         foreach (var user in Accounts)
                                         {
                                             if (user.Type != EnumAccountType.Twitter) continue;
                                             try
                                             {
                                                 var lst = FileHelper.LoadFriendsList(user.Login);
                                                 if (lst == null) continue;
                                                 foreach (var usertmp in lst)
                                                 {
                                                     if (!Friends.Contains(usertmp))
                                                     {
                                                         Friends.Add(usertmp);
                                                     }
                                                 }
                                             }
                                             catch (Exception ex)
                                             {
                                                 TraceHelper.Trace(this, ex);
                                             }
                                         }
                                         RaisePropertyChanged("Friends");
                                     }
                                     catch (Exception ex)
                                     {
                                         TraceHelper.Trace(this, ex);
                                     }
                                 };
                worker.RunWorkerAsync();
            }
            ;
        }

        protected override void InitCommands()
        {
            CloseMultiCommand = new RelayCommand(() => Messenger.Default.Send("CloseMultipost"));
            ShortenUrlCommand = new RelayCommand(ShortenUrl, CanShortenUrl);
            UploadImageCommand = new RelayCommand(UploadImage, CanUploadImage);
            TweetShrinkCommand = new RelayCommand(TweetShrink);
            SendMultiCommand = new RelayCommand(PostStatus, CanPostStatus);
            RepostCommand = new RelayCommand(Repost);
            base.InitCommands();
        }

        private void Repost()
        {
            NewPostMsg = "";
            VisibilityValidation = Visibility.Collapsed;
            VisibilityMultiPost = Visibility.Visible;
        }

        #region Buttons

        #region ShortenUrl

        private void ShortenUrl()
        {
            try
            {
                using (var worker = new BackgroundWorker())
                {
                    var result = string.Empty;

                    worker.DoWork += delegate(object s,
                                              DoWorkEventArgs args)
                                     {
                                         if (worker.CancellationPending)
                                         {
                                             args.Cancel = true;
                                             return;
                                         }

                                         switch (SobeesSettings.UrlShortener)
                                         {
                                             case UrlShorteners.BitLy:
                                                 result =
                                                     BitLyHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                       ProxyHelper.GetConfiguredWebProxy(
                                                                                                                         SobeesSettings), SobeesSettings.BitLyUserName,
                                                                                       SobeesSettings.BitLyPassword);
                                                 break;

                                             case UrlShorteners.Digg:
                                                 result =
                                                     DiggHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                      ProxyHelper.GetConfiguredWebProxy(
                                                                                                                        SobeesSettings));
                                                 break;

                                             case UrlShorteners.IsGd:
                                                 result =
                                                     IsGdHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                      ProxyHelper.GetConfiguredWebProxy(
                                                                                                                        SobeesSettings));
                                                 break;

                                             case UrlShorteners.TinyUrl:
                                                 result =
                                                     TinyUrlHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                         ProxyHelper.GetConfiguredWebProxy(
                                                                                                                           SobeesSettings));
                                                 break;

                                             case UrlShorteners.TrIm:
                                                 result =
                                                     TrImHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                      ProxyHelper.GetConfiguredWebProxy(
                                                                                                                        SobeesSettings));
                                                 break;

                                             case UrlShorteners.Twurl:
                                                 result =
                                                     TwurlHelper.ConvertUrlsToTinyUrls(NewPostMsg,
                                                                                       ProxyHelper.GetConfiguredWebProxy(
                                                                                                                         SobeesSettings));
                                                 break;

                                             case UrlShorteners.MigreMe:
                                                 result =
                                                     MigreMeHelper.ConvertUrlsToTinyUrls(NewPostMsg, ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                                 break;
                                         }
                                     };

                    worker.RunWorkerCompleted += delegate { OnShortenUrlAsyncCompleted(result); };

                    worker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                TraceHelper.Trace(this, ex);
            }
        }

        private bool CanShortenUrl()
        {
            if (string.IsNullOrEmpty(NewPostMsg)) return false;
            var words = NewPostMsg.Split(' ');
            return words.Any(HyperLinkHelper.IsHyperlink);
        }

        private void OnShortenUrlAsyncCompleted(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                NewPostMsg = result;
            }
            else
            {
                //ShowErrorMsg("Shorten URL has failed. Please try again later...");
                ErrorMsg = "Shorten URL has failed. Please try again later...";
                ErrorMsgVisibility = Visibility.Visible;
                TraceHelper.Trace(this, ErrorMsg);
            }
        }

        #endregion ShortenUrl

        #region UploadImage

        private bool CanUploadImage()
        {
            return ListAccountsSelected.Count > 0;
        }

        private void UploadImage()
        {
            try
            {
                const string filter = "Images |*.bmp;*.jpg;*.gif;*.png";
                var ofd = new OpenFileDialog
                          {
                              InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                              Filter = filter,
                              RestoreDirectory = true
                          };

                var dr = ofd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    using (var worker = new BackgroundWorker())
                    {
                        string imgLink = null;
                        string errorMsg = null;

                        worker.DoWork += delegate(object s,
                                                  DoWorkEventArgs args)
                                         {
                                             if (worker.CancellationPending)
                                             {
                                                 args.Cancel = true;
                                                 return;
                                             }
                                             var binaryImageData = File.ReadAllBytes(ofd.FileName);
                                             imgLink = TwitPicHelper.UploadPhoto(ListAccountsSelected[0].Login, ListAccountsSelected[0].PasswordHash,
                                                                                 binaryImageData, null, ofd.FileName,
                                                                                 BGlobals.CIPHER_KEY,
                                                                                 BGlobals.CLIENTNAME, out errorMsg,
                                                                                 ProxyHelper.GetConfiguredWebProxy(SobeesSettings));
                                         };

                        worker.RunWorkerCompleted += delegate { OnUploadImageAsyncCompleted(imgLink, errorMsg); };
                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.Trace(this, ex);
            }
        }

        private void OnUploadImageAsyncCompleted(string imgLink, string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg) && !string.IsNullOrEmpty(imgLink))
            {
                if (string.IsNullOrEmpty(NewPostMsg))
                    NewPostMsg = string.Format("{0} ", imgLink);
                else
                    NewPostMsg += string.Format(" {0}", imgLink);
            }
            else
            {
                TraceHelper.Trace(this, errorMsg);
            }
        }

        #endregion UploadImage

        #region TweetShrink

        private void TweetShrink()
        {
            var result = "";

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
                                     result = TweetShrinkHelper.GetNewTweetShrink(NewPostMsg, ProxyHelper.GetConfiguredWebProxy(
                                                                                                                                SobeesSettings));
                                 };

                worker.RunWorkerCompleted += delegate { OnTweetShrinkAsyncCompleted(result); };

                worker.RunWorkerAsync();
            }
        }

        private void OnTweetShrinkAsyncCompleted(string result)
        {
            NewPostMsg = result;
        }

        #endregion TweetShrink

        #endregion Buttons

        private bool CanPostStatus()
        {
            return NewPostMsg.Length > 0 && ListAccountsSelected.Count > 0;
        }

        /// <summary>
        ///     PostStatus
        /// </summary>
        private void PostStatus()
        {
            try
            {
                foreach (var account in ListAccountsSelected)
                {
                    switch (account.Type)
                    {
                        case EnumAccountType.Twitter:
                            var result = BTwitterResponseResult<TwitterEntry>.CreateInstance();
                            using (var worker = new BackgroundWorker())
                            {
                                var userAccount = account;
                                worker.DoWork += async delegate(object s,
                                                                DoWorkEventArgs args)
                                                 {
                                                     if (worker.CancellationPending)
                                                     {
                                                         args.Cancel = true;
                                                         return;
                                                     }

                                                     result =
                                                         await
                                                             TwitterLibV11.AddTweet(BGlobals.TWITTER_OAUTH_KEY, BGlobals.TWITTER_OAUTH_SECRET,
                                                                                    userAccount.SessionKey,
                                                                                    userAccount.Secret,
                                                                                    NewPostMsg,
                                                                                    false, "");
                                                 };

                                worker.RunWorkerCompleted +=
                                    delegate { OnSendTwitterAsyncCompleted(result.DataResult, result.ErrorMessage); };
                                worker.RunWorkerAsync();
                            }
                            break;

                            //case EnumAccountType.Facebook:
                            //  var userAccountFace = account;

                            //  var currentSession = new DesktopSession(BGlobals.FACEBOOK_WPF_API)
                            //  {
                            //    SessionKey = userAccountFace.SessionKey,
                            //    SessionSecret = userAccountFace.Secret,
                            //    AccessToken = account.AuthToken
                            //  };

                            //var service = BindingManager.CreateInstance(currentSession);
                            //service.Api.Status.SetAsync(NewPostMsg, OnSendFacebookAsyncCompleted, null);
                            break;

                        case EnumAccountType.TwitterSearch:
                            break;

                        //case EnumAccountType.LinkedIn:
                        //  using (var worker = new BackgroundWorker())
                        //  {
                        //    var userAccount = account;
                        //    worker.DoWork += delegate(object s,
                        //      DoWorkEventArgs args)
                        //    {
                        //      if (worker.CancellationPending)
                        //      {
                        //        args.Cancel = true;
                        //        return;
                        //      }

                        //      var helper =
                        //        new OAuthLinkedInV2(BGlobals.LINKEDIN_WPF_KEY,
                        //          BGlobals.LINKEDIN_WPF_SECRET, userAccount.SessionKey, userAccount.Secret);
                        //      helper.PostShare(NewPostMsg);
                        //    };

                        //    worker.RunWorkerCompleted += (sender, args) => OnSendStatusAsyncCompleted(null);
                        //    worker.RunWorkerAsync();
                        //  }
                        //  break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.Trace(this,
                                  ex);
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
                var accent = "ÀÁÂÃÄÅàáâãäåÒÓÔÕÖØòóôõöøÈÉÊËèéêëÌÍÎÏìíîïÙÚÛÜùúûüÿÑñÇç";
                var sansAccent = "AAAAAAaaaaaaOOOOOOooooooEEEEeeeeIIIIiiiiUUUUuuuuyNnCc";

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
                //chaine = chaine.Replace("&", " ");
            }
            catch (Exception ex)
            {
                TraceHelper.Trace("", ex);
                return txt;
            }
            // Retour du résultat
            return chaine;
        }

        private void CheckInsideList(object param)
        {
            var objs = BRelayCommand.CheckParams(param);
            if (objs == null) return;
            if (objs[1].GetType() != typeof(CheckBox)) return;
            var checkBox = (CheckBox) objs[1];

            if (checkBox == null) return;
            var isCheck = checkBox.IsChecked;
            if (checkBox.DataContext.GetType() != typeof(UserAccount)) return;
            var item = checkBox.DataContext as UserAccount;
            if (item != null && (isCheck != null && ((bool) isCheck && !ListAccountsSelected.Contains(item))))
                ListAccountsSelected.Add(item);
            else
                ListAccountsSelected.Remove(item);
        }

        #region Callback

        //private void OnSendFacebookAsyncCompleted(bool result, object state, FacebookException e)
        //{
        //  if (result)
        //  {
        //    OnSendStatusAsyncCompleted(null);
        //    return;
        //  }

        //  if (e != null && !string.IsNullOrEmpty(e.Message))
        //  {
        //    OnSendStatusAsyncCompleted(e.Message);
        //    return;
        //  }

        //  OnSendStatusAsyncCompleted("Facebook - An error occured, please try again later");
        //}

        private void OnSendTwitterAsyncCompleted(Entry entry, string errorMsg1)
        {
            OnSendStatusAsyncCompleted(errorMsg1);
        }

        //private void OnSendMyspaceAsyncCompleted(string msg)
        //{
        //  OnSendStatusAsyncCompleted(null);
        //}

        //private void OnSendLinkedInAsyncCompleted(string msg)
        //{
        //  OnSendStatusAsyncCompleted(null);
        //}

        private void OnSendStatusAsyncCompleted(string errorMsg1)
        {
            if (string.IsNullOrEmpty(errorMsg1))
            {
                VisibilityValidation = Visibility.Visible;
                VisibilityMultiPost = Visibility.Collapsed;
                if (ListAccountsSelected != null) ListAccountsSelected.Clear();
            }
            else
            {
                ErrorMsg = errorMsg1;
                ErrorMsgVisibility = Visibility.Visible;
            }
        }

        #endregion Callback

        #endregion Private Methods

        #endregion Methods
    }
}