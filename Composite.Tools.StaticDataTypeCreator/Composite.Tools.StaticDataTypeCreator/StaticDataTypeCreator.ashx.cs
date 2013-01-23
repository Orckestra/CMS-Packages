using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Composite.Core;
using Composite.Core.Types;
using Composite.Data;
using System.Linq;

namespace Composite.Tools.StaticDataTypeCreator
{
	public class StaticDataTypeCreator : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			var typeName = context.Request["typeName"];
			Type type = TypeManager.GetType(typeName);
			Guid typeId = type.GetImmutableTypeId();
			string typeNamespace = type.Namespace;
			var datatype = StaticDataTypeCreatorFacade.CreateStaticDataType(typeId);
			datatype = ClearDatatypeMarkup(datatype, typeNamespace);
			context.Response.Clear();
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.ContentType = "text/plain";
			context.Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.cs", typeName));
			context.Response.BinaryWrite(Encoding.UTF8.GetPreamble());
			context.Response.Write(datatype);
			context.Response.End();
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

            content = Environment.NewLine + "    #region Composite C1 data type attributes" + content;
			content = string.Format(markup, usingsToAdd, typeNamespace, content);
			content = content.Replace("    }*", "}");

			// #endregion for #region Properties for Composite C1 Data Type
			content = content.Replace("public interface", "#endregion" + Environment.NewLine + "    public interface");

			//wrap field properties with #region Field properties and get set in one line
			content = content.Replace("[ImmutableFieldId", "#region Data field attributes" + Environment.NewLine + "        [ImmutableFieldId");
			content = Regex.Replace(content, @"(])(\s+\w+\s+\w+)(\s+\{\s+get;\s+set;\s+\})", "$1" + Environment.NewLine + "        #endregion$2 { get; set; }");

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
	}
}