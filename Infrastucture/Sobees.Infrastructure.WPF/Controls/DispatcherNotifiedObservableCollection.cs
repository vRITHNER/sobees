using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Sobees.Infrastructure.Controls
{
  public class DispatcherNotifiedObservableCollection<T> : ObservableCollection<T>
  {
    #region Ctors

    public DispatcherNotifiedObservableCollection()
    {
    }

    public DispatcherNotifiedObservableCollection(List<T> list)
      : base(list)
    {
    }

    public DispatcherNotifiedObservableCollection(IEnumerable<T> collection)
      : base(collection)
    {
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Occurs when an item is added, removed, changed, moved, 
    /// or the entire list is refreshed.
    /// </summary>
    public override event NotifyCollectionChangedEventHandler CollectionChanged;


    /// <summary>
    /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> 
    /// event with the provided arguments.
    /// </summary>
    /// <param name="e">Arguments of the event being raised.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventHandler eh = CollectionChanged;
      if (eh != null)
      {
        Dispatcher dispatcher =
          (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
           let dpo = nh.Target as DispatcherObject
           where dpo != null
           select dpo.Dispatcher).FirstOrDefault();

        if (dispatcher != null && dispatcher.CheckAccess() == false)
        {
          dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                                 (Action)(() => OnCollectionChanged(e)));
        }
        else
        {
          try
          {
            foreach (NotifyCollectionChangedEventHandler nh
              in eh.GetInvocationList())
            {
              nh.Invoke(this, e);
            }
          }
          catch (Exception exception)
          {
            Console.WriteLine(exception);
          }
        }
      }
    }

    #endregion
  }
}
