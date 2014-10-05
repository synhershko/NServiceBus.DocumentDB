using Newtonsoft.Json;

namespace NServiceBus.DocumentDB
{
    /// <summary>
    /// This class acts as a bridge between our POCOs and DocumentDB's metadata. While I wish it
    /// wasn't necessary, it helps avoiding casting. The official advice of inheritting from DocDB's
    /// Document class doesn't actually work unfortunately.
    /// </summary>
    public abstract class AbstractDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "_self")]
        public string SelfLink { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
    }
}
