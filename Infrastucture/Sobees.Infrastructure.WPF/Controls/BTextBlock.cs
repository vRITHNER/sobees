#region

using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Helpers;
using Sobees.Infrastructure.UI;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BResolveUrlLibrary;
using Sobees.Tools.Logging;
using Sobees.Tools.Threading.Extensions;
using Sobees.Tools.Web;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

#endregion

namespace Sobees.Infrastructure.Controls
{
  public class BTextBlock : BTranslateTextBox
  {
    #region Private fields

    private TextPointer _endpointer;
    private object _previousbackgroundcolor;
    private bool _selectingText;
    private TextPointer _startpointer;

    #endregion Private fields

    #region Properties

    public int Nb { get; set; }

    #endregion Properties

    #region Dependency properties

    #endregion Dependency properties

    #region Constructors

    public BTextBlock()
    {
      MouseLeftButtonDown += BTextBlockMouseLeftButtonDown;
      MouseLeftButtonUp += BTextBlockMouseLeftButtonUp;
      MouseMove += BTextBlockMouseMove;
      MouseLeave += BTextBlockMouseLeave;
      Loaded += BTextBlockLoaded;
    }

    protected override void OnTextChangedChanged(DependencyPropertyChangedEventArgs e)
    {
      if (string.IsNullOrEmpty(BText)) return;
      var txt = BText;
      Inlines.Clear();
      Inlines.Add(txt);
      var isTranslated = IsTranslated;

      Action mainAction = () =>
      {
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
      };

      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
      task.Wait();

      DoStuffOnText(this, txt);
    }

    protected void BTextBlockLoaded(object sender, RoutedEventArgs e)
    {
      _previousbackgroundcolor = GetValue(TextElement.BackgroundProperty);
      PreviewMouseRightButtonUp += BTextBlockPreviewMouseRightButtonUp;
      Unloaded += BTextBlockUnloaded;
    }

    private void BTextBlockUnloaded(object sender, RoutedEventArgs e)
    {
      MouseLeftButtonDown -= BTextBlockMouseLeftButtonDown;
      MouseLeftButtonUp -= BTextBlockMouseLeftButtonUp;
      MouseMove -= BTextBlockMouseMove;
      MouseLeave -= BTextBlockMouseLeave;
      Loaded -= BTextBlockLoaded;
      PreviewMouseRightButtonUp -= BTextBlockPreviewMouseRightButtonUp;
      Unloaded -= BTextBlockUnloaded;
    }

    private void BTextBlockPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      Clipboard.SetText(BText);
    }

    #endregion Constructors

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
          var str = ((Hyperlink)sender).Tag as string;
          if (str == null) return;
          if (str.StartsWith("#"))
            Messenger.Default.Send(new BMessage("NewAccountFirstUse", new UserAccount(str, EnumAccountType.TwitterSearch)));
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
        TraceHelper.Trace(sender != null ? sender.ToString() : "BTextBlock::RequestNavigate::", ex);
      }
    }

    #endregion Regular Hyperlink Click

    #region Methods

    public override async void DoStuffOnText(BTranslateTextBox block, string text)
    {
      Nb = 0;
      var lst = new List<object>();
      var lst2 = new List<object>();
      if (string.IsNullOrEmpty(text)) return;

      //lst.Add(" "); // We need it! --> HACK when link in first position of the TextBlock!

      var words = Regex.Split(text, @"([\n 」　\(\)\{\}\[\]（）]|\.\.\.)");

      foreach (var word in words)
      {
        // Hyperlinks
        if (HyperLinkHelper.IsHyperlink(word))
        {
          try
          {
            var cleanword = word.Replace("\\/", "/");
            cleanword = await ResolveUrlHelper.ResolveFullShortUrl(cleanword);
            var displayLink = cleanword;
            if (!displayLink.StartsWith("http://"))
              displayLink = $"http://{displayLink}";
            lst.Add(cleanword);
            var element = cleanword;
            Nb++;
            var op = block.Dispatcher.BeginInvoke(DispatcherPriority.Background,
              (DispatcherOperationCallback)delegate
              {
                try
                {
                  var link = new Hyperlink
                  {
                    Focusable = false,
                    NavigateUri = new Uri(CleanUrl(displayLink)),
                    ToolTip = "Open link in the default browser"
                  };
                  link.Inlines.Add(element);
                  link.Click += RequestNavigate;
                  link.Unloaded += HyperLinkUnloaded;
                  lst2.Add(link);
                }
                catch
                {}
                return null;
              }, null);
            op.Completed += delegate
            {
              try
              {
                while (lst2.Any())
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
                    FinalizeWork(block, lst, text);
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
      //if (Nb == 0) //TODO: Verify if nb==0 is necessary !!! vr 21/06/2014
      {
        FinalizeWork(block, lst, text);
      }
    }

    private const string APPNAME = "BTextBock";

    public override void FinalizeWork(TextBlock block, List<Object> lst, string text)
    {
      TraceHelper.Trace(APPNAME, "FinalizeWork::START", true);
      try
      {
        if (BText == null || text != BText && !IsTranslated)
          return; //null;

        block.Inlines.Clear();
        foreach (var obj in lst)
        {
          var s = obj as string;
          if (s != null)
          {
            block.Inlines.Add(s);
            TraceHelper.Trace(APPNAME, "block.Inlines.Add(s)", true);
          }
          else
          {
            block.Inlines.Add((Hyperlink)obj);
            TraceHelper.Trace(APPNAME, "block.Inlines.Add((Hyperlink)obj);", true);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(APPNAME,  ex);
      }
      TraceHelper.Trace(APPNAME, "FinalizeWork::END", true);
    }

    public override object DoStuffOnWord(string word, BTranslateTextBox block, string text, List<object> lst)
    {
      lst.Add(word);
      return word;
    }

    #endregion Methods

    #region Events

    protected void BTextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        _startpointer = GetPositionFromPoint(Mouse.GetPosition(this), true);
        if (_startpointer != null)
        {
          var highlighttext = new TextRange(_startpointer, _startpointer);
        }
        _selectingText = true;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    protected void BTextBlockMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        _endpointer = GetPositionFromPoint(Mouse.GetPosition(this), true);
        _selectingText = false;

        if (_endpointer != null)
        {
          if (_startpointer != null)
          {
            var clipboardtext = new TextRange(_startpointer, _endpointer);
            Clipboard.SetText(clipboardtext.Text);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      var documenttext = new TextRange(ContentStart, ContentEnd);
      //if (_previousbackgroundcolor == null) return;
      documenttext.ApplyPropertyValue(TextElement.BackgroundProperty, _previousbackgroundcolor);
    }

    protected void BTextBlockMouseMove(object sender, MouseEventArgs e)
    {
      if (_selectingText)
      {
        var currentpointer = GetPositionFromPoint(Mouse.GetPosition(this), true);

        try
        {
          if (currentpointer == null) return;
          var highlighttext = new TextRange(_startpointer, currentpointer);
          highlighttext.ApplyPropertyValue(TextElement.BackgroundProperty,
            new SolidColorBrush(Colors.LightSkyBlue));
        }
        catch (Exception ex)
        {
          TraceHelper.Trace(this, ex);
        }
      }
    }

    protected void BTextBlockMouseLeave(object sender, MouseEventArgs e)
    {
      var documenttext = new TextRange(ContentStart, ContentEnd);
      if (_previousbackgroundcolor == null) new SolidColorBrush(Colors.Transparent);
      if (_previousbackgroundcolor != null)
        documenttext.ApplyPropertyValue(TextElement.BackgroundProperty, _previousbackgroundcolor);
      _selectingText = false;
    }

    #endregion Events
  }
}