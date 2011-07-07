﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nuserve.Settings;

namespace nuserve.Infrastructure
{
    public interface IAuthorizePackageOperations
    {
        bool ClientCanPublishPackage(string apiKey, string packageId);
    }

    public class AuthorizePackageOperations : IAuthorizePackageOperations
    {
        ApiSettings settings;

        /// <summary>
        /// Initializes a new instance of the PackageAuthenticationService class.
        /// </summary>
        /// <param name="settings"></param>
        public AuthorizePackageOperations(ApiSettings settings)
        {
            this.settings = settings;
        }

        public bool ClientCanPublishPackage(string apiKey, string packageId)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
                return false;

            return settings.ApiKey.ToLowerInvariant().Trim() == apiKey.ToLowerInvariant().Trim();
        }
    }
}
