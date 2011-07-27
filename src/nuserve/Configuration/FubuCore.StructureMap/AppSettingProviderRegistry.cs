using Microsoft.Practices.ServiceLocation;
using FubuCore.Binding;
using FubuCore.Configuration;
using FubuCore.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.ServiceLocatorAdapter;

namespace FubuCore.StructureMap
{
    /// <summary>
    /// Borrowed from FubuMVC.StructureMap, these dependencies enable
    /// the appSettings magic as facilitated by StructureMap. 
    /// </summary>
    /// <remarks>
    /// Though currently stored in the FubuMvc project, these bits depend 
    /// only on FubuCore and not on any 'Mvc' bits.  Hence my namespace
    /// change from FubuMvc to FubuCore.
    /// </remarks>
    /// <remarks>https://github.com/DarthFubuMVC/fubumvc/blob/master/src/FubuMVC.StructureMap/AppSettingProviderRegistry.cs</remarks>
    public class AppSettingProviderRegistry : Registry
    {
        public AppSettingProviderRegistry()
        {
            For<ISettingsProvider>().Use<AppSettingsProvider>();
            For<IObjectResolver>().Use<ObjectResolver>();
            For<IServiceLocator>().Use<StructureMapServiceLocator>();
            For<IValueConverterRegistry>().Use<ValueConverterRegistry>();
            For<ITypeDescriptorCache>().Use<TypeDescriptorCache>();
            ForSingletonOf<IPropertyBinderCache>().Use<PropertyBinderCache>();
            ForSingletonOf<ICollectionTypeProvider>().Use<DefaultCollectionTypeProvider>();
            ForSingletonOf<IModelBinderCache>().Use<ModelBinderCache>();

            For<IModelBinder>().Use<StandardModelBinder>();
        }
    }
}