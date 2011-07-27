using System;
using System.IO;

namespace nuserve.Infrastructure.Implementation
{
    public class DefaultPathResolutionStrategy : IPathResolutionStrategy
    {
        public string ResolveToPhysicalPath(string path)
        {
            if (path.StartsWith("~/"))
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart('~', '/'));
            }

            // assume path is already a physical path
            return path;
        }
    }
}
