#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Sobees.Library.BGenericLib;

#endregion

namespace Sobees.Views
{
  /// <summary>
  ///   Interaction logic for MultiPostControl.xaml
  /// </summary>
  public partial class MultiPostControl : UserControl
  {
    public MultiPostControl()
    {
      InitializeComponent();
    }
    #region Autocomplete

    private static readonly Regex AutoSuggestPattern = new Regex(@"(^.*@|^d |^D )(\w*)$");
    protected bool IsInAutocompleteMode { get; set; }
    protected bool IgnoreKey { get; set; }



    private void txtTweet_TextChanged(object sender,
                                      TextChangedEventArgs e)
    {
      Suggest(txtTweet,
              AutoSuggestPattern,
              0);
    }

    private void Suggest(TextBox textBox,
                         Regex matchAndReplace,
                         int offset)
    {
        var friends = grMainMulti.Tag as List<User>;
        if (IgnoreKey || friends == null) return;

        var currentFriends = new List<string>();
        string selectedText = string.Empty;
        if (IsInAutocompleteMode)
        {
            IgnoreKey = true;
            selectedText = textBox.SelectedText;
            textBox.SelectedText = string.Empty;
            IgnoreKey = false;
        }

        int length = textBox.Text.Length;
        IsInAutocompleteMode = matchAndReplace.IsMatch(textBox.Text);
        if (!IsInAutocompleteMode)
            return;
        if (textBox.CaretIndex != textBox.Text.Length) return;
        var matchedText = matchAndReplace.Match(textBox.Text);
        if (!matchedText.Success) return;
        var pos = matchedText.Value.IndexOf("@");
        if (pos > 0)
        {
            if (matchedText.Value[pos - 1] != ' ')
            {
                return;
            }
        }
        var userEnteredText = matchedText.Groups[2].Value;
        {
            currentFriends.AddRange(from friend in friends
                                    where friend.NickName.StartsWith(userEnteredText, StringComparison.CurrentCultureIgnoreCase) || userEnteredText.Length == 0
                                    select friend.NickName);
        }
        if (currentFriends.Count != 0)
        {
            currentFriends.Sort();

            int selectedIndex = currentFriends.IndexOf(userEnteredText + selectedText);
            if (selectedIndex < 0) selectedIndex = 0;
            selectedIndex += offset;
            if (selectedIndex < 0) selectedIndex = currentFriends.Count - 1;
            else if (selectedIndex > (currentFriends.Count - 1)) selectedIndex = 0;

            IgnoreKey = true;
            textBox.Text = matchAndReplace.Replace(textBox.Text,
                                                   String.Format("${{1}}{0}",
                                                                 currentFriends[selectedIndex]));
            textBox.Select(length,
                           textBox.Text.Length - length);
            //textBox.Select(textBox.Text.IndexOf(currentFriends[selectedIndex]), textBox.Text.Length - length);
            IgnoreKey = false;
        }
    }


    private void txtTweet_PreviewKeyDown(object sender,
                                         KeyEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.Control && Key.Space == e.Key)
      {
        IsInAutocompleteMode = true;
        Suggest(txtTweet,
                AutoSuggestPattern,
                0);
        e.Handled = true;
      }

      if (!IsInAutocompleteMode) return;

      if (Key.Tab == e.Key)
      {
        IgnoreKey = true;
        txtTweet.Select(txtTweet.Text.Length,
                        0);
        txtTweet.Text += " ";
        e.Handled = true;
        txtTweet.Select(txtTweet.Text.Length,
                        0);
      }
      //HACK: need to ignore the keypress for backspace
      else if (Key.Back == e.Key)
        IgnoreKey = true;
      else if (Key.Up == e.Key)
      {
        Suggest(txtTweet,
                AutoSuggestPattern,
                -1);
        e.Handled = true;
      }
      else if (Key.Down == e.Key)
      {
        Suggest(txtTweet,
                AutoSuggestPattern,
                1);
        e.Handled = true;
      }
      else
        IgnoreKey = false;
    }

    #endregion
  }
}