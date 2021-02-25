using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents Group collection
    /// </summary>
    [DataContract]
    public class NoteCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<Note> Data { get; set; }
    }    
    /// <summary>
    /// Class containing Event information
    /// </summary>
    [DataContract]
    public class Note : GraphDataObject
    {
        /// <summary>
        /// Event id
        /// </summary>
        [DataMember(Name = "id")]
        public long NoteId
        {
            get;
            set;
        }
        /// <summary>
        /// Owner
        /// </summary>
        [DataMember(Name = "from")]
        public IdNamePair From
        {
            get;
            set;
        }

        /// <summary>
        /// Event name
        /// </summary>
        [DataMember(Name = "subject")]
        public string Subject
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "updated_time")]
        public string UpdatedTime
        {
            get;
            set;
        }
        /// <summary>
        /// Event description
        /// </summary>
        [DataMember(Name = "created_time")]
        public string CreatedTime
        {
            get;
            set;
        }
    }

}
