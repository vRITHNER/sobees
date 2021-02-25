using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Infrastructure.Cls;
#if !SILVERLIGHT
using Sobees.Library.BGenericLib;
#else
using TwitterServiceProxy;
#endif

namespace Sobees.Infrastructure.Controls
{
  public class BAlerteMessage : MessageBase
  {
    public BAlerteMessage(string action, ObservableCollection<Entry> parameter,string name, EnumAccountType type)
    {
      Action = action;
      List = parameter;
      Name = name;
      Type = type;
    }

    public ObservableCollection<Entry> List { get; set; }

    public EnumAccountType Type { get; set; }

    public string Name { get; set; }

    public string Action { get; set; }
  }
}