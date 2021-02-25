#region

using Sobees.Library.BFacebookLibV1.Schema;
using Sobees.Library.BFacebookLibV1.Session;
using System;
using System.Globalization;

#endregion

namespace Sobees.Library.BFacebookLibV1.Rest
{
  /// <summary>
  ///   Provides various methods to utilize the Facebook Platform API.
  /// </summary>
  public class Api : RestBase
  {
    #region Public Properties

    ///<summary>
    ///  Gets or sets the Auth REST API object instance.
    ///</summary>
    public Graph Graph { get; private set; }

    ///<summary>
    ///  Gets or sets the Auth REST API object instance.
    ///</summary>
    public Auth Auth { get; private set; }

    ///<summary>
    ///  Gets or sets the Connect REST API object instance.
    ///</summary>
    public Connect Connect { get; private set; }

    ///<summary>
    ///  Gets or sets the Marketplace REST API object instance.
    ///</summary>
    public Marketplace Marketplace { get; private set; }

    ///<summary>
    ///  Gets or sets the Comments REST API object instance.
    ///</summary>
    public Comments Comments { get; private set; }

    ///<summary>
    ///  Gets or sets the Comments REST API object instance.
    ///</summary>
    public Dashboard Dashboard { get; private set; }

    ///<summary>
    ///  Gets or sets the Photos REST API object instance.
    ///</summary>
    public Photos Photos { get; private set; }

    ///<summary>
    ///  Gets or sets the Users REST API object instance.
    ///</summary>
    public Users Users { get; private set; }

    ///<summary>
    ///  Gets or sets the Friends REST API object instance.
    ///</summary>
    public Friends Friends { get; private set; }

    ///<summary>
    ///  Gets or sets the Intl REST API object instance.
    ///</summary>
    public Intl Intl { get; private set; }

    ///<summary>
    ///  Gets or sets the Events REST API object instance.
    ///</summary>
    public Events Events { get; private set; }

    ///<summary>
    ///  Gets or sets the Groups REST API object instance.
    ///</summary>
    public Groups Groups { get; private set; }

    ///<summary>
    ///  Gets or sets the Admin REST API object instance.
    ///</summary>
    public Admin Admin { get; private set; }

    ///<summary>
    ///  Gets or sets the Profile REST API object instance.
    ///</summary>
    public Profile Profile { get; private set; }

    ///<summary>
    ///  Gets or sets the Notifications REST API object instance.
    ///</summary>
    public Notifications Notifications { get; private set; }

    ///<summary>
    ///  Gets or sets the Fbml REST API object instance.
    ///</summary>
    public Fbml Fbml { get; private set; }

    ///<summary>
    ///  Gets or sets the Feed REST API object instance.
    ///</summary>
    public Feed Feed { get; private set; }

    ///<summary>
    ///  Gets or sets the Fql REST API object instance.
    ///</summary>
    public Fql Fql { get; private set; }

    ///<summary>
    ///  Gets or sets the Livemessage REST API object instance.
    ///</summary>
    public LiveMessage LiveMessage { get; private set; }

    ///<summary>
    ///  Gets or sets the Livemessage REST API object instance.
    ///</summary>
    public Message Message { get; private set; }

    ///<summary>
    ///  Gets or sets the Pages REST API object instance.
    ///</summary>
    public Pages Pages { get; private set; }

    ///<summary>
    ///  Gets or sets the Application REST API object instance.
    ///</summary>
    public Application Application { get; private set; }

    ///<summary>
    ///  Gets or sets the Data REST API object instance.
    ///</summary>
    public Data Data { get; private set; }

    ///<summary>
    ///  Gets or sets the Stream REST API object instance.
    ///</summary>
    public Stream Stream { get; private set; }

    ///<summary>
    ///  Gets or sets the Status REST API object instance.
    ///</summary>
    public Status Status { get; private set; }

    ///<summary>
    ///  Gets or sets the Video REST API object instance.
    ///</summary>
    public Video Video { get; private set; }

    ///<summary>
    ///  Gets or sets the Links REST API object instance.
    ///</summary>
    public Links Links { get; private set; }

    ///<summary>
    ///  Gets or sets the Notes REST API object instance.
    ///</summary>
    public Notes Notes { get; private set; }

    ///<summary>
    ///  Gets or sets the AuthToken string.
    ///</summary>
    public string AuthToken { get; set; }

    ///<summary>
    ///  Gets or sets the LoginUrl sring.
    ///</summary>
    public string LoginUrl
    {
      get
      {
        var args = new object[2];
        args[0] = Session.ApplicationKey;
        args[1] = AuthToken;

        return String.Format(CultureInfo.InvariantCulture, Constants.FacebookLoginUrl, args);
      }
    }

    ///<summary>
    ///  Gets or sets the LogOffUrl string.
    ///</summary>
    public string LogOffUrl
    {
      get
      {
        var args = new object[2];
        args[0] = Session.ApplicationKey;
        args[1] = AuthToken;

        return String.Format(CultureInfo.InvariantCulture, Constants.FacebookLogoutUrl, args);
      }
    }

    #endregion Public Properties

    #region Internal Properties

    /// <summary>
    ///   Gets the InstalledCulture CultureInfo object.
    /// </summary>
    internal CultureInfo InstalledCulture { get; private set; }

    #endregion Internal Properties

    #region Methods

    #region Constructor

    /// <summary>
    ///   Facebook API Instance
    /// </summary>
    /// <param name="session"> </param>
    public Api(FacebookSession session)
      : base(session)
    {
      AuthToken = string.Empty;

      InstalledCulture = CultureInfo.CurrentUICulture;

      Session = session;

      Auth = new Auth(Session);
      Video = new Video(Session);
      Marketplace = new Marketplace(Session);
      Admin = new Admin(Session);
      Photos = new Photos(Session);
      Users = new Users(Session);
      Friends = new Friends(Users, Session);
      Events = new Events(Session);
      Groups = new Groups(Session);
      Notifications = new Notifications(Session);
      Profile = new Profile(Session);
      Fbml = new Fbml(Session);
      Feed = new Feed(Session);
      Fql = new Fql(Session);
      LiveMessage = new LiveMessage(Session);
      Message = new Message(Session);
      Batch = new Batch(Session);
      Pages = new Pages(Session);
      Application = new Application(Session);
      Data = new Data(Session);
      Permissions = new Permissions(Session);
      Connect = new Connect(Session);
      Comments = new Comments(Session);
      Stream = new Stream(Session);
      Status = new Status(Session);
      Links = new Links(Session);
      Notes = new Notes(Session);
      Dashboard = new Dashboard(Session);
      Intl = new Intl(Session);
      Graph = new Graph(Session);

      Batch.Batch = Batch;
      Permissions.Permissions = Permissions;
      Batch.Permissions = Permissions;
      Permissions.Batch = Batch;

      foreach (var restBase in new RestBase[]
                                 {
                                   Auth, Video, Marketplace, Admin, Photos, Users, Friends, Events,
                                   Groups, Notifications, Profile, Fbml, Feed, Fql, LiveMessage, Message, Pages,
                                   Application, Data, Connect, Comments,
                                   Stream, Status, Links, Notes
                                 })
      {
        restBase.Batch = Batch;
        restBase.Permissions = Permissions;
      }
    }

    #endregion Constructor

    #region Public Methods

    /// <summary>
    ///   Constructs the URL to use to redirect a user to enable a given extended permission for your application.
    /// </summary>
    /// <param name="permission"> The specific permission to enable. </param>
    /// <returns> The URL to redirect users to. </returns>
    public string ExtendedPermissionUrl(Enums.ExtendedPermissions permission)
    {
      var args = new object[2];
      args[0] = Session.ApplicationKey;
      args[1] = permission;

      return String.Format(CultureInfo.InvariantCulture, Constants.FacebookRequestExtendedPermissionUrl, args);
    }

    #endregion Public Methods

    #endregion Methods
  }
}