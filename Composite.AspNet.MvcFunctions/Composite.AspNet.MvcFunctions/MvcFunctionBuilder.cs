using System;
using System.Linq;
using System.Reflection;
using Composite.AspNet.MvcFunctions.Routing;
using Composite.Core.Routing;
using Composite.Functions;
using Composite.Plugins.Functions.FunctionProviders.MvcFunctions;

namespace Composite.AspNet.MvcFunctions
{
    public class MvcFunctionBuilder
    {
        private readonly MvcFunctionBase _function;

        internal MvcFunctionBuilder(MvcFunctionBase function)
        {
            _function = function;
        }

        public MvcFunctionBuilder AddParameter(string name, Type type = null, bool? isRequired = null, BaseValueProvider defaultValueProvider = null, string label = null, string helpText = null, WidgetFunctionProvider widgetFunctionProvider = null, bool hideInSimpleView = false)
        {
            Verify.ArgumentNotNullOrEmpty(name, "name");

            var info = _function.GetParameterInformation().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            var reflectedInfo = info != null ? GetParameterDefaults(info) : null;


            if (type == null)
            {
                Verify.IsNotNull(reflectedInfo, "Failed to find the type of the parameter '{0}' through reflection", name);

                type = reflectedInfo.Type;
            }

            if (label == null)
            {
                label = name;
            }

            if (helpText == null)
            {
                helpText = String.Empty;
            }

            bool isRequired2 = isRequired ?? reflectedInfo?.IsRequired ?? false;

            if (defaultValueProvider == null)
            {
                defaultValueProvider = reflectedInfo != null
                    ? (BaseValueProvider) new ConstantValueProvider(reflectedInfo.DefaultValue)
                    : new NoValueValueProvider();
            }

            widgetFunctionProvider = widgetFunctionProvider ?? StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(type, isRequired2);

            _function.AddParameter(new ParameterProfile(name, type, isRequired2, defaultValueProvider, widgetFunctionProvider, 
                label, new HelpDefinition(helpText), hideInSimpleView));
            return this;
        }

        /// <summary>
        /// When called, the function will be configured, so the path info value will be appended to the mvc route to be executed.
        /// F.e. "{pageUrl}/a/b/c" will render execute the route  "{controller}/{action}/a/b/c" for action functions.
        /// </summary>
        /// <returns></returns>
        public MvcFunctionBuilder IncludePathInfo()
        {
            _function.UsePathInfoForRouting();
            return this;
        }

        /// <summary>
        /// When called, the function will be configured for its output to be excluded from the full-page caching.
        /// </summary>
        /// <returns></returns>
        public MvcFunctionBuilder PreventOutputCaching()
        {
            _function.PreventFunctionOutputCaching = true;
            return this;
        }

        public MvcFunctionBuilder AddParameter(ParameterProfile parameterProfile)
        {
            _function.AddParameter(parameterProfile);
            return this;
        }

        private ParameterDefaults GetParameterDefaults(ParameterInfo parameterInfo)
        {
            return new ParameterDefaults
            {
                IsRequired = !parameterInfo.IsOptional,
                DefaultValue = parameterInfo.RawDefaultValue,
                Type = parameterInfo.ParameterType
            };
        }

        public void AddUrlMapping<T>(string actionName = null, string fieldName = null)
        {
            AddUrlMapping(typeof (T), actionName, fieldName);
        }

        public void AddUrlMapping<T>(Guid pageId, string actionName = null, string fieldName = null)
        {
            AddUrlMapping(typeof(T), actionName, fieldName);
        }

        public void AddUrlMapping(Type dataType, string actionName = null, string fieldName = null)
        {
            _function.AssignDynamicUrlMapper(dataType, new MvcFunctionDataUrlMapper(dataType, null, actionName, fieldName));
        }

        public void AddUrlMapping(Type dataType, Guid pageId, string actionName = null, string fieldName = null)
        {
            DataUrls.RegisterGlobalDataUrlMapper(dataType, new MvcFunctionDataUrlMapper(dataType, pageId, actionName, fieldName));
        }
    }

    internal class ParameterDefaults
    {
        public bool IsRequired;
        public object DefaultValue;
        public Type Type;
    }
}
