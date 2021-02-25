using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sobees.Library.BGenericLib;

namespace Sobees.Controls.Twitter.Controls
{
  /// <summary>
  /// Interaction logic for UcEditList.xaml
  /// </summary>
  public partial class UcEditList
  {
    private static readonly Regex AutoSuggestPattern = new Regex(@"(^\w)(\w*)$");
    protected bool IsInAutocompleteMode { get; set; }
    protected bool IgnoreKey { get; set; }
    public UcEditList()
    {
      InitializeComponent();
    }
    private void TxtTweetTextChanged(object sender,
                                  TextChangedEventArgs e)
    {
      Suggest(txtUserName,
              AutoSuggestPattern,
              0);
    }

    private void Suggest(TextBox textBox,
                           Regex matchAndReplace,
                           int offset)
    {
        var friends = txtUserName.Tag as List<User>;
        if (IgnoreKey || friends == null) return;

        var currentFriends = new List<string>();
        var selectedText = string.Empty;
        if (IsInAutocompleteMode)
        {
            IgnoreKey = true;
            selectedText = textBox.SelectedText;
            textBox.SelectedText = string.Empty;
            IgnoreKey = false;
        }

        var length = textBox.Text.Length;
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
        var userEnteredText = matchedText.Groups[0].Value;
        {
            currentFriends.AddRange(from friend in friends
                                    where friend.NickName.StartsWith(userEnteredText, StringComparison.CurrentCultureIgnoreCase) || userEnteredText.Length == 0
                                    select friend.NickName);
        }
        if (currentFriends.Count == 0) return;
        currentFriends.Sort();

        var selectedIndex = currentFriends.IndexOf(userEnteredText + selectedText);
        if (selectedIndex < 0) selectedIndex = 0;
        selectedIndex += offset;
        if (selectedIndex < 0) selectedIndex = currentFriends.Count - 1;
        else if (selectedIndex > (currentFriends.Count - 1)) selectedIndex = 0;

        IgnoreKey = true;
        textBox.Text = matchAndReplace.Replace(textBox.Text,
                                               String.Format("{0}",
                                                             currentFriends[selectedIndex]));
        textBox.Select(length,
                       textBox.Text.Length - length);
        //textBox.Select(textBox.Text.IndexOf(currentFriends[selectedIndex]), textBox.Text.Length - length);
        IgnoreKey = false;
    }


    private void TxtTweetPreviewKeyDown(object sender,
                                         KeyEventArgs e)
    {
      if (Keyboard.Modifiers == ModifierKeys.Control && Key.Space == e.Key)
      {
        IsInAutocompleteMode = true;
        Suggest(txtUserName,
                AutoSuggestPattern,
                0);
        e.Handled = true;
      }

      if (!IsInAutocompleteMode) return;

      switch (e.Key)
      {
        case Key.Tab:
          IgnoreKey = true;
          txtUserName.Select(txtUserName.Text.Length,
                             0);
          txtUserName.Text += " ";
          e.Handled = true;
          txtUserName.Select(txtUserName.Text.Length,
                             0);
          break;
        case Key.Back:
          IgnoreKey = true;
          break;
        case Key.Up:
          Suggest(txtUserName,
                  AutoSuggestPattern,
                  -1);
          e.Handled = true;
          break;
        case Key.Down:
          Suggest(txtUserName,
                  AutoSuggestPattern,
                  1);
          e.Handled = true;
          break;
        default:
          IgnoreKey = false;
          break;
      }
    }

    private void txtUserName_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        //((FrameworkElement)sender).Tag = null;
    }
  }
}
