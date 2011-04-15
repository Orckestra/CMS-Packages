using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.String;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class FormsRendererFunction : FormsRendererFunctionBase
	{
		public FormsRendererFunction(EntityTokenFactory entityTokenFactory)
			: base("Renderer", "Composite.Forms", typeof(XhtmlDocument), entityTokenFactory)
		{
			
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				WidgetFunctionProvider typesDropDown = StandardWidgetFunctions.DropDownList(
					this.GetType(), "DataTypesList", "Key", "Value", false);

				yield return new FormsRendererFunctionParameterProfile(
					"DataType",
					typeof(string),
					true,
					new ConstantValueProvider(""),
					typesDropDown);


				yield return new FormsRendererFunctionParameterProfile(
					"IntroText", typeof(string), false, new ConstantValueProvider(""), new WidgetFunctionProvider(new VisualXhtmlEditorFuntion(null)));


				yield return new FormsRendererFunctionParameterProfile(
					"ResponseText", typeof(string), false, new ConstantValueProvider(""), new WidgetFunctionProvider(new VisualXhtmlEditorFuntion(null)));

				yield return new FormsRendererFunctionParameterProfile(
					"ResponseUrl", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.GetDataReferenceWidget<IPage>());

				yield return new FormsRendererFunctionParameterProfile(
					"SendButtonLabel", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"ResetButtonLabel", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Email", typeof(IEnumerable<FormEmailHeader>), false, new ConstantValueProvider(null), null);

				yield return new FormsRendererFunctionParameterProfile(
					"UseCaptcha", typeof(bool), false, new ConstantValueProvider(false), StandardWidgetFunctions.CheckBoxWidget);

			}
		}

		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			XhtmlDocument result = new XhtmlDocument();
			XElement functionCall = new XElement(Composite.Core.Xml.Namespaces.Function10 + "function",
				new XAttribute("name", "Composite.Forms.RendererControl"));
			BaseRuntimeTreeNode paramNode = null;

			foreach (string parameterName in parameters.AllParameterNames)
			{
				try
				{

					if (parameters.TryGetParameterRuntimeTreeNode(parameterName, out paramNode))
					{
						functionCall.Add(paramNode.Serialize());
					}
				}
				//Ignore
				catch { }
			}
			result.Body.Add(
				new XElement(Namespaces.AspNetControls + "form",
					functionCall));
			return result;
		}

		public static IEnumerable<KeyValuePair<Type, string>> DataTypesList()
		{
			IEnumerable<Type> generatedInterfaces = DataFacade.GetGeneratedInterfaces().OrderBy(t => t.FullName);
			//all type expept metatypes
			generatedInterfaces = generatedInterfaces.Except(PageMetaDataFacade.GetAllMetaDataTypes());

			generatedInterfaces = generatedInterfaces.OrderBy(t => t.FullName);
			foreach (var type in generatedInterfaces)
			{
				yield return new KeyValuePair<Type, string>(type, type.FullName);
			}
		}
	}
}
