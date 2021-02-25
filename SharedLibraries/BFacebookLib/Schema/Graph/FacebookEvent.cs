using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    [DataContract]
    public class FacebookEventCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookEvent> Data { get; set; }
    }    /// <summary>
    /// Class containing Event information
    /// </summary>
    [DataContract]
    public class FacebookEvent : GraphDataObject
    {
        /// <summary>
        /// Event id
        /// </summary>
        [DataMember(Name = "id")]
        public long EventId
        {
            get;
            set;
        }
        /// <summary>
        /// Owner
        /// </summary>
        [DataMember(Name = "owner")]
        public IdNamePair Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Event name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "start_time")]
        public string StartTime
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "end_time")]
        public string EndTime
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "updated_time")]
        public string UpdateTime
        {
            get;
            set;
        }
        /// <summary>
        /// Event location
        /// </summary>
        [DataMember(Name = "location")]
        public string Location
        {
            get;
            set;
        }
        /// <summary>
        /// Event location
        /// </summary>
        [DataMember(Name = "privacy")]
        public string Privacy
        {
            get;
            set;
        }


        /// <summary>
        /// Event location information
        /// </summary>
        [DataMember(Name = "venue")]
        public Location Venue
        {
            get;
            set;
        }


    }

}
