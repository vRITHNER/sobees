using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Sobees.Infrastructure.Controls
{
  /// <summary>
  /// Interaction logic for BToggleButton.xaml
  /// </summary>
  public partial class BToggleButton
  {
    #region Dependency properties

    public static DependencyProperty IsCheckedProperty =
      DependencyProperty.Register("IsChecked", typeof(bool), typeof(BToggleButton),
                                  new FrameworkPropertyMetadata(true, IsCheckedPropertyChanged));
    public static readonly DependencyProperty BNumberNewProperty =
  DependencyProperty.Register("BNumberNew", typeof(int), typeof(BToggleButton),
                              new FrameworkPropertyMetadata(0, BNumberNewChanged));

    public static readonly DependencyProperty BPathProperty =
      DependencyProperty.Register("BPath", typeof (string), typeof (BToggleButton),
                                  new FrameworkPropertyMetadata(string.Empty, BPathChanged));

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register("Command", typeof (ICommand), typeof (BToggleButton),
                                  new FrameworkPropertyMetadata(null, CommandChanged));

    public bool IsChecked
    {
      get { return (bool)GetValue(IsCheckedProperty); }
      set { SetValue(IsCheckedProperty, value); }
    }
    public int BNumberNew
    {
      get { return (int)GetValue(BNumberNewProperty); }
      set { SetValue(BNumberNewProperty, value); }
    }
    public string BPath
    {
      get { return (string) GetValue(BPathProperty); }
      set { SetValue(BPathProperty, value); }
    }

    public ICommand Command
    {
      get { return (ICommand) GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// property change event handler for BindingProperty
    /// </summary>
    private static void IsCheckedPropertyChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BToggleButton)depObj).OnIsCheckedChanged(e);
    } 
    private static void BNumberNewChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BToggleButton) depObj).OnBNumberNewChanged(e);
    }

    private static void CommandChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BToggleButton) depObj).OnCommandChanged(e);
    }

    protected virtual void OnBNumberNewChanged(DependencyPropertyChangedEventArgs e)
    {
      ccNumberNew.Visibility = BNumberNew > 0 ? Visibility.Visible : Visibility.Collapsed;
      ccNumberNew.Content = BNumberNew.ToString();
    }

    protected virtual void OnCommandChanged(DependencyPropertyChangedEventArgs e)
    {
      if (Command != null)
      {
        tgbtnMain.Command = Command;
      }
    }
    protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
    {
      tgbtnMain.IsChecked = IsChecked;
    }

    /// <summary>
    /// property change event handler for BindingProperty
    /// </summary>
    private static void BPathChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      ((BToggleButton) depObj).OnBPathChanged(e);
    }

    protected virtual void OnBPathChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(BPath))
      {
        pathMain.Style = (Style)(Application.Current.TryFindResource(BPath));
      }
    }

    #endregion

    public BToggleButton()
    {
      InitializeComponent();
      Loaded += BToggleButtonLoaded;
    }

    void BToggleButtonLoaded(object sender, RoutedEventArgs e)
    {
      tgbtnMain.IsChecked = IsChecked;
    }

    private void txtNumberNew_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      tgbtnMain.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent,tgbtnMain));
    }
  }
}