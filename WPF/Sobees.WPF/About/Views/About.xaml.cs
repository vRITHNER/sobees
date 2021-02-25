using System.Reflection;
using Sobees.Glass;
using Sobees.Tools.Util;
using Sobees.Windows.Extensions;

namespace Sobees.About.Views
{
  /// <summary>
  /// Interaction logic for About.xaml
  /// </summary>
  public partial class About : GlassWindow
  {
    public About(MainWindow mainWindow)
      : base("About", false, mainWindow.GetWindowLocation())
    {
      InitializeComponent();
      Title = string.Format("{0} - Version {1}",
                      Title,
                      AssemblyHelper.GetEntryAssemblyVersion());
    }
  }
}
