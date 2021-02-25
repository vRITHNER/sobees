#region

using Sobees.Infrastructure.ViewModelBase;
using Sobees.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Sobees.Controls
{
  public class BIcon : Grid
  {
    public static DependencyProperty IconViewModelProperty = DependencyProperty.Register("IconViewModel",
      typeof(IconViewModel), typeof(BIcon), null);

    public static DependencyProperty WorkspacesProperty = DependencyProperty.Register("ServiceWorkspaces",
      typeof(ObservableCollection<BWorkspaceViewModel>), typeof(BIcon), null);

    public BIcon()
    {
      Loaded += BIconLoaded;
    }

    public IconViewModel IconViewModel
    {
      get { return GetValue(IconViewModelProperty) as IconViewModel; }
      set { SetValue(IconViewModelProperty, value); }
    }

    public ObservableCollection<BServiceWorkspaceViewModel> ServiceWorkspaces
    {
      get { return GetValue(WorkspacesProperty) as ObservableCollection<BServiceWorkspaceViewModel>; }
      set { SetValue(WorkspacesProperty, value); }
    }

    private void BIconLoaded(object sender,
      RoutedEventArgs e)
    {
      Loaded -= BIconLoaded;

      var cc = new ContentControl
      {
        Content = IconViewModel,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalContentAlignment = HorizontalAlignment.Stretch,
        VerticalContentAlignment = VerticalAlignment.Stretch,
        ContentTemplate = IconViewModel.DataTemplateView
      };

      if (!Children.Contains(cc))
        Children.Add(cc);
    }
  }
}