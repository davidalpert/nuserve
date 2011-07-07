using System;
using Nancy.Bootstrappers.StructureMap;

namespace nuserve.Configuration
{
    public class NuserveNancyBootstrapper : StructureMapNancyBootstrapper
    {
        protected override StructureMap.IContainer GetApplicationContainer()
        {
            IoC.Bootstrap();

            return IoC.Container as StructureMap.IContainer;
        }
    }
}
