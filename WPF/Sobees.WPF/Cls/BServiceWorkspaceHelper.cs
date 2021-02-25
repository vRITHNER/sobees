using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;
using Sobees.ViewModel;
using System;
using System.IO;
using System.Reflection;

namespace Sobees.Cls
{
  public class BServiceWorkspaceHelper
  {
    public static void LoadServiceWorkspace(UserAccount account, BPosition pos, SourceLoaderViewModel model)
    {
      switch (account.Type)
      {
      case EnumAccountType.Twitter:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "Twitter",
          Dll = "Sobees.Controls.Twitter.dll",
          Xap = "Sobees.Controls.Twitter.xap",
          Namespace = "Sobees.Controls.Twitter.ViewModel",
          ClassName = "TwitterViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/twitter.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;

      case EnumAccountType.Facebook:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "Facebook",
          Dll = "Sobees.Controls.Facebook.dll",
          Xap = "Sobees.Controls.Facebook.xap",
          Namespace = "Sobees.Controls.Facebook.ViewModel",
          ClassName = "FacebookViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/facebook.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;

      case EnumAccountType.TwitterSearch:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "Search",
          Dll = "Sobees.Controls.TwitterSearch.dll",
          Xap = "Sobees.Controls.TwitterSearch.xap",
          Namespace = "Sobees.Controls.TwitterSearch.ViewModel",
          ClassName = "TwitterSearchViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/search.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;
      //case EnumAccountType.MySpace:
      //  LoadServiceWorkspace(new BServiceWorkspace
      //  {
      //    DisplayName = "Myspace",
      //    Dll = "Sobees.Controls.Myspace.dll",
      //    Xap = "Sobees.Controls.Myspace.xap",
      //    Namespace = "Sobees.Controls.Myspace.ViewModel",
      //    ClassName = "MyspaceViewModel",
      //    Type = "BServiceWorkspaceViewModel",
      //    Img = "/Sobees.Templates;Component/Images/Services/myspace.png",
      //    IsEnable = true,
      //    Version = "1.0.0.0"
      //  }, pos, model, account);
      //  break;
      case EnumAccountType.LinkedIn:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "Linkedin",
          Dll = "Sobees.Controls.LinkedIn.dll",
          Xap = "Sobees.Controls.LinkedIn.xap",
          Namespace = "Sobees.Controls.LinkedIn.ViewModel",
          ClassName = "LinkedInViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/linkedin.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;

      case EnumAccountType.Rss:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "Rss",
          Dll = "Sobees.Controls.Rss.dll",
          Xap = "Sobees.Controls.Rss.xap",
          Namespace = "Sobees.Controls.Rss.ViewModel",
          ClassName = "RssViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/rss.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;

      case EnumAccountType.NyTimes:
        LoadServiceWorkspace(new BServiceWorkspace
        {
          DisplayName = "NyTimes",
          Dll = "Sobees.Controls.NYTimes.dll",
          Xap = "Sobees.Controls.NYTimes.xap",
          Namespace = "Sobees.Controls.NYTimes.ViewModel",
          ClassName = "NyTimesViewModel",
          Type = "BServiceWorkspaceViewModel",
          Img = "/Sobees.Templates;Component/Images/Services/nytimes.png",
          IsEnable = true,
          Version = "1.0.0.0"
        }, pos, model, account);
        break;

      default:
        throw new ArgumentOutOfRangeException();
      }
    }

    public static void LoadServiceWorkspace(BServiceWorkspaceViewModel swvm, SourceLoaderViewModel slvm)
    {
      LoadServiceWorkspace(swvm.BServiceWorkspace, swvm.PositionInGrid, slvm, swvm.ControlSettings);
    }

    public static void LoadServiceWorkspace(BServiceWorkspace bserviceWorkspace, BPosition positionInGrid, SourceLoaderViewModel slvm)
    {
      LoadServiceWorkspace(bserviceWorkspace, positionInGrid, slvm, string.Empty);
    }

    public static BServiceWorkspaceViewModel LoadServiceWorkspaceBack(BServiceWorkspace bserviceWorkspace, BPosition positionInGrid, SourceLoaderViewModel slvm)
    {
      return LoadServiceWorkspaceBack(bserviceWorkspace, positionInGrid, slvm, string.Empty);
    }

    public static void LoadServiceWorkspace(BServiceWorkspace bserviceWorkspace, BPosition positionInGrid, SourceLoaderViewModel slvm, UserAccount account)
    {
      try
      {
        BServiceWorkspaceViewModel serviceWorkspaceViewModel = null;

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var assemblyFilePath = string.Format("{0}\\Controls\\{1}", @location, bserviceWorkspace.Dll);
        serviceWorkspaceViewModel = LoadServiceWorkspaceViewModel(null, assemblyFilePath, bserviceWorkspace,
                                                                  positionInGrid, account);
        if (serviceWorkspaceViewModel != null)
        {
          BViewModelLocator.SobeesViewModelStatic.ServiceLoaded(serviceWorkspaceViewModel, slvm);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("bServiceWorkspaceHelper::LoadServiceWorkspace:", ex);
      }
    }

    public static void LoadServiceWorkspace(BServiceWorkspace bServiceWorkspace, BPosition positionInGrid, SourceLoaderViewModel slvm, string serviceWorkspaceSettings)
    {
      try
      {
        BServiceWorkspaceViewModel serviceWorkspaceViewModel = null;

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (bServiceWorkspace != null)
        {
          var assemblyFilePath = string.Format("{0}\\Controls\\{1}", @location, bServiceWorkspace.Dll);
          serviceWorkspaceViewModel = LoadServiceWorkspaceViewModel(null, assemblyFilePath, bServiceWorkspace,
                                                                    positionInGrid, serviceWorkspaceSettings);
        }

        if (serviceWorkspaceViewModel != null)
        {
          BViewModelLocator.SobeesViewModelStatic.ServiceLoaded(serviceWorkspaceViewModel, slvm);
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("bServiceWorkspaceHelper::LoadServiceWorkspace:", ex);
      }
    }

    public static BServiceWorkspaceViewModel LoadServiceWorkspaceBack(BServiceWorkspace bServiceWorkspace, BPosition positionInGrid, SourceLoaderViewModel slvm, string serviceWorkspaceSettings)
    {
      try
      {
        BServiceWorkspaceViewModel serviceWorkspaceViewModel = null;

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (bServiceWorkspace != null)
        {
          var assemblyFilePath = string.Format("{0}\\Controls\\{1}", @location, bServiceWorkspace.Dll);
          serviceWorkspaceViewModel = LoadServiceWorkspaceViewModel(null, assemblyFilePath, bServiceWorkspace,
                                                                    positionInGrid, serviceWorkspaceSettings);
        }
        if (serviceWorkspaceViewModel != null)
        {
          return serviceWorkspaceViewModel;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("bServiceWorkspaceHelper::LoadServiceWorkspace:", ex);
      }
      return null;
    }

    private static BServiceWorkspaceViewModel LoadServiceWorkspaceViewModel(Stream stream, string assemblyFilePath, BServiceWorkspace serviceWorkspace, BPosition positionInGrid, string serviceWorkspaceSettings)
    {
      try
      {
        var asm = Assembly.LoadFrom(assemblyFilePath);

        var type = asm.GetType(string.Format("{0}.{1}", serviceWorkspace.Namespace,
                                             serviceWorkspace.ClassName));

        var serviceWorkspaceViewModel =
            Activator.CreateInstance(type, new object[] { positionInGrid, serviceWorkspace, serviceWorkspaceSettings })
            as BServiceWorkspaceViewModel;
        return serviceWorkspaceViewModel;
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("", ex);
        return null;
      }
      finally
      {
      }
    }

    private static BServiceWorkspaceViewModel LoadServiceWorkspaceViewModel(Stream stream, string assemblyFilePath, BServiceWorkspace serviceWorkspace, BPosition positionInGrid, UserAccount account)
    {
      try
      {
        var asm = Assembly.LoadFrom(assemblyFilePath);
        var type = asm.GetType(string.Format("{0}.{1}", serviceWorkspace.Namespace,
                                             serviceWorkspace.ClassName));

        var serviceWorkspaceViewModel =
            Activator.CreateInstance(type, new object[] { positionInGrid, serviceWorkspace, string.Empty })
            as BServiceWorkspaceViewModel;
        if (serviceWorkspaceViewModel != null)
        {
          serviceWorkspaceViewModel.SetAccount(account);
          return serviceWorkspaceViewModel;
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BServiceWorkspaceHelper::BServiceWorkspaceViewModel:", ex);
        return null;
      }
      finally
      {
      }
      return null;
    }
  }
}