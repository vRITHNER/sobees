using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Sobees.Configuration.BGlobals;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for YahooMapImage.xaml
  /// </summary>
  public partial class YahooMapImage
  {
    public static readonly DependencyProperty LatitudeProperty =
      DependencyProperty.Register(
        "Latitude",
        typeof(double),
        typeof(YahooMapImage),
        new PropertyMetadata(GeolocPropertyChanged));

    private static void GeolocPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      try
      {


        var yahoo = d as YahooMapImage;
        if (yahoo == null || yahoo.Latitude == 0.0 || yahoo.Longitude == 0.0) return;
        var url =
            $"{BGlobals.YAHOO_IMG_URL_BASE}appid={BGlobals.YAHOO_APPID}&latitude={yahoo.Latitude}&longitude={yahoo.Longitude}&image_height={yahoo.Height}&image_width={yahoo.Width}&zoom=9";
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
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var reader = new StreamReader((request.GetResponse()).GetResponseStream()))
            {
              var data = reader.ReadToEnd();
              var xdoc = XDocument.Parse(data);
              if (xdoc.Element("Result") == null) return;
              var urlImg = xdoc.Element("Result").Value;
              Application.Current.Dispatcher.BeginInvokeIfRequired(
                () => { yahoo.imgYahoo.Source = new BitmapImage(new Uri(urlImg)); });
            }
          };


          worker.RunWorkerAsync();
        }
      }
      catch (Exception exception)
      {
        Console.WriteLine(exception);
      }
    }

    public static readonly DependencyProperty LongitudeProperty =
      DependencyProperty.Register(
        "Longitude",
        typeof(double),
        typeof(YahooMapImage),
        new PropertyMetadata(GeolocPropertyChanged));

    public YahooMapImage()
    {
      InitializeComponent();
    }



    public double Latitude
    {
      get { return (double)GetValue(LatitudeProperty); }
      set { SetValue(LatitudeProperty, value); }
    }

    public double Longitude
    {
      get { return (double)GetValue(LongitudeProperty); }
      set { SetValue(LongitudeProperty, value); }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      WebHelper.NavigateToUrl(string.Format(BGlobals.DEFAULT_URL_MAP, Latitude, Longitude));
    }
  }
}