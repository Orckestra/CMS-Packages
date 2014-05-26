using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation;
using Composite.Functions;
using Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.String;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Functions
{
	public sealed class FormsRendererControlFunction : FormsRendererFunctionBase
	{
		public FormsRendererControlFunction(EntityTokenFactory entityTokenFactory)
			: base("RendererControl", "Composite.Forms", typeof(object), entityTokenFactory)
		{
			ResourceHandleNameStem = "Composite.Forms.Renderer";
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
                    "ResponseUrl", typeof(string), false, new ConstantValueProvider(""), new WidgetFunctionProvider(FunctionFacade.GetWidgetFunction("Composite.Widgets.String.UrlComboBox")));

				yield return new FormsRendererFunctionParameterProfile(
					"SendButtonLabel", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"ResetButtonLabel", typeof(string), false, new ConstantValueProvider(""), StandardWidgetFunctions.TextBoxWidget);

				yield return new FormsRendererFunctionParameterProfile(
					"Email", typeof(IEnumerable<FormEmail>), false, new ConstantValueProvider(null), null);

				yield return new FormsRendererFunctionParameterProfile(
					"UseCaptcha", typeof(bool), false, new ConstantValueProvider(false), StandardWidgetFunctions.CheckBoxWidget);

			}
		}

		public override object Execute(ParameterList parameters, FunctionContextContainer context)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			if (currentPage == null) throw new InvalidOperationException("The Current HttpContext Handler must be a System.Web.Ui.Page");

			Control control = currentPage.LoadControl("~/Frontend/Composite/Forms/Renderer/Controls/FormsRender.ascx");

			control.GetType().GetProperty("parameters").SetValue(control, parameters, null);

			return control;
		}

		public static IEnumerable<KeyValuePair<Type, string>> DataTypesList()
		{
			IEnumerable<Type> generatedInterfaces = DataFacade.GetGeneratedInterfaces().OrderBy(t => t.FullName);
			//all type expept metatypes
			generatedInterfaces = generatedInterfaces.Except(PageMetaDataFacade.GetAllMetaDataTypes());

			generatedInterfaces = generatedInterfaces.OrderBy(t => t.FullName);
			return generatedInterfaces.Select(type => new KeyValuePair<Type, string>(type, type.FullName)).ToList();
		}
	}
}
