#region

using BUtility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sobees.Cls;
using Sobees.Infrastructure.Commands;
using Sobees.Infrastructure.Model;
using Sobees.Infrastructure.UI;
using Sobees.Infrastructure.ViewModelBase;
using Sobees.Tools.Logging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace Sobees.ViewModel
{
  public class SourceLoaderViewModel : BServiceWorkspaceViewModel
  {
    #region Fields

    private const string APPNAME = "SourceLoaderViewModel";

    private BServiceWorkspace _bServiceWorkspaceToLoad;

    #endregion // Fields

    #region Properties

    public override DataTemplate DataTemplateView
    {
      get
      {
        const string dt = "<DataTemplate x:Name='dtMainView' " +
                          "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                          "xmlns:Views='clr-namespace:Sobees.Views;assembly=Sobees' " +
                          "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >" +
                          "<Views:SourceLoader/> " +
                          "</DataTemplate>";

        var stringReader = new StringReader(dt);
        var xmlReader = XmlReader.Create(stringReader);
        return XamlReader.Load(xmlReader) as DataTemplate;
      }
    }

    public ObservableCollection<BServiceWorkspace> BServiceWorkspaces { get; set; }

    #endregion

    #region Constructors

    public SourceLoaderViewModel(BPosition positionInGrid, BServiceWorkspace serviceWorkspace)
      : base(positionInGrid, serviceWorkspace)
    {
      TraceHelper.Trace(this, "SourceLoaderViewModel CONSTRUCTOR");
      IsServiceWorkspace = false;
      LoadServiceWorkspacesAsync();
      InitCommands();
    }

    #endregion

    #region Commands

    private ICommand _serviceSelectedCommand;
    private object _syncRoot = new object();

    public ICommand ServiceSelectedCommand => _serviceSelectedCommand ?? (_serviceSelectedCommand = new BRelayCommand(ServiceSelected));

    #endregion

    #region Methods

    #region Public Methods

    /// <summary>
    /// LoadServiceWorkspacesAsync
    /// </summary>
    public async void LoadServiceWorkspacesAsync()
    {
      try
      {
        if (BServiceWorkspaces == null)
          BServiceWorkspaces = new ObservableCollection<BServiceWorkspace>();

        BServiceWorkspaces.Clear();

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var xmlFilePath = string.Format("{0}\\Data\\BServiceWorkspaces.xml", @location);

        await Task.Run(() =>
                                {
                                  var xml = XDocument.Load(xmlFilePath);
                                  LoadServiceWorkspaces(xml);
                                });
      }
      catch (Exception ex)
      {
        BLogManager.LogEntry(APPNAME, "LoadServiceWorkspacesAsync", ex);
      }
    }

    #endregion

    #region Private Methods

    protected override void InitCommands()
    {
      //base.InitCommands();
      CloseServiceCommand = new RelayCommand(CloseControl);
      CloseCommand = new RelayCommand(CloseControl);
    }

    private new void CloseControl()
    {
      IsServiceWorkspace = true;
      IsClosed = true;
      Messenger.Default.Send("ServiceClosed");
      Dispose();
    }

    /// <summary>
    /// LoadServiceWorkspaces
    /// </summary>
    /// <param name="xdoc"></param>
    private void LoadServiceWorkspaces(XDocument xdoc)
    {
      try
      {
        var query = (from serviceWorkspace in xdoc.Descendants("ServiceWorkspace")
                     select new BServiceWorkspace
                              {
                                DisplayName =
                                  serviceWorkspace.Attribute("DisplayName").Value,
                                ClassName =
                                  serviceWorkspace.Attribute("ClassName").Value,
                                Namespace =
                                  serviceWorkspace.Attribute("Namespace").Value,
                                Type =
                                  serviceWorkspace.Attribute("Type").Value,
                                Img =
                                  serviceWorkspace.Attribute("Img").Value,
                                Version =
                                  serviceWorkspace.Attribute("Version").Value,
                                Xap =
                                  serviceWorkspace.Attribute("Xap").Value,
                                //"/om/assets/3iob670pm6p31_1/ClientBin/" + serviceWorkspace.Attribute("Xap").Value + ".zip&type=text/plain",
                                Dll =
                                  serviceWorkspace.Attribute("Dll").Value,
                                IsEnable = bool.Parse(serviceWorkspace.Attribute("IsEnable").Value)
                              }).ToList();

        var dispatcher = Dispatcher.CurrentDispatcher;
        
        foreach (var bServiceWorkspace in query)
        {
          var workspace = bServiceWorkspace;
          Action action = () =>
          {
            if (BServiceWorkspaces.Contains(workspace)) return;
            TraceHelper.Trace(this, string.Format("Add ServiceWorkspace: {0}", workspace));
            BServiceWorkspaces.Add(workspace);
          };

         // Action backgroundAction = () => dispatcher.BeginInvoke(DispatcherPriority.Render, action);
          var task = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, UiContext.Instance.Current);
          task.Wait();
        }
      }
      catch (Exception ex)
      {
        TraceHelper.Trace(this, ex);
      }
    }

    #endregion

    #endregion

    #region Event Handler

    private void ServiceSelected(object obj)
    {
      var objs = BRelayCommand.CheckParams(obj);
      if (objs == null) return;

      var serviceWorkspace = objs[0] as BServiceWorkspace;
      if (serviceWorkspace == null) return;
      if (!serviceWorkspace.IsEnable)
        return;

      BServiceWorkspaceHelper.LoadServiceWorkspace(serviceWorkspace, PositionInGrid, this);
    }

    #endregion
  }
}