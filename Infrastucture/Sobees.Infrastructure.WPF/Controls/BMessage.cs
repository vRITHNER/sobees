using GalaSoft.MvvmLight.Messaging;

namespace Sobees.Infrastructure.Controls
{
  public class BMessage : MessageBase
  {
    public BMessage(string action, object parameter)
    {
      Action = action;
      Parameter = parameter;
    }

    public object Parameter { get; set; }

    public string Action { get; set; }
  }
}