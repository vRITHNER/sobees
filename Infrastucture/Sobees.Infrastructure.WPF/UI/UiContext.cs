using System.Threading;
using System.Threading.Tasks;
using Sobees.Infrastructure.Cls;

namespace Sobees.Infrastructure.UI
{
  public class UiContext : SingletonBase<UiContext>
  {
    public TaskScheduler Current { get; set; }

    public UiContext()
    {
      if (Current != null)
        return;

      if (SynchronizationContext.Current == null)
        SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

      Current = TaskScheduler.FromCurrentSynchronizationContext();
    }

    
  }
}