
using FubuCore.Binding;
using StructureMap;
using InMemoryRequestData = FubuCore.Binding.InMemoryRequestData;
using StructureMap.ServiceLocatorAdapter;

namespace FubuMVC.StructureMap
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://github.com/DarthFubuMVC/fubumvc/blob/master/src/FubuMVC.StructureMap/InMemoryStructureMapBindingContext.cs</remarks>
    public class InMemoryStructureMapBindingContext : BindingContext
    {
        private readonly InMemoryRequestData _data;
        private readonly IContainer _container;

        public InMemoryStructureMapBindingContext()
            : this(new InMemoryRequestData(), new Container())
        {
        }

        private InMemoryStructureMapBindingContext(InMemoryRequestData data, IContainer container)
            : base(data, new StructureMapServiceLocator(container))
        {
            _data = data;
            _container = container;
        }

        public InMemoryStructureMapBindingContext WithData(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public InMemoryStructureMapBindingContext WithPropertyValue(object value)
        {
            PropertyValue = value;
            return this;
        }

        public InMemoryRequestData Data { get { return _data; } }
        public IContainer Container { get { return _container; } }

        public object this[string key] { get { return _data[key]; } set { _data[key] = value; } }
    }
}