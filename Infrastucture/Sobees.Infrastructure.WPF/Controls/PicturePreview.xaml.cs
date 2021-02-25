using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for PicturePreview.xaml
  /// </summary>
  public partial class PicturePreview : UserControl
  {
    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register(
        "Text",
        typeof (string),
        typeof (PicturePreview),
        new PropertyMetadata(TextPropertyChanged));

    public PicturePreview()
    {
      InitializeComponent();
    }

    public string Text
    {
      get { return (string) GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var preview = d as PicturePreview;
      if (preview != null) preview.UpdatePreview();
    }

    private void UpdatePreview()
    {
      string txt = Text;
      wpImage.Children.Clear();
      using (var worker = new BackgroundWorker())
      {
        worker.DoWork += delegate(object s,
                                  DoWorkEventArgs args)
                           {
                             try
                             {
                               if (worker.CancellationPending)
                               {
                                 args.Cancel = true;
                                 return;
                               }
                               if (txt != null)
                               {
                                 string[] splited = txt.Split(' ');
                                 foreach (string str in splited)
                                 {
                                   if (!str.Contains("twitpic.com/")) continue;
                                   string[] splited2 = str.Split('/');
                                   string url1 = str;
                                   Application.Current.Dispatcher.BeginInvokeIfRequired(
                                     () =>
                                       {
                                         var btn = new Button
                                                     {
                                                       Style = FindResource("BtnNoStyle") as Style,
                                                       Cursor = Cursors.Hand,
                                                       HorizontalAlignment = HorizontalAlignment.Left,
                                                       VerticalAlignment = VerticalAlignment.Top,
                                                       Margin = new Thickness(0.5),
                                                       Content =
                                                         new Image
                                                           {
                                                             Width = 100,
                                                             Height = 100,
                                                             Source =
                                                               new BitmapImage(
                                                               new Uri("http://twitpic.com/show/thumb/" +
                                                                       splited2[splited2.Count() - 1]))
                                                           }
                                                     };
                                         string url = url1;
                                         btn.Click += delegate { WebHelper.NavigateToUrl(url);};
                                         wpImage.Children.Add(btn);
                                       });
                                 }
                                 foreach (string str in splited)
                                 {
                                   if (!str.Contains("tweetphoto.com/")) continue;
                                   string url1 = str;
                                   Application.Current.Dispatcher.BeginInvokeIfRequired(
                                     () =>
                                     {
                                       var btn = new Button
                                       {
                                         Style = FindResource("BtnNoStyle") as Style,
                                         Cursor = Cursors.Hand,
                                         HorizontalAlignment = HorizontalAlignment.Left,
                                         VerticalAlignment = VerticalAlignment.Top,
                                         Margin = new Thickness(0.5),
                                         Content =
                                           new Image
                                           {
                                             Width = 100,
                                             Height = 100,
                                             Source =
                                               new BitmapImage(
                                               new Uri("http://tweetphotoapi.com/api/tpapi.svc/imagefromurl?url=" +
                                                       url1))
                                           }
                                       };
                                       string url = url1;
                                       btn.Click += delegate { WebHelper.NavigateToUrl(url); };
                                       wpImage.Children.Add(btn);
                                     });
                                 }
                               }
                             }
                             catch (Exception e)
                             {
                               Console.WriteLine(e);
                             }
                           };
        worker.RunWorkerAsync();
      }
    }
  }
}