using System.Collections.Generic;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class JoinEmailsFunction : FormsRendererFunctionBase
	{

		public JoinEmailsFunction(EntityTokenFactory entityTokenFactory)
			: base("JoinEmails", "Composite.Forms", typeof(IEnumerable<FormEmail>), entityTokenFactory)
		{
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				yield return new FormsRendererFunctionParameterProfile(
					"EmailA", typeof(IEnumerable<FormEmail>), true, new ConstantValueProvider(null), null);

				yield return new FormsRendererFunctionParameterProfile(
					"EmailB", typeof(IEnumerable<FormEmail>), true, new ConstantValueProvider(null), null);
			}
		}


		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			var result = new List<FormEmail>();
			result.AddRange(parameters.GetParameter<IEnumerable<FormEmail>>("EmailA"));
			result.AddRange(parameters.GetParameter<IEnumerable<FormEmail>>("EmailB"));
			return result;
		}
	}
}
