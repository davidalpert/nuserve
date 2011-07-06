using System;

namespace nuserve.Settings
{
    public class EndpointSettings
    {
        /// <summary>
        /// Initializes a new instance of the EndpointSettings class.
        /// </summary>
        public EndpointSettings()
        {
            Port = 5656;
            HostName = "localhost";
            Protocol = "http";
        }

        public int Port { get; set; }
        public string HostName { get; set; }
        public string Protocol { get; set; }

        public Uri GetEndpointUri()
        {
            return new UriBuilder(Protocol, HostName, Port).Uri;
        }
    }
}
