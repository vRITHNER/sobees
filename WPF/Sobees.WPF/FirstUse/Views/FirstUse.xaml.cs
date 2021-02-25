#region

using Sobees.Glass;
using Sobees.Windows.Extensions;

#endregion

namespace Sobees.FirstUse.Views
{
  /// <summary>
  ///   Interaction logic for About.xaml
  /// </summary>
  public partial class FirstUse : GlassWindow
  {
    public FirstUse(MainWindow mainWindow)
      : base("FirstUse", false, mainWindow.GetWindowLocation())
    {
      InitializeComponent();
    }
  }
}