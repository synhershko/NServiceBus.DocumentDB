namespace NServiceBus.DocumentDB
{
    /// <summary>
    /// Connection parameters to be used when connecting to DocumentDB
    /// </summary>
    public class ConnectionParameters
    {
        /// <summary>
        /// The url of the DocumentDB endpoint
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Authentication key for the DocumentDB endpoint
        /// </summary>
        public string AuthKey { get; set; }

        /// <summary>
        /// The name of the database to use on the specified DocumentDB endpoint
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
