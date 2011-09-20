using System.Collections.Generic;
using Composite.Core.Xml;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class FormEmailFunction : FormsRendererFunctionBase
	{

		public FormEmailFunction(EntityTokenFactory entityTokenFactory)
			: base("FormEmail", "Composite.Forms", typeof(IEnumerable<FormEmail>), entityTokenFactory)
		{
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				yield return new FormsRendererFunctionParameterProfile(
					"From", typeof(string), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"To", typeof(string), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Cc", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Subject", typeof(string), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Body", typeof(XhtmlDocument), false, new ConstantValueProvider(new XhtmlDocument()), StandardWidgetFunctions.VisualXhtmlDocumentEditorWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"AppendFormData", typeof(bool), false, new ConstantValueProvider(true), StandardWidgetFunctions.CheckBoxWidget);

			}
		}


		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			return new List<FormEmail>()
				{
					new FormEmail()
						{
							From = parameters.GetParameter<string>("From"),
							To = parameters.GetParameter<string>("To"),
							Cc = parameters.GetParameter<string>("Cc"),
							Subject = parameters.GetParameter<string>("Subject"),
							Body = parameters.GetParameter<XhtmlDocument>("Body"),
							AppendFormData = parameters.GetParameter<bool>("AppendFormData")
						}
				};
		}
	}
}
