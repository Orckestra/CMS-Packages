using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Composite.Core;
using Composite.Core.Extensions;
using Composite.Core.Types;
using Composite.Data;
using System.Linq;
using Composite.Data.DynamicTypes;
using Composite.Functions;

namespace Composite.Tools.StaticDataTypeCreator
{
	public class StaticDataTypeCreator : IHttpHandler
	{
        const string FieldPadding = "        ";


		public void ProcessRequest(HttpContext context)
		{
			var typeName = context.Request["typeName"];

			Type type = TypeManager.GetType(typeName);
			Guid typeId = type.GetImmutableTypeId();
			string typeNamespace = type.Namespace;
			var generatedCode = StaticDataTypeCreatorFacade.CreateStaticDataType(typeId);
			generatedCode = ClearDatatypeMarkup(generatedCode, typeNamespace);
            generatedCode = InsertFieldsFormRenderingProfiles(generatedCode, typeId);


            string fileName = typeName.Contains(",") 
                ? typeName.Substring(0, typeName.IndexOf(",", StringComparison.Ordinal))
                : typeName;

            context.Response.Clear();
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.ContentType = "text/plain";
            context.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.cs", fileName));
			context.Response.BinaryWrite(Encoding.UTF8.GetPreamble());
			context.Response.Write(generatedCode);
			context.Response.End();
		}

        private string InsertFieldsFormRenderingProfiles(string generatedCode, Guid typeId)
        {
            var lines = new List<string>(generatedCode.Split(new [] { Environment.NewLine }, StringSplitOptions.None));

            var descriptor = DynamicTypeManager.GetDataTypeDescriptor(typeId);
            foreach (var field in descriptor.Fields)
            {
                if (field.Inherited)
                {
                    continue;
                }

                if(field.FormRenderingProfile == null) continue;

                string label = field.FormRenderingProfile.Label;
                string help = field.FormRenderingProfile.HelpText;
                string widgetFunctionMarkup = field.FormRenderingProfile.WidgetFunctionMarkup;

                if (label == field.Name)
                {
                    label = null;
                }

                if (widgetFunctionMarkup != null)
                {
                    widgetFunctionMarkup = CleanWidgetFunctionMarkup(widgetFunctionMarkup);

                    if (widgetFunctionMarkup == GetDefaultWidgetFunctionMarkup(field))
                    {
                        widgetFunctionMarkup = null;
                    }
                }

                if (label != null || !string.IsNullOrEmpty(help) || widgetFunctionMarkup != null)
                {
                    var sb = new StringBuilder(FieldPadding + "[FormRenderingProfile(");

                    bool first = true;

                    if (label != null)
                    {
                        sb.Append("Label = ");
                        AppendCSharpConstant(sb, label);

                        first = false;
                    }

                    if (!string.IsNullOrEmpty(help))
                    {
                        if(!first) sb.Append(", ");
                        sb.Append("HelpText = ");
                        AppendCSharpConstant(sb, help);

                        first = false;
                    }

                    if (widgetFunctionMarkup != null)
                    {
                        if (!first) sb.Append(", ");
                        sb.Append("WidgetFunctionMarkup = ");
                        AppendCSharpConstant(sb, widgetFunctionMarkup);   
                    }

                    sb.Append(")]");

                    string newLine = sb.ToString();

                    string fieldIdStr = field.Id.ToString();
                    int fieldDefinitionOffset = lines.FindIndex(line => line.Contains(fieldIdStr));
                    Verify.That(fieldDefinitionOffset > 0, "Failed to find the field definition line");

                    lines.Insert(fieldDefinitionOffset + 1, newLine);
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

	    private string CleanWidgetFunctionMarkup(string markup)
	    {
	        return markup
	            .Replace(" label=\"\"", "")
	            .Replace(" bindingsourcename=\"\"", "")
	            .Replace("<f:helpdefinition xmlns:f=\"http://www.composite.net/ns/function/1.0\" helptext=\"\" />", "")
                .Replace("\"></f:widgetfunction>", "\" />");
	    }

	    private void AppendCSharpConstant(StringBuilder code, string value)
	    {
	        
	        if (!value.Contains(Environment.NewLine) && !value.Contains("\""))
	        {
                code.Append("\"");
                code.Append(value.Replace("\\", "\\\\"));
                code.Append("\"");
                return;
	        }

            code.Append("@\"");
	        code.Append(value.Replace("\"", "\"\""));
            code.Append("\"");
	    }

	    private string ClearDatatypeMarkup(string content, string typeNamespace)
		{

			// save attributes with namespaces
			var foreignKeyAttributes = new List<string>();
			var foreignKeys = content.Replace("Composite.Data.ForeignKeyAttribute(", "~").Split('~');
			foreach (var f in foreignKeys.Skip(1))
			{
				var foreignKey = f.GetStringInBetween(@"""", @"""");
				foreignKeyAttributes.Add(foreignKey);
				Log.LogInformation(Assembly.GetExecutingAssembly().GetName().Name, foreignKey);
			}

			var typeUsings = new List<string>();
			var usings = new List<string> { "Composite.Core.WebClient.Renderings.Data", "Composite.Data.Types", "Composite.Data.Hierarchy", "Composite.Data.Hierarchy.DataAncestorProviders", "Composite.Data.ProcessControlled", "Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController", "Composite.Data.Validation.Validators", "Composite.Data", "Microsoft.Practices.EnterpriseLibrary.Validation.Validators", "System" };
			usings.Sort();
			usings.Reverse();

			foreach (var u in usings.Where(u => content.Contains(u)))
			{
				content = content.Replace(u + ".", "");
				typeUsings.Add(u);
			}

			content = content.Replace("[CodeGeneratedAttribute()]", "");
			content = content.Replace("]" + Environment.NewLine + Environment.NewLine + "[", "]" + Environment.NewLine + "[");
			content = content.Replace("{" + Environment.NewLine + "    " + Environment.NewLine + "    [", "{" + Environment.NewLine + "    [");
			content = content.Replace(" {" + Environment.NewLine, Environment.NewLine + "    {" + Environment.NewLine);
			content = content.Replace("{" + Environment.NewLine + Environment.NewLine + "[", "{" + Environment.NewLine + "[");
			content = content.Replace("    {" + Environment.NewLine + "    [Im", "{" + Environment.NewLine + "    [Im");
			content = content.Replace(Environment.NewLine, Environment.NewLine + "    ");
            content = content.Replace("Attribute()]" + Environment.NewLine, "]" + Environment.NewLine);

			// restore attributes with namespaces
			var attributeMask = "ForeignKeyAttribute(\"{0}";
			foreach (var f in foreignKeyAttributes)
			{
				string foreignKeyAttribute = f.Split('.').Last();
				string attributeOld = string.Format(attributeMask, foreignKeyAttribute);
				string attributeNew = string.Format(attributeMask, f);
				content = content.Replace(attributeOld, attributeNew);
			}

			typeUsings.Sort();
			var usingsToAdd = string.Join(string.Empty, typeUsings.Select(u => string.Format("using {0};" + Environment.NewLine, u)));


			string markup = 
@"{0}
namespace {1}
{{{2}}}*";

            // content = Environment.NewLine + "    #region Composite C1 data type attributes" + content;
			content = string.Format(markup, usingsToAdd, typeNamespace, content);
			content = content.Replace("    }*", "}");

			// //#endregion for #region Properties for Composite C1 Data Type
			// content = content.Replace("public interface", "#endregion" + Environment.NewLine + "    public interface");

			// // wrap field properties with #region Field properties and get set in one line
			// content = content.Replace("[ImmutableFieldId", "#region Data field attributes" + Environment.NewLine + FieldPadding + "[ImmutableFieldId");
			//content = Regex.Replace(content, @"(])(\s+[\w<>]+\s+\w+)(\s+\{\s+get;\s+set;\s+\})", "$1" + Environment.NewLine + FieldPadding + "#endregion$2 { get; set; }");
            content = Regex.Replace(content, @"(])(\s+[\w<>]+\s+\w+)(\s+\{\s+get;\s+set;\s+\})", "$1$2 { get; set; }");
			content = content.Replace("Attribute(", "(");

			return content;
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

        private string GetDefaultWidgetFunctionMarkup(DataFieldDescriptor fieldDescriptor)
	    {
            // Auto generating a widget for not code generated data types
            Type fieldType;

            if (!fieldDescriptor.ForeignKeyReferenceTypeName.IsNullOrEmpty())
            {
                string referenceTypeName = fieldDescriptor.ForeignKeyReferenceTypeName;
                Type foreignKeyType = TypeManager.GetType(referenceTypeName);
                Verify.IsNotNull(foreignKeyType, "Failed to find type '{0}'".FormatWith(referenceTypeName));

                var referenceTemplateType = fieldDescriptor.IsNullable ? typeof(NullableDataReference<>) : typeof(DataReference<>);

                fieldType = referenceTemplateType.MakeGenericType(foreignKeyType);
            }
            else
            {
                fieldType = fieldDescriptor.InstanceType;
            }

            var widgetFunctionProvider = StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(fieldType);
            if (widgetFunctionProvider != null)
            {
                return widgetFunctionProvider.SerializedWidgetFunction.ToString();
            }

            return null;
        }
	}
}