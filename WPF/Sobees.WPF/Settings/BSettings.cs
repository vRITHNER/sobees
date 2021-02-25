#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using BUtility;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Cls;
using Sobees.Infrastructure.Cls;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.Tools.Serialization;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Library.BLocalizeLib;
using Sobees.Tools.Logging;
using Sobees.Tools.Serialization;
using Sobees.Tools.Threading;
using Sobees.Tools.Threading.Extensions;
using Sobees.ViewModel;
using Sobees.Configuration.BGlobals;
using Sobees.Infrastructure.UI;

#endregion

namespace Sobees.Settings
{
  public class BSettings
  {
    private const string APPNAME = "BSettings";

    private DispatcherTimer _timer;

    public void WriteSettings(string key,
                              string value)
    {
      Properties.Settings.Default[key] = value;
      Properties.Settings.Default.Save();
    }

    public string ReadSettings(string key)
    {
      return Properties.Settings.Default[key] != null ? Properties.Settings.Default[key].ToString() : null;
    }

    /// <summary>
    ///   Restore Settings from xml file.
    /// </summary>
    public async void RestoreSettings()
    {
      _timer = new DispatcherTimer
                 {
                   Interval = new TimeSpan(0, 0, 0, BGlobals.RESTORESETTINGSTIMER)
                 };
      _timer.Tick += delegate
                       {
                         if (BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Count == 0)
                           Messenger.Default.Send("NoSettingsRestored");
                         if (_timer != null) _timer.Stop();
                       };
      _timer.Start();

      Action mainAction = async () =>
      {
        try
        {
          var strSobeesSettings = ReadSettings("SobeesSettings");
          
          if (string.IsNullOrEmpty(strSobeesSettings))
          {
            await RestoreOldSobeesSettings();
          }
          else
          {
            await RestoreSobeesSettings(strSobeesSettings);
          }
        }
        catch (Exception ex)
        {
          Messenger.Default.Send("NoSettingsRestored");
          BLogManager.LogEntry(APPNAME, ex);
        }
      };

      var task = Task.Factory.StartNew(mainAction, CancellationToken.None, TaskCreationOptions.AttachedToParent, UiContext.Instance.Current);
      task.Wait();

      
      
      //if (BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1.Count == 0)
      //    Messenger.Default.Send("NoSettingsRestored");

      
 
      //Action backgroundAction = () => dispatcher.BeginInvoke(DispatcherPriority.Render, mainAction);
          
        
      //using (var worker = new BackgroundWorker())
      //{
      //  worker.DoWork += delegate(object s,
      //                            DoWorkEventArgs args)
      //                     {
      //                       if (worker.CancellationPending)
      //                       {
      //                         args.Cancel = true;
      //                         return;
      //                       }
      //                       try
      //                       {
      //                         var strSobeesSettings = ReadSettings("SobeesSettings");
      //                         //if (!Debugger.IsAttached) Debugger.Launch();
      //                         if (string.IsNullOrEmpty(strSobeesSettings))
      //                         {
      //                           RestoreOldSobeesSettings();
      //                         }
      //                         else
      //                         {
      //                           RestoreSobeesSettings(strSobeesSettings);
      //                         }
      //                       }
      //                       catch (Exception ex)
      //                       {
      //                         Messenger.Default.Send("NoSettingsRestored");
      //                         BLogManager.LogEntry(APPNAME, ex);
      //                       }
      //                     };
      //  worker.RunWorkerAsync();
      //}
    }

    private async Task<bool> RestoreOldSobeesSettings()
    {
      var file = string.Format("{0}Sobees\\{1}", BGlobals.FOLDERBASESOBEES, BGlobals.FILEAUTOBACKUPNAME);

      if (File.Exists(file))
      {
        ImportSettings(file);
        return await new Task<bool>(() => true);
      }
      
        Messenger.Default.Send("NoSettingsRestored");
        return await new Task<bool>(() => false);
    }

    public void ImportSettings(string name)
    {
      try
      {
        Messenger.Default.Send("ShowWaiting");
        var reader = XmlReader.Create(name);
        Import(reader);
      }
      catch (Exception e)
      {
        Messenger.Default.Send("NoSettingsRestored");
        TraceHelper.Trace(this, e);
      }
      finally
      {
        ProcessHelper.PerformAggressiveCleanup();
      }
    }

    public void ImportSettingsXml(string name)
    {
      try
      {
        //Messenger.Default.Send("ShowWaiting");
        var reader = XmlReader.Create(name);
        Import(reader);
      }
      catch (Exception e)
      {
        var txt = new LocText("Sobees.Configuration.BGlobals:Resources:txtErrorLoadingSettings").ResolveLocalizedValue();
        MessageBox.Show(txt);
        TraceHelper.Trace(this, e);
      }
      finally
      {
        ProcessHelper.PerformAggressiveCleanup();
      }
    }

    public void Import(XmlReader reader)
    {
      reader.MoveToContent();

      //reader.Read();
      if (reader.Name.Equals("AppSettings"))
      {
        reader.ReadStartElement("AppSettings");
        double version;
        if (reader.Name.Equals("SettingsVersion"))
        {
          reader.ReadStartElement("SettingsVersion");
          version = double.Parse(reader.ReadContentAsString());
          reader.ReadEndElement();
        }
        else
        {
          return;
        }
        if (version == 1.0)
          ImportSettingsV1(reader);
        else if (version == 1.5)
          ImportSettingsV15(reader);
      }
    }

    private void ImportSettingsV1(XmlReader reader)
    {
      var serviceWorkspaceGridTemplate = "";
      var serviceWorkspacesString = "";
      var strbDuleSettings = "";
      if (reader.Name.Equals("Data"))
      {
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Data");

          //Inside Data
          if (reader.Name.Equals("ServiceWorkspaceGridTemplate"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("ServiceWorkspaceGridTemplate");
              serviceWorkspaceGridTemplate = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("ServiceWorkspaceGridTemplate");
            }
          }
          if (reader.Name.Equals("ServiceWorkspaces"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("ServiceWorkspaces");
              serviceWorkspacesString = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("ServiceWorkspaces");
            }
          }
          if (reader.Name.Equals("bDuleSettings"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("bDuleSettings");
              strbDuleSettings = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("bDuleSettings");
            }
          }
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Data");
        }
      }

      reader.ReadEndElement();
      reader.Close();
      if (!string.IsNullOrEmpty(serviceWorkspaceGridTemplate))
      {
        var bt =
          GenericSerializer.DeserializeObject(
            serviceWorkspaceGridTemplate.Replace("<bTemplate>", "<BTemplate>").Replace("</bTemplate>", "</BTemplate>").
                                         Replace("bPosition", "BPosition"),
            typeof(BTemplate)) as BTemplate;
        if (bt != null && bt.IsUserSelectedbTemplate)
        {
          //TraceHelper.Trace(this, bt.ToString());
          MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                    {
                                                                      for (
                                                                        var i =
                                                                          BViewModelLocator.SobeesViewModelStatic.
                                                                                            BServiceWorkspaces1.Count - 1;
                                                                        i > -1;
                                                                        i--)
                                                                      {
                                                                        if (BViewModelLocator.SobeesViewModelStatic.
                                                                                              BServiceWorkspaces1[i].CloseCommand != null)
                                                                        {
                                                                          BViewModelLocator.SobeesViewModelStatic.
                                                                                            BServiceWorkspaces1[i].CloseCommand.Execute(
                                                                                              null);
                                                                        }
                                                                      }
                                                                      BViewModelLocator.SobeesViewModelStatic.
                                                                                        BServiceWorkspaces1.Clear();
                                                                      BViewModelLocator.ViewsManagerViewModelStatic.
                                                                                        BServiceWorkspaceGridTemplate1 = bt;
                                                                    });
        }
      }
      if (!string.IsNullOrEmpty(strbDuleSettings))
      {
        var bDuleSettings =
          GenericSerializer.DeserializeObject(strbDuleSettings.Replace("bDuleSettings", "SobeesSettings"),
                                              typeof(SobeesSettings)) as SobeesSettings;
        if (bDuleSettings != null)
          SobeesSettingsLocator.SetSettings(bDuleSettings);
      }
      if (!string.IsNullOrEmpty(serviceWorkspacesString))
      {
        //TraceHelper.Trace(this,
        //                  "RestoreSettings() -> Deserializing ObservableCollection<ServiceWorkspaceViewModel>");
        var serviceWorkspaces =
          GenericCollectionSerializer.DeserializeObject<ObservableCollection<BServiceWorkspaceViewModel>>(
            serviceWorkspacesString.Replace("ServiceWorkspaceViewModel", "BServiceWorkspaceViewModel").Replace(
              "bPosition", "BPosition").Replace("bServiceWorkspace", "BServiceWorkspace"));
        //TraceHelper.Trace(this, "RestoreSettings() -> Deserialization completed!");
        AdjustBServiceWorkspace(serviceWorkspaces);
        if (serviceWorkspaces != null)
        {
          MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                    {
                                                                      try
                                                                      {
                                                                        foreach (var serviceWorkspace in serviceWorkspaces
                                                                          )
                                                                        {
                                                                          //TraceHelper.Trace(this,
                                                                          //                  "RestoreSettings() -> Loading " +
                                                                          //                  serviceWorkspace);

                                                                          // Load the DLL with information from serviceWorkspace.bServiceWorkspace
                                                                          BServiceWorkspaceHelper.LoadServiceWorkspace(
                                                                            serviceWorkspace,
                                                                            null);
                                                                        }
                                                                        Messenger.Default.Send("SettingsRestored");

                                                                        //ThreadHelper.DoEvents();
                                                                      }
                                                                      catch (Exception ex)
                                                                      {
                                                                        TraceHelper.Trace(this, ex);
                                                                      }
                                                                      Messenger.Default.Send("NoSettingsRestored");
                                                                    });
        }
        else
        {
          Messenger.Default.Send("NoSettingsRestored");
        }
      }
    }

    private void ImportSettingsV15(XmlReader reader)
    {
      var serviceWorkspaceGridTemplate = "";
      var serviceWorkspacesString = "";
      var strbDuleSettings = "";
      if (reader.Name.Equals("Data"))
      {
        if (!reader.IsEmptyElement)
        {
          reader.ReadStartElement("Data");

          //Inside Data
          if (reader.Name.Equals("ServiceWorkspaceGridTemplate1"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("ServiceWorkspaceGridTemplate1");
              serviceWorkspaceGridTemplate = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("ServiceWorkspaceGridTemplate1");
            }
          }
          if (reader.Name.Equals("ServiceWorkspaces1"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("ServiceWorkspaces1");
              serviceWorkspacesString = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("ServiceWorkspaces1");
            }
          }
          if (reader.Name.Equals("SobeesSettings"))
          {
            if (!reader.IsEmptyElement)
            {
              reader.ReadStartElement("SobeesSettings");
              strbDuleSettings = reader.ReadContentAsString();
              reader.ReadEndElement();
            }
            else
            {
              reader.ReadStartElement("SobeesSettings");
            }
          }
          reader.ReadEndElement();
        }
        else
        {
          reader.ReadStartElement("Data");
        }
      }

      reader.ReadEndElement();
      reader.Close();
      if (!string.IsNullOrEmpty(serviceWorkspaceGridTemplate))
      {
        var bt =
          GenericSerializer.DeserializeObject(
            serviceWorkspaceGridTemplate,
            typeof(BTemplate)) as BTemplate;
        if (bt != null && bt.IsUserSelectedbTemplate)
        {
          MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                    {
                                                                      var test = BViewModelLocator.SobeesViewModelStatic;
                                                                      for (
                                                                        var i =
                                                                          BViewModelLocator.SobeesViewModelStatic.
                                                                                            BServiceWorkspaces1.Count - 1;
                                                                        i > -1;
                                                                        i--)
                                                                      {
                                                                        if (BViewModelLocator.SobeesViewModelStatic.
                                                                                              BServiceWorkspaces1[i].CloseCommand != null)
                                                                        {
                                                                          BViewModelLocator.SobeesViewModelStatic.
                                                                                            BServiceWorkspaces1[i].CloseCommand.Execute(
                                                                                              null);
                                                                        }
                                                                      }
                                                                      BViewModelLocator.SobeesViewModelStatic.
                                                                                        BServiceWorkspaces1.Clear();
                                                                      BViewModelLocator.ViewsManagerViewModelStatic.
                                                                                        BServiceWorkspaceGridTemplate1 = bt;
                                                                    });
        }
      }
      if (!string.IsNullOrEmpty(strbDuleSettings))
      {
        var bDuleSettings =
          GenericSerializer.DeserializeObject(strbDuleSettings,
                                              typeof(SobeesSettings)) as SobeesSettings;
        if (bDuleSettings != null)
          SobeesSettingsLocator.SetSettings(bDuleSettings);
      }
      if (!string.IsNullOrEmpty(serviceWorkspacesString))
      {
        //TraceHelper.Trace(this, "RestoreSettings() -> Deserializing ObservableCollection<ServiceWorkspaceViewModel>");
        var serviceWorkspaces =
          GenericCollectionSerializer.DeserializeObject<ObservableCollection<BServiceWorkspaceViewModel>>(serviceWorkspacesString);
        //TraceHelper.Trace(this, "RestoreSettings() -> Deserialization completed!");

        //AdjustBServiceWorkspace(serviceWorkspaces);
        if (serviceWorkspaces != null)
        {
          MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                    {
                                                                      foreach (var serviceWorkspace in serviceWorkspaces)
                                                                      {
                                                                       // TraceHelper.Trace(this, "RestoreSettings() -> Loading " + serviceWorkspace);

                                                                        // Load the DLL with information from serviceWorkspace.bServiceWorkspace
                                                                        BServiceWorkspaceHelper.LoadServiceWorkspace(serviceWorkspace, null);
                                                                        ThreadHelper.DoEvents();
                                                                      }
                                                                      Messenger.Default.Send("SettingsRestored");
                                                                    });
        }
        else
        {
          Messenger.Default.Send("NoSettingsRestored");
        }
      }
    }

    private void AdjustBServiceWorkspace(IEnumerable<BServiceWorkspaceViewModel> workspaces)
    {
      foreach (var model in workspaces)
      {
        switch (model.BServiceWorkspace.Dll)
        {
          case "bDule.Controls.Facebook.dll":
            model.BServiceWorkspace.ClassName = "FacebookViewModel";
            model.BServiceWorkspace.Dll = "Sobees.Controls.Facebook.dll";
            model.BServiceWorkspace.Img = "/Sobees.Templates;Component/Images/Services/facebook.png";
            model.BServiceWorkspace.Namespace = "Sobees.Controls.Facebook.ViewModel";
            break;

          case "bDule.Controls.Twitter.dll":
            model.BServiceWorkspace.ClassName = "TwitterViewModel";
            model.BServiceWorkspace.Dll = "Sobees.Controls.Twitter.dll";
            model.BServiceWorkspace.Img = "/Sobees.Templates;Component/Images/Services/twitter.png";
            model.BServiceWorkspace.Namespace = "Sobees.Controls.Twitter.ViewModel";
            break;

          case "bDule.Controls.LinkedIn.dll":
            model.BServiceWorkspace.ClassName = "LinkedInViewModel";
            model.BServiceWorkspace.Dll = "Sobees.Controls.LinkedIn.dll";
            model.BServiceWorkspace.Img = "/Sobees.Templates;Component/Images/Services/linkedin.png";
            model.BServiceWorkspace.Namespace = "Sobees.Controls.LinkedIn.ViewModel";
            break;

          case "bDule.Controls.TwitterSearch.dll":
            model.BServiceWorkspace.ClassName = "TwitterSearchViewModel";
            model.BServiceWorkspace.Dll = "Sobees.Controls.TwitterSearch.dll";
            model.BServiceWorkspace.Img = "/Sobees.Templates;Component/Images/Services/search.png";
            model.BServiceWorkspace.Namespace = "Sobees.Controls.TwitterSearch.ViewModel";
            break;
        }
      }
    }

    public SobeesSettings RestoreSobeesMainSettings()
    {
      //System.Threading.Thread.Sleep(10000);
      try
      {
        //TraceHelper.Trace(this, "RestoreSettings() -> Restoring SobeesSettings");
        var s = ReadSettings("SobeesSettings");
        SobeesSettings sobeesSettings = null;
        if (s != null)
        {
          sobeesSettings = GenericSerializer.DeserializeObject(ReadSettings("SobeesSettings"),
                                                               typeof(SobeesSettings)) as SobeesSettings;
        }

        if (sobeesSettings != null)
        {
          SobeesSettingsLocator.SetSettings(sobeesSettings);
          ((Application.Current) as SobeesApplication).FontSize = sobeesSettings.FontSizeValue;
          return sobeesSettings;
        }
        //TraceHelper.Trace("BSettings::RestoreSobeesMainSettings:", "RestoreSettings() -> There is an error while deserialization of Sobees Settings!");
      }
      catch (Exception ex)
      {
        TraceHelper.Trace("BSettings::RestoreSobeesMainSettings:", ex);
      }

      return null;
    }

    private async Task<bool> RestoreSobeesSettings(string sobeesSettingsToRestore)
    {
      try
      {
        //restoring the SobeesSettings
        //TraceHelper.Trace(this, "RestoreSettings() -> Restoring SobeesSettings");
        var sobeesSettings = GenericSerializer.DeserializeObject(sobeesSettingsToRestore,
                                                                 typeof(SobeesSettings)) as SobeesSettings;
        if (sobeesSettings != null)
        {
          SobeesSettingsLocator.SetSettings(sobeesSettings);
          ((Application.Current) as SobeesApplication).FontSize = sobeesSettings.FontSizeValue;
        }
        else
        {
          TraceHelper.Trace(this, "RestoreSettings() -> There is an error while deserialization of Sobees Settings!");
        }

        //TraceHelper.Trace(this, "RestoreSettings() -> Restoring ServiceWorkspaceGridTemplate1");
        var serviceWorkspaceGridTemplate1 = ReadSettings("ServiceWorkspaceGridTemplate1");

        if (!string.IsNullOrEmpty(serviceWorkspaceGridTemplate1))
        {
          var bt = GenericSerializer.DeserializeObject(serviceWorkspaceGridTemplate1, typeof(BTemplate)) as BTemplate;
          if (bt != null)
          {
            TraceHelper.Trace(this, bt.ToString());
            MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                      {
                                                                        for (
                                                                          var i =
                                                                            BViewModelLocator.SobeesViewModelStatic.
                                                                                              BServiceWorkspaces1.Count - 1;
                                                                          i > -1;
                                                                          i--)
                                                                        {
                                                                          BViewModelLocator.SobeesViewModelStatic.
                                                                                            BServiceWorkspaces1[i].CloseCommand.Execute(
                                                                                              null);
                                                                        }
                                                                        BViewModelLocator.SobeesViewModelStatic.
                                                                                          BServiceWorkspaces1.Clear();
                                                                        BViewModelLocator.ViewsManagerViewModelStatic.
                                                                                          BServiceWorkspaceGridTemplate1 = bt;
                                                                      });
          }
          else
          {
            TraceHelper.Trace(this, "RestoreSettings() -> There is an error while deserialization of serviceWorkspaceGridTemplate1!");
          }
        }

        //TraceHelper.Trace(this, "RestoreSettings() -> Restoring ServiceWorkspaces");
        var serviceWorkspacesString1 = ReadSettings("ServiceWorkspaces1");

        if (!string.IsNullOrEmpty(serviceWorkspacesString1))
        {
          //TraceHelper.Trace(this, "RestoreSettings() -> Deserializing ObservableCollection<ServiceWorkspaceViewModel>");
          var serviceWorkspaces = GenericCollectionSerializer.DeserializeObject<ObservableCollection<BServiceWorkspaceViewModel>>(serviceWorkspacesString1);
          //TraceHelper.Trace(this, "RestoreSettings() -> Deserialization completed!");

          //AdjustBServiceWorkspace(serviceWorkspaces);
          if (serviceWorkspaces != null && serviceWorkspaces.Count > 0)
          {
            MainWindow.GetInstance.Dispatcher.BeginInvokeIfRequired(() =>
                                                                      {
                                                                        foreach (
                                                                          var serviceWorkspace in
                                                                            serviceWorkspaces)
                                                                        {
                                                                          //TraceHelper.Trace(this,
                                                                          //                  "RestoreSettings() -> Loading " +
                                                                          //                  serviceWorkspace);

                                                                          // Load the DLL with information from serviceWorkspace.bServiceWorkspace
                                                                          BServiceWorkspaceHelper.LoadServiceWorkspace(
                                                                            serviceWorkspace,
                                                                            null);
                                                                          ThreadHelper.DoEvents();
                                                                        }
                                                                        Messenger.Default.Send("SettingsRestored");
                                                                      });
          }
          else
          {
            Messenger.Default.Send("NoSettingsRestored");
            return await new Task<bool>(() => false);
          }
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }

      return await new Task<bool>(() => true);
    }

    /// <summary>
    ///   SaveSettings
    /// </summary>
    public async void SaveSettings()
    {
      Action mainAction = async () =>
      {
        // Save the services template
        var template1 = GenericSerializer.SerializeObject(BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1);
        if (template1 != ReadSettings("ServiceWorkspaceGridTemplate1"))
          WriteSettings("ServiceWorkspaceGridTemplate1", template1);

        // Save workspaces
        var serviceWorkspaceViewModels1 = new ObservableCollection<BServiceWorkspaceViewModel>();

        foreach (var serviceWorkspace in BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1)
        {
          if (serviceWorkspace.IsServiceWorkspace)
            serviceWorkspaceViewModels1.Add(serviceWorkspace);
        }

        var serviceWorkspaces1 = GenericCollectionSerializer.SerializeObject(serviceWorkspaceViewModels1);
        if (serviceWorkspaces1 != ReadSettings("ServiceWorkspaces1"))
          WriteSettings("ServiceWorkspaces1", serviceWorkspaces1);

        var sobeesSettings = GenericCollectionSerializer.SerializeObject(SobeesSettingsLocator.SobeesSettingsStatic);
        if (sobeesSettings != ReadSettings("SobeesSettings"))
          WriteSettings("SobeesSettings", sobeesSettings);
      };
      var task = Task.Factory.StartNew(mainAction);
      task.Wait();
    }

    /// <summary>
    ///   SaveBackupSettings
    /// </summary>
    public void SaveBackupSettings()
    {
      ExportSettings(string.Format("{0}{1}", BGlobals.FOLDERBASE, BGlobals.FILEAUTOBACKUPNAME));
    }

    public void ExportSettings(string name)
    {
      try
      {
        var writer = XmlWriter.Create(name);
        WriteExportSettings(writer);
      }
      catch (Exception e)
      {
        TraceHelper.Trace(this, e);
      }
    }

    /// <summary>
    ///   WriteExportSettings
    /// </summary>
    /// <param name="writer"></param>
    public void WriteExportSettings(XmlWriter writer)
    {
      writer.WriteStartDocument();
      writer.WriteStartElement("AppSettings");
      writer.WriteElementString("SettingsVersion", "1.5");
      writer.WriteStartElement("Data");

      // Save the services template
      TraceHelper.Trace(this, "ExportSettings() -> Serializing ServiceWorkspaceGridTemplate");
      writer.WriteElementString("ServiceWorkspaceGridTemplate1",
                                GenericSerializer.SerializeObject(BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1));
      TraceHelper.Trace(this,
                        "ExportSettings() -> Following ServiceWorkspaceGridTemplate serialized: " +
                        BViewModelLocator.ViewsManagerViewModelStatic.BServiceWorkspaceGridTemplate1);

      TraceHelper.Trace(this, "ExportSettings() -> Serializing ObservableCollection<ServiceWorkspaceViewModel>");

      // Save workspaces
      var serviceWorkspaceViewModels = new ObservableCollection<BServiceWorkspaceViewModel>();
      foreach (var serviceWorkspace in BViewModelLocator.SobeesViewModelStatic.BServiceWorkspaces1)
      {
        if (serviceWorkspace.IsServiceWorkspace)
          serviceWorkspaceViewModels.Add(serviceWorkspace);
      }
      var serviceWorkspaces = GenericCollectionSerializer.SerializeObject(serviceWorkspaceViewModels);
      TraceHelper.Trace(this, "ExportSettings() -> Serialization of bservableCollection<ServiceWorkspaceViewModel> completed!");
      writer.WriteElementString("ServiceWorkspaces1", serviceWorkspaces);

      TraceHelper.Trace(this, "ExportSettings() -> Serializing bDuleSettings");
      var bDuleSettings = GenericCollectionSerializer.SerializeObject(SobeesSettingsLocator.SobeesSettingsStatic);
      TraceHelper.Trace(this, "ExportSettings() -> Serialization of bDuleSettings completed!");
      TraceHelper.Trace(this, "ExportSettings() -> Saving bDuleSettings");
      writer.WriteElementString("SobeesSettings", bDuleSettings);
      TraceHelper.Trace(this, "ExportSettings() -> Saving bDuleSettings completed");

      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndDocument();
      writer.Flush();
      writer.Close();
    }
  }
}