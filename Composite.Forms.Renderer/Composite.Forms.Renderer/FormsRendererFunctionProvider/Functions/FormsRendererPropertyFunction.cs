using System.Collections.Generic;
using Composite.C1Console.Forms;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class FormsRendererPropertyFunction : FormsRendererFunctionBase
	{

		public FormsRendererPropertyFunction(EntityTokenFactory entityTokenFactory)
			: base("FormsRendererParameter", "Composite.Forms", typeof(string), entityTokenFactory)
		{
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				yield return new FormsRendererFunctionParameterProfile(
					"PropertyName", typeof(string), true, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);
			}
		}


		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			return new FormsRendererProperty(parameters.GetParameter<string>("PropertyName"));
		}
	}
}
