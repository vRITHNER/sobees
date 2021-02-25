using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Documents;
using System.Windows.Threading;
using Sobees.Infrastructure.Controls;
using Sobees.Tools.Logging;
using Sobees.Tools.Web;

namespace Sobees.Controls.LinkedIn.Cls
{
  class BLinkedInTextBlock :BTextBlock
  {
    public override void DoStuffOnText(BTranslateTextBox block, string text)
    {
      Nb = 0;
      var lst = new List<object>();
      var lst2 = new List<object>();
      if (string.IsNullOrEmpty(text)) return;

      //lst.Add(" "); // We need it! --> HACK when link in first position of the TextBlock!
      var Regex = new System.Text.RegularExpressions.Regex(@"<a href.*?</a>");
      var result = Regex.Matches(text);
      foreach (var replace in result)
      {
        text = text.Replace(replace.ToString(), HttpUtility.UrlEncode(HttpUtility.HtmlEncode(replace.ToString())));
      }
      string[] words = Regex.Split(text, @"([\n \{\}\[\]\(\)])");

      foreach (string word in words)
      {
        // Hyperlinks
        if (HyperLinkHelper.IsHyperlink(word))
        {
          try
          {
            var displayLink = word;
            if (!displayLink.StartsWith("http://"))
              displayLink = "http://" + displayLink;
            lst.Add(word);
            string element = word;
            Nb++;
            var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                       (DispatcherOperationCallback)delegate
                                       {

                                         try
                                         {
                                           var link = new Hyperlink()
                                           {
                                             Focusable = false,
                                             NavigateUri = new Uri(displayLink),
                                             TargetName = "_blank",
                                             ToolTip = "Open link in the default browser"
                                           };
                                           link.Click += RequestNavigate;
                                           link.Unloaded += HyperLinkUnloaded;
                                           link.Inlines.Add(element);
                                           lst2.Add(link);
                                         }
                                         catch
                                         {
                                         }
                                         return null;
                                       }, null);
            op.Completed += delegate
            {
              try
              {
                while (lst2.Count > 0)
                {
                  var link = lst2[0] as Hyperlink;
                  lst2.RemoveAt(0);
                  if (link != null)
                  {
                    var i = lst.IndexOf(((Run)(link.Inlines.FirstInline)).Text);
                    lst.RemoveAt(i);
                    lst.Insert(i, link);
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
          catch
          {
            //TODO:What are we catching here? Why? Log it?
            lst.Add(word);
          }
        }
        else
        {
          DoStuffOnWord(word, block, text, lst);
        }
      }
      lst.Add(" ");
      if (Nb == 0)
      {
        FinalizeWork(block, lst, text);
      }
    }

    public override object DoStuffOnWord(string word, BTranslateTextBox block, string text, List<object> lst)
    {

      var lst2 = new List<object>();
      // @name
      if (word.Contains("href"))
      {
        Nb++;
        lst.Add(word);
        var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                              (DispatcherOperationCallback)delegate
                                              {
                                                var txt = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(word));
                                                string[] txts = System.Text.RegularExpressions.Regex.Split(txt, "(</a>)|(<a href=\")|\">");
                                                var name = new Hyperlink
                                                {
                                                  Focusable = false,
                                                  TargetName = "_blank",
                                                  NavigateUri = new Uri(txts[2]),
                                                  Tag = word
                                                };
                                                name.Click += RequestNavigate;
                                                name.Unloaded += HyperLinkUnloaded;
                                                name.Inlines.Add(txts[3]);
                                                lst2.Add(name);
                                                return null;
                                              }, null);
        op.Completed += delegate
        {
          try
          {
            while (lst2.Count > 0)
            {
              var link = lst2[0] as Hyperlink;
              lst2.RemoveAt(0);
              if (link != null)
              {
                var i = lst.IndexOf(link.Tag as string);
                lst.RemoveAt(i);
                lst.Insert(i, link);
              }
              Nb--;
              if (Nb == 0)
              {
                FinalizeWork(block, lst, HttpUtility.HtmlDecode(HttpUtility.UrlDecode(text)));
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
  }
}