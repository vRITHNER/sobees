#region

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;
#if !SILVERLIGHT
using Sobees.Library.BLocalizeLib;
#else
using Telerik.Windows.Controls;
#endif

#endregion

namespace Sobees.Controls.Facebook.Converters
{
  public class FacebookLikesConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //StringBuilder outString = new StringBuilder();
      try
      {
        var tb = new TextBlock {TextWrapping = new TextWrapping()};

        var isFirst = true;
        if (value == null) return null;
        var like = value as Like;
        if (like != null)
        {
          if (like.Count > 0)
          {
            var nbShowed = 0;
            //When I likes it!
            if (like.LikeIt == 1)
            {
#if SILVERLIGHT
              tb.Inlines.Add(LocalizationManager.GetString("txtYouFBLike"));
              if (like.Count > 1 && like.FriendsLike.Count < 1)
              {
                tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeAnd"));
              }
#else

              tb.Inlines.Add(new LocText("Sobees.Configuration.BGlobals:Resources:txtYouFBLike").ResolveLocalizedValue());
              if (like.Count > 1 && like.FriendsLike.Count < 1 && like.SampleUsersLike.Count < 1)
              {
                tb.Inlines.Add(
                  new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeAnd").ResolveLocalizedValue());
              }

              ////tb.Inlines.Add(new LocText("Sobees.Configuration.BGlobals:Resources:txtYouFBLike").ResolveLocalizedValue());
#endif
              isFirst = false;
              nbShowed++;
            }
            else
            {
#if !SILVERLIGHT
              if (Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("de"))
              {
                tb.Inlines.Add(
                  new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeMeThis").ResolveLocalizedValue());
              }
#endif
            }
            //When some of my friends like it!

#if !SILVERLIGHT
            ////if (like.FriendsLike.Count > 0 && like.LikeIt!=1)
            ////{
            ////  if (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("de"))
            ////  {
            ////    tb.Inlines.Add(new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeThis").ResolveLocalizedValue());
            ////  }
            ////}
#endif
            if (like.FriendsLike != null)
            {
              foreach (var user in like.FriendsLike)
              {
                if (!isFirst)
                {
#if SILVERLIGHT
                  tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeAnd"));
#else
                  tb.Inlines.Add(
                    new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeAnd").ResolveLocalizedValue());
#endif
                }
                else
                {
                  isFirst = false;
                }
                nbShowed++;
                if (user.NickName != null)
                {
                  tb.Inlines.Add(user.NickName);
                }
                //var hyperlink = new Hyperlink(new Run(user.NickName));
                //hyperlink.NavigateUri = new Uri(user.ProfileUrl);
                //hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperlink_RequestNavigate);
                //tb.Inlines.Add(hyperlink);
              }


              //When somebody who like it isn't my friend!

              if (like.SampleUsersLike != null)
              {
                foreach (var user in like.SampleUsersLike)
                {
                  if (!isFirst)
                  {
#if SILVERLIGHT
                    tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeAnd"));
#else
                    tb.Inlines.Add(
                      new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeAnd").ResolveLocalizedValue());
#endif
                  }
                  else
                  {
                    isFirst = false;
                  }
                  nbShowed++;
                  //var hyperlink = new Hyperlink(new Run(user.NickName));
                  //hyperlink.NavigateUri = new Uri(user.ProfileUrl);
                  //hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler(hyperlink_RequestNavigate);
                  //tb.Inlines.Add(hyperlink);
                  //outString.Append(user.NickName);
                  if (user.NickName != null)
                  {
                    tb.Inlines.Add(user.NickName);
                  }
                }
              }
            }
            //When there is more people who like
            if (nbShowed < like.Count)
            {
#if SILVERLIGHT
              tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeAnd"));
#else
              tb.Inlines.Add(new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeAnd").ResolveLocalizedValue());
#endif
              //var hyperlink = new Hyperlink(new Run((like.Count - nbShowed).ToString() + " other"));
              //hyperlink.NavigateUri = new Uri(like.Href);
              //hyperlink.RequestNavigate += new System.Windows.Navigation.RequestNavigateEventHandler();
              //tb.Inlines.Add(hyperlink);
              //outString.Append(" ");
              //outString.Append((like.Count - nbShowed).ToString());
              //outString.Append(" other");
#if SILVERLIGHT
              if (like.Count - nbShowed >1)
              {

                tb.Inlines.Add((like.Count - nbShowed) + LocalizationManager.GetString("txtFBLikeOthers"));
              }
              else
              {
                tb.Inlines.Add((like.Count - nbShowed) + LocalizationManager.GetString("txtFBLikeOther"));
              }
#else

              if (like.Count - nbShowed > 1)
              {
                if (Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("it"))
                {
                  tb.Inlines.Add(like.Count - nbShowed + " ");
                }
                else
                {
                  tb.Inlines.Add((like.Count - nbShowed) +
                                 new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeOthers")
                                   .ResolveLocalizedValue());
                }
              }
              else
              {
                if (Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("it"))
                {
                  tb.Inlines.Add((like.Count - nbShowed) + " ");
                }
                else
                {
                  tb.Inlines.Add((like.Count - nbShowed) +
                                 new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeOther")
                                   .ResolveLocalizedValue());
                }
              }
              ////tb.Inlines.Add((like.Count - nbShowed) + new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeOther").ResolveLocalizedValue());
#endif
            }


            //add the verbs
            if (like.Count == 1 & like.LikeIt != 1)
            {
#if SILVERLIGHT
              tb.Inlines.Add(LocalizationManager.GetString("txtFBLikesThis"));
#else
              tb.Inlines.Add(
                new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikesThis").ResolveLocalizedValue());
#endif
              //tb.Inlines.Add(" likes this");
              //outString.Append(" likes it");
            }
            else
            {
#if SILVERLIGHT
              if (like.Count > 1 & like.LikeIt != 1)
              {
                tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeThis"));
              }
              else switch (like.LikeIt)
                {
                  case 1:
                    tb.Inlines.Add(LocalizationManager.GetString("txtFBLikeMeThis"));
                    break;
                  default:
                    tb.Inlines.Add(LocalizationManager.GetString("txtFBLikesThis"));
                    break;
                }

#else

              if (like.Count > 1 & like.LikeIt != 1)
              {
                if (!Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("de"))
                {
                  tb.Inlines.Add(
                    new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeThis").ResolveLocalizedValue());
                }
              }
              else
                switch (like.LikeIt)
                {
                  case 1:
                    if (!Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("de"))
                    {
                      tb.Inlines.Add(
                        new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeMeThis").ResolveLocalizedValue());
                    }
                    break;
                  default:
                    if (!Thread.CurrentThread.CurrentUICulture.Name.ToLower().Equals("de"))
                    {
                      tb.Inlines.Add(
                        new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikesThis").ResolveLocalizedValue());
                    }
                    break;
                }


              ////tb.Inlines.Add(new LocText("Sobees.Configuration.BGlobals:Resources:txtFBLikeThis").ResolveLocalizedValue());
#endif
              //outString.Append(" like it");
            }
            //return outString.ToString();
#if !SILVERLIGHT
            return tb;
#else
            return tb.Text;
#endif
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this,
          ex);
      }
      return null;
    }

    /*void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      WebHelper.NavigateToUrl(e.Uri.ToString());
    }*/

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}