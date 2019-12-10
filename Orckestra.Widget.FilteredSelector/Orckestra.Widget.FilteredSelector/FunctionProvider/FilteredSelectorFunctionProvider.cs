using System.Collections.Generic;

using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;

using Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation;
using Orckestra.Widget.FilteredSelector.FunctionProvider.Functions;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider
{
    [ConfigurationElementType(typeof(FilteredSelectorFunctionProviderData))]
    public class FilteredSelectorFunctionProvider : IFunctionProvider
    {
        private readonly EntityTokenFactory _entityTokenFactory;
        private List<IFunction> _functions = null;
        public FunctionNotifier FunctionNotifier { set { } }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                if (_functions == null)
                {
                    InitializeStaticTypeFunctions();
                }

                return _functions;
            }
        }

        public FilteredSelectorFunctionProvider(string providerName) => _entityTokenFactory = new EntityTokenFactory(providerName);

        private void InitializeStaticTypeFunctions() => _functions = new List<IFunction> { new SerializeMarkupParamsFunction(_entityTokenFactory) };
    }

    public sealed class FilteredSelectorFunctionProviderAssembler : IAssembler<IFunctionProvider, FunctionProviderData>
    {
        public IFunctionProvider Assemble(
            IBuilderContext context,
            FunctionProviderData objectConfiguration,
            IConfigurationSource configurationSource,
            ConfigurationReflectionCache reflectionCache)
        {
            if (objectConfiguration is null)
            {
                throw new System.ArgumentNullException(nameof(objectConfiguration));
            }

            return new FilteredSelectorFunctionProvider(objectConfiguration.Name);
        }
    }

    [Assembler(typeof(FilteredSelectorFunctionProviderAssembler))]
    public sealed class FilteredSelectorFunctionProviderData : FunctionProviderData { }
}