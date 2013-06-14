using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.C1Console.Actions;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Extensions;
using Composite.Core.ResourceSystem;
using Composite.Core.ResourceSystem.Icons;
using Composite.Data;
using Composite.Data.Types;
using Composite.Functions;
using Composite.Core.Xml;
using Composite.Plugins.Elements.ElementProviders.BaseFunctionProviderElementProvider;

namespace Composite.Tools
{
    public class XslToRazorFunctionConverter : IElementActionProvider
    {
        private static readonly ResourceHandle ActionIcon = GetIconHandle("xslt-based-function");
        private static readonly IEnumerable<ElementAction> NoActions = new ElementAction[0];
        private static readonly ActionGroup PrimaryActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);

        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            var dataEntityToken = entityToken as DataEntityToken;
            if (dataEntityToken == null) return NoActions;

            if (dataEntityToken.InterfaceType != typeof(IXsltFunction)) return NoActions;

            var action = new ElementAction(new ActionHandle(new ConvertFunctionActionToken()))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Convert to Razor",
                    ToolTip = "Creates a new razor function with the same name and parameters while renaming the existing xsl function",
                    Icon = ActionIcon,
                    Disabled = false,
                    ActionLocation = new ActionLocation
                    {
                        ActionType = ActionType.Other,
                        IsInFolder = false,
                        IsInToolbar = true,
                        ActionGroup = PrimaryActionGroup
                    }
                }
            };

            return new [] { action };
        }

        protected static ResourceHandle GetIconHandle(string name)
        {
            return new ResourceHandle(BuildInIconProviderName.ProviderName, name);
        }

        [ActionExecutor(typeof(ConvertFunctionActionExecutor))]
        internal sealed class ConvertFunctionActionToken : ActionToken
        {
            private static IEnumerable<PermissionType> _permissionType = new[] { PermissionType.Edit, PermissionType.Delete };

            public override IEnumerable<PermissionType> PermissionTypes
            {
                get { return _permissionType; }
            }

            public override string Serialize()
            {
                return typeof(ConvertFunctionActionToken).Name;
            }


            public static ActionToken Deserialize(string serializedData)
            {
                return new ConvertFunctionActionToken();
            }
        }

        internal sealed class ConvertFunctionActionExecutor : Composite.C1Console.Actions.IActionExecutor
        {
            public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
            {
                var xslFunction = (IXsltFunction) ((DataEntityToken)entityToken).Data;

                string @namespace = xslFunction.Namespace;
                string name = xslFunction.Name;
                string functionFullName = @namespace + "." + name;

                Guid xslFunctionId = xslFunction.Id;
                var function = FunctionFacade.GetFunction(functionFullName);

                // TODO: default values convertion

                var cshtml = new StringBuilder();

                cshtml.Append(
                    @"@inherits RazorFunction

@functions {
");

                if(!function.Description.IsNullOrEmpty())
                {
                    cshtml.Append(
@"    public override string FunctionDescription
    {
        get { return @""" + function.Description.Replace("\"", "\"\"") + @"""; }
    }
");
                }

                if(function.ReturnType != typeof(XhtmlDocument))
                {
                    cshtml.Append(
@"    public override Type FunctionReturnType
    {
        get { return typeof(" + function.ReturnType.FullName + @"); }
    }
");
                }

                foreach(var parameterProfile in function.ParameterProfiles)
                {
                    string parameterTemplate = 
@"  
    [FunctionParameter(%Properties%)]
    public %Type% %Name% { get; set; }%DefaultValueNote%

";
                    var parameterProperties = new List<string>();

                    if(!parameterProfile.Label.IsNullOrEmpty() 
                        && parameterProfile.Label != parameterProfile.Name)
                    {
                        parameterProperties.Add("Label=\"{0}\"".FormatWith(parameterProfile.Label));
                    }

                    if(parameterProfile.HelpDefinition != null
                        && !string.IsNullOrEmpty(parameterProfile.HelpDefinition.HelpText)) 
                    {
                        parameterProperties.Add("Help=@\"{0}\""
                            .FormatWith(parameterProfile.HelpDefinition.HelpText.Replace("\"", "\"\"")));
                    }

                    string parameterName = parameterProfile.Name;

                    IParameter parameter = DataFacade.GetData<IParameter>()
                        .FirstOrDefault(p => p.OwnerId == xslFunctionId && p.Name == parameterName);

                    string defaultValueTodoHint = string.Empty;
                    if(!parameter.DefaultValueFunctionMarkup.IsNullOrEmpty())
                    {
                        XElement markup = XElement.Parse(parameter.DefaultValueFunctionMarkup);

                        defaultValueTodoHint = @"// TODO: convert default value function markup
    /*" + markup + "*/";

                    }

                    string typeName = GetCSharpFriendlyTypeName(parameterProfile.Type);

                    Verify.IsNotNull(parameter, "Failed to get information about parameter " + parameterName);
                    if(!parameter.WidgetFunctionMarkup.IsNullOrEmpty())
                    {
                        const string start = @"<f:widgetfunction xmlns:f=""http://www.composite.net/ns/function/1.0"" name=""";
                        const string end1 = @""" label="""" bindingsourcename=""""><f:helpdefinition xmlns:f=""http://www.composite.net/ns/function/1.0"" helptext="""" /></f:widgetfunction>";
                        const string end2 = @""" />";
                        

                        if (parameter.WidgetFunctionMarkup.StartsWith(start)
                            && (parameter.WidgetFunctionMarkup.EndsWith(end1)
                                || parameter.WidgetFunctionMarkup.EndsWith(end2)))
                        {
                            string str1 = parameter.WidgetFunctionMarkup.Substring(start.Length);
                            string widgetFunctionName = str1.Substring(0, str1.IndexOf("\""));

                            // Skipping default widget for string fields
                            if (widgetFunctionName != GetDefaultWidgetFunctionName(parameterProfile.Type))
                            {
                                parameterProperties.Add("WidgetFunctionName=\"{0}\"".FormatWith(widgetFunctionName));
                            }
                        }
                        else
                        {
                            parameterProperties.Add("WidgetMarkup=@\"{0}\"".FormatWith(parameter.WidgetFunctionMarkup.Replace("\"", "\"\"")));
                        }
                    }

                    cshtml.Append(parameterTemplate
                                      .Replace("%Properties%", string.Join(", ", parameterProperties))  
                                      .Replace("%Type%", typeName)
                                      .Replace("%Name%", parameterProfile.Name)
                                      .Replace("%DefaultValueNote%", defaultValueTodoHint));
                }

                cshtml.Append(
                    @"}

@{
");
                var functionCalls = DataFacade.GetData<INamedFunctionCall>().Where(fc => fc.XsltFunctionId == xslFunctionId).ToList();
                foreach(var functionCall in functionCalls)
                {
                     cshtml.Append(@"// TODO: convert function call '{0}' 
/* ".FormatWith(functionCall.Name));

                     XElement markup = XElement.Parse(functionCall.SerializedFunction);

                     cshtml.Append(markup);

                    cshtml.Append(@"*/
");
                }


                cshtml.Append(@"}

<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
</head>
<body>

@* TODO: convert XSL template: *@

@*
");
                try
                {
                    IFile file = IFileServices.TryGetFile<IXsltFile>(xslFunction.XslFilePath);
                    cshtml.Append(file.ReadAllText());
                }
                catch(Exception ex)
                {
                    cshtml.Append("Failed to load the file: " + ex);
                }

                cshtml.Append(@"
*@
</body>
</html>");

                string fileFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Razor/" + @namespace.Replace('.', '/'));
                Directory.CreateDirectory(fileFolder);

                string filePath = fileFolder + "/" + name + ".cshtml";
                File.WriteAllText(filePath, cshtml.ToString(), Encoding.UTF8);

                
                xslFunction.Name = xslFunction.Name + "_backup";
                DataFacade.Update(xslFunction);

                var consoleMsgService = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();

                consoleMsgService.RefreshTreeSection(new BaseFunctionFolderElementEntityToken("ROOT:XsltBasedFunctionProviderElementProvider"));
                consoleMsgService.RefreshTreeSection(new BaseFunctionFolderElementEntityToken("ROOT:RazorFunctionProviderElementProvider"));

                consoleMsgService.ShowMessage( DialogType.Message, "XSL -> CSHTML",
                        (@"Template for the razor function '{0}'  have been added. Parts that have to be converted manually:
a) Default values for parameters
b) Function calls
c) Transformation template itself".FormatWith(functionFullName)));

                return null;
            }
        }

        private static string GetDefaultWidgetFunctionName(Type type)
        {
            if (type == typeof(string)) return "Composite.Widgets.String.TextBox";
            if (type == typeof(Guid)) return "Composite.Widgets.Guid.TextBox";
            if (type == typeof(int)) return "Composite.Widgets.Integer.TextBox";
            if (type == typeof(bool)) return "Composite.Widgets.Bool.CheckBox";
            if (type == typeof(Composite.Data.DataReference<Composite.Data.Types.IPage>)) return "Composite.Widgets.DataReference.PageSelector";
            if (type == typeof(Composite.Data.DataReference<Composite.Data.Types.IMediaFileFolder>)) return "Composite.Widgets.MediaFileFolderSelector";
            if (type == typeof(Composite.Data.DataReference<Composite.Data.Types.IMediaFile>)) return "Composite.Widgets.MediaFileSelectorWidgetFunction";
            if (type == typeof(Composite.Data.DataReference<Composite.Data.Types.IImageFile>)) return "Composite.Widgets.ImageSelectorWidgetFunction";
            
            return null;
        }

        private static string GetCSharpFriendlyTypeName(Type type)
        {
            string typeFullName = type.FullName;

            if(type.IsGenericType)
            {
                var sb = new StringBuilder();

                string genericTypeName = GetTypeNameWithoutNamespace(type);

                // Replacing "NullableDataReference`1" -> "Composite.Data.NullableDataReference"
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf("`"));

                sb.Append(genericTypeName);
                sb.Append("<");

                sb.Append(string.Join(",", type.GetGenericArguments().Select(GetCSharpFriendlyTypeName)));

                sb.Append(">");

                return sb.ToString();
            }

            switch (typeFullName)
            {
                case "System.String":
                    return "string";
                case "System.Boolean":
                    return "bool";
                case "System.Object":
                    return "object";
                case "System.Guid":
                    return "Guid";
                case "System.Int32":
                    return "int";
                case "System.Long":
                    return "long";
            }

            return GetTypeNameWithoutNamespace(type);
        }

        private static string GetTypeNameWithoutNamespace(Type type)
        {
            var knownNamespaces = new[] { "System", "Composite.Data", "Composite.Data.Types", "Composite.Core.Xml" };

            return knownNamespaces.Contains(type.Name)
                       ? type.Name
                       : type.Namespace + "." + type.Name;
        }
    }
}