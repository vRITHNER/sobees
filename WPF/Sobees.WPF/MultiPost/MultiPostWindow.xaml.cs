using Sobees.Glass;
using Sobees.Windows.Extensions;

namespace Sobees.MultiPost
{
  /// <summary>
  /// Interaction logic for MultiPostWindow.xaml
  /// </summary>
  public partial class MultiPostWindow : GlassWindow
  {
    public MultiPostWindow(MainWindow mainWindow) :
      base("MultiPostWindow", true, mainWindow.GetWindowLocation())
    {
      InitializeComponent();
    }
  }
}