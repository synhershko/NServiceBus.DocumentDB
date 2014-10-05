using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NServiceBus.Unicast.Subscriptions;

namespace NServiceBus.DocumentDB.Subscriptions
{
    public class Subscription : AbstractDocument
    {
        public Subscription()
        {
            MessageTypes = new HashSet<string>();
        }

        public Address Subscriber { get; set; }

        public string SubscriberAddress { get; set; }

        public HashSet<string> MessageTypes { get; set; }

        public static string HashMessageType(MessageType messageType)
        {
            // use MD5 hash to get a 16-byte hash of the string
            using (var provider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.Default.GetBytes(messageType.TypeName + "/" + messageType.Version.Major);
                var hashBytes = provider.ComputeHash(inputBytes);
                // generate a guid from the hash:
                return new Guid(hashBytes).ToString();
            }
        }
    }
}
