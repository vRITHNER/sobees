﻿#region

using Sobees.Library.BFacebookLibV2.Json;
using Sobees.Library.BFacebookLibV2.Objects.Enums;

#endregion

namespace Sobees.Library.BFacebookLibV2.Objects.Users
{
  #region

  

  #endregion

  public class FacebookUser : SocialJsonObject
  {
    #region Constructors

    private FacebookUser(JsonObjectEx obj) : base(obj)
    {
    }

    #endregion

    #region Static methods

    public static FacebookUser Parse(JsonObjectEx obj)
    {
      if (obj == null) return null;
      return new FacebookUser(obj)
      {
        Id = obj.GetString("id"),
        About = obj.GetString("about"),
        Bio = obj.GetString("bio"),
        Birthday = obj.GetString("birthday"),
        Cover = obj.GetObject("cover", FacebookCoverPhoto.Parse),
        Email = obj.GetString("email"),
        FirstName = obj.GetString("first_name"),
        Gender = obj.GetEnum("gender", FacebookGender.Unknown),
        Hometown = obj.GetObject("hometown", FacebookObject.Parse),
        IsVerified = obj.GetBoolean("is_verified"),
        Languages = obj.GetArray("languages", FacebookObject.Parse) ?? new FacebookObject[0],
        LastName = obj.GetString("last_name"),
        Link = obj.GetString("link"),
        Locale = obj.GetString("locale"),
        Location = obj.GetObject("location", FacebookObject.Parse),
        MiddleName = obj.GetString("middle_name"),
        Name = obj.GetString("name"),
        Timezone = obj.HasValue("timezone") ? obj.GetInt32("timezone") : (int?) null,
        Verified = obj.GetBoolean("verified"),
        Website = obj.GetString("website")
      };
    }

    /// <summary>
    ///   Gets a user from the specified <var>JsonObject</var>.
    /// </summary>
    /// <param name="path"></param>
    public static FacebookUser Parse(string path)
    {
      if (path == null) return null;
      var obj = JsonObjectEx.ParseJson(path);
      return Parse(obj);
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the ID of the user. As of v2.0 of the Facebook Graph API, IDs are unique to each app and cannot be
    ///   used across different apps.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    ///   Gets the "about me" section of the person's profile.
    /// </summary>
    public string About { get; private set; }

    /// <summary>
    ///   Gets the person's bio.
    /// </summary>
    public string Bio { get; private set; }

    /// <summary>
    ///   Gets the birthday of the user. The birthday is specified using the <code>MM/DD/YYYY</code> format.
    /// </summary>
    public string Birthday { get; private set; }

    /// <summary>
    ///   Gets information about the cover photo of the user.
    /// </summary>
    public FacebookCoverPhoto Cover { get; private set; }

    /// <summary>
    ///   Gets the email address of the user.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    ///   Gets the first name of the user.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    ///   Gets the gender of the user.
    /// </summary>
    public FacebookGender Gender { get; private set; }

    /// <summary>
    ///   Gets information about the hometown of the user.
    /// </summary>
    public FacebookObject Hometown { get; private set; }

    /// <summary>
    ///   People with large numbers of followers can have the authenticity of their identity manually verified by
    ///   Facebook. This field indicates whether the user profile is verified in this way.
    /// </summary>
    public bool IsVerified { get; private set; }

    /// <summary>
    ///   Gets an array of Facebook Pages representing the languages this person knows.
    /// </summary>
    public FacebookObject[] Languages { get; private set; }

    /// <summary>
    ///   Gets the last name of the user.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    ///   Gets a link to the profile of the user.
    /// </summary>
    public string Link { get; private set; }

    /// <summary>
    ///   Gets the selected locale of the user.
    /// </summary>
    public string Locale { get; private set; }

    /// <summary>
    ///   Gets the current location of the user as entered by them on their profile. This is not a check in field.
    /// </summary>
    public FacebookObject Location { get; private set; }

    /// <summary>
    ///   Gets the middle name of the user.
    /// </summary>
    public string MiddleName { get; private set; }

    /// <summary>
    ///   Gets the full name of the user.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   Gets the picture.
    /// </summary>
    /// <value>The picture.</value>
    public string Picture { get; private set; }

    /// <summary>
    ///   Gets the selected timezone of the user. The timezone is specified as the offset in hours from UTC.
    /// </summary>
    public int? Timezone { get; private set; }


    /// <summary>
    ///   Indicates whether the user account has been verified. This is distinct from the <code>is_verified</code>
    ///   field. Someone is considered verified if they take any of the following actions:
    ///   - Register for mobile
    ///   - Confirm their account via SMS
    ///   - Enter a valid credit card
    /// </summary>
    public bool Verified { get; private set; }

    /// <summary>
    ///   Gets the URL for the website of the user.
    /// </summary>
    public string Website { get; private set; }

    #endregion
  }
}