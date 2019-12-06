using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;
using Composite.Functions;
using Composite.Functions.Plugins.WidgetFunctionProvider;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.Foundation;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;

using Orckestra.Widget.FilteredSelector.WidgetProvider.Functions;

namespace Orckestra.Widget.FilteredSelector.WidgetProvider
{
    [ConfigurationElementType(typeof(FilteredSelectorWidgetFunctionProviderData))]
    internal sealed class FilteredSelectorWidgetProvider : IDynamicTypeWidgetFunctionProvider
    {
        public FilteredSelectorWidgetProvider(string providerName) => _entityTokenFactory = new EntityTokenFactory(providerName);
        private readonly EntityTokenFactory _entityTokenFactory = null;
        private List<IWidgetFunction> _widgetDynamicTypeFunctions = null;
        public IEnumerable<IWidgetFunction> Functions { get; }
        public WidgetFunctionNotifier WidgetFunctionNotifier { get; set; }
        private void InitializeDynamicTypeFunctions()
        {
            _widgetDynamicTypeFunctions = new List<IWidgetFunction>();

            IEnumerable<Type> dataInterfaces = DataFacade.GetAllKnownInterfaces(UserType.Developer)
                .Where(typeof(IPageRelatedData).IsAssignableFrom);

            object[] args = new object[] { _entityTokenFactory };

            _widgetDynamicTypeFunctions.AddRange(
                from t in dataInterfaces
                select (IWidgetFunction)Activator
                .CreateInstance(typeof(SelectFilteredDataWidgetFunction<>)
                .MakeGenericType(t), args));
        }
        public IEnumerable<IWidgetFunction> DynamicTypeDependentFunctions
        {
            get
            {
                if (_widgetDynamicTypeFunctions == null)
                {
                    InitializeDynamicTypeFunctions();
                }
                foreach (IWidgetFunction widgetFunction in _widgetDynamicTypeFunctions)
                {
                    yield return widgetFunction;
                }
            }
        }
    }

    [Assembler(typeof(FilteredSelectorWidgetFunctionProviderAssembler))]
    internal sealed class FilteredSelectorWidgetFunctionProviderData : WidgetFunctionProviderData { }

    internal sealed class FilteredSelectorWidgetFunctionProviderAssembler : IAssembler<IWidgetFunctionProvider, WidgetFunctionProviderData>
    {
        public IWidgetFunctionProvider Assemble(
            IBuilderContext context,
            WidgetFunctionProviderData objectConfiguration,
            IConfigurationSource configurationSource,
            ConfigurationReflectionCache reflectionCache)
        {
            return new FilteredSelectorWidgetProvider(objectConfiguration.Name);
        }
    }
}