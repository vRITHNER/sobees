/*
 * In App.xaml:
 * <Application.Resources>
 *     <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:bDule.WPF.ViewModel"
 *                                  x:Key="Locator" />
 * </Application.Resources>
 *
 * In the View:
 * DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
 *
 * OR (WPF only):
 *
 * xmlns:vm="clr-namespace:bDule.WPF.ViewModel"
 * DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
*/

using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight;

namespace Sobees.ViewModel
{
  /// <summary>
  /// This class contains static references to all the view models in the
  /// application and provides an entry point for the bindings.
  /// <para>
  /// Use the <strong>mvvmlocatorproperty</strong> snippet to add ViewModels
  /// to this locator.
  /// </para>
  /// <para>
  /// In Silverlight and WPF, place the ViewModelLocatorTemplate in the App.xaml resources:
  /// </para>
  /// <code>
  /// &lt;Application.Resources&gt;
  ///     &lt;vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:bDule.WPF.ViewModel"
  ///                                  x:Key="Locator" /&gt;
  /// &lt;/Application.Resources&gt;
  /// </code>
  /// <para>
  /// Then use:
  /// </para>
  /// <code>
  /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
  /// </code>
  /// <para>
  /// You can also use Blend to do all this with the tool's support.
  /// </para>
  /// <para>
  /// See http://www.galasoft.ch/mvvm/getstarted
  /// </para>
  /// <para>
  /// In <strong>*WPF only*</strong> (and if databinding in Blend is not relevant), you can delete
  /// the Main property and bind to the ViewModelNameStatic property instead:
  /// </para>
  /// <code>
  /// xmlns:vm="clr-namespace:NAMESPACE.ViewModel"
  /// DataContext="{Binding Source={x:Static vm:ViewModelLocator.ViewModelNameStatic}}"
  /// </code>
  /// </summary>
  public class BViewModelLocator
  {
    #region Fields

    protected static SettingsViewModel _settingsViewModel;
    protected static SearchViewModel _searchViewModel;
    protected static SobeesViewModel _sobeesViewModel;
    protected static MultiPostViewModel _multiPostViewModel;
    protected static FirstLaunchControlViewModel _FirstLaunchControlViewModel;
    protected static ViewsManagerViewModel _viewsManagerViewModel;
    protected static AboutViewModel _aboutViewModel;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the ViewModelLocator class.
    /// </summary>
    public BViewModelLocator()
    {
      if (ViewModelBase.IsInDesignModeStatic)
      {
        // Create design time view models
      }
      else
      {
        CreateMainTemplate();
      }
    }

    #endregion Constructors

    #region Properties Static

    public static SearchViewModel SearchViewModelStatic => _searchViewModel ?? (_searchViewModel = new SearchViewModel());

    public static SettingsViewModel SettingsViewModelStatic => _settingsViewModel ?? (_settingsViewModel = new SettingsViewModel());

    public static SobeesViewModel SobeesViewModelStatic
    {
      get
      {
        if (_sobeesViewModel == null)
          CreateMainTemplate();

        return _sobeesViewModel;
      }
    }

    public static FirstLaunchControlViewModel FirstLaunchControlViewModelStatic => _FirstLaunchControlViewModel ?? (_FirstLaunchControlViewModel = new FirstLaunchControlViewModel());

    public static AboutViewModel AboutViewModelStatic => _aboutViewModel ?? (_aboutViewModel = new AboutViewModel());

    public static ViewsManagerViewModel ViewsManagerViewModelStatic
    {
      get
      {
        if (_viewsManagerViewModel == null)
        {
          CreateMainTemplate();
        }

        return _viewsManagerViewModel;
      }
    }

    public static MultiPostViewModel MultiPostViewModelStatic => _multiPostViewModel ?? (_multiPostViewModel = new MultiPostViewModel());

    #endregion Properties Static

    #region Properties

    [SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
    public SettingsViewModel SettingsViewModel => SettingsViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
  "CA1822:MarkMembersAsStatic",
  Justification = "This non-static member is needed for data binding purposes.")]
    public SearchViewModel SearchViewModel => SearchViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
    public MultiPostViewModel MultiPostViewModel => MultiPostViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
    public SobeesViewModel SobeesViewModel => SobeesViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
    public FirstLaunchControlViewModel FirstLaunchControlViewModel => FirstLaunchControlViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
      "CA1822:MarkMembersAsStatic",
      Justification = "This non-static member is needed for data binding purposes.")]
    public ViewsManagerViewModel ViewsManagerViewModel => ViewsManagerViewModelStatic;

    [SuppressMessage("Microsoft.Performance",
  "CA1822:MarkMembersAsStatic",
  Justification = "This non-static member is needed for data binding purposes.")]
    public AboutViewModel AboutViewModel => AboutViewModelStatic;

    #endregion Properties

    #region Methods

    /// <summary>
    /// Provides a deterministic way to create the Main property.
    /// </summary>
    public static void CreateMainTemplate()
    {
      if (_sobeesViewModel == null)
        _sobeesViewModel = new SobeesViewModel();
      if (_viewsManagerViewModel == null)
        _viewsManagerViewModel = new ViewsManagerViewModel();
    }

    /// <summary>
    /// Provides a deterministic way to delete the Main property.
    /// </summary>
    public static void ClearMainTemplate()
    {
      _sobeesViewModel.Cleanup();
      _sobeesViewModel = null;
      _viewsManagerViewModel.Cleanup();
      _viewsManagerViewModel = null;
      DisposeUcFirstLauchControl();
      DisposeSettings();
      DisposeSearch();
    }

    public static void DisposeSettings()
    {
      if (_settingsViewModel == null) return;
      _settingsViewModel.Cleanup();
      _settingsViewModel = null;
    }

    public static void DisposeSearch()
    {
      if (_searchViewModel == null) return;
      _searchViewModel.Cleanup();
      _searchViewModel = null;
    }

    public static void DisposeUcFirstLauchControl()
    {
      if (_FirstLaunchControlViewModel == null) return;
      _FirstLaunchControlViewModel.Cleanup();
      _FirstLaunchControlViewModel = null;
    }

    /// <summary>
    /// Cleans up all the resources.
    /// </summary>
    public static void Dispose()
    {
      ClearMainTemplate();
    }

    #endregion Methods
  }
}