using System;
using System.Collections.Generic;
using NServiceBus.Timeout.Core;

namespace NServiceBus.DocumentDB.Timeouts
{
    /// <summary>
    /// Internal class used for persisting timeouts
    /// </summary>
    public class Timeout : AbstractDocument
    {
        public Timeout()
        {
            
        }

        public Timeout(TimeoutData td)
        {
            Destination = td.Destination;
            SagaId = td.SagaId;
            State = td.State;
            Time = td.Time;
            OwningTimeoutManager = td.OwningTimeoutManager;
            Headers = td.Headers;
        }

        /// <summary>
        /// The address of the client who requested the timeout.
        /// 
        /// </summary>
        public Address Destination { get; set; }

        /// <summary>
        /// The saga ID.
        /// 
        /// </summary>
        public Guid SagaId { get; set; }

        /// <summary>
        /// Additional state.
        /// 
        /// </summary>
        public byte[] State { get; set; }

        /// <summary>
        /// The time at which the timeout expires.
        /// 
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The timeout manager that owns this particular timeout
        /// 
        /// </summary>
        public string OwningTimeoutManager { get; set; }

        /// <summary>
        /// Store the headers to preserve them across timeouts
        /// 
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
    }
}
