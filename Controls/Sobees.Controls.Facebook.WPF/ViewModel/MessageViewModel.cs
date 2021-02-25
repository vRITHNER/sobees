#region

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Controls.Facebook.Cls;
using Sobees.Library.BFacebookLibV2.Objects.Message;
using Sobees.Library.BFacebookLibV2.Objects.Thread;
using Sobees.Tools.Web;

#endregion

namespace Sobees.Controls.Facebook.ViewModel
{
  public class MessageViewModel : BFacebookViewModel
  {
    #region Constructors

    public MessageViewModel(FacebookViewModel model, Messenger messenger)
      : base(model, messenger)
    {
      InitCommands();
    }

    #endregion

    #region Commands

    public RelayCommand GotoFacebookMessageCommand { get; set; }

    #endregion

    #region Methods

    private void GotoFacebookMessage(object param)
    {
      WebHelper.NavigateToUrl("http://www.facebook.com/inbox/?tid=" + _threadId);
    }

    #endregion

    public void UpdateMessage(FacebookThread entry)
    {
      Messages = new ObservableCollection<FacebookMessage>();
      MessagesTemp = new ObservableCollection<FacebookMessage>();
      MailDisplay = entry;
      _threadId = entry.Id;
      foreach (var comment in entry.Comments)
      {
        Messages.Add(comment);
      }
    }

    protected override void InitCommands()
    {
      base.InitCommands();
      GotoFacebookMessageCommand =
        new RelayCommand(() => WebHelper.NavigateToUrl(string.Format("http://www.facebook.com/inbox/?tid={0}", _threadId)));
      CloseCommand = new RelayCommand(() => MessengerInstance.Send("CloseThreadMessage"));
    }

    #region Fields

    private FacebookThread _mailDisplay;
    private long _threadId;

    public override double GetRefreshTime()
    {
      return Settings.RefreshTime;
    }

    #endregion

    #region Properties

    public ObservableCollection<FacebookMessage> Messages { get; set; }
    public ObservableCollection<FacebookMessage> MessagesTemp { get; set; }

    public FacebookThread MailDisplay
    {
      get { return _mailDisplay; }
      set
      {
        _mailDisplay = value;
        RaisePropertyChanged();
      }
    }

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string DT = "<DataTemplate x:Name='dtMessageView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                          "xmlns:Views='clr-namespace:Sobees.Controls.Facebook.Views;assembly=Sobees.Controls.Facebook'>" +
                          "<Grid HorizontalAlignment='Stretch' VerticalAlignment='Stretch'>" +
                          "<Views:Message HorizontalAlignment='Stretch' VerticalAlignment='Stretch'/>" +
                          "</Grid>" +
                          "</DataTemplate>";
        var stringReader = new StringReader(DT);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    #endregion
  }
}