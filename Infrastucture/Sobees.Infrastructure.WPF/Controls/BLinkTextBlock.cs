using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Helpers;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BGenericLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Web;

namespace Sobees.Infrastructure.Controls
{
  public class BLinkTextBlock : BTranslateTextBox
  {
    #region Private fields

    #endregion

    #region Properties

    #endregion

    #region Dependency properties


    public int Nb { get; set; }

    #endregion

    #region Constructors

    protected override void OnTextChangedChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(BText))
      {
        var txt = BText;
        Inlines.Clear();
        Inlines.Add(txt);
        var isTranslated = IsTranslated;

#if !SILVERLIGHT
        using (var worker = new BackgroundWorker())
        {
          worker.DoWork += delegate(object s,
                                    DoWorkEventArgs eargs)
          {
            if (worker.CancellationPending)
            {
              eargs.Cancel = true;
              return;
            }

#endif
            if (isTranslated)
            {
              try
              {
                //txt = Translator.Translate(txt, SobeesSettingsLocator.SobeesSettingsStatic.LanguageTranslator).Replace("@ ", "@").Replace("# ", "#");
                var targetLang = SobeesSettingsLocator.SobeesSettingsStatic.Language;
                txt = BingApiHelper.TranslateText(txt, targetLang);
              }
              catch (Exception exception)
              {
                Console.WriteLine(exception);
              }
            }
            DoStuffOnText(this, txt);
#if !SILVERLIGHT
          };

          worker.RunWorkerCompleted += delegate { };

          worker.RunWorkerAsync();
        }
#endif
      }
    }

    #endregion

    #region Regular Hyperlink Click

    public static void HyperLinkUnloaded(object sender, RoutedEventArgs e)
    {
      var hyperLink = sender as Hyperlink;
      if (hyperLink == null) return;
      hyperLink.Unloaded -= HyperLinkUnloaded;
      hyperLink.Click -= RequestNavigate;
    }

    public static void RequestNavigate(object sender, RoutedEventArgs e)
    {
      try
      {
        var model = ((TextBlock)((Hyperlink)sender).Parent).Tag as BWorkspaceViewModel;
        if (model != null && !string.IsNullOrEmpty(((Hyperlink)sender).Tag as string))
        {
          var str = ((Hyperlink) sender).Tag as string;
          if (str != null)
            if(str.StartsWith("#"))
            {
              Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount(str, EnumAccountType.TwitterSearch)));
            }
            else
            {
              model.ShowUserProfile(str);
            }
        }
        else
        {
          WebHelper.NavigateToUrl(((Hyperlink)sender).NavigateUri.ToString());
        }
        
      }
      catch (Exception ex)
      {
        if (sender != null)
          TraceHelper.Trace(sender.ToString(), ex);
        else
          TraceHelper.Trace("BTextBlock::RequestNavigate::", ex);
      }
    }

    #endregion

    #region Methods

    public override void DoStuffOnText(BTranslateTextBox block, string text)
    {
      Nb = 0;
      var lst = new List<object>();
      var lst2 = new List<object>();
      if (string.IsNullOrEmpty(text)) return;

      //lst.Add(" "); // We need it! --> HACK when link in first position of the TextBlock!

      var words = Regex.Split(text, @"([\n 」　\(\)\{\}\[\]（）]|\.\.\.)");

      foreach (string word in words)
      {
        // Hyperlinks
        if (HyperLinkHelper.IsHyperlink(word))
        {
          try
          {
            string displayLink = word;
            if (!displayLink.StartsWith("http://") && !displayLink.StartsWith("https://"))
              displayLink = $"http://{displayLink}";
            lst.Add(word);
            var element = word;
            Nb++;
            var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                                                  (DispatcherOperationCallback)(o =>
                                                                                                  {
                                                                                                    try
                                                                                                    {
                                                                                                      ;
                                                                                                      var link = new Hyperlink
                                                                                                                   {
                                                                                                                     Focusable
                                                                                                                       =
                                                                                                                       false,
                                                                                                                     NavigateUri
                                                                                                                       =
                                                                                                                       new Uri
                                                                                                                       (CleanUrl
                                                                                                                          (displayLink)),
                                                                                                                     ToolTip
                                                                                                                       =
                                                                                                                       "Open link in the default browser"
                                                                                                                   };
                                                                                                      link.Inlines.Add(
                                                                                                        element);
                                                                                                      link.Click +=
                                                                                                        RequestNavigate;
                                                                                                      link.Unloaded +=
                                                                                                        HyperLinkUnloaded;
                                                                                                      lst2.Add(link);
                                                                                                    }
                                                                                                    catch
                                                                                                    {
                                                                                                    }
                                                                                                    return null;
                                                                                                  }), null);
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
                                      int i = lst.IndexOf(((Run)(link.Inlines.FirstInline)).Text);
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
    private new static string CleanUrl(string displayLink)
    {
      if (!char.IsLetterOrDigit(displayLink[0]))
      {
        displayLink = displayLink.Remove(0, 1);
      }
      if (!char.IsLetterOrDigit(displayLink[displayLink.Length - 1]))
      {
        displayLink = displayLink.Remove(displayLink.Length - 1, 1);
      }
      return displayLink.Replace(";", "").Replace(",", "");
    }

    public override void FinalizeWork(TextBlock block, List<Object> lst, string text)
    {
      block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                   (DispatcherOperationCallback)
                                   delegate
                                     {
                                       try
                                       {
                                         if (text != BText.ToString() && !IsTranslated)
                                         {
                                           return null;
                                         }
                                         block.Inlines.Clear();
                                         foreach (object obj in lst)
                                         {
                                           if (obj is string)
                                           {
                                             block.Inlines.Add((string)obj);
                                           }
                                           else
                                           {
                                             block.Inlines.Add((Hyperlink)obj);
                                           }
                                         }
                                       }
                                       catch (Exception ex)
                                       {
                                         TraceHelper.Trace("ImageCacheHelper", ex);
                                       }
                                       return null;
                                     },
                                   null);
    }


    public override object DoStuffOnWord(string word, BTranslateTextBox block, string text, List<object> lst)
    {
      lst.Add(word);
      return word;
    }

    #endregion

    #region Events

    #endregion
  }
}