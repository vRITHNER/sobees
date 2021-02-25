using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{


    /// <summary>
    /// Contains user information
    /// </summary>
    [DataContract]
    public class GraphDataObject
    {
        [DataMember(Name = "metadata")]
        public Metadata Metadata { get; set; }

        [DataMember(Name = "paging")]
        public Paging Paging { get; set; }
    }

    /// <summary>
    /// Contains user information
    /// </summary>
    [DataContract]
    public class Metadata
    {
        [DataMember(Name = "connections")]
        public Connections Connections { get; set; }
    }
    /// <summary>
    /// Contains user information
    /// </summary>
    [DataContract]
    public class Connections
    {
        [DataMember(Name = "photos")]
        public string Photos { get; set; }
        [DataMember(Name = "comments")]
        public string Comments { get; set; }
        [DataMember(Name = "feed")]
        public string Feed { get; set; }
        [DataMember(Name = "noreply")]
        public string NoReply { get; set; }
        [DataMember(Name = "maybe")]
        public string Maybe { get; set; }
        [DataMember(Name = "invited")]
        public string Invited { get; set; }
        [DataMember(Name = "attending")]
        public string Attending { get; set; }
        [DataMember(Name = "declined")]
        public string Declined { get; set; }
        [DataMember(Name = "picture")]
        public string Picture { get; set; }
        [DataMember(Name = "members")]
        public string Members { get; set; }
        [DataMember(Name = "tagged")]
        public string Tagged { get; set; }
        [DataMember(Name = "links")]
        public string Links { get; set; }
        [DataMember(Name = "groups")]
        public string Groups { get; set; }
        [DataMember(Name = "albums")]
        public string Albums { get; set; }
        [DataMember(Name = "statuses")]
        public string Statuses { get; set; }
        [DataMember(Name = "videos")]
        public string Videos { get; set; }
        [DataMember(Name = "notes")]
        public string Notes { get; set; }
        [DataMember(Name = "posts")]
        public string Posts { get; set; }
        [DataMember(Name = "events")]
        public string Events { get; set; }
        [DataMember(Name = "home")]
        public string Home { get; set; }
        [DataMember(Name = "friends")]
        public string Friends { get; set; }
        [DataMember(Name = "Activities")]
        public string Activities { get; set; }
        [DataMember(Name = "interests")]
        public string Interests { get; set; }
        [DataMember(Name = "music")]
        public string Music { get; set; }
        [DataMember(Name = "books")]
        public string Books { get; set; }
        [DataMember(Name = "movies")]
        public string Movies { get; set; }
        [DataMember(Name = "television")]
        public string Television { get; set; }
        [DataMember(Name = "likes")]
        public string Likes { get; set; }
        [DataMember(Name = "inbox")]
        public string Inbox { get; set; }
        [DataMember(Name = "outbox")]
        public string Outbox { get; set; }
        [DataMember(Name = "updates")]
        public string Updates { get; set; }
    }

    /// <summary>
    /// Contains user information
    /// </summary>
    [DataContract]
    public class Paging
    {
        [DataMember(Name = "next")]
        public string Next { get; set; }
        [DataMember(Name = "previous")]
        public string Previous { get; set; }
    }

    /// <summary>
    /// </summary>
    [DataContract]
    public class IdNamePair
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// name
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
    public class LinkNamePair
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember(Name = "link")]
        public string Link
        {
            get;
            set;
        }

        /// <summary>
        /// name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name
        {
            get;
            set;
        }

    }


}
