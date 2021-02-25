﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinkedIn.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LinkedIn.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Count must be larger then 0..
        /// </summary>
        internal static string CountOutOfRangeMessage {
            get {
                return ResourceManager.GetString("CountOutOfRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;countryCode&apos; cannot be null or empty, when a postalCode is provided..
        /// </summary>
        internal static string CountryCodeArgumentMessage {
            get {
                return ResourceManager.GetString("CountryCodeArgumentMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A new share must contain a comment and/or (a title and a submittedUri)..
        /// </summary>
        internal static string InvalidCreateShareArguments {
            get {
                return ResourceManager.GetString("InvalidCreateShareArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The apiRequest must contain a valid url..
        /// </summary>
        internal static string InvalidUrlApiRequestMessage {
            get {
                return ResourceManager.GetString("InvalidUrlApiRequestMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The visibility code cannot be unknown..
        /// </summary>
        internal static string InvalidVisibilityCode {
            get {
                return ResourceManager.GetString("InvalidVisibilityCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be longer then {1} characters..
        /// </summary>
        internal static string MaxLengthMessageFormat {
            get {
                return ResourceManager.GetString("MaxLengthMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be empty..
        /// </summary>
        internal static string NotEmptyMessageFormat {
            get {
                return ResourceManager.GetString("NotEmptyMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be an empty string..
        /// </summary>
        internal static string NotEmptyStringMessageFormat {
            get {
                return ResourceManager.GetString("NotEmptyStringMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be null..
        /// </summary>
        internal static string NotNullMessageFormat {
            get {
                return ResourceManager.GetString("NotNullMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;Associations&apos;, &apos;Connections&apos;, &apos;Honors&apos;, &apos;Proposal Comments&apos;, &apos;Specialties&apos;, &apos;Summary&apos;, &apos;Relation to viewer&apos; fields can only be requested in profile requets..
        /// </summary>
        internal static string ProfileFieldsContainsInvalidCollectionFields {
            get {
                return ResourceManager.GetString("ProfileFieldsContainsInvalidCollectionFields", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;SitePublicProfileRequestUrl&apos; field can only be requested for public profile requests..
        /// </summary>
        internal static string ProfileFieldsContainsSitePublicProfileRequest {
            get {
                return ResourceManager.GetString("ProfileFieldsContainsSitePublicProfileRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Count must be between 1 and 25..
        /// </summary>
        internal static string SearchCountOutOfRangeMessage {
            get {
                return ResourceManager.GetString("SearchCountOutOfRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start must be larger then 0..
        /// </summary>
        internal static string StartOutOfRangeMessage {
            get {
                return ResourceManager.GetString("StartOutOfRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your status text exceeded the maximum length..
        /// </summary>
        internal static string StatusOutOfRangeMessage {
            get {
                return ResourceManager.GetString("StatusOutOfRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} must be larger then zero..
        /// </summary>
        internal static string TimeStampOutOfRangeMessageFormat {
            get {
                return ResourceManager.GetString("TimeStampOutOfRangeMessageFormat", resourceCulture);
            }
        }
    }
}
