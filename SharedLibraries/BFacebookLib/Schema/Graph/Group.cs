using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents Group collection
    /// </summary>
    [DataContract]
    public class GroupCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Group> Data { get; set; }
    }
    /// <summary>
    /// Contains Group information
    /// </summary>
    [DataContract]
    public class Group : GraphDataObject,IEquatable<Group>
    {

        [DataMember(Name = "id")]
        public long GroupId
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

        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        [DataMember(Name = "description")]
        public string Description
        {
            get;
            set;
        }

        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }


        [DataMember(Name = "venue")]
        public Location Venue
        {
            get;
            set;
        }
        
        [DataMember(Name = "privacy")]
        public string Privacy
        {
            get;
            set;
        }

        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }
        #region IEquatable<Group> Members

        public bool Equals(Group other)
        {
            return (other != null && other.GroupId == this.GroupId);
        }

        #endregion
    }

}
