
using Composite.Functions;

using Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider.Functions
{
    public sealed class SerializeMarkupParamsFunction : FilteredSelectorFunctionBase
    {
        public SerializeMarkupParamsFunction(EntityTokenFactory entityTokenFactory)
            : base(
                  Resources.default_text.SerializeMarkupParamsFuncName,
                  Constants.serializeFuncNamespace,
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
            $"{Constants.pageIdParamName}:\"{parameters.GetParameter<string>(Constants.pageIdParamName)}\";" +
            $"{Constants.typeNameParamName}:\"{parameters.GetParameter<string>(Constants.typeNameParamName)}\";" +
            $"{Constants.sitemapScopeIdParamName}:\"{parameters.GetParameter<int>(Constants.sitemapScopeIdParamName)}\";";
        }

        private void SetParameterProfiles()
        {
            WidgetFunctionProvider tb = StandardWidgetFunctions.TextBoxWidget;

            AddParameterProfile(
                new ParameterProfile(
                    Constants.pageIdParamName,
                    typeof(string),
                    true,
                    new ConstantValueProvider(null),
                    tb,
                    null,
                    Resources.default_text.SerializeMarkupParamsFuncPageIdLabel,
                    new HelpDefinition(Resources.default_text.SerializeMarkupParamsFuncPageIdHelp)));
            
            AddParameterProfile(
                new ParameterProfile(
                    Constants.typeNameParamName,
                    typeof(string),
                    true,
                    new ConstantValueProvider(null),
                    tb,
                    null,
                    Resources.default_text.SerializeMarkupParamsFuncTypeNameLabel,
                    new HelpDefinition(Resources.default_text.SerializeMarkupParamsFuncTypeNameHelp)));

            AddParameterProfile(
                new ParameterProfile(
                    Constants.sitemapScopeIdParamName,
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
