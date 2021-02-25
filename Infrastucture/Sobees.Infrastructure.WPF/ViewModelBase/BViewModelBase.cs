using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Controls;
using Sobees.Tools.Logging;
using System.Collections.ObjectModel;
using Sobees.Library.BGenericLib;

namespace Sobees.Infrastructure.ViewModelBase
{
  public class BViewModelBase : GalaSoft.MvvmLight.ViewModelBase
  {
    #region Custom Events
    
    #endregion

    #region Fields
    #endregion

    #region Constructors

    public BViewModelBase()
    {
      //Messenger.Default.Register<string>(this, DoAction);
      InitCommands();
    }

    #endregion

    #region Properties

    public virtual DataTemplate DataTemplateView { get; set; }

    public EnumAccountType AccountType
    {
      get
      {
        //if (GetType().ToString() == "Sobees.Controls.Facebook.ViewModel.FacebookViewModel")
        //{
        //  return EnumAccountType.Facebook;
        //}
        if (GetType().ToString() == "Sobees.Controls.Twitter.ViewModel.TwitterViewModel")
        {
          return EnumAccountType.Twitter;
        }
        if (GetType().ToString() == "Sobees.Controls.TwitterSearch.ViewModel.TwitterSearchViewModel")
        {
          return EnumAccountType.TwitterSearch;
        }
        //if (GetType().ToString() == "Sobees.Controls.Myspace.ViewModel.MyspaceViewModel")
        //{
        //  return EnumAccountType.MySpace;
        //}
        //if (GetType().ToString() == "Sobees.Controls.LinkedIn.ViewModel.LinkedInViewModel")
        //{
        //  return EnumAccountType.LinkedIn;
        //}
        if (GetType().ToString() == "Sobees.Controls.SmartFeed.ViewModel.SmartFeedViewModel")
        {
            return EnumAccountType.SmartFeed;
        }
        //if (GetType().ToString() == "Sobees.Controls.FacebookPage.ViewModel.FacebookPageViewModel")
        //{
        //    return EnumAccountType.FacebookPage;
        
        return EnumAccountType.Twitter;
      }
    }
    #endregion

    #region DisplayName

    /// <summary>
    /// Returns the user-friendly name of this object.
    /// Child classes can set this property to a new value,
    /// or override it to determine the value on-demand.
    /// </summary>
    public virtual string DisplayName { get; protected set; }

    #endregion // DisplayName

    #region Methods
    /// <summary>
    /// Used when a string arrived into the Messenger
    /// </summary>
    /// <param name="param">A string that represents the fonction to execute.</param>
    public virtual void DoAction(string param)
    {
      switch (param)
      {
        default:
          break;
      }
    }
    public virtual void DoActionMessage(BMessage obj)
    {
    }
    /// <summary>
    /// Initialize all needed Commands
    /// </summary>
    protected virtual void InitCommands()
    {

    }

    #endregion

    #region Alerts Methods


    public virtual void ShowAlerts(ObservableCollection<Entry> collection,string name, EnumAccountType type)
    {
      if (collection != null && collection.Count >0)
      {
        Messenger.Default.Send(new BAlerteMessage("ShowAlerts", collection, name,type));
      }
    }
    public void ShowAlerts(ObservableCollection<Entry> collection, string name)
    {
      if (collection != null)
      {
       
      }
    }


    #endregion

    #region IDisposable Members

    /// <summary>
    /// Child classes can override this method to perform 
    /// clean-up logic, such as removing event handlers
    /// </summary>
    protected void Dispose()
    {
      base.Cleanup();
    }

#if DEBUG
    /// <summary>
    /// Useful for ensuring that ViewModel objects are properly garbage collected.
    /// </summary>
    ~BViewModelBase()
    {
      string msg = $"{GetType().Name} ({DisplayName}) ({GetHashCode()}) Finalized";
      TraceHelper.Trace(this, msg);
    }
#endif

    #endregion // IDisposable Members

    #region Events

    
    #endregion

    #region Settings

    public void UpdateSettings()
    {
    }

    #endregion
   
  }
}