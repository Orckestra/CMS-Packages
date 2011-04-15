using System.Collections.Generic;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class FormEmailHeaderFunction : FormsRendererFunctionBase
	{

		public FormEmailHeaderFunction(EntityTokenFactory entityTokenFactory)
			: base("FormEmailHeader", "Composite.Forms", typeof(IEnumerable<FormEmailHeader>), entityTokenFactory)
		{
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				yield return new FormsRendererFunctionParameterProfile(
					"From", typeof(IEnumerable<char>), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"To", typeof(IEnumerable<char>), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Cc", typeof(IEnumerable<char>), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Subject", typeof(IEnumerable<char>), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

			}
		}


		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			return new List<FormEmailHeader>(){
				new FormEmailHeader(
				  parameters.GetParameter<IEnumerable<char>>("From")
				, parameters.GetParameter<IEnumerable<char>>("To")
				, parameters.GetParameter<IEnumerable<char>>("Cc")
				, parameters.GetParameter<IEnumerable<char>>("Subject")
				)
			};
		}
	}
}
