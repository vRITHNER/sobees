#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Sobees.Tools.Logging;

#endregion

namespace Sobees.Infrastructure.Controls
{
  public class BLinkTweetTextBlock : BLinkTextBlock
  {
    #region Private fields

    #endregion

    #region Contructors

    #endregion

    #region Methds

    public override object DoStuffOnWord(string word, BTranslateTextBox block, string text, List<object> lst)
    {
      var lst2 = new List<object>();
      // @name
      if (word.StartsWith("@"))
      {
        var foundUsername = Regex.Match(word, @"@(\w+)(?<suffix>.*)");
        if (foundUsername.Success)
        {
          var userName = foundUsername.Groups[1].Captures[0].Value;
          lst.Add(userName);
          Nb++;
          var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            (DispatcherOperationCallback) (o =>
            {
              var name = new Hyperlink
              {
                Focusable = false,
                Tag = userName,
                ToolTip = $"View @{userName}'s recent tweets",
                NavigateUri = new Uri($"http://twitter.com/{userName}"),
              };
              name.Inlines.Add(userName);

              name.Click += RequestNavigate;
              name.Unloaded += HyperLinkUnloaded;
              lst2.Add("@");
              lst2.Add(name);
              lst2.Add(foundUsername.Groups["suffix"].Captures[0].Value);
              return null;
            }), null);
          op.Completed += delegate
          {
            try
            {
              while (lst2.Count > 0 && lst2.Count%3 == 0)
              {
                var txt1 = lst2[0];
                lst2.RemoveAt(0);
                var link = lst2[0] as Hyperlink;
                lst2.RemoveAt(0);
                var txt2 = lst2[0];
                lst2.RemoveAt(0);
                if (link != null)
                {
                  var i = lst.IndexOf(((Run) (link.Inlines.FirstInline)).Text);
                  lst.RemoveAt(i);
                  lst.Insert(i, txt2);
                  lst.Insert(i, link);
                  lst.Insert(i, txt1);
                }
                Nb--;
                if (Nb == 0)
                {
                  FinalizeWork(block, lst, text);
                }
              }
            }
            catch (Exception ex)
            {
              TraceHelper.Trace(this, ex);
            }
          };
        }
        else
        {
          lst.Add(word);
        }
      }
        // #hashtag
      else if (word.StartsWith("#"))
      {
        var hashtag = String.Empty;
        var foundHashtag = Regex.Match(word, @"#(\w+)(?<suffix>.*)");
        const string hashtagUrl = "http://search.twitter.com/search?q=%23{0}";
        if (!foundHashtag.Success) return lst;
        lst.Add(foundHashtag.Groups[1].Captures[0].Value);
        Nb++;
        var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
          (DispatcherOperationCallback) delegate
          {
            hashtag = foundHashtag.Groups[1].Captures[0].Value;
            var tag = new Hyperlink
            {
              Focusable = false,
              ToolTip = "Show statuses that include this hashtag",
              Tag = $"#{hashtag}",
              NavigateUri = new Uri(String.Format(hashtagUrl, hashtag))
            };
            tag.Inlines.Add(hashtag);


            // The main application has access to the Settings class, where a 
            // user-defined hashtagUrl can be stored.  This hardcoded one that
            // is used to set the NavigateUri is just a default behavior that
            // will be used if the click event is not handled for some reason.

            tag.Click += RequestNavigate;
            tag.Unloaded += HyperLinkUnloaded;

            lst2.Add("#");
            lst2.Add(tag);
            lst2.Add(foundHashtag.Groups["suffix"].Captures[0].Value);
            return null;
          }, null);
        op.Completed += delegate
        {
          try
          {
            while (lst2.Count > 0 && lst2.Count%3 == 0)
            {
              var txt1 = lst2[0];
              lst2.RemoveAt(0);
              var link = lst2[0] as Hyperlink;
              lst2.RemoveAt(0);
              var txt2 = lst2[0];
              lst2.RemoveAt(0);
              if (link != null)
              {
                var i = lst.IndexOf(((Run) (link.Inlines.FirstInline)).Text);
                lst.RemoveAt(i);
                lst.Insert(i, txt2);
                lst.Insert(i, link);
                lst.Insert(i, txt1);
              }
              Nb--;
              if (Nb == 0)
              {
                FinalizeWork(block, lst, text);
              }
            }
          }
          catch (Exception ex)
          {
            TraceHelper.Trace(this, ex);
          }
        };
      }
      else
      {
        return base.DoStuffOnWord(word, block, text, lst);
      }
      return lst;
    }

    #endregion

    #region Clickable #hashtag

    public static readonly RoutedEvent HashtagClickEvent = EventManager.RegisterRoutedEvent(
      "HashtagClick", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (BLinkTweetTextBlock));

    public event RoutedEventHandler HashtagClick
    {
      add { AddHandler(HashtagClickEvent, value); }
      remove { RemoveHandler(HashtagClickEvent, value); }
    }

    #endregion

    #region Clickable @name

    public static readonly RoutedEvent NameClickEvent = EventManager.RegisterRoutedEvent(
      "NameClick", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (BLinkTweetTextBlock));

    public event RoutedEventHandler NameClick
    {
      add { AddHandler(NameClickEvent, value); }
      remove { RemoveHandler(NameClickEvent, value); }
    }

    private static void name_Click(object sender, RoutedEventArgs e)
    {
      // The event handler in the main application can handle the click event in a custom
      // fashion (i.e., show the tweets in Witty or launch a URL, etc).  That behavior is
      // not implemented here.
      try
      {
        if (e.OriginalSource is Hyperlink)
        {
          var h = e.OriginalSource as Hyperlink;
          if (h.Parent is BTweetTextBlock)
          {
            var p = h.Parent as BTweetTextBlock;
            p.RaiseEvent(new RoutedEventArgs(NameClickEvent, h));
            return;
          }
        }

        // As a fallback (i.e., if the event is not handled), we launch the hyperlink's
        // URL in a web browser

        Process.Start(((Hyperlink) sender).NavigateUri.ToString());
      }
      catch
      {
        //TODO: Log specific URL that caused error
        MessageBox.Show("There was a problem launching the specified URL.", "Error", MessageBoxButton.OK,
          MessageBoxImage.Exclamation);
      }
    }

    #endregion
  }
}