using System;
using FubuCore.Configuration;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace FubuCore.StructureMap
{
    /// <summary>
    /// Borrowed from FubuMVC.StructureMap, this Type Scanning
    /// <see cref="IRegistrationConvention"/> is what tells
    /// StructureMap how to 'build up' instances of concrete
    /// *Settings classes with values provided by the registered
    /// <see cref="ISettingsProvider"/>.
    /// </summary>
    /// <remarks>
    /// Though currently stored in the FubuMvc project, these bits depend 
    /// only on FubuCore and not on any 'Mvc' bits.  Hence my namespace
    /// change from FubuMvc to FubuCore.
    /// </remarks>
    /// <remarks>https://github.com/DarthFubuMVC/fubumvc/blob/master/src/FubuMVC.StructureMap/SettingsScanner.cs</remarks>
    public class SettingsScanner : IRegistrationConvention
    {
        public void Process(Type type, Registry graph)
        {
            if (!type.Name.EndsWith("Settings") || type.IsInterface) return;

            graph.For(type).LifecycleIs(InstanceScope.Singleton).Use(c => c.GetInstance<ISettingsProvider>().SettingsFor(type));
        }
    }
}