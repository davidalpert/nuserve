using System;

namespace nuserve.Settings
{
    public class ApiSettings
    {
        /// <summary>
        /// Initializes a new instance of the ApiSettings class.
        /// </summary>
        public ApiSettings()
        {
            ApiKey = String.Empty;
        }

        public string ApiKey { get; set; }
    }
}
