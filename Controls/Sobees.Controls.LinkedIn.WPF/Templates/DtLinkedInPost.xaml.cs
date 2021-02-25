#region

using System.Windows;

#endregion

namespace Sobees.Controls.LinkedIn.Templates
{
  /// <summary>
  ///   Interaction logic for DtLinkedInPost.xaml
  /// </summary>
  public partial class DtLinkedInPost
  {
    public DtLinkedInPost()
    {
      InitializeComponent();
    }

    private void ButtonClick(object sender, RoutedEventArgs e)
    {
      BtnComment.IsChecked = false;
      TxtComment.Text = string.Empty;
    }
  }
}