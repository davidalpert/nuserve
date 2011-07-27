using System;

namespace nuserve.Infrastructure
{
    public interface IPathResolutionStrategy
    {
        string ResolveToPhysicalPath(string pathToRepo);
    }
}
