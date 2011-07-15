using System;

namespace nuserve.Infrastructure
{
    public interface IAuthorizePackageOperations
    {
        bool ClientCanPublishPackage(string apiKey, string packageId);
    }
}
