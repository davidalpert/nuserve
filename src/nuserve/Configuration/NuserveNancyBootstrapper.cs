using System;
using Nancy.Bootstrappers.StructureMap;

namespace nuserve.Configuration
{
    /// <summary>
    /// Configures Nancy to use StructureMap as a bootstrapper, delegating
    /// base configuration to our <see cref="IoC"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nancy is smart enough to scan the current assembly looking for an
    /// <see cref="INancyBootstrapper"/> so simply including this bootstrapper
    /// in the project is enough to wire up Nancy to use our custom 
    /// StructureMap container when building Nancy Modules, such as our
    /// <see cref="PackageFilesModule"/> which handles <i>push</i> requests
    /// from <i>nuget.exe</i>.
    /// </para>
    /// <para>
    /// Now that Nancy is using our StructureMap container, which knows about
    /// FubuCore's appSettings magic thanks to the <see cref="NugetServerRegistry"/>, 
    /// any modules that depend on a *Settings instance will automatically 
    /// get the default values overriden by appSettings in nuserve.exe.config
    /// on startup.
    /// </para>
    /// </remarks>
    public class NuserveNancyBootstrapper : StructureMapNancyBootstrapper
    {
        protected override StructureMap.IContainer GetApplicationContainer()
        {
            IoC.Bootstrap();

            return IoC.Container as StructureMap.IContainer;
        }
    }
}
