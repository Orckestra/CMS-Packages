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
                throw new ArgumentException(nameof(SitemapScope);
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
            keySelector.Add(
                new XAttribute("OptionsKeyField", optionsObjectKeyPropertyName),
                new XAttribute("OptionsLabelField", optionsObjectLabelPropertyName),
                new XAttribute("Required", required));

            XElement keySelectorOptions = new XElement(uc + $"{keySelTag}.Options");
            keySelector.Add(keySelectorOptions);

            XElement staticMethodCall = new XElement(f + statMethTag);
            staticMethodCall.Add(
                new XAttribute("Type", TypeManager.SerializeType(optionsGeneratingStaticType)),
                new XAttribute("Method", optionsGeneratingStaticMethodName));
            keySelectorOptions.Add(staticMethodCall);

            XElement staticMethodCallParams = new XElement(uc + $"{statMethTag}.Parameters");
            staticMethodCall.Add(staticMethodCallParams);

            XElement functionComposer = new XElement(ft + "function");
            functionComposer.Add(new XAttribute("name", compFuncName));
            staticMethodCallParams.Add(functionComposer);

            XElement functionParamType = new XElement(ft + "param");
            functionParamType.Add(
                new XAttribute("name", Constants.TypeNameParamName),
                new XAttribute("value", optionsGeneratingStaticMethodParameterValue));
            functionComposer.Add(functionParamType);

            XElement functionPageId = new XElement(ft + "param");
            functionPageId.Add(new XAttribute("name", Constants.PageIdParamName));
            functionPageId.Add(new XElement(bf + "read", new XAttribute("source", "PageId")));
            functionComposer.Add(functionPageId);

            XElement functionSitescope = new XElement(ft + "param");
            functionSitescope.Add(
                new XAttribute("name", Constants.SitemapScopeIdParamName),
                new XAttribute("value", (int)sitemapScope));
            functionComposer.Add(functionSitescope);

            return keySelector;
        }


        public static IEnumerable<KeyValuePair<SitemapScope, string>> GetAssociationPageRestrictions()
        {
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Current, Resources.default_text.SitemapScope_Current);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.All, Resources.default_text.SitemapScope_All);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.AncestorsAndCurrent, Resources.default_text.SitemapScope_AncestorsAndCurrent);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Ancestors, Resources.default_text.SitemapScope_Ancestors);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Parent, Resources.default_text.SitemapScope_Parent);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Descendants, Resources.default_text.SitemapScope_Descendants);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.DescendantsAndCurrent, Resources.default_text.SitemapScope_DescendantsAndCurrent);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Children, Resources.default_text.SitemapScope_Children);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Siblings, Resources.default_text.SitemapScope_Siblings);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level1, Resources.default_text.SitemapScope_Level1);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level1AndDescendants, Resources.default_text.SitemapScope_Level1AndDescendants);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level1AndSiblings, Resources.default_text.SitemapScope_Level1AndSiblings);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level2, Resources.default_text.SitemapScope_Level2);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level2AndDescendants, Resources.default_text.SitemapScope_Level2AndDescendants);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level2AndSiblings, Resources.default_text.SitemapScope_Level2AndSiblings);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level3, Resources.default_text.SitemapScope_Level3);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level3AndDescendants, Resources.default_text.SitemapScope_Level3AndDescendants);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level3AndSiblings, Resources.default_text.SitemapScope_Level3AndSiblings);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level4, Resources.default_text.SitemapScope_Level4);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level4AndDescendants, Resources.default_text.SitemapScope_Level4AndDescendants);
            yield return new KeyValuePair<SitemapScope, string>(SitemapScope.Level4AndSiblings, Resources.default_text.SitemapScope_Level4AndSiblings);
        }
        public static IEnumerable GetOptions(string options) => FilteredSelectorWidgetProcessingManager.GetParameters(options);
        public override XElement GetWidgetMarkup(ParameterList parameters, string label, HelpDefinition helpDefinition, string bindingSourceName)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            return BuildFormMarkUp(parameters, label, helpDefinition, bindingSourceName, GetType(), "GetOptions", TypeManager.SerializeType(typeof(T)), "Key", "Label", true);
        }
        internal static string GetWidgetName()
        {
            return CommonNamespace + ".DataReference." + typeof(T).FullName.Replace(".", "")
                + $".{Resources.default_text.SelectFilteredDataWidgetFuncName}";
        }
        private void SetParameterProfiles()
        {
            WidgetFunctionProvider dropDown = StandardWidgetFunctions.DropDownList(GetType(), "GetAssociationPageRestrictions", "Key", "Value", false, true);

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
