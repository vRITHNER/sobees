using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Sobees.Library.BTwitterLib;

namespace Sobees.Controls.Twitter.Cls
{
  public class TweetTemplateSelector : ContentControl
  {

    private const string CtclDefaultTemplate = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:Templates='clr-namespace:Sobees.Controls.Twitter.Templates;assembly=Sobees.Controls.Twitter' ><Templates:DtTweet /></DataTemplate>";

    
    private const string CtclAdsTemplate =
      "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:Templates='clr-namespace:Sobees.Controls.Twitter.Templates;assembly=Sobees.Controls.Twitter' ><Templates:DtTweetAds /></DataTemplate>";

    protected override void OnContentChanged(object oldContent, object newContent)
    {
      var entry = DataContext as TwitterEntry;
      var dt = CtclDefaultTemplate;
      if (entry != null)
        switch (entry.PostType)
        {

          case 0:
            dt = CtclDefaultTemplate;
            break;

          case 3:
            dt = CtclAdsTemplate;
            break;

          default:
            dt = CtclDefaultTemplate;
            break;

        }
      var stringReader = new StringReader(dt);
      var xmlReader = XmlReader.Create(stringReader);
      ContentTemplate = XamlReader.Load(xmlReader) as DataTemplate;
      return;
    }
  }
}