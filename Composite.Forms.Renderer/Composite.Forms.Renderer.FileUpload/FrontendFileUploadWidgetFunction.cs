using System.Xml.Linq;
using Composite.Functions;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.Foundation;

namespace Composite.Forms.Renderer.FileUpload
{
	public class FrontendFileUploadWidgetFunction : CompositeWidgetFunctionBase
	{
		public const string CompositeName = "Composite.Forms.Renderer.FrontendFileUpload";

		public FrontendFileUploadWidgetFunction(EntityTokenFactory entityTokenFactory)
			: base(CompositeName, typeof(string), entityTokenFactory)
		{
		}

		public override XElement GetWidgetMarkup(ParameterList parameters, string label, HelpDefinition help, string bindingSourceName)
		{
			return base.BuildBasicWidgetMarkup("FrontendFileUpload", "Text", label, help, bindingSourceName);
		}
	}
}
