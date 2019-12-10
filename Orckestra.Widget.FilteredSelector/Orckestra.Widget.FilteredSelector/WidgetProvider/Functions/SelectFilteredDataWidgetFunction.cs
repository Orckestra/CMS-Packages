using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

using Composite.Core.Types;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Functions;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.Foundation;

namespace Orckestra.Widget.FilteredSelector.WidgetProvider.Functions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SelectFilteredDataWidgetFunction<T> : CompositeWidgetFunctionBase where T : class, IData
    {
        public SelectFilteredDataWidgetFunction(EntityTokenFactory entityTokenFactory)
            : base(GetWidgetName(), typeof(DataReference<T>), entityTokenFactory)
        {
            SetParameterProfiles();
            Description = Resources.default_text.SelectFilteredDataWidgetFuncDescr;
            Name = Resources.default_text.SelectFilteredDataWidgetFuncName;
        }

        public override string Description { get; }
        public override string Name { get; }
        private XElement BuildFormMarkUp(
            ParameterList parameters,
            string label,
            HelpDefinition helpDefinition,
            string bindingSourceName,
            Type optionsGeneratingStaticType,
            string optionsGeneratingStaticMethodName,
            object optionsGeneratingStaticMethodParameterValue,
            string optionsObjectKeyPropertyName,
            string optionsObjectLabelPropertyName,
            bool required)
        {
            if (!parameters.TryGetParameter(nameof(SitemapScope), out SitemapScope sitemapScope))
            {
                throw new ArgumentException(nameof(SitemapScope));
            }

            string
                keySelTag = "KeySelector",
                statMethTag = "StaticMethodCall",
                compFuncName = $"{Constants.SerializeFuncNamespace}.{Resources.default_text.SerializeMarkupParamsFuncName}";

            XNamespace
                uc = Namespaces.BindingFormsStdUiControls10,
                f = Namespaces.BindingFormsStdFuncLib10,
                bf = Namespaces.BindingForms10,
                ft = Namespaces.Function10;

            XElement keySelector = StandardWidgetFunctions.BuildBasicFormsMarkup(uc, keySelTag, "Selected", label, helpDefinition, bindingSourceName);

            XElement keySelectorOptions = new XElement(uc + $"{keySelTag}.Options",
                new XElement(f + statMethTag,
                new XAttribute("Type", TypeManager.SerializeType(optionsGeneratingStaticType)),
                new XAttribute("Method", optionsGeneratingStaticMethodName),
                    new XElement(uc + $"{statMethTag}.Parameters",
                        new XElement(ft + "function",
                        new XAttribute("name", compFuncName),
                            new XElement(ft + "param",
                            new XAttribute("name", Constants.TypeNameParamName),
                            new XAttribute("value", optionsGeneratingStaticMethodParameterValue)),
                            new XElement(ft + "param", new XAttribute("name", Constants.PageIdParamName),
                                new XElement(bf + "read", new XAttribute("source", "PageId"))),
                            new XElement(ft + "param",
                            new XAttribute("name", Constants.SitemapScopeIdParamName),
                            new XAttribute("value", (int)sitemapScope))))));
            keySelector.Add(
                new XAttribute("OptionsKeyField", optionsObjectKeyPropertyName),
                new XAttribute("OptionsLabelField", optionsObjectLabelPropertyName),
                new XAttribute("Required", required),
                keySelectorOptions);

            return keySelector;
        }


        public static IEnumerable<KeyValuePair<SitemapScope, string>> GetAssociationPageRestrictions()
        {
            return new Dictionary<SitemapScope, string>() 
            {
                {SitemapScope.Current, Resources.default_text.SitemapScope_Current},
                {SitemapScope.All, Resources.default_text.SitemapScope_All},
                {SitemapScope.AncestorsAndCurrent, Resources.default_text.SitemapScope_AncestorsAndCurrent},
                {SitemapScope.Ancestors, Resources.default_text.SitemapScope_Ancestors},
                {SitemapScope.Parent, Resources.default_text.SitemapScope_Parent},
                {SitemapScope.Descendants, Resources.default_text.SitemapScope_Descendants},
                {SitemapScope.DescendantsAndCurrent, Resources.default_text.SitemapScope_DescendantsAndCurrent},
                {SitemapScope.Children, Resources.default_text.SitemapScope_Children},
                {SitemapScope.Siblings, Resources.default_text.SitemapScope_Siblings},
                {SitemapScope.Level1, Resources.default_text.SitemapScope_Level1},
                {SitemapScope.Level1AndDescendants, Resources.default_text.SitemapScope_Level1AndDescendants},
                {SitemapScope.Level1AndSiblings, Resources.default_text.SitemapScope_Level1AndSiblings},
                {SitemapScope.Level2, Resources.default_text.SitemapScope_Level2},
                {SitemapScope.Level2AndDescendants, Resources.default_text.SitemapScope_Level2AndDescendants},
                {SitemapScope.Level2AndSiblings, Resources.default_text.SitemapScope_Level2AndSiblings},
                {SitemapScope.Level3, Resources.default_text.SitemapScope_Level3},
                {SitemapScope.Level3AndDescendants, Resources.default_text.SitemapScope_Level3AndDescendants},
                {SitemapScope.Level3AndSiblings, Resources.default_text.SitemapScope_Level3AndSiblings},
                {SitemapScope.Level4, Resources.default_text.SitemapScope_Level4},
                {SitemapScope.Level4AndDescendants, Resources.default_text.SitemapScope_Level4AndDescendants},
                {SitemapScope.Level4AndSiblings, Resources.default_text.SitemapScope_Level4AndSiblings}};
        }
        public static IEnumerable GetOptions(string options) => FilteredSelectorWidgetProcessingManager.GetParameters(options);
        public override XElement GetWidgetMarkup(ParameterList parameters, string label, HelpDefinition helpDefinition, string bindingSourceName)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            return BuildFormMarkUp(parameters, label, helpDefinition, bindingSourceName, GetType(), nameof(GetOptions), TypeManager.SerializeType(typeof(T)), "Key", "Label", true);
        }
        internal static string GetWidgetName()
        {
            return CommonNamespace + ".DataReference." + typeof(T).FullName.Replace(".", "")
                + $".{Resources.default_text.SelectFilteredDataWidgetFuncName}";
        }
        private void SetParameterProfiles()
        {
            WidgetFunctionProvider dropDown = StandardWidgetFunctions.DropDownList(GetType(), nameof(GetAssociationPageRestrictions), "Key", "Value", false, true);

            AddParameterProfile(
                new ParameterProfile(
                    typeof(SitemapScope).Name,
                    typeof(SitemapScope),
                    true,
                    new ConstantValueProvider(SitemapScope.Current),
                    dropDown,
                    null,
                    "Sitemap scope",
                    new HelpDefinition(Resources.default_text.SelectFilteredDataWidgetFuncSitescopeHelp)));
        }
    }
}
