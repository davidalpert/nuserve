using System;
using NuGet.Server.Infrastructure;

namespace nuserve.Infrastructure
{
    public interface IServerPackageRepositoryFactory
    {
        IServerPackageRepository BuildServerPackageRepository();
    }
}
