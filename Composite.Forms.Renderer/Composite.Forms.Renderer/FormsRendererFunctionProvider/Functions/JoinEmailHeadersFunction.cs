using System.Collections.Generic;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class JoinEmailHeadersFunction : FormsRendererFunctionBase
	{

		public JoinEmailHeadersFunction(EntityTokenFactory entityTokenFactory)
			: base("JoinEmailHeaders", "Composite.Forms", typeof(IEnumerable<FormEmailHeader>), entityTokenFactory)
		{
		}

		protected override IEnumerable<FormsRendererFunctionParameterProfile> FunctionParameterProfiles
		{
			get
			{
				yield return new FormsRendererFunctionParameterProfile(
					"EmailHeaderA", typeof(IEnumerable<FormEmailHeader>), true, new ConstantValueProvider(null), null);

				yield return new FormsRendererFunctionParameterProfile(
					"EmailHeaderB", typeof(IEnumerable<FormEmailHeader>), true, new ConstantValueProvider(null), null);
			}
		}


		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			var result = new List<FormEmailHeader>();
			result.AddRange(parameters.GetParameter<IEnumerable<FormEmailHeader>>("EmailHeaderA"));
			result.AddRange(parameters.GetParameter<IEnumerable<FormEmailHeader>>("EmailHeaderB"));
			return result;
		}
	}
}
