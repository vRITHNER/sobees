using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents user collection
    /// </summary>
    [DataContract]
    public class FacebookUserCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookUser> Data { get; set; }
    }
    /// <summary>
    /// Contains user information
    /// </summary>
    [DataContract]
    public class FacebookUser : GraphDataObject
    {
        /// <summary>
        /// User id
        /// </summary>
        [DataMember(Name = "id")]
        public string UserId
        {
            get;
            set;
        }
        /// <summary>
        /// Firstname
        /// </summary>
        [DataMember(Name = "first_name")]
        public string FirstName
        {
            get;
            set;
        }
        /// <summary>
        /// Last name
        /// </summary>
        [DataMember(Name = "last_name")]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Full name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// link
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }

        /// <summary>
        /// About me information
        /// </summary>
        [DataMember(Name = "about")]
        public string About
        {
            get;
            set;
        }


        /// <summary>
        /// Birthday of user
        /// </summary>
        [DataMember(Name = "birthday")]
        public string Birthday
        {
            get;
            set;
        }
        /// <summary>
        /// Books information
        /// </summary>
        [DataMember(Name = "hometown")]
        public IdNamePair HometownLocation
        {
            get;
            set;
        }
        /// <summary>
        /// Books information
        /// </summary>
        [DataMember(Name = "location")]
        public IdNamePair Location
        {
            get;
            set;
        }
        /// <summary>
        /// work history
        /// </summary>
        [DataMember(Name = "work")]
        public List<Work> Work
        {
            get;
            set;
        }
        /// <summary>
        /// education
        /// </summary>
        [DataMember(Name = "education")]
        public List<Education> Education
        {
            get;
            set;
        }

        /// <summary>
        /// Sex of user
        /// </summary>
        [DataMember(Name = "gender")]
        public string Gender
        {
            get;
            set;
        }
        /// <summary>
        /// Interests
        /// </summary>
        [DataMember(Name = "interested_in")]
        public List<string> InterestedIn
        {
            get;
            set;
        }
        /// <summary>
        /// Meeting For
        /// </summary>
        [DataMember(Name = "meeting_for")]
        public List<string> MeetingFor
        {
            get;
            set;
        }

        /// <summary>
        /// RelationshipStatus
        /// </summary>
        [DataMember(Name = "relationship_status")]
        public string RelationshipStatus
        {
            get;
            set;
        }

        /// <summary>
        /// religion
        /// </summary>
        [DataMember(Name = "religion")]
        public string Religion
        {
            get;
            set;
        }

        /// <summary>
        /// political info of user
        /// </summary>
        [DataMember(Name = "political")]
        public string Political
        {
            get;
            set;
        }


        /// <summary>
        /// Activities
        /// </summary>
        [DataMember(Name = "timezone")]
        public int TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Books information
        /// </summary>
        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }

        /// <summary>
        /// current location
        /// </summary>
        [DataMember(Name = "email")]
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// current location
        /// </summary>
        [DataMember(Name = "website")]
        public string Website
        {
            get;
            set;
        }

        /// <summary>
        /// significant_other_id
        /// </summary>
        [DataMember(Name = "significant_other")]
        public IdNamePair SignificantOther
        {
            get;
            set;
        }

    }


    /// <summary>
    /// Contains location information
    /// </summary>
    [DataContract]
    public class Location
    {
        /// <summary>
        /// Street
        /// </summary>
        [DataMember(Name = "street")]
        public string Street
        {
            get;
            set;
        }

        /// <summary>
        /// City
        /// </summary>
        [DataMember(Name = "city")]
        public string City
        {
            get;
            set;
        }
        /// <summary>
        /// State
        /// </summary>
        [DataMember(Name = "state")]
        public string State
        {
            get;
            set;
        }
        /// <summary>
        /// Zip
        /// </summary>
        [DataMember(Name = "zip")]
        public string Zip
        {
            get;
            set;
        }
        /// <summary>
        /// Country
        /// </summary>
        [DataMember(Name = "country")]
        public string Country
        {
            get;
            set;
        }
        /// <summary>
        /// Latitude
        /// </summary>
        [DataMember(Name = "latitude")]
        public string Latitude
        {
            get;
            set;
        }
        /// <summary>
        /// Longitude
        /// </summary>
        [DataMember(Name = "Longitude")]
        public string Longitude
        {
            get;
            set;
        }


    }
    /// <summary>
    /// </summary>
    [DataContract]
    public class Work
    {
        /// <summary>
        /// Street
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

    }
    /// <summary>
    /// </summary>
    [DataContract]
    public class Education
    {
        /// <summary>
        /// School
        /// </summary>
        [DataMember(Name = "school")]
        public IdNamePair School
        {
            get;
            set;
        }
        /// <summary>
        /// Degree
        /// </summary>
        [DataMember(Name = "degree")]
        public IdNamePair Degree
        {
            get;
            set;
        }
        /// <summary>
        /// School
        /// </summary>
        [DataMember(Name = "year")]
        public IdNamePair Year
        {
            get;
            set;
        }
        /// <summary>
        /// School
        /// </summary>
        [DataMember(Name = "concentration")]
        public List<IdNamePair> Concentration
        {
            get;
            set;
        }

    }


}
