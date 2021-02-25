using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sobees.Library.BFacebookLibV1.Schema.Graph
{
    /// <summary>
    /// Represents Group collection
    /// </summary>
    [DataContract]
    public class FacebookItemCollection : GraphDataObject
    {
        [DataMember(Name = "data")]
        public List<FacebookItem> Data { get; set; }
    }    
    
    /// <summary>
    /// </summary>
    [DataContract]
    public class FacebookItem : GraphDataObject
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
        /// <summary>
        /// name
        /// </summary>
        [DataMember(Name = "category")]
        public string Category
        {
            get;
            set;
        }
    }
}
