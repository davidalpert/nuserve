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
            PackageListUri = "http://localhost:8080/packages";
            PackageManagerUri = "http://localhost:8080/";
        }

        public string PackageListUri { get; set; }
        public string PackageManagerUri { get; set; }
    }
}
