using System.Windows;

namespace Sobees.Infrastructure.ViewModelBase
{
  public interface IBSettingsViewModel
  {
    bool IsDirty { get; set; }
    void SaveSettings();
    void CloseSettings();
    DataTemplate SettingsControl { get; set; }
    BWorkspaceViewModel CurrentViewModel { get; set; }
    string Title { get; set; }
    
  }
}