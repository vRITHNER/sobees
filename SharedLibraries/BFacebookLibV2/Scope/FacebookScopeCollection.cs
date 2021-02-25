#region

using System.Collections.Generic;

#endregion

namespace Sobees.Library.BFacebookLibV2.Scope
{

  #region

  #endregion

  /// <summary>
  ///   JSON (JavaScript Object Notation) Utility Methods.
  /// </summary>
  public class FacebookScopeCollection : List<PermissionsEnum.ExtendedPermissions>
  {
    public static FacebookScopeCollection GetPermissionList()
    {
      var permissions = new FacebookScopeCollection
      {
        PermissionsEnum.ExtendedPermissions.public_profile,
        PermissionsEnum.ExtendedPermissions.user_friends,
        PermissionsEnum.ExtendedPermissions.publish_actions,
        //PermissionsEnum.ExtendedPermissions.publish_stream,
        PermissionsEnum.ExtendedPermissions.user_posts,
        PermissionsEnum.ExtendedPermissions.user_status,
        PermissionsEnum.ExtendedPermissions.publish_pages,
        PermissionsEnum.ExtendedPermissions.manage_pages
      };

      return permissions;
    }
  }
}