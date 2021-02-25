namespace Sobees.ViewModel
{
public enum EnumTypeFrontView
{
  None,
  ChangeLayout,
  TemplateLoader,
  ChangeView,
  SyncAccount,
  WaitingStart,
#if SILVERLIGHT
  FirstUse,
  Settings,
  About,
  MultiPost,
#endif
}
}