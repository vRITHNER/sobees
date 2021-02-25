#region

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

#endregion

namespace Sobees.Controls.Facebook.Templates
{
  

  /// <summary>
  ///   Interaction logic for DtStream.xaml
  /// </summary>
  public partial class DtStream
  {
    public DtStream()
    {
      InitializeComponent();
    }

    private void btnComment_Click(object sender, RoutedEventArgs e)
    {
      var btn = sender as Button;
      if (btn != null)
        if (btn.Name == btnComment1.Name)
        {
          var entry = DataContext as FacebookFeedEntry;
          if (entry != null)
            if (!entry.Comments.Any() && entry.NbComments > 0)
            {
              btnShowAllComment.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btnShowAllComment));
              btnShowAllComment.Visibility = Visibility.Collapsed;
              btnShowAllComment.Command.Execute(entry.Id);
            }
        }
      stkItemCommentDetails.Visibility = stkItemCommentDetails.Visibility == Visibility.Collapsed
        ? Visibility.Visible
        : Visibility.Collapsed;
    }

    private void BtnCancelClick(object sender, RoutedEventArgs e)
    {
      txtblkComment.Text = string.Empty;
      stkItemCommentDetails.Visibility = Visibility.Collapsed;
    }
  }
}