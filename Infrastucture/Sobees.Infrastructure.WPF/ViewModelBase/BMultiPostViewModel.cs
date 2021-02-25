using GalaSoft.MvvmLight.Messaging;

namespace Sobees.Infrastructure.ViewModelBase
{
  public class BMultiPostViewModel : BViewModelBase
  {
    #region Constructors

    protected BMultiPostViewModel(IMessenger messenger)
    {
      MessengerInstance = messenger;
    }

    #endregion
  }
}