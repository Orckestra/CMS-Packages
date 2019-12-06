
using Composite.Functions;

using Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider.Functions
{
    public sealed class SerializeMarkupParamsFunction : FilteredSelectorFunctionBase
    {
        public SerializeMarkupParamsFunction(EntityTokenFactory entityTokenFactory)
            : base(
                  Resources.default_text.SerializeMarkupParamsFuncName,
                  Constants.SerializeFuncNamespace,
                  Resources.default_text.SerializeMarkupParamsFuncDescr,
                  Resources.default_text.SerializeMarkupParamsFuncDescr,
                  typeof(string),
                  entityTokenFactory)
        {
            SetParameterProfiles();
        }

        public override object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            return
            $"{Constants.PageIdParamName}:\"{parameters.GetParameter<string>(Constants.PageIdParamName)}\";" +
            $"{Constants.TypeNameParamName}:\"{parameters.GetParameter<string>(Constants.TypeNameParamName)}\";" +
            $"{Constants.SitemapScopeIdParamName}:\"{parameters.GetParameter<int>(Constants.SitemapScopeIdParamName)}\";";
        }

        private void SetParameterProfiles()
        {
            WidgetFunctionProvider tb = StandardWidgetFunctions.TextBoxWidget;

            AddParameterProfile(
                new ParameterProfile(
                    Constants.PageIdParamName,
                    typeof(string),
                    true,
                    new ConstantValueProvider(null),
                    tb,
                    null,
                    Resources.default_text.SerializeMarkupParamsFuncPageIdLabel,
                    new HelpDefinition(Resources.default_text.SerializeMarkupParamsFuncPageIdHelp)));
            
            AddParameterProfile(
                new ParameterProfile(
                    Constants.TypeNameParamName,
                    typeof(string),
                    true,
                    new ConstantValueProvider(null),
                    tb,
                    null,
                    Resources.default_text.SerializeMarkupParamsFuncTypeNameLabel,
                    new HelpDefinition(Resources.default_text.SerializeMarkupParamsFuncTypeNameHelp)));

            AddParameterProfile(
                new ParameterProfile(
                    Constants.SitemapScopeIdParamName,
                    typeof(string),
                    true,
                    new ConstantValueProvider(null),
                    tb,
                    null,
                    Resources.default_text.SerializeMarkupParamsFuncSitemapScopeIdLabel,
                    new HelpDefinition(Resources.default_text.SerializeMarkupParamsFuncSitemapScopeIdHelp)));
        }
    }
}
